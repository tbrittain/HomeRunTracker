using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace HomeRunTracker.Frontend.Components;

public partial class HomeRunTable
{
    private IQueryable<HomeRunRecord> _items = null!;
    private HashSet<HomeRunRecord> _homeRuns = new();
    private bool _isLoading;
    private readonly GridSort<HomeRunRecord> _teamSort = GridSort<HomeRunRecord>.ByAscending(x => x.TeamName);
    private readonly GridSort<HomeRunRecord> _distanceSort = GridSort<HomeRunRecord>.ByAscending(x => x.TotalDistance);

    [Parameter] public DateTime DateTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await HomeRunHubService.StartHubConnection();
        HomeRunHubService.SubscribeToHubMethods();
    }

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _homeRuns.Clear();
        _items = new List<HomeRunRecord>().AsQueryable();

        var homeRuns = await HttpService.GetHomeRunsAsync(DateTime == DateTime.Today ? null : DateTime.Date);
        _homeRuns = homeRuns.ToHashSet();
        _items = _homeRuns.AsQueryable();

        _isLoading = false;
        await InvokeAsync(StateHasChanged);

        if (DateTime.Date != DateTime.Today)
        {
            HomeRunHubService.OnHomeRunReceived -= OnHomeRunReceived;
            HomeRunHubService.OnHomeRunUpdated -= OnHomeRunUpdated;
        }
        else
        {
            HomeRunHubService.OnHomeRunReceived += OnHomeRunReceived;
            HomeRunHubService.OnHomeRunUpdated += OnHomeRunUpdated;
        }

        await base.OnParametersSetAsync();
    }

    private async Task OnHomeRunUpdated(HomeRunUpdatedNotification arg)
    {
        var homeRun = _homeRuns.Single(_ => _.Hash == arg.HomeRunHash);
        homeRun.HighlightUrl = arg.HighlightUrl;

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnHomeRunReceived(HomeRunRecord homeRun)
    {
        _homeRuns.Add(homeRun);
        _items = _homeRuns.AsQueryable();

        await InvokeAsync(StateHasChanged);
    }

    private record RgbColor(int R, int G, int B)
    {
        public override string ToString()
        {
            return $"rgb({R}, {G}, {B})";
        }
    }

    private static RgbColor GetColorForDistance(double distance)
    {
        switch (distance)
        {
            case 400:
                return new RgbColor(255, 255, 255);
            case < 350:
                return new RgbColor(0, 0, 255);
            case > 450:
                return new RgbColor(255, 0, 0);
            case < 400:
            {
                var percent = (distance - 350) / 50.0;
                var r = (int) (255 * percent);
                var g = (int) (255 * percent);
                return new RgbColor(r, g, 255);
            }
            case > 400:
            {
                var percent = (distance - 400) / 50.0;
                var g = (int) (255 * (1 - percent));
                var b = (int) (255 * (1 - percent));
                return new RgbColor(255, g, b);
            }
            default:
                return new RgbColor(0, 0, 0);
        }
    }
}