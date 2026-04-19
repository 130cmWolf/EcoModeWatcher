using System.Diagnostics;
using System.Runtime.InteropServices;

static class EcoModeManager
{
    const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1;
    const uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 0x1;
    const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
    const uint PROCESS_SET_INFORMATION = 0x0200;
    const int  ProcessPowerThrottling = 4;

    [StructLayout(LayoutKind.Sequential)]
    struct PROCESS_POWER_THROTTLING_STATE
    {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetProcessInformation(
        IntPtr hProcess,
        int ProcessInformationClass,
        ref PROCESS_POWER_THROTTLING_STATE ProcessInformation,
        uint ProcessInformationSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetProcessInformation(
        IntPtr hProcess,
        int ProcessInformationClass,
        ref PROCESS_POWER_THROTTLING_STATE ProcessInformation,
        uint ProcessInformationSize);

    public static IEnumerable<(string Name, int Pid)> GetEcoModeProcesses()
    {
        foreach (var proc in Process.GetProcesses())
        {
            int pid = proc.Id;
            string name = proc.ProcessName;
            proc.Dispose();
            if (IsEcoMode(pid))
                yield return (name, pid);
        }
    }

    static bool IsEcoMode(int pid)
    {
        var handle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;
        try
        {
            var state = new PROCESS_POWER_THROTTLING_STATE
            {
                Version = PROCESS_POWER_THROTTLING_CURRENT_VERSION
            };
            if (!GetProcessInformation(handle, ProcessPowerThrottling, ref state,
                    (uint)Marshal.SizeOf<PROCESS_POWER_THROTTLING_STATE>()))
                return false;

            return (state.ControlMask & PROCESS_POWER_THROTTLING_EXECUTION_SPEED) != 0
                && (state.StateMask   & PROCESS_POWER_THROTTLING_EXECUTION_SPEED) != 0;
        }
        finally
        {
            CloseHandle(handle);
        }
    }

    public static bool DisableEcoMode(int pid)
    {
        var handle = OpenProcess(PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;
        try
        {
            var state = new PROCESS_POWER_THROTTLING_STATE
            {
                Version     = PROCESS_POWER_THROTTLING_CURRENT_VERSION,
                ControlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
                StateMask   = 0
            };
            return SetProcessInformation(handle, ProcessPowerThrottling, ref state,
                (uint)Marshal.SizeOf<PROCESS_POWER_THROTTLING_STATE>());
        }
        finally
        {
            CloseHandle(handle);
        }
    }
}
