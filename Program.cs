using System.Windows.Forms;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var settings = AppSettings.Load();
using var trayApp = new TrayApp(settings);
Application.Run();
