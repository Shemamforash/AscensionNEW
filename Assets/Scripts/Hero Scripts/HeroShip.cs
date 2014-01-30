using UnityEngine;
using System.Collections;

public class HeroShip : ShipFunctions
{
	private bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private string invasionWeapon, secondaryBonus;
	private int system;

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	void ShipAbilities()
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
