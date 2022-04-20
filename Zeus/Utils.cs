using System;
using System.Diagnostics;
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
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static string GetWikiPage(string item) =>
            string.Concat("https://terraria.wiki.gg/wiki", "/", item.Replace(" ", "_"));

        public static void OpenWikiPage(string item) => OpenLink(GetWikiPage(item));
        public static void OpenLink(string link)
        {
            try
            {
                Process.Start(link);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    link = link.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", link);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", link);
                }
                else
                {
                    throw;
                }
            }            
        }

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

        public static void TryDo(Action action)
        {
            bool success = true;

            try
            {
                action();
            }
            catch (Exception e)
            {
                success = false;
                WriteError();
                
                Console.WriteLine(e);
            }

            if (success)
                WriteDone();
        }

        public static void WriteDone()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Done");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Done");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
