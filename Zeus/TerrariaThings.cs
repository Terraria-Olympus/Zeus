using System;
using System.IO;
using System.Text;

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
    }
}
