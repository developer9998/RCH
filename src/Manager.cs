using System;
using System.Collections.Generic;
using HarmonyLib;
using RCH.CI;
using Photon.Pun;
using Photon.Realtime;

namespace RCH
{
    [HarmonyPatch(typeof(GorillaScoreBoard))]
    [HarmonyPatch("GetBeginningString", MethodType.Normal)]
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
            { "{caught}",   "out" },
            { "{blue}",     "out" },
            { "{red}",      "out" },
            { "{pubname}",  "out" }
        };

        internal static string[] CustomTexts = new string[]
        {
            "ROOM ID: {pubname} GAME ROOM: {mode}", // The current text used for the base game.
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
                System.IO.File.WriteAllText(RchPlugin.IndexPath, value.ToString());
            }
        }

        private static bool enabled = true;
        internal static bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
            }
        }

        internal static string GenDynamicText(string text)
        {
            foreach(string key in DynamicDict.Keys) text = text.Replace(key, DynamicDict[key]);

            if (text.Length > 45)
                return text.Substring(0, 45); // Cuts down the length of the text if it's too long.

            return text;
        }

        internal static void UpdateDict()
        {
            DynamicDict["{name}"] = PhotonNetwork.CurrentRoom.Name;
            DynamicDict["{region}"] = PhotonNetwork.CloudRegion.Replace("/*", "").ToUpper();
            DynamicDict["{mode}"] = GorillaGameManager.instance.GameMode();
            DynamicDict["{public}"] = PhotonNetwork.CurrentRoom.IsVisible ? "PUBLIC" : "PRIVATE";
            DynamicDict["{count}"] = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            DynamicDict["{max}"] = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            DynamicDict["{ping}"] = PhotonNetwork.GetPing().ToString();
            DynamicDict["{caught}"] = GetTaggedPlayers().ToString();
            DynamicDict["{blue}"] = GetPlayersOfTeam(false).ToString();
            DynamicDict["{red}"] = GetPlayersOfTeam(true).ToString();
            DynamicDict["{pubname}"] = PhotonNetwork.CurrentRoom.IsVisible ? PhotonNetwork.CurrentRoom.Name : "-PRIVATE-";
        }

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

        internal static void ForceUpdate()
        {
            if (boards.Count != 0)
            {
                Console.WriteLine("Forcing scoreboard updates");
                foreach (GorillaScoreBoard board in boards)
                {
                    board.RedrawPlayerLines();
                    Console.WriteLine($"{board.name} updated");
                }
            }
        }


        internal static int GetTaggedPlayers()
        {
            int teamPlayers = 0;
            int[] infectedArray = new int[5] { 1, 2, 3, 7, 11 };

            if (GorillaGameManager.instance != null)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (infectedArray.Contains(rig.setMatIndex))
                        teamPlayers++;
                }
            }
            // Checks all the VRRig's setMatIndex variable to see if they're on an array of "infected" integers

            return teamPlayers;
        }

        internal static int GetPlayersOfTeam(bool isRedTeam)
        {
            int teamPlayers = 0;
            int[] blueArray = new int[3] { 4, 5, 6 };
            int[] redArray = new int[3] { 8, 9, 10 };

            if (GorillaGameManager.instance != null)
            {
                if (GorillaGameManager.instance.GetComponent<GorillaBattleManager>() != null)
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (blueArray.Contains(rig.setMatIndex) && !isRedTeam)
                            teamPlayers++;
                        if (redArray.Contains(rig.setMatIndex) && isRedTeam)
                            teamPlayers++;
                    }
                }
            }
            // Checks all the VRRig's setMatIndex variable to see if they're on an array of "infected" integers

            return teamPlayers;
        }

        private static bool Prefix(ref string __result)
        {
            UpdateDict();
            __result = GenDynamicText(CustomTexts[Index]) + "\n   PLAYER      COLOR   MUTE   REPORT";

            return !Enabled;
        }
    }

    [HarmonyPatch(typeof(GorillaScoreBoard))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    public class ScoreboardAwakePatch
    {
        private static void Prefix(GorillaScoreBoard __instance)
        {
            if (!Manager.boards.Contains(__instance)) { Manager.boards.Add(__instance); }
            if (!__instance.gameObject.GetComponent<ScoreboardBeginningManager>()) { __instance.gameObject.AddComponent<ScoreboardBeginningManager>(); }
        }
    }

    [HarmonyPatch(typeof(PhotonNetwork))]
    [HarmonyPatch("Disconnect", MethodType.Normal)]
    public class OnRoomDisconnected
    {
        private static void Prefix()
        {
            if (!PhotonNetwork.InRoom) return;
            Manager.boards.Clear();
        }
    }
}
