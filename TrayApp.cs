using System.Windows.Forms;

sealed class TrayApp : IDisposable
{
    readonly NotifyIcon _notifyIcon;
    readonly System.Threading.Timer _timer;
    readonly ContextMenuStrip _menu;
    readonly Icon? _icon;
    volatile bool _running = true;
    bool _disposed;

    public TrayApp(AppSettings settings)
    {
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream("NoNapChrome.NoNapChrome.ico");
        if (stream != null)
            _icon = new Icon(stream);

        _menu = new ContextMenuStrip();
        _menu.Items.Add("(&A)bout", null, (_, _) => { using var f = new AboutForm(); f.ShowDialog(); });
        _menu.Items.Add(new ToolStripSeparator());
        _menu.Items.Add("(&E)xit", null, (_, _) => Exit());

        _notifyIcon = new NotifyIcon
        {
            Icon = _icon ?? SystemIcons.Application,
            Text = "NoNapChrome",
            ContextMenuStrip = _menu,
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
        _menu.Dispose();
        _icon?.Dispose();
    }
}
