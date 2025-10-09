using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;

public class HotkeyManager : IDisposable
{
    [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [Flags] public enum HotkeyModifiers : uint { None = 0, Alt = 1, Control = 2, Shift = 4, Win = 8 }

    private const int WM_HOTKEY = 0x0312;
    private readonly int HotkeyId;
    private readonly HotkeyWindow hotkeyWindow;
    public Action HotkeyPressed;

    public HotkeyManager(int id)
    {
        HotkeyId = id;
        hotkeyWindow = new HotkeyWindow(WM_HOTKEY, HotkeyId, m => HotkeyPressed?.Invoke());
    }

    public bool TryParseHotkey(string hotkeyString, out uint modifiers, out Keys key)
    {
        modifiers = 0; key = Keys.None;
        if (hotkeyString.ToLower() == "none") return true;
        try
        {
            var parts = hotkeyString.Split('+').Select(p => p.Trim()).ToList();
            foreach (var part in parts.Take(parts.Count - 1))
                if (Enum.TryParse<HotkeyModifiers>(part, true, out var mod)) modifiers |= (uint)mod;
            if (Enum.TryParse<Keys>(parts.Last(), true, out key)) return true;
        } catch { }
        return false;
    }

    public bool Register(string hotkeyString)
    {
        Unregister();
        if (hotkeyString.ToLower() == "none") return true;
        if (TryParseHotkey(hotkeyString, out uint mod, out Keys key))
            if (RegisterHotKey(hotkeyWindow.Handle, HotkeyId, mod, (uint)key)) return true;
        MessageBox.Show($"הקיצור '{hotkeyString}' נכשל ברישום");
        return false;
    }

    public void Unregister() => UnregisterHotKey(hotkeyWindow.Handle, HotkeyId);
    public void Dispose() { Unregister(); hotkeyWindow.DestroyHandle(); }
}

internal class HotkeyWindow : NativeWindow
{
    private readonly int hotkeyMsg, hotkeyId;
    private readonly Action<Message> hotkeyCallback;
    public HotkeyWindow(int msg, int id, Action<Message> callback)
    {
        hotkeyMsg = msg; hotkeyId = id; hotkeyCallback = callback;
        CreateHandle(new CreateParams());
    }
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == hotkeyMsg && m.WParam.ToInt32() == hotkeyId) hotkeyCallback(m);
    }
}
