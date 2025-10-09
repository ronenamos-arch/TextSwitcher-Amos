using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class TextSwitcherApp : Form
{
    private TrayManager trayManager;

    [DllImport("user32.dll")] 
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    
    private const byte VK_CONTROL = 0x11;
    private const byte VK_C = 0x43;
    private const byte VK_V = 0x56;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public TextSwitcherApp()
    {
        this.ShowInTaskbar = false;
        this.Visible = false;
        this.WindowState = FormWindowState.Minimized;
        this.Opacity = 0;
        trayManager = new TrayManager(this);
    }

    private async Task PressKeyAsync(byte key)
    {
        keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
        await Task.Delay(50);
        keybd_event(key, 0, 0, UIntPtr.Zero);
        await Task.Delay(50);
        keybd_event(key, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        await Task.Delay(50);
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private string SafeGetClipboard()
    {
        try { return Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty; }
        catch { return string.Empty; }
    }

    private void SafeSetClipboard(string text)
    {
        try { Clipboard.SetText(text); }
        catch { }
    }

    public async void PerformTextConversion()
    {
        try
        {
            string originalClip = SafeGetClipboard();
            
            await PressKeyAsync(VK_C);
            await Task.Delay(200);
            
            string selectedText = SafeGetClipboard();
            
            if (string.IsNullOrEmpty(selectedText) || selectedText == originalClip)
            {
                if (!string.IsNullOrEmpty(originalClip))
                    SafeSetClipboard(originalClip);
                return;
            }
            
            string direction = Converter.DetectTextDirection(selectedText);
            string converted = Converter.ConvertLayout(selectedText, direction);
            
            SafeSetClipboard(converted);
            await Task.Delay(100);
            
            await PressKeyAsync(VK_V);
            await Task.Delay(200);
            
            await Task.Delay(1000);
            if (!string.IsNullOrEmpty(originalClip))
                SafeSetClipboard(originalClip);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "TextSwitcher");
        }
    }

    public async void SwapCaseClipboard()
    {
        try
        {
            string originalClip = SafeGetClipboard();
            
            await PressKeyAsync(VK_C);
            await Task.Delay(200);
            
            string selectedText = SafeGetClipboard();
            
            if (string.IsNullOrEmpty(selectedText) || selectedText == originalClip)
            {
                if (!string.IsNullOrEmpty(originalClip))
                    SafeSetClipboard(originalClip);
                return;
            }
            
            string swapped = Converter.SwapCase(selectedText);
            
            SafeSetClipboard(swapped);
            await Task.Delay(100);
            
            await PressKeyAsync(VK_V);
            await Task.Delay(200);
            
            await Task.Delay(1000);
            if (!string.IsNullOrEmpty(originalClip))
                SafeSetClipboard(originalClip);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "TextSwitcher");
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TextSwitcherApp());
    }
}
