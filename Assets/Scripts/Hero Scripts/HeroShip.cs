using UnityEngine;
using System.Collections;

public class HeroShip : ShipFunctions
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem, canTeleport;
	private string invasionWeapon;
	private int system, gridChildren;

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

			if(systemListConstructor.systemList[system].systemOwnedBy == enemyOneTurnScript.playerRace || systemListConstructor.systemList[system].systemOwnedBy == enemyTwoTurnScript.playerRace)
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

		shipFunctions.UpdateShips ();
		heroScript.primaryPower = shipFunctions.primaryWeaponPower;
		heroScript.armour = shipFunctions.armourRating;
		heroScript.movementSpeed = shipFunctions.engineValue;

		if(heroScript.heroTier2 == "Diplomat")
		{
			invasionWeapon = "Dropships";

			heroScript.secondaryPower = shipFunctions.dropshipPower;
			heroScript.secondaryCollateral = shipFunctions.dropshipCollateral;

			if(heroScript.heroTier3 == "Smuggler")
			{
				heroScript.tradeRoutes = shipFunctions.logisticsRating;
			}

			DiplomatAbilities(system);
		}

		if(heroScript.heroTier2 == "Infiltrator")
		{
			invasionWeapon = "Bombs";

			heroScript.secondaryPower = shipFunctions.bombPower;
			heroScript.secondaryCollateral = shipFunctions.bombCollateral;

			canViewSystem = true;

			systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

			if(systemListConstructor.systemList[system].systemOwnedBy != playerTurnScript.playerRace)
			{
				if(shipFunctions.stealthValue >= systemSIMData.antiStealthPower)
				{
					hasStealth = true;
				}

				else
				{
					hasStealth = false;
				}
			}

			if(shipFunctions.infiltratorEngine == true)
			{
				canTeleport = true;
			}
		}

		if(heroScript.heroTier2 == "Soldier")
		{
			invasionWeapon = "Artillery";

			heroScript.secondaryPower = shipFunctions.artilleryPower;
			heroScript.secondaryCollateral = shipFunctions.artilleryCollateral;

			if(shipFunctions.soldierPrimary == true)
			{
				heroScript.primaryPower = heroScript.primaryPower * 2;
			}
		}
	}

	public void DiplomatAbilities(int system)
	{
		DiplomaticPosition position = null;

		if(systemListConstructor.systemList[system].systemOwnedBy == enemyOneTurnScript.playerRace)
		{
			position = diplomacyScript.playerEnemyOneRelations;
		}
		
		if(systemListConstructor.systemList[system].systemOwnedBy == enemyTwoTurnScript.playerRace)
		{
			position = diplomacyScript.playerEnemyTwoRelations;
		}

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
