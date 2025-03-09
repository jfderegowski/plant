using System;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using UnityEngine;

public class WindowCreator : MonoBehaviour
{
    // Import funkcji z user32.dll
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr CreateWindowEx(
        uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
        int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu,
        IntPtr hInstance, IntPtr lpParam
    );

    private IntPtr mainWindowHandle;
    private IntPtr secondWindowHandle;

    void Start()
    {
        mainWindowHandle = GetActiveWindow();
        CreateSecondWindow();
    }

    [Button]
    void CreateSecondWindow()
    {
        // Parametry okna
        uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
        int SW_SHOW = 5;

        // Utwórz drugie okno
        secondWindowHandle = CreateWindowEx(
            0, "UnityWndClass", "Drugie Okno", WS_OVERLAPPEDWINDOW,
            100, 100, 800, 600, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero
        );

        if (secondWindowHandle != IntPtr.Zero)
            ShowWindow(secondWindowHandle, SW_SHOW);
    }
}
