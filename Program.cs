using System.Windows.Forms;

using var mutex = new System.Threading.Mutex(true, "EcoModeWatcher-SingleInstance", out bool createdNew);
if (!createdNew) return;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var settings = AppSettings.Load();
using var trayApp = new TrayApp(settings);
Application.Run();
