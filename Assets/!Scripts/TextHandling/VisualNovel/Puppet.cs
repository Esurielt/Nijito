/** ChangeLog
 * 8/30/2020 - Added layer code.
 */

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

		/*
		 * Everything here is layers. For that reason, even the character's base is layer.
		 */

		/* Ok, so some planning
		 * base layer  + main clothes + expression + hair + addons
		 * 
		 * Base layer is... well, the base. That's normal.
		 * Some things, like layers, come directly from the character that the puppet is based on.
		 * These things are going to stay essentially fixed once we set up the character.
		 * 
		 * Other things will just be added on based on commands.
		 * 
		 * Either way, we want a good system for creating various layers. This should be able
		 * to be done at run time.
		 *
		 * So I guess what we can do is create a child game object for each... layer type?
		 * This way, things can be sorted correctly.
		 *
		 * But to make the sorting be controlled, we'll make them in the editor.
		 * Then we can go get them at runtime and then use those as the parents.
		 * 
		 * We can add things onto the layer, and wipe out everything on the layer
		 * as we please.
		 * 
		 */

		#pragma warning disable CS0649

		[SerializeField] private float movementSpeed = 5f;

		[Header("Layers")]
		[Tooltip("This is the object that gets mirrored. All renderers (which we want to flip) should be a child of this one.")]
		[SerializeField] private RectTransform layerContainer;
		[SerializeField] private GameObject layerImagePrefab;

		[Space(5)]
		[SerializeField] private RectTransform baseLayer;
		[SerializeField] private RectTransform clothesLayer;
		[SerializeField] private RectTransform hairLayer;

		[Space(5)]
		[SerializeField] private RectTransform expressionLayer;
		[SerializeField] private RectTransform addonLayer;
		#pragma warning restore CS0649


		private PuppetPreset preset;
		private Facing defaultFacing = Facing.Left;

		private RectTransform rTransform;
		private float targetHorizontalPos;


		public void Configure(PuppetPreset preset)
		{
			this.preset = preset;
			layerContainer.sizeDelta = preset.size;

			//baseLayer.sprite = this.preset.baseImage;
			//baseRenderer.rectTransform.sizeDelta = preset.size;

			// Set up all of the generally wanted puppet things
			// (i.e. base, clothes, and hair)
			AddImageToLayer(baseLayer, preset.baseImage);
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
			Vector3 scale = layerContainer.localScale;
			scale.x = Mathf.Abs(scale.x) * (newFacing == defaultFacing ? 1 : -1);
			layerContainer.localScale = scale;
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

		private Image AddImageToLayer(RectTransform layer, Sprite layerSprite)
		{
			GameObject newImageObj = Instantiate(layerImagePrefab, layer) as GameObject;
			newImageObj.name = (layerSprite != null ? layerSprite.name : layer.name + "IMG");

			Image newImageComp = newImageObj.GetComponent<Image>();
			newImageComp.sprite = layerSprite;

			return newImageComp;
		}

		private void ClearLayer(RectTransform layer)
		{
			foreach(Transform imgObj in layer)
			{
				Destroy(imgObj.gameObject);
			}
		}

		private void Awake()
		{
			rTransform = GetComponent<RectTransform>();
			Assert.IsNotNull(rTransform, "Puppets should be part of the UI, not in the scene itself!");

			Assert.IsNotNull(layerImagePrefab.GetComponent<Image>(), "The layer prefab should have an image.");
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
