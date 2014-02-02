using UnityEngine;
using System.Collections;

public class HeroShip : ShipFunctions
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private string invasionWeapon, auxiliaryWeapon;
	private int system;
	private GameObject invasionButtonObject;
	private UILabel invasionButtonLabel;

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroMovement = gameObject.GetComponent<HeroMovement> ();
		invasionButtonObject = GameObject.Find ("Invasion Button");
		invasionButtonLabel = invasionButtonObject.GetComponent<UILabel> ();
		NGUITools.SetActive (invasionButtonObject, false);
	}

	void Update()
	{
		if(heroMovement.TestForProximity (gameObject.transform.position, heroMovement.HeroPositionAroundStar(heroScript.heroLocation)) == true)
		{
			system = RefreshCurrentSystem(heroScript.heroLocation);

			if(systemListConstructor.systemList[system].systemOwnedBy == enemyOneTurnScript.playerRace || systemListConstructor.systemList[system].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				NGUITools.SetActive(invasionButtonObject, true);

				if(canViewSystem == true)
				{
					invasionButtonLabel.text = "Enter System";
				}
				else
				{
					invasionButtonLabel.text = "Invade System";
				}
			}
		}

		else
		{
			NGUITools.SetActive(invasionButtonObject, false);
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
