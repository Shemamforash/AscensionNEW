using UnityEngine;
using System.Collections;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, linkedHeroObject = null, merchantLine, invasionObject;
	public int currentLevel = 1, thisHeroNumber, noOfColonisedPlanets;
	public float heroSciBonus = 0, heroIndBonus = 0, heroMonBonus = 0, offensivePower = 14.0f, defensivePower, invasionStrength;
	public string heroTier2, heroTier3, heroOwnedBy;
	private Vector3 position;
	public bool isInvading = false;

	void Start()
	{
		offensivePower = 14.0f;

		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		int i = RefreshCurrentSystem (heroLocation);

		if (systemListConstructor.systemList [i].systemOwnedBy == playerTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		}
		if (systemListConstructor.systemList [i].systemOwnedBy == enemyOneTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		}
		if (systemListConstructor.systemList [i].systemOwnedBy == enemyTwoTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		}
	}

	public void Update()
	{
		if(linkedHeroObject != null && heroGUIScript.heroIsMoving == true)
		{
			Destroy(merchantLine);

			CreateConnectionLine();
		}
	}

	public void CreateConnectionLine()
	{
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
		
		merchantLine = (GameObject)Instantiate (heroGUIScript.merchantQuad, midPoint, directQuat);
		
		merchantLine.transform.localScale = scale;
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

				tier3HeroScript.CheckTier3Heroes (heroScript);
			}
		}

		if(isInvading == true)
		{
			ContinueInvasion();
		}
	}

	public void LevelUp()
	{
		heroGUIScript.selectedHero = thisHeroNumber;
		heroGUIScript.openHeroLevellingScreen = true;
	}

	public void StartSystemInvasion()
	{
		int i = RefreshCurrentSystem (heroLocation);

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == false)
			{
				continue;
			}

			++noOfColonisedPlanets;
		}

		invasionStrength = offensivePower / noOfColonisedPlanets;

		isInvading = true;

		invasionObject = (GameObject)Instantiate (heroGUIScript.invasionQuad, systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[i].systemObject.transform.rotation);
	}

	private void ContinueInvasion()
	{
		int i = RefreshCurrentSystem (heroLocation);

		bool planetsRemaining = false;

		invasionStrength = offensivePower / noOfColonisedPlanets;

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == false)
			{
				continue;
			}

			systemListConstructor.systemList[i].planetOwnership[j] -= (int)invasionStrength;

			if(systemListConstructor.systemList[i].planetOwnership[j] < 0)
			{
				systemListConstructor.systemList[i].planetColonised[j] = false;
				systemListConstructor.systemList[i].planetImprovementLevel[j] = 0;
				--noOfColonisedPlanets;
			}

			planetsRemaining = true;
		}

		if(planetsRemaining == false)
		{
			isInvading = false;

			systemListConstructor.systemList[i].systemOwnedBy = playerTurnScript.playerRace;
			systemListConstructor.systemList[i].tradeRoute = null;

			Destroy (invasionObject);

			lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();

			lineRenderScript.SetRaceLineColour("None");

			systemListConstructor.systemList[i].systemObject.renderer.material = heroGUIScript.unownedMaterial;
		}
	}
}


