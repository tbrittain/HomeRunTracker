using Blazored.Modal;
using HomeRunTracker.Frontend.Services;
using HomeRunTracker.Frontend.Services.HttpService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrleansClient(clientBuilder =>
{
    clientBuilder.UseLocalhostClustering();
});
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddBlazoredModal();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddScoped<HomeRunHubService>();
builder.Services.AddScoped<TimezoneService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();