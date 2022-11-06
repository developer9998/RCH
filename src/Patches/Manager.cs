using Photon.Pun;
using RCH.CI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RCH.Patches
{
    public static class Manager
    {
        public static List<GorillaScoreBoard> boards = new List<GorillaScoreBoard>();
        internal static Dictionary<string, string> DynamicDict = new Dictionary<string, string>()
        {
            { "{name}",     "out" },
            { "{region}",   "out" },
            { "{mode}",     "out" },
            { "{public}",   "out" },
            { "{count}",    "out" },
            { "{max}",      "out" },
            { "{ping}",     "out" },
            { "{pubname}",  "out" },
        };

        public static int caught = 0;

        internal static string[] CustomTexts = new string[]
        {
            "{count}/{max}    {mode} ROOM ID: {pubname}", // Displays the player count with the limit as well as some room info.
            "{count}/{max}    {public} {mode}, {name}", // Same as the previous one but it shows any room code you're in and not just the public lobby codes.
            "{count}/{max}    -ROOM HIDDEN-",  // Same as the previous one but without the room info.
            "ROOM ID: {name}", // Displays just the room code even in a private lobby.
            "-ROOM HIDDEN-" // Doesn't display any data.
        };

        private static int index = 0;
        internal static int Index
        {
            get => index;
            set
            {
                if (value < 0) index = CustomTexts.Length - 1;
                else index = value % CustomTexts.Length;
                RchPlugin.ResetSettings();
            }
        }

        private static bool enabled = true;
        internal static bool Enabled
        {
            get => enabled;
            set
            {
                if (value) HarmonyPatches.ApplyHarmonyPatches();
                else HarmonyPatches.RemoveHarmonyPatches();

                enabled = value;
                RchPlugin.ResetSettings();
                if (PhotonNetwork.InRoom) ForceUpdate();
            }
        }

        /// <summary>
        /// Returns the text that will be used at the top of the scoreboard.
        /// </summary>
        /// <param name="text">The text that is used.</param>
        /// <returns></returns>
        internal static string GenDynamicText(string text)
        {
            foreach (string key in DynamicDict.Keys) text = text.Replace(key, DynamicDict[key]);

            if (text.Length > 45)
                return text.Substring(0, 45); // Cuts down the length of the text if it's too long.

            return text;
        }

        /// <summary>
        /// Updates the Dictionary that the text uses for the scoreboard.
        /// </summary>
        internal static void UpdateDict()
        {
            DynamicDict["{name}"] = PhotonNetwork.CurrentRoom.Name;
            DynamicDict["{region}"] = PhotonNetwork.CloudRegion.Replace("/*", "").ToUpper();
            DynamicDict["{mode}"] = GorillaGameManager.instance.GameMode();
            DynamicDict["{public}"] = PhotonNetwork.CurrentRoom.IsVisible ? "PUBLIC" : "PRIVATE";
            DynamicDict["{count}"] = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            DynamicDict["{max}"] = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            DynamicDict["{ping}"] = PhotonNetwork.GetPing().ToString();
            DynamicDict["{pubname}"] = PhotonNetwork.CurrentRoom.IsVisible ? PhotonNetwork.CurrentRoom.Name : "-PRIVATE-";
        }

        /// <summary>
        /// Updates the Computer Interface view.
        /// </summary>
        public static void UpdateView()
        {
            if (RchView.Instance != null)
            {
                RchView.Instance.DrawScreen();
                Console.WriteLine("Updated CI screen");
                return;
            }

            Console.WriteLine("Failed to update CI screen (probably not on RCH screen)");
        }

        /// <summary>
        /// Forces the scoreboards to update.
        /// </summary>
        internal static void ForceUpdate()
        {
            // Prevents the player from updating the scoreboards if they're not in a room.
            if (!PhotonNetwork.InRoom) return;

            Console.WriteLine("Forcing scoreboard updates");
            if (boards.Count != 0)
            {
                foreach (GorillaScoreBoard board in boards)
                {
                    if (board != null)
                    {
                        board.RedrawPlayerLines();
                        Console.WriteLine($"{board.name} updated");
                    }
                    else
                    {
                        boards.Remove(board);
                    }
                }
                return;
            }
            else
            {
                if (Resources.FindObjectsOfTypeAll<GorillaScoreBoard>().Length != 0)
                {
                    foreach (GorillaScoreBoard board in Resources.FindObjectsOfTypeAll<GorillaScoreBoard>())
                    {
                        if (board != null)
                        {
                            board.RedrawPlayerLines();
                            Console.WriteLine($"{board.name} updated");
                        }
                    }
                    return;
                }
            }
            Console.WriteLine("Failed to update scoreboards");
        }
    }
}
