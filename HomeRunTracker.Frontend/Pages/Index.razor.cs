using Microsoft.AspNetCore.Components;

namespace HomeRunTracker.Frontend.Pages;

public partial class Index
{
    private bool _isDateGreaterOrEqualToToday = true;
    private DateTime _date = DateTime.Today;

    private DateTime Date
    {
        get => _date;
        set
        {
            _date = value;

            _isDateGreaterOrEqualToToday = value >= DateTime.Today;
        }
    }

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (uri.Query.Contains("date"))
        {
            var date = uri.Query.Split("date=")[1];
            if (DateTime.TryParse(date, out var parsedDate))
            {
                if (parsedDate.Date <= DateTime.Today)
                {
                    Date = parsedDate;
                }
            }
        }

        base.OnInitialized();
    }

    private void AddDay()
    {
        Date = Date.AddDays(1);
        var newUri = NavigationManager.GetUriWithQueryParameter("date", Date.ToString("yyyy-MM-dd"));
        NavigationManager.NavigateTo(newUri);
    }

    private void RemoveDay()
    {
        Date = Date.AddDays(-1);
        var newUri = NavigationManager.GetUriWithQueryParameter("date", Date.ToString("yyyy-MM-dd"));
        NavigationManager.NavigateTo(newUri);
    }

    private void GoToToday()
    {
        Date = DateTime.Today;
        var newUri = NavigationManager.GetUriWithQueryParameter("date", Date.ToString("yyyy-MM-dd"));
        NavigationManager.NavigateTo(newUri);
    }
}