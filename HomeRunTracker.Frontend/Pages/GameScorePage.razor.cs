using HomeRunTracker.Common.Models.Notifications;
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
        GridSort<GameScoreModel>.ByDescending(x => x.GameScore);
    
    private readonly GridSort<GameScoreModel> _inningsSort = 
        GridSort<GameScoreModel>.ByDescending(x => x.Outs);
    
    private readonly GridSort<ScoringPlayModel> _teamSort = 
        GridSort<ScoringPlayModel>.ByAscending(x => x.TeamName);

    [CascadingParameter] public DateTime Date { get; set; }

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

    public void Dispose()
    {
        ScoringPlayHubService.OnGameScoreReceived -= OnOnGameScoreReceived;
        GC.SuppressFinalize(this);
    }
}