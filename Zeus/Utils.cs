using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Zeus
{
    public static class Utils
    {
        private static Regex capRegex = new("([A-Z])");
        public static string AddSpacesAfterCaps(string s) => capRegex.Replace(s, " $1").Trim();
        public static string RemoveSpaces(string s) => s.Replace(" ", "");

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void HideConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, 0);
        }

        public static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, 5);
        }
    }
}
