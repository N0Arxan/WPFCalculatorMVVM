using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CalculatorApp.ViewModels
{
    /// <summary>
    /// A utility class for interacting with the Windows Desktop Window Manager (DWM) API.
    /// Provides functionality to modify window attributes, such as enabling dark mode for the title bar.
    /// </summary>
    public class WindowUtil
    {
        /// <summary>
        /// DWM Window Attribute for enabling immersive dark mode.
        /// Constant value is 20 for Windows 11 22H2+ (originally 19 for older builds).
        /// </summary>
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;


        /// <summary>
        /// Sets the value of a Desktop Window Manager (DWM) non-client rendering attribute for a window.
        /// This is a P/Invoke declaration for the native dwmapi.dll function.
        /// </summary>
        /// <param name="hwnd">A handle to the window for which the attribute value is to be set.</param>
        /// <param name="dwAttribute">A flag describing which value to set.</param>
        /// <param name="pvAttribute">A pointer to an object containing the attribute value.</param>
        /// <param name="cbAttribute">The size, in bytes, of the attribute value.</param>
        /// <returns>An HRESULT success or error code.</returns>
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute,
            int cbAttribute);

        /// <summary>
        /// Enables or disables immersive dark mode for the specified window's title bar and borders.
        /// </summary>
        /// <param name="window">The WPF window to modify. Can be null.</param>
        /// <param name="enabled">A boolean value indicating whether to enable (true) or disable (false) dark mode.</param>
        public static void SetImmersiveDarkMode(Window? window, bool enabled)
        {
            if (window == null) return;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            int useDarkMode = enabled ? 1 : 0;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }

    }
}