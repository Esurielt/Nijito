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
		/// If **and wait** is given, then dialogue pauses until
		/// the animation finishes. Otherwise, it will continue 
		/// while the animation plays. (This is ignored if
		/// the animation loops indefinitely.)
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
		/// &lt;&lt;background IMAGE [now] [and wait]&gt;&gt;\n 
		///
		/// Displays IMAGE in the background, where IMAGE is some
		/// background image that has been configured in Unity.
		/// 
		/// Normally, the new background will fade in.
		/// However, if **now** is given,
		/// no fade occurs and the change is instant.
		/// 
		/// If **and wait** is given, then the dialogue sequence
		/// will wait for the transition to finish. This does
		/// nothing if **now** is also given, since then the
		/// transition is instant.
		///
		/// If **None** is for IMAGE (or IMAGE is invalid), then a
		/// blank background is shown instead.
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
		/// &lt;&lt;fade to [black|white|IMAGE] [overlay] [now]&gt;&gt;\n 
		/// &lt;&lt;fade out [overlay] [now]&gt;&gt;\n 
		/// &lt;&lt;fade in [now]&gt;&gt;\n 
		///
		/// In the first form, fades out to **black**/**white**/IMAGE,
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
		/// Either way, if **now** is given, the fade happens without
		/// any delay. If **now** is not given, dialogue pauses until
		/// the fade is completed.
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
		public void Fade(string[] args)
		{
			Debug.LogWarning("Not implemented yet: fade");
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
		/// If **wait** is given, no text will display until
		/// the sound finishes playing.
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
		/// &lt;&lt;music TRACK [now]&gt;&gt;\n 
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
		/// However, if **now** is specified, then the fade is skipped
		/// and the new song will simply start to play.
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



	}

}
