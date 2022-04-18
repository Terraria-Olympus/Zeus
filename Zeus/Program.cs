using System;
using System.IO;
using Zeus.DataStructures;

namespace Zeus
{
    public static class Program
    {
        public static int LastChecksum = 0;

        public static string PlayerFilePath;
        public static PlayerFileData PlayerFileData;

        public static void Main()
        {
            Console.WriteLine("Type name of player you'd like to track. Hit enter when done: ");
                
            FindingPlayer:
            {
                string playerName = Console.ReadLine();
                string filePath = Path.Combine(TerrariaThings.GetSavePath(), "Players", playerName + ".plr");

                if (!filePath.Contains(playerName))
                {
                    Console.WriteLine("Player not found. Input a new name.");
                    goto FindingPlayer;
                }

                PlayerFilePath = filePath;
                goto FileTrackingLoop;
            }

            FileTrackingLoop:
            {
                int checksum = File.ReadAllBytes(PlayerFilePath).GetHashCode();

                if (checksum != LastChecksum)
                {
                    LastChecksum = checksum;
                    UpdateFromFile();

                    foreach (JourneyItemResearchInfo info in PlayerFileData.CreativeSacs)
                    {
                        Console.WriteLine(info.Key + ": " + info.Amount);
                    }
                }

                goto FileTrackingLoop;
            }
        }

        public static void UpdateFromFile()
        {
            PlayerFileData = PlayerFileData.Load(PlayerFilePath);
        }
    }
}
