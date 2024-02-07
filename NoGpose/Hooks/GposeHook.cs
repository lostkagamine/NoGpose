using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace NoGpose.Hooks;

public unsafe class GposeHook : IDisposable
{
    private delegate bool DEnterGpose(UIModule* self);

    private readonly Hook<DEnterGpose> enterGposeHook;
    
    public GposeHook()
    {
        var addr = (nint)UIModule.StaticVTable.EnterGPose;
        Service.PluginLog.Info($"gpose addr = {addr}");
        
        enterGposeHook = Service.GameInteropProvider.HookFromAddress<DEnterGpose>(
            addr,
            this.DetourEnterGpose);
        enterGposeHook.Enable();
    }

    public void Dispose()
    {
        enterGposeHook.Dispose();
    }
    
    public bool DetourEnterGpose(UIModule* self)
    {
        Win32.ShutdownSystem();
        var proc = Win32.GetCurrentProcess();
        Win32.TerminateProcess(proc, 69);
        
        return false;
    }
}
