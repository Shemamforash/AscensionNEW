using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string[] playerStates = new string[3]; //player-enemy1, player-enemy2, enemy1-enemy2
	public string tempState;
	public GameObject invasionQuad;
	public Material unownedMaterial;

	public DiplomaticPosition playerEnemyOneRelations = new DiplomaticPosition();
	public DiplomaticPosition playerEnemyTwoRelations = new DiplomaticPosition();
	public DiplomaticPosition enemyOneEnemyTwoRelations = new DiplomaticPosition();

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
		thisObject.ceaseFirePeriodExpired = true;
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
}

public class DiplomaticPosition
{
	public string diplomaticState;
	public int firstContactCounter, peaceCounter, turnsAtPeace, turnsAtColdWar;
	public float warBonus, peaceBonus;
	public bool canDeclareWar, ceaseFirePeriodExpired, hasMadeContact;
}
