using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using HomeRunTracker.Frontend.Models;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace HomeRunTracker.Frontend.Components;

public partial class HomeRunTable
{
    private IQueryable<HomeRunModel> _items = null!;
    private HashSet<HomeRunModel> _homeRuns = new();
    private bool _isLoading;
    private readonly GridSort<HomeRunModel> _teamSort = GridSort<HomeRunModel>.ByAscending(x => x.TeamName);
    private readonly GridSort<HomeRunModel> _distanceSort = GridSort<HomeRunModel>.ByAscending(x => x.TotalDistance);
    private readonly GridSort<HomeRunModel> _exitVelocitySort = GridSort<HomeRunModel>.ByDescending(x => x.LaunchSpeed);
    private readonly GridSort<HomeRunModel> _launchAngleSort = GridSort<HomeRunModel>.ByDescending(x => x.LaunchAngle);
    private readonly GridSort<HomeRunModel> _leverageIndexSort = GridSort<HomeRunModel>.ByDescending(x => x.LeverageIndex);

    [Parameter]
    public DateTime DateTime { get; set; }

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
        _items = new List<HomeRunModel>().AsQueryable();

        var homeRunDtos = await HttpService.GetHomeRunsAsync(DateTime == DateTime.Today ? null : DateTime.Date);
        var homeRuns = homeRunDtos
            .Select(x => x.Adapt<HomeRunModel>()).ToList();
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

    private async Task OnHomeRunReceived(HomeRunRecord homeRunDto)
    {
        var homeRun = homeRunDto.Adapt<HomeRunModel>();
        _homeRuns.Add(homeRun);
        _items = _homeRuns.AsQueryable();

        await InvokeAsync(StateHasChanged);
    }
}