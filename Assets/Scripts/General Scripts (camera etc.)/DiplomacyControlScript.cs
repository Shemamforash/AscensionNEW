using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string[] playerStates = new string[3]; //player-enemy1, player-enemy2, enemy1-enemy2
	public string tempState;
	private int noOfColonisedPlanets;
	private float invasionStrength;
	public GameObject invasionQuad;
	public Material unownedMaterial;

	public DiplomaticPosition playerEnemyOneRelations = new DiplomaticPosition();
	public DiplomaticPosition playerEnemyTwoRelations = new DiplomaticPosition();
	public DiplomaticPosition enemyOneEnemyTwoRelations = new DiplomaticPosition();

	public DiplomaticPosition[] allRelations;

	void Start()
	{
		SetUpRelationsList (playerEnemyOneRelations);
		SetUpRelationsList (playerEnemyTwoRelations);
		SetUpRelationsList (enemyOneEnemyTwoRelations);
	}

	public void SetUpRelationsList(DiplomaticPosition thisObject)
	{
		thisObject.diplomaticState = "";
		thisObject.peaceCounter = 0;
		thisObject.firstContactCounter = 10;
		thisObject.turnsAtColdWar = 0;
		thisObject.turnsAtPeace = 0;
		thisObject.warBonus = 0.0f;
		thisObject.peaceBonus = 0.0f;
		thisObject.canDeclareWar = false;
		thisObject.ceaseFirePeriodExpired = false;
		thisObject.hasMadeContact = false;
	}

	public void CheckForWarDeclarationAndPeaceExpiration(DiplomaticPosition tempObject)
	{
		if(tempObject.turnsAtColdWar > 10)
		{
			tempObject.canDeclareWar = true;
		}

		if(tempObject.turnsAtPeace > 10)
		{
			tempObject.ceaseFirePeriodExpired = true;
		}
	}

	public void CheckForDiplomaticStateChange(DiplomaticPosition tempObject)
	{
		RefreshNumbers (tempObject);

		InvokeDiplomaticStateBonuses (tempObject);

		CheckForWarDeclarationAndPeaceExpiration (tempObject);

		if(tempObject.peaceCounter > 50 || tempObject.ceaseFirePeriodExpired == false)
		{
			tempObject.diplomaticState = "Peace";

			if(tempObject.peaceCounter < 50)
			{
				tempObject.peaceCounter = 50;
			}
		}

		if(tempObject.peaceCounter < -50)
		{
			tempObject.diplomaticState = "War";
		}

		if(tempObject.peaceCounter < 50 &&  tempObject.peaceCounter > -50 && tempObject.ceaseFirePeriodExpired == true)
		{
			tempObject.diplomaticState = "Cold War";
		}

		RefreshNumbers (tempObject);
	}

	public void InvokeDiplomaticStateBonuses(DiplomaticPosition tempObject)
	{
		if(tempObject.peaceCounter > -50)
		{
			tempObject.peaceBonus = (tempObject.peaceCounter + 50) / 1000.0f;

			if(tempObject.peaceCounter > 50)
			{
				tempObject.peaceBonus = 0.1f;
			}
		}

		if(tempObject.peaceCounter < 50)
		{
			tempObject.warBonus = (tempObject.peaceCounter - 50) / 100.0f;

			if(tempObject.peaceCounter < -50)
			{
				tempObject.warBonus = 1.0f;
			}
		}

		if (tempObject.diplomaticState == "War")
		{
			//prevent adjacency bonuses, merchants etc.
		}

		if (tempObject.diplomaticState == "Peace")
		{
			//prevent invasions, start timer for peace
		}

		if(tempObject.diplomaticState == "Cold War")
		{
			//increase possibility of stealth detection
		}
	}

	private void RefreshNumbers(DiplomaticPosition tempObject)
	{
		if(tempObject.peaceCounter > 100)
		{
			tempObject.peaceCounter = 100;
		}

		if(tempObject.peaceCounter < -100)
		{
			tempObject.peaceCounter = -100;
		}
	}

	public void StartSystemInvasion(HeroScriptParent heroScript)
	{
		int i = RefreshCurrentSystem (heroScript.heroLocation);
		
		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == false)
			{
				continue;
			}
			
			++noOfColonisedPlanets;
		}
		
		invasionStrength = heroScript.offensivePower / noOfColonisedPlanets;
		
		heroScript.isInvading = true;
		
		heroScript.invasionObject = (GameObject)Instantiate (invasionQuad, systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[i].systemObject.transform.rotation);

		guiPlanScript = systemListConstructor.systemList [i].systemObject.GetComponent<GUISystemDataScript> ();

		guiPlanScript.underInvasion = true;
	}
	
	public void ContinueInvasion(HeroScriptParent heroScript)
	{
		int i = RefreshCurrentSystem (heroScript.heroLocation);
		
		bool planetsRemaining = false;
		
		invasionStrength = heroScript.offensivePower / noOfColonisedPlanets;
		
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
				systemListConstructor.systemList[i].planetOwnership[j] = 0;
			}
		}

		noOfColonisedPlanets = 0;

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == true)
			{
				planetsRemaining = true;
				++noOfColonisedPlanets;
			}
		}

		if(planetsRemaining == false)
		{
			heroScript.isInvading = false;
			
			systemListConstructor.systemList[i].systemOwnedBy = null;

			systemListConstructor.systemList[i].tradeRoute = null;
			
			Destroy (heroScript.invasionObject);
			
			lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();
			
			lineRenderScript.SetRaceLineColour("None");
			
			systemListConstructor.systemList[i].systemObject.renderer.material = unownedMaterial;

			guiPlanScript = systemListConstructor.systemList [i].systemObject.GetComponent<GUISystemDataScript> ();
			
			guiPlanScript.underInvasion = false;
		}
	}
}

public class DiplomaticPosition
{
	public string diplomaticState;
	public int firstContactCounter, peaceCounter, turnsAtPeace, turnsAtColdWar;
	public float warBonus, peaceBonus;
	public bool canDeclareWar, ceaseFirePeriodExpired, hasMadeContact;
}
