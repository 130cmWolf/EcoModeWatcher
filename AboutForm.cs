using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

sealed class AboutForm : Form
{
    public AboutForm()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";

        Text = "About EcoModeWatcher";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(320, 140);

        var labelName = new Label
        {
            Text = $"EcoModeWatcher  v{version}",
            Font = new Font(Font.FontFamily, 11, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20),
        };

        var link = new LinkLabel
        {
            Text = "https://github.com/130cmWolf/EcoModeWatcher",
            AutoSize = true,
            Location = new Point(20, 60),
        };
        link.LinkClicked += (_, _) =>
            Process.Start(new ProcessStartInfo(link.Text) { UseShellExecute = true });

        var btnOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Size = new Size(80, 28),
            Location = new Point(ClientSize.Width / 2 - 40, 100),
        };

        AcceptButton = btnOk;
        Controls.AddRange([labelName, link, btnOk]);
    }
}
