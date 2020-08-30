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
		public struct ImagePreset
		{
			public CharacterEmotion emotion;
			public Sprite img;

			/*
			[Tooltip("X is width, Y is height")]
			public Vector2 size;
			*/
		}

		public Sprite baseImage;
		//public Vector2 size;
		public ImagePreset[] presets;

		[ContextMenu("Add Missing Emotions")]
		private void AddMissingEmotions()
		{
			CharacterEmotion[] allEmotions = CharacterEmotion.GetStandardEmotions();
			List<CharacterEmotion> missingEmotions = new List<CharacterEmotion>(); 

			foreach(CharacterEmotion emote in allEmotions)
			{
				if(!presets.Any((ip) => emote == ip.emotion))
				{
					missingEmotions.Add(emote);
				}
			}

			List<ImagePreset> updatedPresets = new List<ImagePreset>(presets);
			foreach(CharacterEmotion emote in missingEmotions)
			{
				ImagePreset newPreset = new ImagePreset();
				newPreset.emotion = emote;

				updatedPresets.Add(newPreset);
			}

			presets = updatedPresets.ToArray();

		}
	}
}
