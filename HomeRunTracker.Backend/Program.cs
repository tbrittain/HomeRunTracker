using System.Net;
using System.Reflection;
using HomeRunTracker.Backend.Actions.Game.Handlers;
using HomeRunTracker.Backend.Actions.Game.Notifications;
using HomeRunTracker.Backend.Actions.GameScore.Handlers;
using HomeRunTracker.Backend.Actions.GameScore.Notifications;
using HomeRunTracker.Backend.Actions.ScoringPlay.Handlers;
using HomeRunTracker.Backend.Actions.ScoringPlay.Notifications;
using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Backend.Utils;
using HomeRunTracker.Infrastructure.LeverageIndex;
using HomeRunTracker.Infrastructure.MlbApiService;
using HomeRunTracker.Infrastructure.PitcherGameScore;
using MediatR;
using Serilog;

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

builder.Host.UseSerilog();

builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly()))
    .AddMlbApiService()
    .AddLeverageIndexService()
    .AddPitcherGameScoreService()
    .AddScoped<INotificationHandler<GameStoppedNotification>, GameStoppedHandler>()
    .AddScoped<INotificationHandler<ScoringPlayNotification>, HomeRunHandler>()
    .AddScoped<INotificationHandler<ScoringPlayUpdatedNotification>, ScoringPlayUpdatedHandler>()
    .AddScoped<INotificationHandler<GameScoreNotification>, GameScoreHandler>()
    .AddSingleton<MlbCurrentDayGamePollingService>()
    .AddHostedService<MlbCurrentDayGamePollingService>(p =>
        p.GetRequiredService<MlbCurrentDayGamePollingService>())
    .AddSignalR();

builder.Services.AddControllers();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.With(new GrainLoggingOptions(app.Services))
    .CreateLogger();

app.MapHub<ScoringPlayHub>("scoring-play-hub");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.Run();