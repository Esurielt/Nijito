using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Yarn.Unity;

namespace Dialogue.VN
{

	/// <summary>
	/// Implements several stage commands for the Yarn scripts to use.
	///
	/// These commands are generic commands, and they don't interact
	/// specifically with characters. Character commands are in the
	/// [CharacterCommands](@ref Dialogue.VN.CharacterCommands) class.
	///
	/// Note that, whenever you have a name of an asset (e.g. a
	/// music track or a sound effect) which contains a space, you
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
	///  * 9/13/2020: Changed fade to
	///    [stage-fade](@ref Dialogue.VN.StageCommands.StageFade).
	///    Added quickly/slowly options.
	///    Moved now/quickly/slowly and wait explanations to the class header.
	///
	///  * 9/12/2020: Added
	///    [wait](@ref Dialogue.VN.StageCommands.Wait) and 
	///    [itembox](@ref Dialogue.VN.StageCommands.ItemBox).
	///  
	/// </summary>
	public class StageCommands : MonoBehaviour
	{
		[Header("Setup")]

		[SerializeField]
		private DialogueRunner dialogueRunner;


		void Awake()
		{
			Assert.IsNotNull(dialogueRunner);
		}

		/// <summary>
		/// &lt;&lt;animate-stage ANIMATION [and wait]&gt;&gt;\n 
		///
		/// Plays ANIMATION, which is an animation which has been
		/// set up on the stage in Unity.
		///
		/// If **and wait** is given when playing a looping animation,
		/// then dialogue pauses until the animation plays once.
		///
		/// Only one animation can play at a time, and specifying
		/// **None** (or an invalid animation) will cause the current
		/// animation to stop.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<animate-stage Earthquake and wait>>
		/// Play the "Earthquake" animation and wait for it to finish.
		///
		///     <<animate-stage None>>
		/// Stop the current stage animation, if any.
		/// </example>
		/// \warning Not implemented yet.
		public void AnimateStage(string[] args)
		{
			Debug.LogWarning("Not implemented yet: animate-stage");
		}

		/// <summary>
		/// &lt;&lt;background IMAGE [now|quickly|slowly] [and wait]&gt;&gt;\n 
		///
		/// Displays IMAGE in the background, where IMAGE is some
		/// background image that has been configured in Unity.
		/// 
		/// If IMAGE is **None** (or invalid), then a
		/// black background is shown instead.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<background "Some Awesome Stage" now>>
		/// Show "Some Awesome Stage" immediately.
		///
		///     <<background Home and wait>>
		/// Show "Home," and wait until we finish fading into it.
		/// </example>
		/// \warning Not implemented yet.
		public void Background(string[] args)
		{
			Debug.LogWarning("Not implemented yet: background");
		}

		/// <summary>
		/// &lt;&lt;itembox ITEM&gt;&gt;\n 
		/// &lt;&lt;itembox hide [ITEM]&gt;&gt;\n 
		///
		/// In the first form, displays ITEM in an item box,
		/// where ITEM is the name of an item specified in Unity.
		///
		/// The second form hides the named item box. If no name is
		/// given, then all item boxes are hidden.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<itembox EmployeesOnlySign>>
		/// Display an item called "EmployeesOnlySign."
		///
		///     <<itembox hide EmployeesOnlySign>>
		/// Hide the display for an item called "EmployeesOnlySign."
		///
		///     <<itembox hide>>
		/// Hide whatever item(s) are currently being displayed, if any.
		/// </example>
		/// \warning Not implemented yet.
		public void ItemBox(string[] args)
		{
			Debug.LogWarning("Not implemented yet: itembox");
		}

		/// <summary>
		/// &lt;&lt;sound SOUND-EFFECT [at VOLUME%] [and wait]&gt;&gt;\n 
		///
		/// Plays SOUND-EFFECT once.
		/// 
		/// The list of sound effects (and their names) must be
		/// configured in Unity.
		/// 
		/// If given, VOLUME adjusts how loud the sound plays. Can
		/// be any percentage between 0% and 100%.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<sound Beep>>
		/// If the sound named "Beep" is configured, it's played.
		///
		///     <<sound "Breaking glass">>
		/// If the sound named "Breaking glass."
		///
		///     <<sound Beep at 50%>>
		/// Same as before, but "Beep" is played at half volume.
		///
		///     <<sound Beep wait>>
		/// Same as before, but game waits until the sound finishes.
		/// </example>
		/// \warning Not implemented yet.
		public void Sound(string[] args)
		{
			Debug.LogWarning("Not implemented yet: sound");
		}

		/// <summary>
		/// &lt;&lt;stage-fade to [black|white|IMAGE] [overlay] [now|quickly|slowly] [and wait]&gt;&gt;\n 
		/// &lt;&lt;stage-fade out [overlay] [now|quickly|slowly] [and wait]&gt;&gt;\n 
		/// &lt;&lt;stage-fade in [now|quickly|slowly] [and wait]&gt;&gt;\n 
		///
		/// In the first form, fades out to **black**, **white**, or IMAGE,
		/// where IMAGE is the name of an image that's been configured
		/// in Unity. If IMAGE is invalid, **black** is used instead.
		///
		/// The second form is a shortcut the first form that
		/// always fades to black.
		///
		/// In the third form, the fade effect is removed,
		/// showing everything.
		///
		/// If **overlay** is given, then everything, even the text
		/// box, is also hidden by the fade effect.
		/// Note that, even if this is given, the text box will
		/// be unhidden if any more dialogue is printed while we're
		/// faded out.
		/// 
		/// (Note that this is a great way to hide various other
		/// stage directions, like moving characters around, etc.)
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<fade out>>
		/// Turns the screen black without hiding the dialogue box.
		///
		///     <<fade to white overlay now>>
		/// Immediately turns the screen white, covering the dialogue box.
		///
		///     <<fade in now>>
		/// Immediately fades back in.
		/// 
		/// </example>
		/// \warning Not implemented yet.
		public void StageFade(string[] args)
		{
			Debug.LogWarning("Not implemented yet: stage-fade");
		}

		/// <summary>
		/// &lt;&lt;music TRACK [now|quickly|slowly]&gt;&gt;\n 
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
		/// If no song is playing, the new song will play immediately.
		/// If a song *is* playing, then the old one will fade out and
		/// the new song will begin to play after a short delay. 
		/// 
		/// Options which control speed will alter the rate at which
		/// the songs fade.
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<music "Beautiful Lie">> 
		/// Plays a song named "Beautiful Lie."
		/// (No, we aren't ripping tracks from Danganronpa.)
		///
		///     <<music "Never Gonna Give You Up" now>> 
		/// Get Rick-Rolled without letting the previous song fade out.
		///
		///     <<music None>> 
		/// Stops the current track.
		///
		///     <<music None now>> 
		/// Stops the current track without any fade out.
		/// </example>
		/// \warning Not implemented yet.
		public void Music(string[] args)
		{
			Debug.LogWarning("Not implemented yet.");
		}

		/// <summary>
		/// &lt;&lt;wait SECONDS&gt;&gt;\n 
		///
		/// Waits for SECONDS, and the continues to the next text box.
		/// 
		/// Note that this doesn't pause any on-going animations. It
		/// simply stops future text boxes and commands from
		/// triggering until the wait is over.
		/// 
		/// This is part of Yarn. See [Yarn's documentation](https://yarnspinner.dev/docs/unity/working-with-commands/#wait).
		///
		/// </summary> <example>
		///
		/// ## Examples
		///
		///     <<wait 2>>
		/// Wait for 2 seconds.
		///
		///     <<wait 0.5>>
		/// Wait for half a second.
		/// </example>
		public void Wait(string[] args)
		{
			// Never implement this
		}

	

	}

}
