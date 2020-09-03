using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Yarn.Unity;

/*
 * Background
 * ShakeScreen
 * FadeOut
 * FadeIn
 * Music
 * Sounds
 * 
 * Move
 * Outfit
 * Default Outfit
 * Animate
 * Facing
 * Emotion
 */

namespace Dialogue.VN
{
	// Note that the CDATA tags are to make the C# XML stuff not
	// choke on all of the angle brackets.

	/// <summary>
	/// Implements several commands for the Yarn scripts to use. See
	/// the comments for each function.
	/// </summary>
	public class VisualNovelCommands : MonoBehaviour
	{
		[Header("Setup")]

		[SerializeField]
		private DialogueRunner dialogueRunner;
		[SerializeField]
		private PuppetMaster puppetMaster;

		[Header("Design config")]

		[FormerlySerializedAs("positions")]
		[SerializeField]
		private RectTransform[] puppetPoints;

		private void Awake()
		{
			Assert.IsNotNull(puppetMaster);

			dialogueRunner.AddCommandHandler("move", Move);
			dialogueRunner.AddCommandHandler("turn", Turn);
			dialogueRunner.AddCommandHandler("face", Turn);

			// TODO Delete this
			dialogueRunner.AddCommandHandler("setTextureIndex", SetTexture);
		}

		/// <summary>
		/// &lt;&lt;move CHARACTER [to] TO_POINT [from FROM_POINT]&gt;&gt;
		///
		/// Causes CHARACTER to slide over to TO_POINT. If FROM_POINT is
		/// specified, the character will warp there before moving.
		/// If trying to move a character into view, please make sure to
		/// specify FROM_POINT. Otherwise, your character may start
		/// somewhere you weren't expecting!
		/// 
		/// Points are created in Unity and are listed in the "puppetPoints"
		/// variable. For this reason, the number and names of points is
		/// flexible, and could possibly change. Current points PROBABLY are:
		///		offleft, left, middle, right, offright
		///
		/// Note that point names are case-insensitive, but character names are not.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<move Ibuki Middle>> 
		/// Move Ibuki to the middle (from wherever she was before).
		/// 
		///     <<move Ibuki to Middle>>
		/// Same as before, just looks more like English.
		/// 
		///     <<move Ibuki to Left from OffLeft>>
		/// Move Ibuki from the off the left edge of the screen to the left position.
		/// Use something like this if the character was offscreen.
		/// </example>
		public void Move(string[] args)
		{
			#region Argument handling
			Assert.IsTrue(args.Length >= 2);
			string charName = args[0];
			string destName = "";
			string fromName = "";

			for(int i = 1; i < args.Length; i++)
			{
				switch(args[i])
				{
					case "to":
						i++;
						destName = args[i];
						break;

					case "from":
						i++;
						fromName = args[i];
						break;

					default:
						// Only take an unlabeled parameter if we don't
						// already have a destination.
						if(string.IsNullOrEmpty(destName))
						{
							destName = args[i];
						}
						else
						{
							ReportInvalidArgument("move", args[i]);
						}
						break;
				}
			}
			#endregion

			Puppet charPuppet = puppetMaster.GetPuppet(charName);

			if(!string.IsNullOrEmpty(fromName))
			{
				charPuppet.Warp(GetNamedPoint(fromName));
			}

			charPuppet.SetMovementDestination(GetNamedPoint(destName));
		}

		/// <summary>
		/// &lt;&lt;turn CHARACTER DIRECTION&gt;&gt;\n 
		/// &lt;&lt;face CHARACTER DIRECTION&gt;&gt;
		/// 
		/// Causes CHARACTER to face in DIRECTION, where DIRECTION is
		/// either "left" or "right" (DIRECTION is case-insensitive).
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<turn Ibuki Right>> 
		/// Make Ibuki face towards the right.
		/// </example>
		public void Turn(string[] args)
		{
			#region Argument handling
			Assert.IsTrue(args.Length >= 2);
			string charName = args[0];
			string facingName = args[1];
			#endregion

			Puppet charPuppet = puppetMaster.GetPuppet(charName);
			Puppet.Facing newFacing;
			if(facingName.Equals("left", StringComparison.InvariantCultureIgnoreCase))
			{
				newFacing = Puppet.Facing.Left;
			}
			else if(facingName.Equals("right", StringComparison.InvariantCultureIgnoreCase))
			{
				newFacing = Puppet.Facing.Right;
			}
			else
			{
				throw new InvalidEnumArgumentException("Invalid facing: " + facingName);
			}

			Debug.Log("Changing facing to " + newFacing.ToString());

			charPuppet.SetFacing(newFacing);
		}


		/// <summary>
		/// &lt;&lt;emote CHARACTER EMOTION&gt;&gt;\n 
		/// 
		/// Changes CHARACTER's face to show EMOTION, where EMOTION is the
		/// (case-insensitive) name of one of the character's emotions.
		/// 
		/// Each character has their own range of emotions; some may have
		/// special emotions, and others might be missing some of the common
		/// ones. The list is configured in Unity.
		/// 
		/// Here's the list of common emotions:<br>
		/// * Anger
		/// * Concern
		/// * Laugh
		/// * Sad
		/// * Serious
		/// * Smile
		/// * Surprised
		///  
		/// Finally, there's one special emotion: **None**. This clears all
		/// emotions, resetting the character back to their default, neutral
		/// face that they use when they first come onto the stage.
		/// The **None** emotion will always be available.
		///
		/// Using an invalid emotion will print an error in the debug console
		/// and then act as though you had used **None** instead.
		/// </summary>
		/// <example>
		/// 
		///     <<emote Ibuki Laugh>> 
		/// Make Ibuki Laugh.
		///
		///     <<emote Ibuki None>> 
		/// Return Ibuki to her default emotion.
		/// </example>
		/// \warning Not implemented yet.
		public void Emote(string[] args)
		{
			Debug.LogWarning("Not implemented yet: Emote");
		}


		/// <summary>
		/// &lt;&lt;animate CHARACTER ANIMATION&gt;&gt;\n 
		///
		/// Make CHARACTER play ANIMATION, where ANIMATION is the
		/// case-insensitive name of a character animation that has
		/// been created in Unity. These animations can either be
		/// one-off or looping; it depends on the animation.
		///
		/// The **None** animation can be used to stop all current
		/// animations. If an invalid animation is given, Unity
		/// throws an error and treats it like **None** was used
		/// instead.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<animate Ibuki Shake>> 
		/// Make Ibuki play the "Shake" animation.
		/// 
		///     <<animate Ibuki None>> 
		/// Stop whatever animation Ibuki is playing, if any.
		///
		/// </example>
		/// \warning Not implemented yet.
		public void Animate(string[] args)
		{
			Debug.LogWarning("Not implemented yet: Animate");
		}

		/// <summary>
		/// &lt;&lt;outfit CHARACTER COSTUME&gt;&gt;\n 
		///
		/// Make CHARACTER where COSTUME, where COSTUME is a
		/// (case-insensitive) name for one of CHARACTER's costumes.
		/// 
		/// Costumes are specified within Unity, so their names aren't
		/// fixed. However, every character will have a default
		/// **VR** and **RL** costumes. Normally, characters will
		/// enter the stage with their RL costume, but this may
		/// be configured with
		/// [outfit-all](@ref Dialogue.VN.VisualNovelCommands.OutfitAll).
		///
		/// If COSTUME is **None** or an invalid costume, the
		/// character's default costume is used instead.
		/// (Again, default is controlled via
		/// [outfit-all](@ref Dialogue.VN.VisualNovelCommands.OutfitAll).)
		///
		/// Note that changing costumes is instant.
		/// Making characters change while they're
		/// on screen might be strange; get them off the screen
		/// first. (If you need characters changing on screen with
		/// some animation involved, this could be implemented!)
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<outfit Ibuki VR>> 
		/// Make Ibuki wear her default VR outfit.
		///
		///		<<outfit Ibuki None>>
		/// Make Ibuki wear her current default costume.
		///
		/// </example>
		/// \warning Not implemented yet.
		public void Outfit(string[] args)
		{
			Debug.LogWarning("Not implemented yet: Outfit");
		}

		/// <summary>
		/// &lt;&lt;outfit-all [VR|RL] [default-only]&gt;&gt;\n 
		///
		/// Make all characters wear either their VR or RL costumes.
		/// This also changes all incoming characters to have on
		/// their VR/RL costume.
		/// 
		/// If *default-only* is specified, loaded characters will
		/// not have their costume changed. (Note: all characters
		/// who have been used in the scene are loaded, even if
		/// they're not visible!)
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<outfit-all VR>> 
		/// Make all characters wear their virtual reality outfits.
		///
		///		<<outfit-all RL default-only>>
		/// Make all future characters wear their real-life outfits.
		///
		/// </example>
		/// \warning Not implemented yet.
		public void OutfitAll(string[] args)
		{
			Debug.LogWarning("Not implemented yet: OutfitAll");
		}

		/// <summary>
		/// &lt;&lt;music TRACK&gt;&gt;\n 
		///
		/// Cause background music named TRACK to start playing,
		/// where TRACK is the (case-insensitive) name for a song.
		/// 
		/// Tracks and their names are defined in Unity, so we
		/// can't put a full list here. If TRACK is **None**, the
		/// current song is stopped. If TRACK cannot be found,
		/// an error is sent to the debug console and it is treated
		/// as **None**.
		/// 
		/// Note that TRACK can have spaces. (Some songs might have spaces
		/// in their name, so this is allowed to keep things simple.)
		///
		/// If no song is playing, the new song will play immediately.
		/// If a song *is* playing, then the old one will fade out and
		/// the new song will begin to play after a short delay. If you'd
		/// rather have the songs switch abruptly,
		/// see [music-now](@ref Dialogue.VN.VisualNovelCommands.MusicNow).
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<music Beautiful Lie>> 
		/// Plays a song named "Beautiful Lie."
		/// (No, we aren't ripping tracks from Danganronpa.)
		///
		///     <<music None>> 
		/// Stops the current track.
		/// </example>
		/// \warning Not implemented yet.
		public void Music(string[] args)
		{
			Debug.LogWarning("Not implemented yet.");
		}


		/// <summary>
		/// &lt;&lt;music-now TRACK&gt;&gt;\n 
		///
		/// Cause background music named TRACK to start playing immediately,
		/// where TRACK is the (case-insensitive) name for a song.
		/// 
		/// This is like [music](@ref Dialogue.VN.VisualNovelCommands.Music)
		/// except there is no fade between songs. If used with **None**,
		/// then the music cuts to silence.
		/// Note that if nothing is currently playing,
		/// this is no different from [music](@ref Dialogue.VN.VisualNovelCommands.Music).
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<music-now Never Gonna Give You Up>> 
		/// Plays a song named "Never Gonna Give You Up" instantly.
		///
		///     <<music-now None>> 
		/// Stops the current track immediately.
		/// </example>
		/// \warning Not implemented yet.
		public void MusicNow(string[] args)
		{
			Debug.LogWarning("Not implemented yet.");
		}

		public void Sound(string[] args)
		{
			// Will need a version that waits for sound to finish too
		}

		// TODO Make background thing

		private void SetTexture(string[] args)
		{
			Assert.IsTrue(args.Length >= 2);
			string charName = args[0];
			string indexName = args[1];

			Puppet charPuppet = puppetMaster.GetPuppet(charName);
			int index = int.Parse(indexName);

			charPuppet.SetTexture(index);
		}

		private RectTransform GetNamedPoint(string posName)
		{
			return puppetPoints.FirstOrDefault(
				(rt) => posName.Equals(rt.name, System.StringComparison.OrdinalIgnoreCase)
			); 
		}

		private void ReportInvalidArgument(string command, string arg)
		{
			Debug.LogWarning("Invalid argument for " + command + ": " + arg);
		}
	}
}
