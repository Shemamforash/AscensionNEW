using UnityEngine;
using System.Collections;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation;
	public int currentLevel = 1;
	public float heroSciBonus = 10, heroIndBonus = 10, heroMonBonus = 10;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
	}

	public void LevelUp()
	{
		if(playerTurnScript.GP > 0)
		{
			--playerTurnScript.GP;
			heroGUIScript.openHeroLevellingScreen = true;
			heroGUIScript.selectedHero = gameObject;
		}
	}

	/*private void President()
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
	}*/
}
