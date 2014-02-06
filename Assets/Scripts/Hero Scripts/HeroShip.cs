using UnityEngine;
using System.Collections;

public class HeroShip : ShipFunctions
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private string invasionWeapon, auxiliaryWeapon;
	private int system;

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroMovement = gameObject.GetComponent<HeroMovement> ();
	}

	void Update()
	{
		if(heroMovement.TestForProximity (gameObject.transform.position, heroMovement.HeroPositionAroundStar(heroScript.heroLocation)) == true)
		{
			system = RefreshCurrentSystem(heroScript.heroLocation);

			if(systemListConstructor.systemList[system].systemOwnedBy == enemyOneTurnScript.playerRace || systemListConstructor.systemList[system].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				NGUITools.SetActive(heroGUI.invasionButton, true);

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
			}
		}

		else
		{
			NGUITools.SetActive(heroGUI.invasionButton, false);
			NGUITools.SetActive(heroGUI.embargoButton, false);
		}
	}

	public void ShipAbilities()
	{
		system = RefreshCurrentSystem(heroScript.heroLocation);

		if(heroScript.heroTier2 == "Diplomat")
		{
			invasionWeapon = "Dropships";

			if(systemListConstructor.systemList[system].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				DiplomatAbilities(diplomacyScript.playerEnemyOneRelations);
			}

			if(systemListConstructor.systemList[system].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				DiplomatAbilities(diplomacyScript.playerEnemyTwoRelations);
			}
		}

		if(heroScript.heroTier2 == "Infiltrator")
		{
			invasionWeapon = "Bombs";

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
		}

		if(heroScript.heroTier2 == "Soldier")
		{
			invasionWeapon = "Artillery";
		}
	}

	void DiplomatAbilities(DiplomaticPosition position)
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
