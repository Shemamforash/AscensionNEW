using UnityEngine;
using System.Collections;

public class HeroScript : MasterScript 
{
	public string[] heroesInSystem = new string[3];
	public string[] allLinkableSystems = new string[60];
	public string linked = null;

	public float heroSciBonus, heroIndBonus, heroMonBonus;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
	}

	public void CheckHeroesInSystem()
	{
		foreach(string hero in heroesInSystem)
		{
			switch(hero)
			{
			case "President":
				President ();
				break;

			case "Merchant":
				Merchant();
				break;

			default:
				break;
			}
		}
	}

	public void AddMerchant()
	{
		for(int i = 0; i < 60; ++i)
		{
			if(turnInfoScript.systemList[i] == null)
			{
				continue;
			}

			string tempSystem = turnInfoScript.systemList[i].name;
			
			lineRenderScript = GameObject.Find (tempSystem).GetComponent<LineRenderScript>();
			
			if(lineRenderScript.ownedBy != null)
			{
				heroScript = GameObject.Find (tempSystem).GetComponent<HeroScript>();
				
				for(int j = 0; j < 3; ++j)
				{
					if(heroScript.heroesInSystem[j] == "Merchant")
					{
						allLinkableSystems[i] = tempSystem;
					}
				}
			}
		}
	}

	private void Merchant()
	{
		if(linked != null)
		{
			guiPlanScript = GameObject.Find (linked).GetComponent<GUISystemDataScript>();

			heroSciBonus = guiPlanScript.tempTotalSci / 2;
			heroIndBonus = guiPlanScript.tempTotalInd / 2;
			heroMonBonus = guiPlanScript.tempTotalMon / 2;
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

			heroScript = system.GetComponent<HeroScript>();

			int presidentNum = 0;

			foreach(string hero in heroScript.heroesInSystem)
			{
				if(hero == "President")
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
