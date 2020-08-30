using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dialogue.VN
{
	public class PuppetMaster : MonoBehaviour
	{
		public GameObject puppetPrefab;
		public RectTransform puppetSpawnPoint;

		private Dictionary<string, Puppet> puppets;
		private Dictionary<string, PuppetPreset> costumes;

		private void Awake()
		{
			puppets = new Dictionary<string, Puppet>();

			Assert.IsNotNull(
				puppetPrefab.GetComponent<Puppet>(),
				"Puppet prefab must have the Dialogue.VN.Puppet component attached to it!"
			);

			costumes = new Dictionary<string, PuppetPreset>();

			//PuppetCostume[] costumeArray = Resources.FindObjectsOfTypeAll<PuppetCostume>();
			PuppetPreset[] costumeArray = Resources.LoadAll<PuppetPreset>("VisualNovel/PuppetPresets");
			foreach(PuppetPreset costume in costumeArray)
			{
				costumes.Add(costume.name, costume);
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
			if(costumes.TryGetValue(characterName, out costume))
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
