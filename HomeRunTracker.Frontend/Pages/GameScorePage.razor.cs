using Blazored.Modal;
using Blazored.Modal.Services;
using HomeRunTracker.Backend.Actions.GameScore.Notifications;
using HomeRunTracker.Frontend.Components;
using HomeRunTracker.Frontend.Models;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace HomeRunTracker.Frontend.Pages;

public partial class GameScorePage
{
    private IQueryable<GameScoreModel> _items = null!;
    private HashSet<GameScoreModel> _gameScores = new();
    private bool _isLoading;
    
    private readonly GridSort<GameScoreModel> _gameScoreSort = 
        GridSort<GameScoreModel>.ByAscending(x => x.GameScore);
    
    private readonly GridSort<GameScoreModel> _inningsSort = 
        GridSort<GameScoreModel>.ByDescending(x => x.Outs);
    
    private readonly GridSort<GameScoreModel> _teamSort = 
        GridSort<GameScoreModel>.ByAscending(x => x.TeamName);

    [CascadingParameter] public DateTime Date { get; set; }
    
    [CascadingParameter] public IModalService Modal { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await ScoringPlayHubService.StartHubConnection();
        ScoringPlayHubService.SubscribeToHubMethods();
        ScoringPlayHubService.OnGameScoreReceived += OnOnGameScoreReceived;

        await base.OnInitializedAsync();
    }

    private async Task OnOnGameScoreReceived(GameScoreNotification arg)
    {
        var gameScoreModel = arg.GameScoreRecord.Adapt<GameScoreModel>();

        var existingGameScore = _gameScores.FirstOrDefault(x => x.GameId == gameScoreModel.GameId
                                                                && x.PitcherId == gameScoreModel.PitcherId);

        if (existingGameScore is not null) _gameScores.Remove(existingGameScore);

        _gameScores.Add(gameScoreModel);
        _items = _gameScores.AsQueryable();
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _gameScores.Clear();
        _items = new List<GameScoreModel>().AsQueryable();

        var gameScoreDtos = await HttpService.GetGameScoresAsync(Date == DateTime.Today ? null : Date.Date);
        var gameScores = gameScoreDtos
            .Select(x => x.Adapt<GameScoreModel>()).ToList();
        _gameScores = gameScores.ToHashSet();
        _items = _gameScores.AsQueryable();

        _isLoading = false;
        await InvokeAsync(StateHasChanged);

        await base.OnParametersSetAsync();
    }
    
    private void OnVideoButtonClicked(GameScoreModel model)
    {
        var title = model.HighlightTitle!;
        var header = model.HighlightDescription!;
        var videoSrc = model.HighlightUrl!;

        var parameters = new ModalParameters()
            .Add(nameof(VideoPlayer.HeaderText), header)
            .Add(nameof(VideoPlayer.VideoSrc), videoSrc);
        
        var options = new ModalOptions
        {
            Size = ModalSize.ExtraLarge
        };
        
        Modal.Show<VideoPlayer>(title, parameters, options);
    }

    public void Dispose()
    {
        ScoringPlayHubService.OnGameScoreReceived -= OnOnGameScoreReceived;
        GC.SuppressFinalize(this);
    }
}