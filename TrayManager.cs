using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

public class TrayManager : IDisposable
{
    private NotifyIcon trayIcon;
    private HotkeyManager conversionHotkeyManager, caseSwapHotkeyManager;
    private readonly TextSwitcherApp mainApp;
    private TemplateManager templateManager;
    private QuickAccessForm quickAccessForm;
    public AppSettings CurrentSettings { get; private set; }

    public TrayManager(TextSwitcherApp app)
    {
        mainApp = app;
        CurrentSettings = AppSettings.Load();
        templateManager = new TemplateManager();
        quickAccessForm = new QuickAccessForm(mainApp, templateManager);

        conversionHotkeyManager = new HotkeyManager(1);
        conversionHotkeyManager.HotkeyPressed += mainApp.PerformTextConversion;
        caseSwapHotkeyManager = new HotkeyManager(2);
        caseSwapHotkeyManager.HotkeyPressed += mainApp.SwapCaseClipboard;

        var trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("🚀 גישה מהירה", null, (s,e) => ShowQuickAccess());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("⚙️ הגדרות", null, (s,e) => ShowSettings());
        trayMenu.Items.Add("ℹ️ אודות", null, (s,e) => MessageBox.Show("TextSwitcher-Amos v2.0\n\nמבוסס על TextSwitcher של duz\nשופר על ידי Ronen Amos\n\n© 2025", "אודות"));
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("❌ יציאה", null, (s,e) => { Dispose(); Application.Exit(); });

        Icon appIcon;
        try { appIcon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico")); }
        catch { appIcon = SystemIcons.Information; }

        trayIcon = new NotifyIcon { Icon = appIcon, ContextMenuStrip = trayMenu, Visible = true };
        trayIcon.DoubleClick += (s,e) => ShowQuickAccess();
        LoadAndRegisterHotkeys();
    }

    private void ShowQuickAccess()
    {
        if (quickAccessForm.Visible)
            quickAccessForm.Hide();
        else
            quickAccessForm.Show();
    }

    private void ShowSettings()
    {
        try
        {
            // Try to show MainForm if it exists, otherwise show a message
            MessageBox.Show("הגדרות יתווספו בגרסה הבאה.\n\nכרגע ניתן להשתמש ב:\nF10 - המרת טקסט\nF6 - החלפת אותיות\n\nלחץ פעמיים על האייקון בטריי לגישה מהירה!", "הגדרות", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch
        {
            MessageBox.Show("הגדרות יתווספו בגרסה הבאה.", "הגדרות");
        }
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
        quickAccessForm?.Close();
        quickAccessForm?.Dispose();
        trayIcon?.Dispose();
        conversionHotkeyManager?.Dispose();
        caseSwapHotkeyManager?.Dispose();
    }
}
