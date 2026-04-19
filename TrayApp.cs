using System.Windows.Forms;

sealed class TrayApp : IDisposable
{
    readonly NotifyIcon _notifyIcon;
    readonly System.Threading.Timer _timer;
    readonly Icon _icon;
    volatile bool _running = true;
    bool _disposed;

    public TrayApp(AppSettings settings)
    {
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream("EcoModeWatcher.EcoDisable.ico");
        _icon = stream != null ? new Icon(stream) : SystemIcons.Application;

        var menu = new ContextMenuStrip();
        menu.Items.Add("(&A)bout", null, (_, _) => new AboutForm().ShowDialog());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("(&E)xit", null, (_, _) => Exit());

        _notifyIcon = new NotifyIcon
        {
            Icon = _icon,
            Text = "EcoMode Watcher",
            ContextMenuStrip = menu,
            Visible = true,
        };

        _timer = new System.Threading.Timer(_ => Tick(), null, 0, settings.Watch);
    }

    void Tick()
    {
        if (!_running) return;
        try
        {
            foreach (var (name, pid) in EcoModeManager.GetEcoModeProcesses())
            {
                if (name.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                    EcoModeManager.DisableEcoMode(pid);
            }
        }
        catch { }
    }

    void Exit()
    {
        Dispose();
        Application.Exit();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _running = false;
        _timer.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _icon.Dispose();
    }
}
