using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

public class TrayManager : IDisposable
{
    private NotifyIcon trayIcon;
    private HotkeyManager conversionHotkeyManager, caseSwapHotkeyManager;
    private readonly TextSwitcherApp mainApp;
    public AppSettings CurrentSettings { get; private set; }

    public TrayManager(TextSwitcherApp app)
    {
        mainApp = app;
        CurrentSettings = AppSettings.Load();
        conversionHotkeyManager = new HotkeyManager(1);
        conversionHotkeyManager.HotkeyPressed += mainApp.PerformTextConversion;
        caseSwapHotkeyManager = new HotkeyManager(2);
        caseSwapHotkeyManager.HotkeyPressed += mainApp.SwapCaseClipboard;
        
        var trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("הגדרות", null, (s,e) => new MainForm(this).Show());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("אודות", null, (s,e) => MessageBox.Show("TextSwitcher v1.0\n\nפותח על ידי duz\n© 2025", "אודות"));
        trayMenu.Items.Add("יציאה", null, (s,e) => { Dispose(); Application.Exit(); });

        Icon appIcon;
        try { appIcon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico")); }
        catch { appIcon = SystemIcons.Information; }

        trayIcon = new NotifyIcon { Icon = appIcon, ContextMenuStrip = trayMenu, Visible = true };
        trayIcon.DoubleClick += (s,e) => new MainForm(this).Show();
        LoadAndRegisterHotkeys();
    }

    public void LoadAndRegisterHotkeys()
    {
        CurrentSettings = AppSettings.Load();
        conversionHotkeyManager.Register(CurrentSettings.Hotkey);
        caseSwapHotkeyManager.Register(CurrentSettings.CaseSwapHotkey);
        trayIcon.Text = $"TextSwitcher\nהיפוך: {CurrentSettings.Hotkey}\nרישיות: {CurrentSettings.CaseSwapHotkey}";
    }

    public void Dispose()
    {
        trayIcon?.Dispose();
        conversionHotkeyManager?.Dispose();
        caseSwapHotkeyManager?.Dispose();
    }
}
