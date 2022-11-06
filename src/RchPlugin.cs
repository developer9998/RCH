using BepInEx;
using Bepinject;
using HarmonyLib;
using Photon.Pun;
using RCH.Patches;
using System;
using System.IO;

namespace RCH
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class RchPlugin : BaseUnityPlugin
    {
        internal void Start()
        {
            try { Zenjector.Install<CI.MainInstaller>().OnProject(); }
            catch { Console.WriteLine("RchView not installed"); }

            string HeaderPath = Path.Combine(Path.GetDirectoryName(typeof(RchPlugin).Assembly.Location), "RCH_Headers.txt");
            if (File.Exists(HeaderPath)) Manager.CustomTexts = File.ReadAllLines(HeaderPath);
            else File.WriteAllLines(HeaderPath, Manager.CustomTexts);

            Console.WriteLine($"\nRCH loaded headers:\n{File.ReadAllText(HeaderPath)}");

            string IndexPath = Path.Combine(Path.GetDirectoryName(typeof(RchPlugin).Assembly.Location), "RCH_Options.txt");
            if (File.Exists(IndexPath)) { if (File.ReadAllLines(IndexPath).Length == 0 || File.ReadAllLines(IndexPath).Length == 1) ResetSettings(); }
            else { File.WriteAllText(IndexPath, $"{Manager.Index}\n{Manager.Enabled}"); }

            Console.WriteLine($"\nRCH loaded current index:\n{Manager.Index}");

            Manager.Index = int.Parse(File.ReadAllLines(IndexPath)[0]);
            Manager.Enabled = bool.Parse(File.ReadAllLines(IndexPath)[1]);
        }

        /// <summary>
        /// Sets the settings to their default value.
        /// </summary>
        public static void ResetSettings() => File.WriteAllText(Path.Combine(Path.GetDirectoryName(typeof(RchPlugin).Assembly.Location), "RCH_Headers.txt"), $"{Manager.Index}\n{Manager.Enabled}");

        [HarmonyPatch(typeof(GorillaScoreBoard))]
        [HarmonyPatch("Awake", MethodType.Normal)]
        internal class ScoreboardAwakePatch
        {
            /// <summary>
            /// When the scoreboard starts up.
            /// </summary>
            /// <param name="__instance">The scoreboard.</param>
            internal static void Prefix(GorillaScoreBoard __instance)
            {
                if (!Manager.boards.Contains(__instance)) { Manager.boards.Add(__instance); }
                if (!__instance.gameObject.GetComponent<ScoreboardBeginningManager>()) { __instance.gameObject.AddComponent<ScoreboardBeginningManager>(); }
            }
        }

        [HarmonyPatch(typeof(GorillaScoreBoard))]
        [HarmonyPatch("GetBeginningString", MethodType.Normal)]
        internal class ScoreboardGBSPatch
        {
            /// <summary>
            /// When the scoreboard needs to get the beginning string. (The text on the top)
            /// </summary>
            /// <param name="__result"></param>
            /// <returns>The scoreboard.</returns>
            internal static bool Prefix(ref string __result)
            {
                Manager.UpdateDict();
                __result = Manager.GenDynamicText(Manager.CustomTexts[Manager.Index]) + "\n   PLAYER      COLOR   MUTE   REPORT";

                return !Manager.Enabled;
            }
        }

        [HarmonyPatch(typeof(PhotonNetwork))]
        [HarmonyPatch("Disconnect", MethodType.Normal)]
        internal class OnDisconnectPatch
        {
            /// <summary>
            /// When this player leaves a room.
            /// </summary>
            internal static void Prefix()
            {
                if (PhotonNetwork.InRoom)
                    Manager.boards.Clear();
            }
        }
    }
}
