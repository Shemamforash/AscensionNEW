using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, linkedHeroObject = null, merchantLine, invasionObject;
	public HeroDetailsWindow heroDetails;
	public int currentLevel = 1, noOfColonisedPlanets, heroAge, movementPoints;
	public float heroSciBonus = 0, heroIndBonus = 0, offensivePower = 14.0f, defensivePower = 14.0f, invasionStrength, speed;
	public string heroTier2, heroTier3, heroOwnedBy, heroShipType;
	public bool isInvading = false, canLevelUp;
	private GameObject levelUpLabel;

	void Start()
	{
		offensivePower = 14.0f;
		heroAge = 0;

		shipFunctions = gameObject.GetComponent<ShipFunctions> ();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroShip = gameObject.GetComponent<HeroShip> ();

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

		movementPoints = 1;

		heroGUI.heroDetailsContainer.GetComponent<UIGrid> ().enabled = true;

		heroDetails = new HeroDetailsWindow ();

		heroDetails.window = NGUITools.AddChild (heroGUI.heroDetailsContainer, heroGUI.heroDetailsPrefab);

		heroDetails.dropDownOne = heroDetails.window.transform.Find ("First Specialisation").gameObject.GetComponent<UIPopupList>();
		EventDelegate.Add (heroDetails.dropDownOne.gameObject.GetComponent<UIPopupList> ().onChange, heroGUI.SetSpecialisation);
		heroDetails.dropDownTwo = heroDetails.window.transform.Find ("Second Specialisation").gameObject.GetComponent<UIPopupList>();
		EventDelegate.Add (heroDetails.dropDownTwo.gameObject.GetComponent<UIPopupList> ().onChange, heroGUI.SetSpecialisation);

		heroGUI.heroDetailsContainer.GetComponent<UIGrid>().repositionNow = true;

		NGUITools.SetActive (heroDetails.window, false);
	}

	void Update()
	{
		if(levelUpLabel != null)
		{
			Vector3 position = cameraFunctionsScript.cameraMain.WorldToViewportPoint (gameObject.transform.position);
			
			position = overlayGUI.uiCamera.ViewportToWorldPoint (position);
			
			Vector3 newPosition = new Vector3(position.x, position.y, -37.0f);
			
			levelUpLabel.transform.position = newPosition;
		}
	}

	public DiplomaticPosition FindDiplomaticConnection()
	{
		int i = RefreshCurrentSystem (heroLocation);

		if(heroOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyTwoRelations;
			}
		}

		if(heroOwnedBy == enemyOneTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				return diplomacyScript.enemyOneEnemyTwoRelations;
			}
		}

		if(heroOwnedBy == enemyTwoTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyTwoRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				return diplomacyScript.enemyOneEnemyTwoRelations;
			}
		}

		return null;
	}

	public void CreateConnectionLine()
	{
		if(heroMovement.heroIsMoving == true && merchantLine != null)
		{
			Destroy(merchantLine);
		}

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
		
		merchantLine = (GameObject)Instantiate (heroGUI.merchantQuad, midPoint, directQuat);
		
		merchantLine.transform.localScale = scale;
	}

	public void HeroEndTurnFunctions()
	{
		heroSciBonus = 10.0f; 
		heroIndBonus = 10.0f;

		offensivePower = 14.0f;
		defensivePower = 7.0f;

		if(heroTier2 != "")
		{
			tier2HeroScript.CheckTier2Heroes (gameObject);

			if(heroTier3 != "")
			{
				heroGUI.selectedHero = gameObject;

				tier3HeroScript.CheckTier3Heroes (gameObject);
			}
		}

		if(isInvading == true)
		{
			ContinueInvasion();
		}

		heroAge++;

		if(heroAge == 10 || heroAge == 20)
		{
			levelUpLabel = NGUITools.AddChild(heroGUI.buttonContainer, heroGUI.levelUpPrefab);

			levelUpLabel.transform.Find ("Label").GetComponent<UILabel>().depth = 1;

			EventDelegate.Add(levelUpLabel.GetComponent<UIButton>().onClick, LevelUp);
		}

		movementPoints += 4;

		heroShip.ShipAbilities ();
	}

	public void LevelUp()
	{
		NGUITools.Destroy (UIButton.current.gameObject);
		heroGUI.selectedHero = gameObject;
		++heroScript.currentLevel;
		canLevelUp = true;
		heroGUI.OpenHeroDetails();
	}

	public void StartSystemInvasion()
	{
		int i = RefreshCurrentSystem (heroLocation);
		
		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
			{
				continue;
			}
			
			++noOfColonisedPlanets;
		}
		
		invasionStrength = offensivePower / noOfColonisedPlanets;
		
		isInvading = true;
		
		invasionObject = (GameObject)Instantiate (diplomacyScript.invasionQuad, systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[i].systemObject.transform.rotation);
		
		systemSIMData = systemListConstructor.systemList [i].systemObject.GetComponent<SystemSIMData> ();
		
		systemSIMData.underInvasion = true;
	}
	
	public void ContinueInvasion()
	{
		int i = RefreshCurrentSystem (heroScript.heroLocation);
		
		bool planetsRemaining = false;
		
		invasionStrength = offensivePower / noOfColonisedPlanets;
		
		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
			{
				continue;
			}
			
			systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership -= (int)invasionStrength;
			
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership < 0)
			{
				WipePlanetInfo(i, j);
			}
		}
		
		noOfColonisedPlanets = 0;
		
		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
			{
				planetsRemaining = true;
				++noOfColonisedPlanets;
			}
		}
		
		if(planetsRemaining == false)
		{
			isInvading = false;
			
			systemListConstructor.systemList[i].systemOwnedBy = null;
			
			systemListConstructor.systemList[i].tradeRoute = null;
			
			Destroy (invasionObject);
			
			lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();
			
			lineRenderScript.SetRaceLineColour("None");
			
			systemListConstructor.systemList[i].systemObject.renderer.material = diplomacyScript.unownedMaterial;
			
			systemSIMData = systemListConstructor.systemList [i].systemObject.GetComponent<SystemSIMData> ();
			
			systemSIMData.underInvasion = false;
		}
	}
}


