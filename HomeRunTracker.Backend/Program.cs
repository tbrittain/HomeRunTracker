using System.Net;
using System.Reflection;
using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Handlers;
using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    // In order to support multiple hosts forming a cluster, they must listen on different ports.
    // Use the --InstanceId X option to launch subsequent hosts.
    var instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    siloBuilder
        .UseLocalhostClustering(
            siloPort: 11111 + instanceId,
            gatewayPort: 30000 + instanceId,
            primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111))
        .AddActivityPropagation()
        .AddMemoryGrainStorage("PubSubStore");

#if DEBUG
    siloBuilder.UseDashboard(options =>
    {
        options.Username = "admin";
        options.Password = "admin";
        options.Port = 8080 + instanceId;
    });
#endif
});

builder.WebHost.UseKestrel((ctx, kestrelOptions) =>
{
    // To avoid port conflicts, each Web server must listen on a different port.
    var instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    kestrelOptions.ListenLocalhost(5001 + instanceId);
});

builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<INotificationHandler<GameStoppedNotification>, GameRemovedHandler>();
builder.Services.AddScoped<INotificationHandler<ScoringPlayNotification>, HomeRunHandler>();
builder.Services.AddScoped<INotificationHandler<ScoringPlayUpdatedNotification>, ScoringPlayUpdatedHandler>();
builder.Services.AddScoped<INotificationHandler<GameScoreNotification>, GameScoreHandler>();

builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddSingleton<MlbCurrentDayGamePollingService>();
builder.Services.AddSingleton<LeverageIndexService>();
builder.Services.AddSingleton<PitcherGameScoreService>();
builder.Services.AddHostedService<MlbCurrentDayGamePollingService>(p =>
    p.GetRequiredService<MlbCurrentDayGamePollingService>());

var app = builder.Build();

app.MapHub<ScoringPlayHub>("scoring-play-hub");

app.MapGet("api/scoring-plays", async (IClusterClient client, DateTime? date) =>
{
    var grain = client.GetGrain<IGameListGrain>(0);
    var scoringPlays = await grain.GetScoringPlays(date ?? DateTime.Now);
    return Results.Ok(scoringPlays);
});

app.MapGet("api/game-scores", async (IClusterClient client, DateTime? date) =>
{
    var grain = client.GetGrain<IGameListGrain>(0);
    var gameScores = await grain.GetGameScores(date ?? DateTime.Now);
    return Results.Ok(gameScores);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.Run();