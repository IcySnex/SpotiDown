using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PInvoke;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WinRT.Interop;

namespace SpotiDown.Helpers;

public class Window
{
    private delegate IntPtr WinProc(IntPtr hWnd, User32.WindowMessage Msg, IntPtr wParam, IntPtr lParam);
    private static WinProc? newWndProc = null;
    private static IntPtr oldWndProc = IntPtr.Zero;
    [DllImport("user32")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, User32.WindowLongIndexFlags nIndex, WinProc newProc);
    [DllImport("user32.dll")]
    static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, User32.WindowMessage Msg, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential)]
    struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    static int MinWidth = 0;
    static int MinHeight = 0;

    private static IntPtr NewWindowProc(IntPtr hWnd, User32.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        if (Msg == User32.WindowMessage.WM_GETMINMAXINFO)
        {
            MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            minMaxInfo.ptMinTrackSize.x = MinWidth;
            minMaxInfo.ptMinTrackSize.y = MinHeight;
            Marshal.StructureToPtr(minMaxInfo, lParam, true);

        }
        return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }

    public static IntPtr GetHandle(Microsoft.UI.Xaml.Window Window) => 
        WindowNative.GetWindowHandle(Window);

    public static void SetSize(Microsoft.UI.Xaml.Window Window, int Width, int Height)
    {
        IntPtr HWND = GetHandle(Window);
        float scalingFactor = (float)User32.GetDpiForWindow(HWND) / 96;
        User32.SetWindowPos(HWND, User32.SpecialWindowHandles.HWND_TOP, 0, 0, (int)(Width * scalingFactor), (int)(Height * scalingFactor), User32.SetWindowPosFlags.SWP_NOMOVE);
    }

    public static void SetMinSize(Microsoft.UI.Xaml.Window Window, int Width, int Height)
    {
        IntPtr HWND = GetHandle(Window);
        var dpi = User32.GetDpiForWindow(HWND);

        MinWidth = (int)(Width * (float)dpi / 96);
        MinHeight = (int)(Height * (float)dpi / 96);

        newWndProc = new WinProc(NewWindowProc);
        oldWndProc = SetWindowLong(HWND, User32.WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
    }

    public static void SetIcon(Microsoft.UI.Xaml.Window Window, string iconName)
    {
        var hwnd = GetHandle(Window);
        var hIcon = User32.LoadImage(IntPtr.Zero, iconName,
                  User32.ImageType.IMAGE_ICON, 16, 16, User32.LoadImageFlags.LR_LOADFROMFILE);

        User32.SendMessage(hwnd, User32.WindowMessage.WM_SETICON, (IntPtr)0, hIcon);
    }

    public static async Task<ContentDialogResult> Alert(XamlRoot Root, string Title, object Content, string CloseText = "Cancel", string? PrimaryText = null, string? SecondaryText = null)
    {
        var cd = new ContentDialog
        {
            Title = Title,
            Content = Content,
            CloseButtonText = CloseText,
            XamlRoot = Root
        };
        if (PrimaryText != null)
            cd.PrimaryButtonText = PrimaryText;
        if (SecondaryText != null)
            cd.SecondaryButtonText = SecondaryText;
        return await cd.ShowAsync();
    }
}
