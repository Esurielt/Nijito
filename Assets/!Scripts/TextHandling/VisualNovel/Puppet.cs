using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Sirenix.Serialization;
using System;

namespace Dialogue.VN
{
	public class Puppet : MonoBehaviour
	{
		public enum Facing
		{
			Left,
			Right
		}

		// TODO: Allow movement curve?
		// https://answers.unity.com/questions/1207389/can-animation-curves-be-used-to-control-variables.html

		public Image imageRenderer;
		public float movementSpeed = 5f;

		PuppetCostume costume;

		private Facing defaultFacing = Facing.Left;

		private RectTransform rTransform;
		private float targetHorizontalPos;


		public void Configure(PuppetCostume targetCostume)
		{
			costume = targetCostume;
			imageRenderer.sprite = costume.defaultSprite;
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
			Vector3 scale = imageRenderer.transform.localScale;
			scale.x = (newFacing == defaultFacing ? 1 : -1);
			imageRenderer.transform.localScale = scale;
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
			Assert.IsNotNull(imageRenderer, "Puppets must have a RawImage!");
			
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
