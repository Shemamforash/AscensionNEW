using UnityEngine;
using System.Collections;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation,  linked = null;
	public int currentLevel = 1;
	public float heroSciBonus = 0, heroIndBonus = 0, heroMonBonus = 0;
	public string heroTier2, heroTier3;
	private Vector3 position;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	public void HeroPositionAroundStar()
	{
		int i = 0;

		for (i = 0; i < 3; ++i) 
		{
			int j = RefreshCurrentSystem(heroLocation);

			if(systemListConstructor.systemList[j].heroesInSystem[i] == gameObject)
			{
				break;
			}
		}

		if(i == 0)
		{
			position.x = gameObject.transform.position.x;
			position.y = gameObject.transform.position.y + 1.5f;
		}

		if(i == 1)
		{
			position.x = gameObject.transform.position.x + 0.75f;
			position.y = gameObject.transform.position.y - 0.5f;
		}

		if(i == 2)
		{
			position.x = gameObject.transform.position.x - 0.75f;
			position.y = gameObject.transform.position.y - 0.5f;
		}

		position.z = gameObject.transform.position.z;

		gameObject.transform.position = position;
	}

	public virtual void HeroBonusFunction()
	{
		heroSciBonus += 10; 
		heroIndBonus += 10;
		heroMonBonus += 10;
	}

	public void HeroEndTurnFunctions()
	{
		heroSciBonus = 0; 
		heroIndBonus = 0;
		heroMonBonus = 0;

		HeroBonusFunction ();

		if(heroTier2 != "")
		{
			tier2HeroScript.CheckTier2Heroes (gameObject);

			if(heroTier3 != "")
			{
				tier3HeroScript.selectedHero = gameObject;
				tier3HeroScript.CheckTier3Heroes ();
			}
		}
	}

	public void LevelUp()
	{
		heroGUIScript.selectedHero = gameObject;
		heroGUIScript.openHeroLevellingScreen = true;
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


