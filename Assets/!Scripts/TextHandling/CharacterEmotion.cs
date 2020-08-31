using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Dialogue
{
	/// <summary>
	/// This is literally nothing more than a file in the project.
	/// Think of it like an enum, except we can create as many as we
	/// want to.
	///
	/// This is intended promarily for controlling puppets during
	/// the VN sequences. If it gets used for other things (like
	/// gabs mid-song), well... that's fine too?
	///
	/// Just keep in mind that this will be kept very minimal.
	/// </summary>
	[CreateAssetMenu(fileName = "NewEmotion", menuName = "Nijito/Emotion", order = 1)]
	public class CharacterEmotion : ScriptableObject
	{
		[Tooltip(
			"Set to true if all characters use this emotion."
			+ "This is just a hint for the editor so it can guess which "
			+ "emotions to assume all characters will have. "
		)]
		[SerializeField]
		private bool isStandardEmotion = true;

		public static CharacterEmotion[] GetAllEmotions()
		{
			//return Resources.FindObjectsOfTypeAll<CharacterEmotion>();
			return Resources.LoadAll<CharacterEmotion>("");
		}

		public static CharacterEmotion[] GetStandardEmotions()
		{
			List<CharacterEmotion> result = new List<CharacterEmotion>( GetAllEmotions() );

			result.RemoveAll((e) => !e.isStandardEmotion);

			return result.ToArray();
		}
	}
}
