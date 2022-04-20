using System.Text.RegularExpressions;

namespace Zeus
{
    public static class Utils
    {
        private static Regex capRegex = new("([A-Z])");
        public static string AddSpacesAfterCaps(string s) => capRegex.Replace(s, " $1").Trim();
        public static string RemoveSpaces(string s) => s.Replace(" ", "");
    }
}
