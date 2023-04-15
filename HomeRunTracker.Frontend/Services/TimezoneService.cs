using Microsoft.JSInterop;

namespace HomeRunTracker.Frontend.Services;

public class TimezoneService
{
    private readonly IJSRuntime _jsRuntime;
    private TimeSpan? _userOffset;

    public TimezoneService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<TimeSpan> GetLocalOffset()
    {
        if (_userOffset != null) return _userOffset.Value;

        var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
        _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);

        return _userOffset.Value;
    }
}