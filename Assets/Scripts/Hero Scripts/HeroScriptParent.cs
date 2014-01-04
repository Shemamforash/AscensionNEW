using UnityEngine;
using System.Collections;

public class HeroScriptParent : MasterScript 
{
	public GameObject[] allLinkableSystems = new GameObject[60];
	public float heroSciBonus, heroIndBonus, heroMonBonus;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
	}

	public void CheckHeroesInSystem()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(heroesInSystem[i] == null)
			{
				continue;
			}

			if(heroesInSystem[i].name == "Merchant")
			{
				merchantScript = heroesInSystem[i].GetComponent<MerchantHeroScript>();
				merchantScript.Merchant();
				continue;
			}

			if(heroesInSystem[i].name == "President")
			{
				President ();
				continue;
			}
		}
	}

	private void President()
	{
		techTreeScript.sciencePercentBonus += 0.05f;
		techTreeScript.industryPercentBonus += 0.05f;
		techTreeScript.moneyPercentBonus += 0.05f;

		foreach(GameObject system in lineRenderScript.connections)
		{
			if(system == null)
			{
				break;
			}

			heroScript = system.GetComponent<HeroScriptParent>();

			int presidentNum = 0;

			foreach(GameObject hero in heroScript.heroesInSystem)
			{
				if(hero.name == "President")
				{
					if(presidentNum > 0 && techTreeScript.leadership == false)
					{
						break;
					}

					else
					{
						techTreeScript.sciencePercentBonus += 0.025f;
						techTreeScript.industryPercentBonus += 0.025f;
						techTreeScript.moneyPercentBonus += 0.025f;
					}

					presidentNum++;
				}
			}
		}
	}
}
