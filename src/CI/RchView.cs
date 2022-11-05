using System;
using System.Collections.Generic;
using System.Text;
using ComputerInterface;
using ComputerInterface.ViewLib;
using RCH.Patches;
using UnityEngine;

namespace RCH.CI
{
    internal class RchView : ComputerView
    {
        public static RchView Instance;

        /// <summary>
        /// Highlights the dynamic text.
        /// </summary>
        /// <param name="text">The text that is being highlighted.</param>
        /// <returns></returns>
        internal string HighlightDynamic(string text)
        {
            foreach(string key in Manager.DynamicDict.Keys)
            {
                string highlightedKey;
                highlightedKey = key.Replace("{", "<color=#F2BB05>{<color=#F9E900>");
                highlightedKey = highlightedKey.Replace("}", "</color>}</color>");

                text = text.Replace(key, highlightedKey);
            }

            return text;
        }

        /// <summary>
        /// When the Computer Interface view is shown.
        /// </summary>
        /// <param name="args">The arguments used in this method.</param>
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            Instance = this;

            DrawScreen();
        }

        /// <summary>
        /// Updates the Computer Interface screen.
        /// </summary>
        public void DrawScreen()
        {
            SetText(str =>
            {
                str.BeginCenter();
                str.MakeBar('-', SCREEN_WIDTH, 0, "FFFFFF10").AppendLine();
                str.AppendClr("Room Code Hider", "FF0066").AppendLine();
                str.Append("By <color=#38FF8D>Frogrilla</color>").AppendLine();
                str.MakeBar('-', SCREEN_WIDTH, 0, "FFFFFF10").AppendLines(2).EndAlign();
                str.Append($"Current Header:\n{HighlightDynamic(Manager.CustomTexts[Manager.Index])}").AppendLine();
            });
        }

        /// <summary>
        /// When a key is pressed on the keyboard in this menu.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        public override void OnKeyPressed(EKeyboardKey key)
        {
            switch (key)
            {
                case EKeyboardKey.Right:
                    Manager.Index++;
                    Manager.ForceUpdate();
                    break;
                case EKeyboardKey.Left:
                    Manager.Index--;
                    Manager.ForceUpdate();
                    break;
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
            }
            DrawScreen();
        }
    }
}
