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
 * AnimateStage
 * Fade
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
	/// <summary>
	/// Implements several character commands for the Yarn scripts to use.
	///
	/// These commands are specific to manipulating characters.
	/// General commands are in the
	/// [StageCommands](@ref Dialogue.VN.StageCommands) class.
	///
	/// Note that, whenever you have a name of an asset (e.g. an
	/// animation) which contains a space, you
	/// must wrap it in double quotes.
	/// 
	/// For any command with options such as **now**, **quickly**,
	/// and **slowly**, these options control the relative speed
	/// of the movement. Note that the exact speed depends on the
	/// command being used.
	///   * **now**: Instant; all delays and animations are skipped.
	///   * **quickly**: Snappy and quick. Good for fast or hurried motions.
	///   * (nothing): Standard speed. Should work for general situations.
	///   * **slowly**: Drags things out.
	///
	/// For commands with **wait** or **and wait** options, those
	/// will cause the dialogue system to pause until the command
	/// finishes being carried out. This does nothing if **now** is
	/// also used. Also be careful of combining this with **slowly**
	/// too often; that might make things draggy.
	///
	/// ## Changelog
	///  * 9/13/2020: Added
	///    [fade](@ref Dialogue.VN.CharacterCommands.Fade).
	///    Added quickly/slowly options.
	///    Moved now/quickly/slowly and wait explanations to the class header.
	/// 
	///  * 9/12/2020: Added
	///    [addon](@ref Dialogue.VN.CharacterCommands.Addon).
	///  
	/// </summary>
	public class CharacterCommands : MonoBehaviour
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
			Assert.IsNotNull(dialogueRunner);

			dialogueRunner.AddCommandHandler("move", Move);
			dialogueRunner.AddCommandHandler("turn", Turn);
			//dialogueRunner.AddCommandHandler("face", Turn);

			// TODO Delete this
			dialogueRunner.AddCommandHandler("setTextureIndex", SetTexture);
		}


		/// <summary>
		/// &lt;&lt;addon CHARACTER [lingering|clear] ADDON&gt;&gt;\n 
		/// &lt;&lt;addon CHARACTER clear&gt;&gt;\n 
		/// 
		/// Applies the ADDON to CHARACTER, where ADDON is the name
		/// of an addon specified on Unity.
		/// 
		/// Addons usually have some animation tied to them.
		/// Once the animation is done playing, the addon will
		/// vanish.
		/// 
		/// However, specifying **lingering** will cause the addon
		/// stay until **clear** is used.
		///
		/// Specifying **clear** without ADDON will clear all
		/// lingering addons.
		/// 
		/// </summary>
		/// <example>
		///
		/// ## Examples
		/// 
		///     <<addon Ai !!!bubble>> 
		///     AI: The what?!
		/// Give Ai a the "!!!bubble" addon. It will vanish after
		/// the addon's animation finishes. (And can vanish while
		/// the dialogue is still being written out.)
		///
		///     <<addon Ai lingering !!!bubble>> 
		///     AI: The what?!
		///     AMI: Ai, he’s joking, but he has a point.
		///     <<addon Ai clear !!!bubble>> 
		/// Give Ai a the "!!!bubble" addon. In this case, the
		/// addon will persist until the second command is reached.
		///
		///     <<addon Ai lingering !!!bubble>> 
		///     AI: The what?!
		///     AMI: Ai, he’s joking, but he has a point.
		///     <<addon Ai clear>>
		/// Same as before.
		///
		///     <<addon Ai lingering !!!bubble>> 
		///     <<addon Ai lingering sweat>> 
		///     AI: The what?!
		///     AMI: Ai, he’s joking, but he has a point.
		///     <<addon Ai clear>>
		/// Give Ai a the "!!!bubble" addon and sweat addons.
		/// In this case, the clear command will remove all addons.
		/// </example>
		/// \warning Not implemented yet.
		public void Addon(string[] args)
		{
			Debug.LogWarning("Not implemented yet: addon");
		}

		/// <summary>
		/// &lt;&lt;animate CHARACTER ANIMATION [and wait]&gt;&gt;\n 
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
		/// If **and wait** is given for a looping animation,
		/// the animation is allowed to play once before the
		/// dialogue runner continues.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<animate Ibuki Shake>> 
		/// Make Ibuki play the "Shake" animation.
		///
		///     <<animate Ibuki Shake and wait>> 
		/// Make Ibuki play the "Shake" animation, and prevent
		/// any more dialogue boxes from playing until Ibuki
		/// finishes with the animation.
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
		/// ## Examples
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
			Debug.LogWarning("Not implemented yet: emote");
		}

		/// <summary>
		/// &lt;&lt;fade in CHARACTER POSITION [now|quickly|slowly] [and wait]&gt;&gt;\n 
		/// &lt;&lt;fade out CHARACTER [now|quickly|slowly] [and wait]&gt;&gt;\n 
		///
		/// In the first form, causes CHARACTER to fade into view
		/// at POSITION.
		/// 
		/// In the second form, causes CHARACTER to fade out of view.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<fade in Nimura Right slowly>>
		///     NIMURA: I'm fading in right now!
		/// Fades Nimura in at the right position.
		/// %Dialogue starts printing as the fade goes on.
		/// "slowly" is optional, and makes her linger longer.
		///
		///     <<fade in Nimura Right and wait>>
		///     NIMURA: I just faded in.
		/// Fades Nimura in at the right position.
		/// %Dialogue doesn't print until the fade finishes.
		///
		///     <<fade out Nimura quickly>>
		///     NIMURA: Gotta run!
		/// Fades Nimura out quickly.
		/// </example>
		/// \warning Not implemented yet.
		public void Fade(string[] args)
		{
			Debug.LogWarning("Not implemented yet: fade");
		}

		/// <summary>
		/// &lt;&lt;move CHARACTER [to] TO_POINT [from FROM_POINT] [now|slowly|quickly] [and wait]&gt;&gt;
		/// <br>
		/// (Continued; see subsection)
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
		///
		/// ### Stacking
		/// 
		/// If two or more characters end up on the same point, then
		/// they'll "stack," with their positions being staggered
		/// so they aren't all on the same position.
		/// 
		/// Note that this is still in development. More details on
		/// how this looks/works once that's done.
		/// 
		/// 
		/// ### Pushing and Pulling
		/// 
		///     <<move CHARACTER [to] TO_POINT [from FROM_POINT] [now|slowly|quickly] [and wait]
		///            [and pull|push TARGET1 [, TARGET2, TARGET3, ...] [to TARGET_POSITION]]>>
		///
		/// If **pull** or **push** is given, then every TARGET gets
		/// pulled/pushed by CHARACTER. It's possible to push/pull 
		/// several targets at a time, separating their names with
		/// spaces.
		///
		/// By default, each TARGET ends up on the same spot as
		/// CHARACTER. However, if TARGET_POSITION is also specified,
		/// then all TARGETs end up there.
		/// 
		/// The assumption is that CHARACTER will move past every
		/// TARGET. Any TARGETs which CHARACTER does not go past
		/// won't be pushed. (This is subject to change?)
		/// 
		/// 
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<move Ibuki Middle and wait>> 
		/// Move Ibuki to the middle (from wherever she was before).
		/// The VN will wait until Ibuki finishes moving.
		/// 
		///     <<move Ibuki to Middle>>
		/// Same as before, except without the waiting. (And it looks
		/// a little more like English.)
		///
		///     <<move Ibuki Middle now>>
		/// Same as before, but Ibuki will "teleport" to the middle.
		/// 
		///     <<move Ibuki to Left from OffLeft>>
		/// Move Ibuki from the off the left edge of the screen to the left position.
		/// Use something like this if the character was off screen.
		///
		///     <<move Ibuki to Middle and push Nimura to Left and push Ami to OffLeft>>
		/// Move Ibuki to the middle.<br>
		/// When Ibuki reaches Nimura, Nimura will be forced over to the left.<br>
		/// When Ibuki reaches Ami, Ami will be forced off the left edge.
		///
		///     <<move Ibuki to Middle and push Nimura, Ami to Left>>
		/// Move Ibuki to the middle.<br>
		/// When Ibuki reaches Nimura and/or Ami, they will both be to the Left position.
		/// 
		/// 
		/// </example>
		/// \warning Implementation in progress
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

					case "and":
						i++;
						Assert.AreEqual(args[i], "wait");

						// TODO Implement 'move ... and wait'
						Debug.LogWarning("'move ... and wait' isn't supported yet!");
						break;

					case "now":
						// TODO Implement 'now'
						// TODO Implement 'slowly'
						// TODO Implement 'quickly'
						Debug.LogWarning("'now' isn't supported yet!");
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
		/// [outfit-all](@ref Dialogue.VN.CharacterCommands.OutfitAll).
		///
		/// If COSTUME is **None** or an invalid costume, the
		/// character's default costume is used instead.
		/// (Again, default is controlled via
		/// [outfit-all](@ref Dialogue.VN.CharacterCommands.OutfitAll).)
		///
		/// Note that changing costumes is instant.
		/// Making characters change while they're
		/// on screen might be strange; get them off the screen
		/// first. (If you need characters changing on screen with
		/// some animation involved, let a coder know!)
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
		/// If **default-only** is specified, loaded characters will
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
		/// &lt;&lt;turn CHARACTER DIRECTION&gt;&gt;\n 
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
