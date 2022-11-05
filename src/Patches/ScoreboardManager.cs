using System.Linq;
using UnityEngine;

namespace RCH.Patches
{
    public class ScoreboardBeginningManager : MonoBehaviour
    {
        internal ScoreboardBeginningButton[] buttonArray = new ScoreboardBeginningButton[2]; // The array of buttons that are used to swap the scoreboard headers, we only need two though.

        internal void Start()
        {
            CreateButton(0, new Vector3(-71.7f, 61.5f, 1.8f), new Vector3(103.3446f, 12.97541f, 5.282512f));
            CreateButton(1, new Vector3(33.8f, 61.5f, 1.8f), new Vector3(103.3446f, 12.97541f, 5.282512f));
        }

        /// <summary>
        /// Creates a button that will be used to interact with the scoreboard.
        /// </summary>
        /// <param name="buttonIndex">The index the button will be in the array.</param>
        /// <param name="localPosition">The localPosition the button will be at.</param>
        /// <param name="localScale">The localScale the button will be at.</param>
        internal void CreateButton(int buttonIndex, Vector3 localPosition, Vector3 localScale)
        {
            GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.transform.SetParent(transform, false);
            button.layer = 18;

            button.GetComponent<MeshRenderer>().forceRenderingOff = true;
            button.GetComponent<BoxCollider>().isTrigger = true;

            ScoreboardBeginningButton buttonComponennt = button.AddComponent<ScoreboardBeginningButton>();

            buttonComponennt.scoreboardBeginningManager = this;
            buttonArray[buttonIndex] = buttonComponennt;

            button.transform.localPosition = localPosition;
            button.transform.localEulerAngles = Vector3.zero;
            button.transform.localScale = localScale;
        }

        /// <summary>
        /// When one of the scoreboard buttons used to switch the text is pressed.
        /// </summary>
        /// <param name="button">The button that was pressed.</param>
        public void ButtonPress(ScoreboardBeginningButton button)
        {
            if (!buttonArray.Contains(button)) return;

            if (button == buttonArray[0])
            {
                Manager.Index--;
                Manager.ForceUpdate();
                Manager.UpdateView();
                return;
            }

            Manager.Index++;
            Manager.ForceUpdate();
            Manager.UpdateView();
        }
    }

    public class ScoreboardBeginningButton : MonoBehaviour
    {
        internal ScoreboardBeginningManager scoreboardBeginningManager;
        internal float debounceTime = 0.15f;
        internal float touchTime;

        internal void OnTriggerEnter(Collider collider)
        {
            if (!(touchTime + debounceTime < Time.time)) return;

            touchTime = Time.time;
            if (!(collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)) return;

            if (collider.GetComponent<GorillaTriggerColliderHandIndicator>() != null)
            {
                ButtonActivation();
                GorillaTagger.Instance.StartVibration(collider.GetComponent<GorillaTriggerColliderHandIndicator>().isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
            }
        }

        /// <summary>
        /// When the button was successfully pressed.
        /// </summary>
        internal void ButtonActivation() => scoreboardBeginningManager.ButtonPress(this);
    }
}
