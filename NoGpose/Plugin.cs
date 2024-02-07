using Dalamud.IoC;
using Dalamud.Plugin;
using NoGpose.Hooks;

namespace NoGpose
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "No Gpose";

        private DalamudPluginInterface PluginInterface { get; init; }
        public Configuration Configuration { get; init; }

        public GposeHook Hook;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
            
            PluginInterface.Create<Service>();
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            Hook = new();
        }

        public void Dispose()
        {
            Hook.Dispose();
        }
    }
}
