using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Photon.Pun;
using RCH.CI;

namespace RCH
{
    [HarmonyPatch(typeof(GorillaScoreBoard))]
    [HarmonyPatch("GetBeginningString")]
    internal static class Manager
    {
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
            "{count}/{max}    {public} {mode}, {pubname}", // Displays the player count with the limit as well as some room info.
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
                if (value) HarmonyPatches.ApplyHarmonyPatches();
                else HarmonyPatches.RemoveHarmonyPatches();

                enabled = value;
                if (PhotonNetwork.InRoom) ForceUpdate();
            }
        }

        internal static string GenDynamicText(string text)
        {
            foreach(string key in DynamicDict.Keys) text = text.Replace(key, DynamicDict[key]);

            if (text.Length > 37)
                return text.Substring(0, 37); // Cuts down the length of the text if it's too long.

            return text;
        }

        internal static void UpdateDict()
        {
            DynamicDict["{name}"]   = PhotonNetwork.CurrentRoom.Name;
            DynamicDict["{region}"] = PhotonNetwork.CloudRegion.Replace("/*","").ToUpper();
            DynamicDict["{mode}"]   = GorillaGameManager.instance.GameMode();
            DynamicDict["{public}"] = PhotonNetwork.CurrentRoom.IsVisible ? "PUBLIC" : "PRIVATE";
            DynamicDict["{count}"]  = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            DynamicDict["{max}"]    = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            DynamicDict["{ping}"]   = PhotonNetwork.GetPing().ToString();
            DynamicDict["{caught}"] = GetTaggedPlayers().ToString();
            DynamicDict["{blue}"] = GetPlayersOfTeam(false).ToString();
            DynamicDict["{red}"] = GetPlayersOfTeam(true).ToString();
            DynamicDict["{pubname}"] = PhotonNetwork.CurrentRoom.IsVisible ? PhotonNetwork.CurrentRoom.Name : "-PRIVATE-";
        }

        internal static void UpdateView()
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
            Console.WriteLine("Forcing scoreboard updates");
            if (PhotonNetwork.InRoom)
            {
                foreach (GorillaScoreBoard board in GorillaUIParent.instance.GetComponentsInChildren<GorillaScoreBoard>())
                {
                    board.RedrawPlayerLines();
                    Console.WriteLine($"{board.name} updated");
                }
            }
            else
            {
                Console.WriteLine($"Not in room, cannot update");
            }
        }

        internal static int GetTaggedPlayers()
        {
            int taggedPlayers = 0;

            if (GorillaGameManager.instance != null)
            {
                if (GorillaGameManager.instance.GetComponent<GorillaTagManager>() != null) { foreach (int infectedArray in GorillaGameManager.instance.GetComponent<GorillaTagManager>().currentInfectedArray) if (infectedArray != 0) taggedPlayers++; }
                else if (GorillaGameManager.instance.GetComponent<GorillaHuntManager>() != null) { foreach (int huntedArray in GorillaGameManager.instance.GetComponent<GorillaHuntManager>().currentHuntedArray) if (huntedArray != 0) taggedPlayers++; }
                else if (GorillaGameManager.instance.GetComponent<GorillaBattleManager>() != null) { foreach (int playerLivesArray in GorillaGameManager.instance.GetComponent<GorillaBattleManager>().playerLivesArray) if (playerLivesArray == 0) taggedPlayers++; }
            }

            return taggedPlayers;
        }

        internal static bool HasFlag(GorillaBattleManager.BattleStatus state, GorillaBattleManager.BattleStatus statusFlag)
        {
            return (state & statusFlag) > GorillaBattleManager.BattleStatus.None;
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

            return teamPlayers;
        }


        internal static bool Prefix(ref string __result)
        {
            UpdateDict();
            __result = GenDynamicText(CustomTexts[Index]) + "\n   PLAYER      COLOR   MUTE   REPORT";
            return !Enabled;
        }
    }
}
