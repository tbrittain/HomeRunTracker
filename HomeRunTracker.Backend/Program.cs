using System.Net;
using System.Reflection;
using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Handlers;
using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) => {
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
builder.Services.AddScoped<INotificationHandler<HomeRunNotification>, HomeRunHandler>();
builder.Services.AddSingleton<MlbApiPollingService>();
builder.Services.AddHostedService<MlbApiPollingService>(p => p.GetRequiredService<MlbApiPollingService>());

var app = builder.Build();

app.MapHub<HomeRunHub>("home-run-hub");
app.MapGet("api/home-runs", async (IClusterClient client) =>
{
    var grain = client.GetGrain<IGameListGrain>(0);
    var homeRuns = (await grain.GetHomeRunsAsync())
        .OrderByDescending(hr => hr.TotalDistance)
        .ToList();
    return Results.Ok(homeRuns);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.Run();