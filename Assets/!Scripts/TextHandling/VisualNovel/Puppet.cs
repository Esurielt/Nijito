using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Sirenix.Serialization;
using System;

namespace Dialogue.VN
{
	/// <summary>
	/// This is component represents a puppet on the stage.
	/// Puppets can be configured via Dialogue.VN.PuppetPreset objects.
	/// </summary>
	public class Puppet : MonoBehaviour
	{
		public enum Facing
		{
			Left,
			Right
		}

		// TODO: Allow movement curve?
		// https://answers.unity.com/questions/1207389/can-animation-curves-be-used-to-control-variables.html

		[Tooltip("This is the object that gets mirrored. All renderers (which we want to flip) should be a child of this one.")]
		public Transform flipTarget;
		[PreviouslySerializedAs("imageRenderer")]
		public Image baseRenderer;
		public float movementSpeed = 5f;


		private PuppetPreset preset;
		private Facing defaultFacing = Facing.Left;

		private RectTransform rTransform;
		private float targetHorizontalPos;


		public void Configure(PuppetPreset targetPreset)
		{
			preset = targetPreset;
			baseRenderer.sprite = preset.baseImage;
		}

		/// <summary>
		/// Sets a new destination where we want to slide to.
		/// </summary>
		/// <param name="moveDestination">Position, with 0 being left edge and 1 being right edge.</param>
		public void SetMovementDestination(float moveDestination)
		{
			targetHorizontalPos = moveDestination;
		}

		/// <summary>
		/// Sets a new destination where we want to slide to.
		/// </summary>
		/// <param name="rt">Transform to use as a reference, where the average of the min/max anchors' x values are used.</param>
		public void SetMovementDestination(RectTransform rt)
		{
			SetMovementDestination((rt.anchorMin.x + rt.anchorMax.x)/2);
		}

		/// <summary>
		/// Snap to the given position.
		/// This also cancels out any movement.
		/// </summary>
		/// <param name="newHorizontalPos">Position, with 0 being left edge and 1 being right edge.</param>
		public void Warp(float newHorizontalPos)
		{
			SetPosition(newHorizontalPos);
			SetMovementDestination(newHorizontalPos);
		}

		/// <summary>
		/// Snap to the given position.
		/// This also cancels out any movement.
		/// </summary>
		/// <param name="rt">Transform to use as a reference, where the average of the min/max anchors' x values are used.</param>
		public void Warp(RectTransform rt)
		{
			Warp((rt.anchorMin.x + rt.anchorMax.x)/2);
		}

		public void SetFacing(Facing newFacing)
		{
			Vector3 scale = flipTarget.localScale;
			scale.x = Mathf.Abs(scale.x) * (newFacing == defaultFacing ? 1 : -1);
			flipTarget.localScale = scale;
		}

		[Obsolete]
		public void SetTexture(int index)
		{
			//imageRenderer.sprite = sprites[index];
			Debug.LogWarning("Using deprecated SetTexture");
		}

		private void SetPosition(float newHorizontalPos)
		{
			rTransform.anchorMin = new Vector2(newHorizontalPos, rTransform.anchorMin.y);
			rTransform.anchorMax = new Vector2(newHorizontalPos, rTransform.anchorMax.y);
		}

		private void Awake()
		{
			rTransform = GetComponent<RectTransform>();
			Assert.IsNotNull(rTransform, "Puppets should be part of the UI, not in the scene itself!");
			Assert.IsNotNull(baseRenderer, "Puppets must have a RawImage!");
			
		}

		private void Update()
		{
			if(!Mathf.Approximately(rTransform.anchorMin.x, targetHorizontalPos))
			{
				float newHorizontalPos = Mathf.Lerp(rTransform.anchorMin.x, targetHorizontalPos, movementSpeed * Time.deltaTime);
				SetPosition(newHorizontalPos);
			}
		}


	}
}
