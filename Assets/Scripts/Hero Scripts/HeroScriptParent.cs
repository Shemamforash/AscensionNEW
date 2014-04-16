using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, invasionObject;
	public int currentLevel = 1, planetInvade = -1, system;
	public int lH2Spent, antiSpent, radioSpent, blueCSpent; 
	public string heroOwnedBy, heroType;
	public bool isInvading = false, isBusy;
	public float heroAge, classModifier, maxArmour, currentArmour, primaryPower, secondaryPower, secondaryCollateral, invasionStrength, movementSpeed;
	public int aiInvadeTarget = -1, aiProtectTarget = -1;
	public float armourMod = 1f, resourceMod = 1f, movementMod = 1f, collateralMod = 1f, cloakMod = 1f, primaryMod = 1f, cooldownMod = 1f, secondaryMod = 1f;

	void Start()
	{
		heroAge = Time.time;
	
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroShip = gameObject.GetComponent<HeroShip> ();

		system = RefreshCurrentSystem (heroLocation);

		movementSpeed = 1;
		currentArmour = 100;
	}

	void Update()
	{
		system = RefreshCurrentSystem (heroLocation);
	}

	public DiplomaticPosition FindDiplomaticConnection()
	{
		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(heroOwnedBy == diplomacyScript.relationsList[i].playerOne.playerRace)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == diplomacyScript.relationsList[i].playerTwo.playerRace)
				{
					return diplomacyScript.relationsList[i];
				}
			}

			if(heroOwnedBy == diplomacyScript.relationsList[i].playerTwo.playerRace)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == diplomacyScript.relationsList[i].playerOne.playerRace)
				{
					return diplomacyScript.relationsList[i];
				}
			}
		}

		return null;
	}

	private void AIHeroFunctions()
	{
		int i = RefreshCurrentSystem(heroLocation);

		if(aiInvadeTarget != -1)
		{
			if(i == aiInvadeTarget && isInvading == false)
			{
				systemInvasion.StartSystemInvasion(i);
			}
		}
	}

	public void HeroEndTurnFunctions(TurnInfo thisPlayer)
	{
		heroShip = gameObject.GetComponent<HeroShip> ();
		heroShip.ShipAbilities (thisPlayer);

		if(thisPlayer.isPlayer == false)
		{
			AIHeroFunctions();
		}

		if(thisPlayer == playerTurnScript)
		{
			if(isInvading == true)
			{
				systemInvasion.hero = this;
				systemDefence = systemListConstructor.systemList[system].systemObject.GetComponent<SystemDefence>();

				if(systemDefence.canEnter == false)
				{
					systemInvasion.ContinueInvasion(system);
				}
				if(systemDefence.canEnter == true && planetInvade != -1)
				{
					systemInvasion.PlanetInvasion(this, system, planetInvade, false);
				}
			}

			if(isInvading == false && currentArmour != maxArmour)
			{
				currentArmour += maxArmour * 0.02f;

				if(currentArmour >= maxArmour)
				{
					currentArmour = maxArmour;
				}
			}
		}
	}
}


