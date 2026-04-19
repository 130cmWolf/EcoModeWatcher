using System.Windows.Forms;

sealed class TrayApp : IDisposable
{
    readonly NotifyIcon _notifyIcon;
    readonly System.Threading.Timer _timer;
    volatile bool _running = true;

    public TrayApp(AppSettings settings)
    {
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream("EcoModeWatcher.EcoDisable.ico");
        var icon = stream != null ? new Icon(stream) : SystemIcons.Application;

        var menu = new ContextMenuStrip();
        menu.Items.Add("(&e)xit", null, (_, _) => Exit());

        _notifyIcon = new NotifyIcon
        {
            Icon = icon,
            Text = "EcoMode Watcher",
            ContextMenuStrip = menu,
            Visible = true,
        };

        _timer = new System.Threading.Timer(_ => Tick(), null, 0, settings.Watch);
    }

    void Tick()
    {
        if (!_running) return;
        foreach (var (name, pid) in EcoModeManager.GetEcoModeProcesses())
        {
            if (name.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                EcoModeManager.DisableEcoMode(pid);
        }
    }

    void Exit()
    {
        _running = false;
        _timer.Dispose();
        _notifyIcon.Visible = false;
        Application.Exit();
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _timer.Dispose();
    }
}
