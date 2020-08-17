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

		Dictionary<string, Puppet> puppets;

		private void Awake()
		{
			puppets = new Dictionary<string, Puppet>();

			Assert.IsNotNull(
				puppetPrefab.GetComponent<Puppet>(),
				"Puppet prefab must have the Dialogue.VN.Puppet component attached to it!"
			);

			// TODO Load character information from resources
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

			// TODO Configure character stuff.
			newPuppetObj.name = characterName;	// TODO Puppet should be able to set this itself.

			return newPuppet;
		}

	}
}
