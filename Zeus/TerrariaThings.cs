using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Zeus.Id;

namespace Zeus
{
    public static class TerrariaThings
    {
        public static byte[] PlayerEncryptionKey = new UnicodeEncoding().GetBytes("h3y_gUyZ");

        public static string GetSavePath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games", "Terraria");
            else
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
        }
        
        public static (byte r, byte g, byte b) ReadRGB(this BinaryReader reader) =>
            (reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

        private static Regex sacrificeFileSplitRegex = new("\r\n|\r|\n");
        public static Dictionary<int, int> LoadSacrificeCountsNeededByItemIdFromFile(string path)
        {
            Dictionary<int, int> ret = new();
            string[] array = sacrificeFileSplitRegex.Split(File.ReadAllText(path));

            foreach (string line in array)
            {
                if (line.StartsWith("//"))
                    continue;

                string[] parts = line.Split("\t");
                if (parts.Length >= 3 && ItemID.Search.TryGetId(parts[0], out int itemId))
                {
                    string category = parts[1].ToLower();
                    int? amount = LoadSacrificeCountsNeededByItemIdFromFile_ParseCategory(category);

                    if (!ret.ContainsKey(itemId) && amount != null)
                        ret.Add(itemId, amount.Value);
                }
            }

            return ret;
        }

        public static int? LoadSacrificeCountsNeededByItemIdFromFile_ParseCategory(string category)
        {
            switch (category)
            {
                case "":
                case "a":
                    return 50;
                case "b":
                    return 25;
                    
                case "c":
                    return 5;
                    
                case "d":
                    return 1;
                    
                case "e":
                    return null;
                    
                case "f":
                    return 2;
                    
                case "g":
                    return 3;
                    
                case "h":
                    return 10;
                    
                case "i":
                    return 15;
                    
                case "j":
                    return 30;
                    
                case "k":
                    return 99;
                    
                case "l":
                    return 100;
                    
                case "m":
                    return 200;
                    
                case "n":
                    return 20;
                    
                case "o":
                    return 400;
                    
                default:
                    throw new Exception("Category " + category + " uknown.");
            }

        }
    }
}
