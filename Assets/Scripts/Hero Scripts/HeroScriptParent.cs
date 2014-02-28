using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, linkedHeroObject = null, merchantLine, invasionObject;
	public HeroDetailsWindow heroDetails;
	public int currentLevel = 1, movementSpeed, planetInvade = -1, system;
	public int primaryPower, secondaryPower, secondaryCollateral, invasionStrength, armour;
	public string heroTier2, heroTier3, heroOwnedBy, heroShipType;
	public bool isInvading = false, canLevelUp, reachedLevel2, reachedLevel3;
	private float heroAge;
	private GameObject levelUpLabel;

	void Start()
	{
		heroAge = Time.time;
	
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroShip = gameObject.GetComponent<HeroShip> ();

		system = RefreshCurrentSystem (heroLocation);

		movementSpeed = 1;

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
		system = RefreshCurrentSystem (heroLocation);

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
		if(heroOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[0].playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}

			if(turnInfoScript.allPlayers.Count > 1)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[1].playerRace)
				{
					return diplomacyScript.playerEnemyTwoRelations;
				}
			}
		}

		if(heroOwnedBy == turnInfoScript.allPlayers[0].playerRace)
		{
			if(systemListConstructor.systemList[system].systemOwnedBy == playerTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}
			if(turnInfoScript.allPlayers.Count > 1)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[1].playerRace)
				{
					return diplomacyScript.enemyOneEnemyTwoRelations;
				}
			}
		}

		if(turnInfoScript.allPlayers.Count > 1)
		{
			if(heroOwnedBy == turnInfoScript.allPlayers[1].playerRace)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == playerTurnScript.playerRace)
				{
					return diplomacyScript.playerEnemyTwoRelations;
				}
				if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[0].playerRace)
				{
					return diplomacyScript.enemyOneEnemyTwoRelations;
				}
			}
		}

		return null;
	}

	public void CreateConnectionLine(GameObject playerSys, GameObject enemySys)
	{
		float distance = Vector3.Distance(playerSys.transform.position, enemySys.transform.position);
		
		float rotationZRad = Mathf.Acos ((enemySys.transform.position.y - playerSys.transform.position.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(playerSys.transform.position.x < enemySys.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		Vector3 midPoint = (playerSys.transform.position + enemySys.transform.position)/2;
		
		Vector3 scale = new Vector3(0.2f, distance, 0.0f);
		
		Quaternion directQuat = new Quaternion();
		
		directQuat.eulerAngles = rotation;
		
		merchantLine = (GameObject)Instantiate (heroGUI.merchantQuad, midPoint, directQuat);
		
		merchantLine.transform.localScale = scale;
	}

	public void HeroEndTurnFunctions()
	{
		if(isInvading == true)
		{
			ContinueInvasion();

			if(systemDefence.canEnter == true && planetInvade != -1)
			{
				PlanetInvasion();
			}
		}

		if(heroAge + 6 <= Time.time && reachedLevel2 == false) 
		{
			reachedLevel2 = true;
			AddLevelUpDelegate();
		}

		if(heroAge + 7 <= Time.time && reachedLevel3 == false)
		{
			reachedLevel3 = true;
			AddLevelUpDelegate();
		}

		heroShip.ShipAbilities ();
	}

	private void AddLevelUpDelegate()
	{
		levelUpLabel = NGUITools.AddChild(heroGUI.buttonContainer, heroGUI.levelUpPrefab);
		
		levelUpLabel.transform.Find ("Label").GetComponent<UILabel>().depth = 1;
		
		EventDelegate.Add(levelUpLabel.GetComponent<UIButton>().onClick, LevelUp);
	}

	public void LevelUp()
	{
		NGUITools.Destroy (UIButton.current.gameObject);
		heroGUI.selectedHero = gameObject;
		++heroScript.currentLevel;
		canLevelUp = true;
		NGUITools.SetActive (heroGUI.heroDetailsContainer, false);
		heroGUI.OpenHeroDetails();
	}

	public void PlanetInvasion()
	{
		if(systemListConstructor.systemList [system].planetsInSystem [planetInvade].underEnemyControl == false)
		{
			systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetDefence -= secondaryPower;
			systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetOwnership -= secondaryCollateral;

			if(systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetOwnership <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetColonised = false;
				systemListConstructor.systemList [system].planetsInSystem [planetInvade].improvementsBuilt.Clear ();
				systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetImprovementLevel = 0;
				systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetOwnership = 0;
				planetInvade = -1;
			}
			else if(systemListConstructor.systemList [system].planetsInSystem [planetInvade].planetDefence <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planetInvade].underEnemyControl = true;
				planetInvade = -1;
			}
		}

		CheckSystemStatus ();
	}

	private void CheckSystemStatus()
	{
		bool systemDestroyed = true;
		bool systemEnemyControlled = true;

		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			if(systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == false)
			{
				continue;
			}

			if(systemListConstructor.systemList [system].planetsInSystem [i].underEnemyControl == false)
			{
				systemEnemyControlled = false;
			}

			if(systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == true)
			{
				systemDestroyed = false;
			}
		}

		if(systemDestroyed == true)
		{
			DestroySystem();
		}
		else if(systemEnemyControlled == true)
		{
			OwnSystem();
		}
	}

	private void DestroySystem()
	{
		systemListConstructor.systemList [system].systemDefence = 0;
		systemListConstructor.systemList [system].systemOwnedBy = null;

		turnInfoScript = GameObject.Find ("ScriptsContainer").GetComponent<TurnInfo> ();

		systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.emptyMaterial;

		lineRenderScript = systemListConstructor.systemList [system].systemObject.GetComponent<LineRenderScript> ();

		lineRenderScript.SetRaceLineColour ("None");

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		systemDefence.underInvasion = false;
	}

	private void OwnSystem()
	{
		systemListConstructor.systemList [system].systemOwnedBy = playerTurnScript.playerRace;
		systemListConstructor.systemList [system].systemObject.renderer.material = playerTurnScript.materialInUse;

		lineRenderScript = systemListConstructor.systemList [system].systemObject.GetComponent<LineRenderScript> ();
		techTreeScript = systemListConstructor.systemList [system].systemObject.GetComponent<TechTreeScript> ();
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemDefence.underInvasion = false;
		
		lineRenderScript.SetRaceLineColour (playerTurnScript.playerRace);

		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				for(int k = 0; k < techTreeScript.listOfImprovements.Count; ++k)
				{
					if(systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt[j] == techTreeScript.listOfImprovements[k].improvementName)
					{
						if(techTreeScript.listOfImprovements[k].improvementCategory != "Generic" || techTreeScript.listOfImprovements[k].improvementCategory != playerTurnScript.playerRace)
						{
							systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.RemoveAt(j);
							techTreeScript.listOfImprovements[k].hasBeenBuilt = false;
						}
					}
				}
			}
		}
	}

	public void StartSystemInvasion()
	{
		isInvading = true;
		
		invasionObject = (GameObject)Instantiate (diplomacyScript.invasionQuad, systemListConstructor.systemList[system].systemObject.transform.position, 
		                                          systemListConstructor.systemList[system].systemObject.transform.rotation);

		systemListConstructor.systemList [system].enemyHero = gameObject;

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = true;
	}
	
	public void ContinueInvasion()
	{		
		systemListConstructor.systemList [system].systemDefence -= primaryPower;

		if(systemListConstructor.systemList [system].systemDefence <= 0)
		{
			systemListConstructor.systemList [system].systemDefence = 0;
			systemDefence.canEnter = true;
			Destroy(invasionObject);
		}
	}
}


