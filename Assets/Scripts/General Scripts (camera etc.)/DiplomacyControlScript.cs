using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiplomacyControlScript : MasterScript 
{
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
		thisObject.firstContactTimer = Time.time;
		thisObject.timeAtColdWar = Time.time;
		thisObject.timeAtPeace = Time.time;
		thisObject.warBonus = 0.0f;
		thisObject.peaceBonus = 0.0f;
		thisObject.canDeclareWar = false;
		thisObject.ceaseFirePeriodExpired = true;
		thisObject.hasMadeContact = false;
	}

	public void CheckForWarDeclarationAndPeaceExpiration(DiplomaticPosition tempObject)
	{
		if(tempObject.timeAtColdWar > tempObject.timeAtColdWar + 40.0f)
		{
			tempObject.canDeclareWar = true;
		}

		if(tempObject.timeAtPeace > tempObject.timeAtPeace + 40.0f)
		{
			tempObject.ceaseFirePeriodExpired = true;
		}
	}

	public void PeaceTimer()
	{
		if(playerEnemyOneRelations.hasMadeContact == true)
		{
			if(Time.time > playerEnemyOneRelations.firstContactTimer + 30.0f)
			{
				playerEnemyOneRelations.ceaseFirePeriodExpired = true;
			}
		}
		if(playerEnemyTwoRelations.hasMadeContact == true)
		{
			if(Time.time > playerEnemyTwoRelations.firstContactTimer + 30.0f)
			{
				playerEnemyTwoRelations.ceaseFirePeriodExpired = true;
			}
		}
		if(enemyOneEnemyTwoRelations.hasMadeContact == true)
		{
			if(Time.time > enemyOneEnemyTwoRelations.firstContactTimer + 30.0f)
			{
				enemyOneEnemyTwoRelations.ceaseFirePeriodExpired = true;
			}
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
	public int peaceCounter;
	public float warBonus, peaceBonus, firstContactTimer, timeAtPeace, timeAtColdWar;
	public bool canDeclareWar, ceaseFirePeriodExpired, hasMadeContact;
}
