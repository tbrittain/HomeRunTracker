using Blazored.Modal;
using Blazored.Modal.Services;
using HomeRunTracker.Common.Enums;
using HomeRunTracker.Common.Models.Notifications;
using HomeRunTracker.Frontend.Components;
using HomeRunTracker.Frontend.Models;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace HomeRunTracker.Frontend.Pages;

public partial class ScoringPlayPage
{
    private IQueryable<ScoringPlayModel> _items = null!;
    private HashSet<ScoringPlayModel> _scoringPlays = new();
    private bool _isLoading;
    private TimeSpan _localOffset = TimeSpan.Zero;
    private bool _onlyShowHomeRuns = true;
    
    private readonly GridSort<ScoringPlayModel> _teamSort = 
        GridSort<ScoringPlayModel>.ByAscending(x => x.TeamName);
    
    private readonly GridSort<ScoringPlayModel> _distanceSort = 
        GridSort<ScoringPlayModel>.ByAscending(x => x.TotalDistance);
    
    private readonly GridSort<ScoringPlayModel> _exitVelocitySort = 
        GridSort<ScoringPlayModel>.ByDescending(x => x.LaunchSpeed);
    
    private readonly GridSort<ScoringPlayModel> _launchAngleSort = 
        GridSort<ScoringPlayModel>.ByDescending(x => x.LaunchAngle);

    private readonly GridSort<ScoringPlayModel> _leverageIndexSort =
        GridSort<ScoringPlayModel>.ByDescending(x => x.LeverageIndex);

    private readonly GridSort<ScoringPlayModel> _dateTimeOffsetSort =
        GridSort<ScoringPlayModel>.ByDescending(x => x.DateTimeOffset);
    
    private readonly GridSort<ScoringPlayModel> _playResultSort = 
        GridSort<ScoringPlayModel>.ByDescending(x => x.PlayResult);

    [CascadingParameter] public DateTime Date { get; set; }
    
    [CascadingParameter] public IModalService Modal { get; set; } = default!;

    private bool OnlyShowHomeRuns
    {
        get => _onlyShowHomeRuns;
        set
        {
            _onlyShowHomeRuns = value;
            FilterScoringPlays(_onlyShowHomeRuns);
            StateHasChanged();
        }
    }

    private void FilterScoringPlays(bool onlyShowHomeRuns)
    {
        if (onlyShowHomeRuns)
        {
            _items = _scoringPlays
                .Where(x => x.Result == EPlayResult.HomeRun)
                .AsQueryable();
        }
        else
        {
            _items = _scoringPlays.AsQueryable();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await ScoringPlayHubService.StartHubConnection();
        ScoringPlayHubService.SubscribeToHubMethods();
        ScoringPlayHubService.OnScoringPlayReceived += OnScoringPlayReceived;
        ScoringPlayHubService.OnScoringPlayUpdated += OnScoringPlayUpdated;
    }

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _scoringPlays.Clear();
        _items = new List<ScoringPlayModel>().AsQueryable();

        var scoringPlayDtos = await HttpService.GetScoringPlaysAsync(Date == DateTime.Today ? null : Date.Date);
        var scoringPlays = scoringPlayDtos
            .Select(x => x.Adapt<ScoringPlayModel>()).ToList();
        _scoringPlays = scoringPlays.ToHashSet();
        FilterScoringPlays(OnlyShowHomeRuns);

        _isLoading = false;
        await InvokeAsync(StateHasChanged);

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _localOffset = await TimezoneService.GetLocalOffset();
        await InvokeAsync(StateHasChanged);

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnScoringPlayUpdated(ScoringPlayUpdatedNotification arg)
    {
        if (arg.GameStartTime.Date != Date.Date) return;

        var homeRun = _scoringPlays.Single(_ => _.Hash == arg.HomeRunHash);
        homeRun.HighlightUrl = arg.HighlightUrl;

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnScoringPlayReceived(ScoringPlayNotification arg)
    {
        if (arg.GameStartTime.Date != Date.Date) return;

        var homeRunDto = arg.ScoringPlay;
        var homeRun = homeRunDto.Adapt<ScoringPlayModel>();
        _scoringPlays.Add(homeRun);
        FilterScoringPlays(OnlyShowHomeRuns);

        await InvokeAsync(StateHasChanged);
    }

    private void OnVideoButtonClicked(ScoringPlayModel model)
    {
        var header = model.Description;
        var videoSrc = model.HighlightUrl!;

        var parameters = new ModalParameters()
            .Add(nameof(VideoPlayer.HeaderText), header)
            .Add(nameof(VideoPlayer.VideoSrc), videoSrc);
        
        var options = new ModalOptions
        {
            Size = ModalSize.ExtraLarge
        };
        
        Modal.Show<VideoPlayer>("", parameters, options);
    }

    public void Dispose()
    {
        ScoringPlayHubService.OnScoringPlayReceived -= OnScoringPlayReceived;
        ScoringPlayHubService.OnScoringPlayUpdated -= OnScoringPlayUpdated;
        GC.SuppressFinalize(this);
    }
}