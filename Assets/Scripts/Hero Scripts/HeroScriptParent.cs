using UnityEngine;
using System.Collections;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, linkedHeroObject = null, clone;
	public int currentLevel = 1, thisHeroNumber;
	public float heroSciBonus = 0, heroIndBonus = 0, heroMonBonus = 0;
	public string heroTier2, heroTier3;
	private Vector3 position;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	public void Update()
	{
		if(linkedHeroObject != null)
		{
			Destroy(clone);

			float distance = Vector3.Distance(gameObject.transform.position, linkedHeroObject.transform.position);
			
			float rotationZRad = Mathf.Acos ((linkedHeroObject.transform.position.y - gameObject.transform.position.y) / distance);
			
			float rotationZ = rotationZRad * Mathf.Rad2Deg;

			if(gameObject.transform.position.x < linkedHeroObject.transform.position.x)
			{
				rotationZ = -rotationZ;
			}
			
			Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);
			
			Vector3 midPoint = (gameObject.transform.position + linkedHeroObject.transform.position)/2;
			
			Vector3 scale = new Vector3(0.2f, distance, 0.0f);
			
			Quaternion directQuat = new Quaternion();
			
			directQuat.eulerAngles = rotation;
			
			clone = (GameObject)Instantiate (heroGUIScript.merchantQuad, midPoint, directQuat);
			
			clone.transform.localScale = scale;
		}
	}

	public Vector3 HeroPositionAroundStar()
	{
		if(thisHeroNumber == 0)
		{
			position.x = heroLocation.transform.position.x;
			position.y = heroLocation.transform.position.y + 1.5f;
		}

		if(thisHeroNumber == 1)
		{
			position.x = heroLocation.transform.position.x + 0.75f;
			position.y = heroLocation.transform.position.y - 0.5f;
		}

		if(thisHeroNumber == 2)
		{
			position.x = heroLocation.transform.position.x - 0.75f;
			position.y = heroLocation.transform.position.y - 0.5f;
		}

		position.z = heroLocation.transform.position.z;

		return position;
	}

	public void HeroBonusFunction()
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
				heroGUIScript.selectedHero = thisHeroNumber;
				tier3HeroScript.CheckTier3Heroes ();
			}
		}
	}

	public void LevelUp()
	{
		heroGUIScript.selectedHero = thisHeroNumber;
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


