using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CalculatorApp.ViewModels;

public class WindowUtil
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    /// <summary>
    /// Enables or disables immersive dark mode on the window's title bar.
    /// </summary>
    public static void SetImmersiveDarkMode(Window? window, bool enabled)
    {
        if (window == null) return;

        IntPtr hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero) return;

        int useDarkMode = enabled ? 1 : 0;
        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
    }
    
}