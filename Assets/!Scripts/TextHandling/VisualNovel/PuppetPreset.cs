using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Dialogue.VN
{
	/// <summary>
	/// Puppet costumes contain all of the visual data for a character
	/// that gets used during VN sequences. This includes images
	/// for moods, etc.
	/// </summary>
	[CreateAssetMenu(fileName = "NewPuppetCostume", menuName = "Nijito/Puppet Costume", order = 1)]
	public class PuppetPreset : ScriptableObject
	{


		[System.Serializable]
		public struct EmotionPreset
		{
			public CharacterEmotion emotion;
			public Sprite img;

			/*
			[Tooltip("X is width, Y is height")]
			public Vector2 size;
			*/
		}

		public Sprite baseImage;
		public Vector2 size;
		public EmotionPreset[] emotionPresets;

		[ContextMenu("Add Missing Emotions")]
		private void AddMissingEmotions()
		{
			CharacterEmotion[] allEmotions = CharacterEmotion.GetStandardEmotions();
			List<CharacterEmotion> missingEmotions = new List<CharacterEmotion>(); 

			foreach(CharacterEmotion emote in allEmotions)
			{
				if(!emotionPresets.Any((ip) => emote == ip.emotion))
				{
					missingEmotions.Add(emote);
				}
			}

			List<EmotionPreset> updatedPresets = new List<EmotionPreset>(emotionPresets);
			foreach(CharacterEmotion emote in missingEmotions)
			{
				EmotionPreset newPreset = new EmotionPreset();
				newPreset.emotion = emote;

				updatedPresets.Add(newPreset);
			}

			emotionPresets = updatedPresets.ToArray();

		}
	}
}
