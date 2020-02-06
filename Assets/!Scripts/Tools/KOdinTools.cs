using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using KEditorExtensions;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace KOdinTools
{
    public class GUIColorLookupAttribute : Attribute
    {
        //replace this at some point. dictionary maintenance is too cumbersome. tagging classes with colors is simpler.

        public struct ColorPack
        {
            public readonly Color Main, Fields;
            public ColorPack(Color main, Color fields)
            {
                Main = main; Fields = fields;
            }
            public ColorPack(bool fieldsOnly, Color oneColor)
                :this(fieldsOnly ? Color.white : oneColor, fieldsOnly ? oneColor: Color.white) { }

            public ColorPack(bool fieldsOnly, float r, float g, float b, float a = 1)
                :this(fieldsOnly, new Color(r, g, b, a)) { }
        }
        private static Dictionary<string, ColorPack> Dict = new Dictionary<string, ColorPack>()
        {
            { "EffectBurst", new ColorPack(true, 1f, .9f, .9f) },
            { "SecondaryEffect", new ColorPack(true, 1f, 1f, .5f) },
            { "CounterAttackEffect", new ColorPack(true, .8f, 1f, 1f) },
            { "StatChange", new ColorPack(true, .9f, .9f, 1f) },
            { "CombatRecipe", new ColorPack(true, .9f, 1f, .9f) },
        };

        public Color Main { get; protected set; }
        public Color Fields { get; protected set; }

        public GUIColorLookupAttribute(ColorPack uniqueColors)
        {
            Main = uniqueColors.Main;
            Fields = uniqueColors.Fields;
        }
        public GUIColorLookupAttribute(string key)
        {
            if (Dict.TryGetValue(key, out ColorPack colorPack))
            {
                Main = colorPack.Main;
                Fields = colorPack.Fields;
            }
            else
            {
                Main = Color.white;
                Fields = Color.white;
            }
        }
    }
}

//drawers

namespace KOdinTools
{
#if UNITY_EDITOR
    public class Drawer_GUIColorLookup : OdinAttributeDrawer<GUIColorLookupAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var colorAttribute = Property.GetAttribute<GUIColorLookupAttribute>();

            ColorScope.Begin(colorAttribute.Main, GUI.contentColor, colorAttribute.Fields);

            CallNextDrawer(label);

            ColorScope.End();
        }
    }

    public abstract class Drawer_ValueWithExampleString<TValue> : OdinValueDrawer<TValue>
    {
        protected const string Ex = "Ex.) ";
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CoreLayoutBlock(label);

            if (ValidateExample)
            {
                DrawExample();
            }
        }
        protected virtual void CoreLayoutBlock(GUIContent label)
        {
            CallNextDrawer(label);
        }
        protected abstract string ExampleString { get; }
        protected virtual bool ValidateExample => true;
        protected virtual bool StartWithEx => true;

        protected virtual void DrawExample()
        {
            string text = ExampleString;
            if (StartWithEx)
            {
                text = Ex + text;
            }
            EditorGUILayout.LabelField(text);
        }
    }
#endif
}
