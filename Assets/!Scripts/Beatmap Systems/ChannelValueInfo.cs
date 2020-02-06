using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    [CreateAssetMenu(fileName = "New Channel Value Info", menuName = "Beatmap/Channel Value Info")]
    public class ChannelValueInfo : ScriptableObject
    {
        public Sprite EditorSprite;
        public GameObject GameObject;
    }
}
