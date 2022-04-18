using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Zeus.DataStructures
{
    public class PlayerFileData
    {
        public List<JourneyItemResearchInfo> CreativeSacs = new();

        public static PlayerFileData Load(string path)
        {
            PlayerFileData ret = new();

            byte[] buffer = File.ReadAllBytes(path);

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.None;

            using MemoryStream stream = new(buffer);
            using CryptoStream cryptoStream =
                new(stream, rijndael.CreateDecryptor(TerrariaThings.PlayerEncryptionKey, TerrariaThings.PlayerEncryptionKey), CryptoStreamMode.Read);
            using BinaryReader reader = new(cryptoStream);

            int fileVersion = reader.ReadInt32();
            if (fileVersion >= 135)
            {
                // Read metadata bytes
                reader.ReadUInt64();
                reader.ReadUInt32();
                reader.ReadUInt64();
            }

            // Read name bytes
            reader.ReadString();

            if (fileVersion > 248)
                return null;

            // Read difficulty byte
            if (fileVersion >= 10)
            {
                if (fileVersion >= 17)
                    reader.ReadByte();
                else
                    reader.ReadBoolean();
            }

            // Read playtime bytes
            if (fileVersion >= 138)
                reader.ReadInt64();

            // Read hair bytes
            reader.ReadInt32();
            if (fileVersion >= 82)
                reader.ReadByte(); // Read past hair dye bytes

            // Read visible accessory bytes
            if (fileVersion >= 124)
            {
                reader.ReadByte();
                reader.ReadByte();
            }
            else if (fileVersion >= 83)
                reader.ReadByte();

            // Read hideMisc bytes
            if (fileVersion >= 119)
                reader.ReadByte();

            if (fileVersion <= 17)
            {
                // This empty if clause is required for vanilla parity
            }
            else if (fileVersion < 107)
                reader.ReadBoolean(); // Read gender bytes
            else
                reader.ReadByte(); // Read skin variant bytes

            // Read max life bytes
            reader.ReadInt32();
            reader.ReadInt32();

            // Read max mana bytes
            reader.ReadInt32();
            reader.ReadInt32();

            // Read Demon Heart bytes
            if (fileVersion >= 125)
                reader.ReadBoolean();

            // Read Torch God bytes
            if (fileVersion >= 229)
            {
                reader.ReadBoolean();
                reader.ReadBoolean();
            }

            // Read downedDD2EventAnyDifficulty bytes
            if (fileVersion >= 182)
                reader.ReadBoolean();

            // Read tax money bytes
            if (fileVersion >= 128)
                reader.ReadInt32();

            // Read Hair Colour, Skin Colour, Eye Colour, Shirt Colour, Under Shirt Colour, Pants Colour, and Shoe Colour bytes
            reader.ReadRGB();
            reader.ReadRGB();
            reader.ReadRGB();
            reader.ReadRGB();
            reader.ReadRGB();
            reader.ReadRGB();
            reader.ReadRGB();

            // Inventory reading, whoo
            if (fileVersion >= 38)
            {
                // Unsure what these readings are, accessories/armour?
                if (fileVersion < 124)
                {
                    int armorArrayLengthMaybe = 11;
                    if (fileVersion >= 81)
                        armorArrayLengthMaybe = 16;

                    for (int i = 0; i < armorArrayLengthMaybe; i++)
                    {
                        reader.ReadInt32();
                        reader.ReadByte();
                    }
                }
                // Unsure what these readings are, accessories/armour?
                else
                {
                    int newArmorArrayLengthMaybe = 20;
                    for (int i = 0; i < newArmorArrayLengthMaybe; i++)
                    {
                        reader.ReadInt32();
                        reader.ReadByte();
                    }
                }

                // Read dye array bytes
                if (fileVersion >= 47)
                {
                    int dyeArrayLength = 3;
                    if (fileVersion >= 81)
                        dyeArrayLength = 8;

                    if (fileVersion >= 124)
                        dyeArrayLength = 10;

                    for (int i = 0; i < dyeArrayLength; i++)
                    {
                        reader.ReadInt32();
                        reader.ReadByte();
                    }
                }

                // Read inventory items
                if (fileVersion >= 58)
                {
                    for (int i = 0; i < 58; i++)
                    {
                        int itemId = reader.ReadInt32();
                        if (itemId >= 5125)
                        {
                            reader.ReadInt32();
                            reader.ReadByte();
                            if (fileVersion >= 114)
                                reader.ReadBoolean();
                        }
                        else
                        {
                            reader.ReadInt32(); // NetId
                            reader.ReadByte(); // Prefix
                            if (fileVersion >= 114)
                                reader.ReadBoolean(); // Favourited
                        }
                    }
                }
                // Older inventory reading?
                else
                {
                    for (int i = 0; i < 48; i++)
                    {
                        int num10 = reader.ReadInt32();
                        if (num10 >= 5125)
                        {
                            reader.ReadInt32();
                            reader.ReadByte();
                        }
                        else
                        {
                            reader.ReadInt32();
                            reader.ReadByte();
                        }
                    }
                }

                // Equipment screen reading
                if (fileVersion >= 117)
                {
                    // Misc equip reading
                    if (fileVersion < 136)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (i != 1)
                            {
                                reader.ReadInt32(); // Item Id
                                reader.ReadByte(); // Prefix

                                reader.ReadInt32();
                                reader.ReadByte();
                            }
                        }
                    }
                    // Old misc equip reading?
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            reader.ReadInt32();
                            reader.ReadByte();

                            reader.ReadInt32();
                            reader.ReadByte();
                        }
                    }
                }

                // Read piggy bank and safe
                if (fileVersion >= 58)
                {
                    // Piggy bank
                    for (int i = 0; i < 40; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefix
                    }

                    // Safe
                    for (int i = 0; i < 40; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefix
                    }
                }
                // Read piggy bank and safe but differnt
                else
                {
                    // Piggy bank
                    for (int i = 0; i < 20; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefix
                    }

                    // Safe
                    for (int i = 0; i < 20; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefix
                    }
                }

                // Defenders forge reading
                if (fileVersion >= 182)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefi
                    }
                }

                // Void vault reading
                if (fileVersion >= 198)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        reader.ReadInt32(); // Type
                        reader.ReadInt32(); // Stack
                        reader.ReadByte(); // Prefix
                    }
                }

                // voidVaultInfo reading
                if (fileVersion >= 199)
                    reader.ReadByte();
            }
            else
            {
                // Old armour/accessory reading
                for (int i = 0; i < 8; i++)
                {
                    reader.ReadString(); // Legay item name
                    if (fileVersion >= 36)
                        reader.ReadByte(); // Prefix
                }

                if (fileVersion >= 6)
                {
                    // Old something-in-the-armour-array-from-[8, 11] (vanity armour slots?)
                    for (int i = 8; i < 11; i++)
                    {
                        reader.ReadString(); // Legacy item name
                        if (fileVersion >= 36)
                            reader.ReadByte(); // Prefix
                    }
                }

                // Old inventory loading
                for (int i = 0; i < 44; i++)
                {
                    reader.ReadString(); // Legacy name
                    reader.ReadInt32(); // Stack
                    if (fileVersion >= 36)
                        reader.ReadByte(); // Prefix
                }

                // Inventory indices 44 to 48, ammo or coins?
                if (fileVersion >= 15)
                {
                    for (int i = 44; i < 48; i++)
                    {
                        reader.ReadString(); // Legacy name
                        reader.ReadInt32(); // Stack
                        if (fileVersion >= 36)
                            reader.ReadByte(); // Prefix
                    }
                }

                // Old piggy bank loading
                for (int i = 0; i < 20; i++)
                {
                    reader.ReadString(); // Legacy name
                    reader.ReadInt32(); // Stack
                    if (fileVersion >= 36)
                        reader.ReadByte(); // Prefix
                }

                // Old safe loading
                if (fileVersion >= 20)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        reader.ReadString(); // Legacy name
                        reader.ReadInt32(); // Stack
                        if (fileVersion >= 36)
                            reader.ReadByte(); // Prefix
                    }
                }
            }

            // Load buff time
            if (fileVersion >= 11)
            {
                int buffLimit = 22;
                if (fileVersion < 74)
                    buffLimit = 10;

                for (int i = 0; i < buffLimit; i++)
                {
                    reader.ReadInt32(); // Buff type
                    reader.ReadInt32(); // Buff time
                }
            }

            // Load world spawn points
            for (int i = 0; i < 200; i++)
            {
                int x = reader.ReadInt32(); // spawnX
                if (x == -1)
                    break;

                reader.ReadInt32(); // spawnY
                reader.ReadInt32(); // spawnI, I believe a discriminator between worlds with the same names?
                reader.ReadString(); // World spawn names
            }

            // hbLocked, whatever that is
            if (fileVersion >= 16)
                reader.ReadBoolean();

            // hideInfoMax, whatever that is
            if (fileVersion >= 115)
            {
                int hideInfoMax = 13;
                for (int i = 0; i < hideInfoMax; i++)
                {
                    reader.ReadBoolean();
                }
            }

            // Finished angler quests
            if (fileVersion >= 98)
                reader.ReadInt32();

            // D-Pad radial bindings
            if (fileVersion >= 162)
            {
                for (int i = 0; i < 4; i++)
                {
                    reader.ReadInt32();
                }
            }

            // Builder accessory toggles
            if (fileVersion >= 164)
            {
                int builderAcessoryOptionMax = 8;
                if (fileVersion >= 167)
                    builderAcessoryOptionMax = 10;

                if (fileVersion >= 197)
                    builderAcessoryOptionMax = 11;

                if (fileVersion >= 230)
                    builderAcessoryOptionMax = 12;

                for (int i = 0; i < builderAcessoryOptionMax; i++)
                {
                    reader.ReadInt32();
                }

                // Note: the option is set to 1 if non is loaded for it
            }

            // Bartender [Tavernkeep] quest log...?
            if (fileVersion >= 181)
                reader.ReadInt32();

            // You ded?
            if (fileVersion >= 200)
            {
                bool ded = reader.ReadBoolean(); // ded

                if (ded)
                    reader.ReadInt32(); // Respawn timer
            }

            // Last time the player was saved
            if (fileVersion >= 202)
                reader.ReadInt64(); // Read last time they were saved. Set to UtcNow.ToBinary() if not saved

            // Golf score
            if (fileVersion >= 206)
                reader.ReadInt32();

            // Creative tracker load
            if (fileVersion >= 218)
                ret.CreativeSacs = LoadCreativeSacrifices(reader);

            // Temporary item slot contents...?
            if (fileVersion >= 214)
                reader.ReadByte();

            // Not loading creative powers because it needs me to recreate the loading code for each which is no
            //if (fileVersion >= 220)
            //    LoadCreativePowers(reader, fileVersion);

            return ret;
        }

        public static List<JourneyItemResearchInfo> LoadCreativeSacrifices(BinaryReader reader)
        {
            List<JourneyItemResearchInfo> ret = new();
            int num = reader.ReadInt32();
            
            for (int i = 0; i < num; i++)
            {
                string key = reader.ReadString();
                int amount = reader.ReadInt32();
                ret.Add(new(key, amount));
            }

            return ret;
        }

        public static void LoadCreativePowers(BinaryReader reader, int fileVersion)
        {
            while (reader.ReadBoolean())
            {
                reader.ReadUInt16();
            }
        }
    }
}
