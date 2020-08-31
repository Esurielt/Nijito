using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dialogue.VN
{
	public class PuppetMaster : MonoBehaviour
	{
		public string puppetPresetPath = "VN/PuppetPresets";
		public GameObject puppetPrefab;
		public RectTransform puppetSpawnPoint;

		[SerializeField]
		private bool debugLoading = false;

		private Dictionary<string, Puppet> puppets;
		private Dictionary<string, PuppetPreset> presets;

		private void Awake()
		{
			puppets = new Dictionary<string, Puppet>();

			Assert.IsNotNull(
				puppetPrefab.GetComponent<Puppet>(),
				"Puppet prefab must have the Dialogue.VN.Puppet component attached to it!"
			);

			presets = new Dictionary<string, PuppetPreset>();

			//PuppetCostume[] costumeArray = Resources.FindObjectsOfTypeAll<PuppetCostume>();
			PuppetPreset[] presetArray = Resources.LoadAll<PuppetPreset>(puppetPresetPath);
			foreach(PuppetPreset preset in presetArray)
			{
				presets.Add(preset.name, preset);

				if(debugLoading)
				{
					Debug.Log("Loaded preset: " + preset.name);
				}
			}

			if(debugLoading)
			{
				Debug.Log("Loaded " + presets.Count + " presets!");
			}
		}

		public Puppet GetPuppet(string characterName)
		{
			Puppet result;
			if(!puppets.TryGetValue(characterName, out result))
			{
				result = MakePuppet(characterName);
				puppets.Add(characterName, result);
			}

			return result;
		}

		private Puppet MakePuppet(string characterName)
		{
			GameObject newPuppetObj = Instantiate( puppetPrefab, transform );

			Puppet newPuppet = newPuppetObj.GetComponent<Puppet>();
			newPuppet.Warp(puppetSpawnPoint);

			PuppetPreset costume;
			if(presets.TryGetValue(characterName, out costume))
			{
				newPuppet.Configure(costume);
			}
			else
			{
				Debug.LogWarning("No preset for character named " + characterName);
			}

			return newPuppet;
		}

	}
}
