using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KEditorExtensions
{
    /// <summary>
    /// Handle GUI color changes like EditorGUILayout's Horizontal and Vertical scopes!
    /// </summary>
    public static class ColorScope
    {
        //internal stack of prior colors to fall back to
        private static Stack<System.Tuple<Color, Color, Color>> OldColorStack = new Stack<System.Tuple<Color, Color, Color>>();

        /// <summary>
        /// Begin a new color region.
        /// </summary>
        /// <param name="color">GUI.color value for the region.</param>
        /// <param name="contentColor">GUI.contentColor value for the region.</param>
        /// <param name="backgroundColor">GUI.backgroundColor value for the region.</param>
        public static void Begin(Color color, Color contentColor, Color backgroundColor)
        {
            OldColorStack.Push(new System.Tuple<Color, Color, Color>(GUI.color, GUI.contentColor, GUI.backgroundColor));
            SetGUIColor(color, contentColor, backgroundColor);
        }
        /// <summary>
        /// Begin a new color region.
        /// </summary>
        /// <param name="color">GUI.color value for the region.</param>
        public static void Begin(Color color)
        {
            Begin(color, GUI.contentColor, GUI.backgroundColor);
        }
        /// <summary>
        /// Begin a new color region. This overload adjusts only alpha values (between 0 and 1).
        /// </summary>
        /// <param name="colorAlpha">GUI.color alpha value for the region.</param>
        /// <param name="contentColor">GUI.contentColor alpha value for the region.</param>
        /// <param name="backgroundColor">GUI.backgroundColor alpha value for the region.</param>
        public static void Begin(float colorAlpha, float contentColorAlpha, float backgroundColorAlpha)
        {
            Begin(
                AdjustAlpha(GUI.color, colorAlpha),
                AdjustAlpha(GUI.contentColor, contentColorAlpha),
                AdjustAlpha(GUI.backgroundColor, backgroundColorAlpha)
            );
        }
        /// <summary>
        /// Begin a new color region. This overload adjusts only the alpha value (between 0 and 1).
        /// </summary>
        /// <param name="colorAlpha">GUI.color alpha value for the region.</param>
        /// <param name="combineWithExistingAlpha">If true, new alpha value will be relative to the current alpha value.</param>
        public static void Begin(float colorAlpha, bool combineWithExistingAlpha = true)
        {
            float multiplier = combineWithExistingAlpha ? GUI.color.a : 1;
            Begin(colorAlpha * multiplier, GUI.contentColor.a, GUI.backgroundColor.a);
        }

        /// <summary>
        /// End the current color region.
        /// </summary>
        public static void End()
        {
            if (OldColorStack.Count == 0)
            {
                Debug.LogWarning("You are popping more color regions than you are pushing! Check that you are using Begin() and End() the same number of times.");
                return;
            }

            var oldColor = OldColorStack.Pop();

            SetGUIColor(oldColor.Item1, oldColor.Item2, oldColor.Item3);
        }

        //actual method for setting the gui colors
        private static void SetGUIColor(Color color, Color contentColor, Color backgroundColor)
        {
            GUI.color = color;
            GUI.contentColor = contentColor;
            GUI.backgroundColor = backgroundColor;
        }
        //quick function to get a color with adjusted alpha value
        private static Color AdjustAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
