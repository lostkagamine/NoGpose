using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace NoGpose;

public class Win32
{
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    private struct TokPriv1Luid
    {
        public int Count;
        public long Luid;
        public int Attr;
    }

    [DllImport("Advapi32.dll")]
    private static extern bool InitiateSystemShutdownA(
        string machineName,
        string? message,
        uint timeout,
        bool forceAppsClosed,
        bool rebootAfterShutdown);

    [DllImport("kernel32.dll")]
    internal static extern IntPtr GetCurrentProcess();
    
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(
        IntPtr h,
        int acc,
        ref IntPtr phtok);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LookupPrivilegeValue(
        string? host,
        string name,
        ref long pluid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool AdjustTokenPrivileges(
        IntPtr htok,
        bool disall,
        ref TokPriv1Luid newst,
        int len,
        IntPtr prev,
        IntPtr relen);

    [DllImport("kernel32.dll")]
    internal static extern bool TerminateProcess(IntPtr hProcess, int exitCode);

    private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
    private const int TOKEN_QUERY = 0x08;
    private const int SE_PRIVILEGE_ENABLED = 0x02;
    private const string SE_SHUTDOWN_PRIVILEGE = "SeShutdownPrivilege";

    private static void GainShutdownPrivilege()
    {
        TokPriv1Luid tp;
        var process = GetCurrentProcess();
        var hToken = IntPtr.Zero;
        OpenProcessToken(process, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken);
        tp.Count = 1;
        tp.Luid = 0;
        tp.Attr = SE_PRIVILEGE_ENABLED;
        LookupPrivilegeValue(null, SE_SHUTDOWN_PRIVILEGE, ref tp.Luid);
        AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
    }
    
    public static void ShutdownSystem()
    {
        GainShutdownPrivilege();
        InitiateSystemShutdownA(
            "",
            null,
            0,
            true,
            false);
    }
}
