using HarmonyLib;
using Photon.Pun;
using System;
using UnityEngine;

namespace RCH
{
    public class ScoreboardBeginningManager : MonoBehaviour
    {
        internal ScoreboardBeginningButton[] buttonArray = new ScoreboardBeginningButton[2]; // The array of buttons that are used to swap the scoreboard headers, we only need two though.

        internal void Start()
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
                Manager.UpdateView();
                return;
            }

            if (button == buttonArray[1])
            {
                Manager.Index++;
                Manager.ForceUpdate();
                Manager.UpdateView();
                return;
            }

            Console.WriteLine("Button component is not in array");
        }
    }

    public class ScoreboardBeginningButton : MonoBehaviour
    {
        internal ScoreboardBeginningManager scoreboardBeginningManager;
        internal float debounceTime = 0.25f;
        internal float touchTime;

        internal void OnTriggerEnter(Collider collider)
        {
            if (!(touchTime + debounceTime < Time.time)) return;

            touchTime = Time.time;
            Debug.Log("collision detected" + collider, collider);
            if (!(collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)) return;

            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();

            if (component != null)
            {
                ButtonActivation();
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
                GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
            }
        }

        internal void ButtonActivation()
        {
            scoreboardBeginningManager.ButtonPress(this);
        }
    }
}
