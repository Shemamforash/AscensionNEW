using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroShip : MasterScript
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private string invasionWeapon;
	private int system, gridChildren;
	private List<TradeRoute> allTradeRoutes = new List<TradeRoute>();

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroMovement = gameObject.GetComponent<HeroMovement> ();
	}

	void Update()
	{
		int tempChildren = 0;

		if(heroMovement.TestForProximity (gameObject.transform.position, heroMovement.HeroPositionAroundStar(heroScript.heroLocation)) == true)
		{
			system = RefreshCurrentSystem(heroScript.heroLocation);

			if(systemListConstructor.systemList[system].systemOwnedBy != playerTurnScript.playerRace && systemListConstructor.systemList[system].systemOwnedBy != null)
			{
				NGUITools.SetActive(heroGUI.invasionButton, true);

				++tempChildren;

				if(canViewSystem == true)
				{
					heroGUI.invasionButton.GetComponent<UILabel>().text = "Enter System";
				}
				else
				{
					heroGUI.invasionButton.GetComponent<UILabel>().text = "Invade System";
				}
			}

			if(canEmbargo == true)
			{
				NGUITools.SetActive(heroGUI.embargoButton, true);

				++tempChildren;
			}
			if(canPromote == true)
			{
				NGUITools.SetActive(heroGUI.promoteButton, true);

				++tempChildren;
			}
		}

		else
		{
			NGUITools.SetActive(heroGUI.invasionButton, false);
			NGUITools.SetActive(heroGUI.embargoButton, false);
			NGUITools.SetActive(heroGUI.promoteButton, false);
		}

		if(tempChildren != gridChildren)
		{
			gridChildren = tempChildren;

			float gridWidth = (gridChildren * heroGUI.buttonContainer.GetComponent<UIGrid>().cellWidth) / 2 - (heroGUI.buttonContainer.GetComponent<UIGrid>().cellWidth/2);
			
			heroGUI.buttonContainer.transform.localPosition = new Vector3(systemGUI.playerSystemInfoScreen.transform.localPosition.x - gridWidth, 
			                                                         heroGUI.turnInfoBar.transform.localPosition.y + 50.0f, 
			                                                         systemGUI.planetListGrid.transform.localPosition.z);
			
			heroGUI.buttonContainer.GetComponent<UIGrid>().repositionNow = true;
		}
	}

	public void ShipAbilities()
	{
		system = RefreshCurrentSystem(heroScript.heroLocation);

		ShipFunctions.UpdateShips ();
		heroScript.primaryPower = ShipFunctions.primaryWeaponPower;
		heroScript.armour = ShipFunctions.armourRating;
		heroScript.movementSpeed = ShipFunctions.engineValue;

		if(heroScript.heroTier2 == "Diplomat")
		{
			invasionWeapon = "Dropships";

			heroScript.secondaryPower = ShipFunctions.dropshipPower;
			heroScript.secondaryCollateral = ShipFunctions.dropshipCollateral;

			if(heroScript.heroTier3 == "Merchant")
			{
				int numberOfMerchants = 0;

				for(int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; i++)
				{
					HeroScriptParent tempScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

					if(tempScript.heroTier3 == "Merchant")
					{
						++numberOfMerchants;
					}
				}

				MerchantFunctions((ShipFunctions.logisticsRating + 1) * numberOfMerchants);
			}

			DiplomatAbilities(system);
		}

		if(heroScript.heroTier2 == "Infiltrator")
		{
			invasionWeapon = "Bombs";

			heroScript.secondaryPower = ShipFunctions.bombPower;
			heroScript.secondaryCollateral = ShipFunctions.bombCollateral;

			canViewSystem = true;

			systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

			if(systemListConstructor.systemList[system].systemOwnedBy != playerTurnScript.playerRace)
			{
				if(ShipFunctions.stealthValue >= systemSIMData.antiStealthPower)
				{
					hasStealth = true;
				}

				else
				{
					hasStealth = false;
				}
			}

			if(ShipFunctions.infiltratorEngine == true)
			{
				heroScript.movementSpeed = 1000;
			}
		}

		if(heroScript.heroTier2 == "Soldier")
		{
			invasionWeapon = "Artillery";

			heroScript.secondaryPower = ShipFunctions.artilleryPower;
			heroScript.secondaryCollateral = ShipFunctions.artilleryCollateral;

			if(ShipFunctions.soldierPrimary == true)
			{
				heroScript.primaryPower = heroScript.primaryPower * 2;
			}
		}
	}

	public void DiplomatAbilities(int system)
	{
		DiplomaticPosition position = null;

		if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[0].playerRace)
		{
			position = diplomacyScript.playerEnemyOneRelations;
		}

		if(turnInfoScript.allPlayers.Count > 1)
		{
			if(systemListConstructor.systemList[system].systemOwnedBy == turnInfoScript.allPlayers[1].playerRace)
			{
				position = diplomacyScript.playerEnemyTwoRelations;
			}
		}

		if(position != null)
		{
			if(position.diplomaticState == "War")
			{
				canPromote = false;
				canEmbargo = true;
			}
			if(position.diplomaticState == "Cold War")
			{
				canPromote = true;
				canEmbargo = true;
			}
			if(position.diplomaticState == "Peace")
			{
				canPromote = true;
				canEmbargo = false;
			}
		}
	}

	private void MakeNewTradeRoutes()
	{
		float tempIndSci = 0;
		int chosenEnemySystem = -1, chosenPlayerSystem = -1;

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j) //For all enemy players
			{
				if(systemListConstructor.systemList[i].systemOwnedBy == turnInfoScript.allPlayers[j].playerRace) //If system is owned by enemy
				{
					for(int k = 0; k < systemListConstructor.systemList[i].permanentConnections.Count; ++k) //For all connections in system
					{
						int system = RefreshCurrentSystem(systemListConstructor.systemList[i].permanentConnections[k]);

						if(systemListConstructor.systemList[system].systemOwnedBy == playerTurnScript.playerRace) //If connection is owned by player
						{
							bool skip = false;

							for(int l = 0; l < allTradeRoutes.Count; ++l)
							{
								if(allTradeRoutes[l].playerSystem == systemListConstructor.systemList[system].systemObject)
								{
									skip = true;
								}
							}

							if(skip == false)
							{
								systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

								float temp = systemSIMData.totalSystemIndustry + systemSIMData.totalSystemScience; //Get the system output

								if(temp > tempIndSci) //If its larger than the previous output
								{
									chosenEnemySystem = i; //Set the enemy system to connect to this system
								}
							}
						}
					}
				}
			}
		}

		if(chosenEnemySystem != -1)
		{
			tempIndSci = 0;

			for(int i = 0; i < systemListConstructor.systemList[chosenEnemySystem].permanentConnections.Count; ++i) //For all connections in enemy system
			{
				int system = RefreshCurrentSystem(systemListConstructor.systemList[chosenEnemySystem].permanentConnections[i]);
				
				if(systemListConstructor.systemList[system].systemOwnedBy == playerTurnScript.playerRace) //If connection is owned by player
				{
					bool skip = false;

					for(int l = 0; l < allTradeRoutes.Count; ++l)
					{
						if(allTradeRoutes[l].playerSystem == systemListConstructor.systemList[system].systemObject)
						{
							skip = true;
						}
					}

					Debug.Log (chosenEnemySystem);

					if(skip == false)
					{
						systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();
						
						float temp = systemSIMData.totalSystemIndustry + systemSIMData.totalSystemScience; //Get the system output
						
						if(temp >= tempIndSci) //If its larger than the previous output
						{
							chosenPlayerSystem = system; //Set the player system to connect to this system
						}
					}
				}
			}

			TradeRoute route = new TradeRoute();
			
			route.playerSystem = systemListConstructor.systemList[chosenPlayerSystem].systemObject;
			route.enemySystem = systemListConstructor.systemList[chosenEnemySystem].systemObject;
			
			heroScript.CreateConnectionLine(route.playerSystem, route.enemySystem);
			
			allTradeRoutes.Add (route);
		}
	}

	public void MerchantFunctions(int links)
	{
		Debug.Log (links);

		for(int i = 0; i < allTradeRoutes.Count; ++i)
		{
			int pSys = RefreshCurrentSystem(allTradeRoutes[i].playerSystem);

			if(systemListConstructor.systemList[pSys].systemOwnedBy != playerTurnScript.playerRace)
			{
				int eSys = RefreshCurrentSystem(allTradeRoutes[i].enemySystem);
				bool notEnemyOwned = false;

				for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j)
				{
					if(systemListConstructor.systemList[eSys].systemOwnedBy != turnInfoScript.allPlayers[i].playerRace)
					{
						notEnemyOwned = true;
						break;
					}
				}

				if(notEnemyOwned == true)
				{
					GameObject.Destroy(allTradeRoutes[i].connectorObject);
					allTradeRoutes.RemoveAt(i);
				}
			}
		}

		for(int i = 0; i < links; ++i)
		{
			if(i < allTradeRoutes.Count)
			{
				int pSys = RefreshCurrentSystem(allTradeRoutes[i].playerSystem);
				int eSys = RefreshCurrentSystem(allTradeRoutes[i].enemySystem);

				SystemSIMData pSysData = systemListConstructor.systemList[pSys].systemObject.GetComponent<SystemSIMData>();
				SystemSIMData eSysData = systemListConstructor.systemList[eSys].systemObject.GetComponent<SystemSIMData>();

				float pIndTransfer = pSysData.totalSystemIndustry / 2;
				float pSciTransfer = pSysData.totalSystemScience / 2;

				Debug.Log (pIndTransfer);

				float eIndTransfer = eSysData.totalSystemIndustry / 2;
				float eSciTransfer = eSysData.totalSystemScience / 2;

				eSysData.totalSystemIndustry += pIndTransfer;
				eSysData.totalSystemScience += pSciTransfer;
				pSysData.totalSystemIndustry += eIndTransfer;
				pSysData.totalSystemScience += eSciTransfer;
			}

			else if(i >= allTradeRoutes.Count)
			{
				MakeNewTradeRoutes();
			}
		}
	}
}

public class TradeRoute
{
	public GameObject playerSystem, enemySystem, connectorObject;
}
