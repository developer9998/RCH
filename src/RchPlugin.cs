using BepInEx;
using Bepinject;
using System;
using System.IO;
using WebSocketSharp;

namespace RCH
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class RchPlugin : BaseUnityPlugin
    {
        public static string IndexPath;
        internal void Start()
        {
            try { Zenjector.Install<CI.MainInstaller>().OnProject(); }
            catch { Console.WriteLine("RchView not installed"); }

            string HeaderPath = Path.Combine(Path.GetDirectoryName(typeof(RchPlugin).Assembly.Location), "CustomHeaders.txt");
            if (File.Exists(HeaderPath)) Manager.CustomTexts = File.ReadAllLines(HeaderPath);
            else File.WriteAllLines(HeaderPath, Manager.CustomTexts);

            IndexPath = Path.Combine(Path.GetDirectoryName(typeof(RchPlugin).Assembly.Location), "CustomIndex.txt");
            if (File.Exists(IndexPath))
            {
                if (File.ReadAllLines(IndexPath)[0].IsNullOrEmpty()) File.WriteAllText(IndexPath, 0.ToString()); // Index is set to 0 at default, so no need to change it here if there's nothing to change it to.
                else try { Manager.Index = int.Parse(File.ReadAllLines(IndexPath)[0]); } catch { File.WriteAllText(IndexPath, 0.ToString()); };
            }
            else File.WriteAllText(IndexPath, 0.ToString());

            Console.WriteLine($"\nRCH loaded headers:\n{File.ReadAllText(HeaderPath)}");

            HarmonyPatches.ApplyHarmonyPatches();
            Manager.Enabled = true;
        }
    }
}
