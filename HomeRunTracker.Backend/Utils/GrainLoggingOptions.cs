using Serilog.Core;
using Serilog.Events;

namespace HomeRunTracker.Backend.Utils;

public class GrainLoggingOptions : ILogEventEnricher
{
    private readonly IGrainFactory _grainFactory;

    public GrainLoggingOptions(IServiceProvider serviceProvider)
    {
        _grainFactory = serviceProvider.GetRequiredService<IGrainFactory>();
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.ContainsKey("GrainId"))
        {
            return;
        }

        var grainId = int.Parse(logEvent.Properties["GrainId"].ToString());
        var grain = _grainFactory.GetGrain<IGrainWithIntegerKey>(grainId);

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("GrainId", grain.GetPrimaryKeyLong().ToString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("GrainType", grain.GetType().FullName));
    }
}