using UnityEngine;
using Sirenix.OdinInspector;

namespace CommonSupers
{
    [HideMonoScript]
    public abstract class TemplatedObject_Base : SerializedScriptableObject
    {
        [PropertyTooltip("The name the players will see."), PropertyOrder(-10)]
        [ShowInInspector]
        protected string externalName;

        /// <summary>
        /// Return the player-visible name of this object. Returns internal name if no external name is defined.
        /// </summary>
        public string ExternalName { get { return string.IsNullOrEmpty(externalName) ? this.name : externalName; } }

        [TabGroup("Basic", "Cosmetic", UseFixedHeight = true)]
        [PropertyTooltip("The icon used to represent this object to players.")]
        [PreviewField(Alignment = ObjectFieldAlignment.Right)]
        public Sprite Icon;

        //cosmetic info
        [TabGroup("Basic", "Cosmetic")]
        [TextArea, Title("Description")]
        [LabelText("Description (Flavorful)")]
        public string DescriptionFlavorful;

        [TabGroup("Basic", "Cosmetic")]
        [TextArea]
        [LabelText("Description (Technical)")]
        public string DescriptionTechnical;
    }

//#if UNITY_EDITOR
//    public class Drawer_LockableAttribute<T> : OdinAttributeDrawer<LockableAttribute, T>
//    {
//        protected override void DrawPropertyLayout(GUIContent label)
//        {
//            var attr = Property.GetAttribute<LockableAttribute>();

//            EditorGUILayout.BeginHorizontal();

//            Rect buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.singleLineHeight));

//            GUIContent content = BuildGUIContent(attr.IsLocked);

//            if (GUI.Button(buttonRect, content))
//            {
//                //toggle
//                attr.IsLocked = !attr.IsLocked;
//            }

//            EditorGUI.BeginDisabledGroup(attr.IsLocked);

//            CallNextDrawer(label);

//            EditorGUI.EndDisabledGroup();

//            EditorGUILayout.EndHorizontal();
//        }

//        private GUIContent BuildGUIContent(bool isLocked)
//        {
//            return new GUIContent()
//            {
//                image = isLocked ?
//                    Sirenix.Utilities.Editor.EditorIcons.LockUnlocked.Active :
//                    Sirenix.Utilities.Editor.EditorIcons.LockUnlocked.Active,

//                tooltip = isLocked ?
//                    "Object is locked. Click to unlock." :
//                    "Object is not locked. Click to lock.",
//            };
//        }
//    }
//#endif
}
