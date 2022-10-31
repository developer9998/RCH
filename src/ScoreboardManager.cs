using HarmonyLib;
using System;
using UnityEngine;

namespace RCH
{
    public class ScoreboardBeginningManager : MonoBehaviour
    {
        internal ScoreboardBeginningButton[] buttonArray = new ScoreboardBeginningButton[2]; // The array of buttons that are used to swap the scoreboard headers, we only need two though.

        internal void Awake()
        {
            CreateButton(0, new Vector3(-71.7f, 61.5f, 1.8f), new Vector3(103.3446f, 12.97541f, 5.282512f)); // Creates a button with some data (The index, local position, and local scale)
            CreateButton(1, new Vector3(33.8f, 61.5f, 1.8f), new Vector3(103.3446f, 12.97541f, 5.282512f));
        }

        internal void CreateButton(int buttonIndex, Vector3 localPosition, Vector3 localScale)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.transform.SetParent(transform, false);
            button.transform.localPosition = localPosition;
            button.transform.localEulerAngles = Vector3.zero;
            button.transform.localScale = localScale;
            button.layer = 18;

            button.GetComponent<MeshRenderer>().forceRenderingOff = true;
            button.GetComponent<BoxCollider>().isTrigger = true;

            ScoreboardBeginningButton buttonComponennt = button.AddComponent<ScoreboardBeginningButton>();

            buttonComponennt.scoreboardBeginningManager = this;
            buttonArray[buttonIndex] = buttonComponennt;
        }

        public void ButtonPress(ScoreboardBeginningButton button)
        {
            if (button == buttonArray[0])
            {
                Manager.Index--;
                Manager.ForceUpdate();
                return;
            }

            if (button == buttonArray[1])
            {
                Manager.Index++;
                Manager.ForceUpdate();
                return;
            }

            Console.WriteLine("Button component is not in array");
            Destroy(button); // Destroys the button component if it's not in the array, it's useless and we don't need it. 
        }
    }

    public class ScoreboardBeginningButton : GorillaPressableButton
    {
        internal ScoreboardBeginningManager scoreboardBeginningManager;

        internal void Awake() { debounceTime = 0.05f; }

        public override void ButtonActivation() { base.ButtonActivation(); try { scoreboardBeginningManager.ButtonPress(this); } catch { Console.WriteLine("ScoreboardBeginningManager null"); Destroy(this); } }
    }
}
