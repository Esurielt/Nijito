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
		/// </summary>
		/// <example>
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
		/// </summary>
		/// <example>
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
