using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace NoGpose;

public class Service
{
    [PluginService]
    public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    
    [PluginService]
    public static IPluginLog PluginLog { get; private set; } = null!;
}
