using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string tempState;
	public GameObject invasionQuad;
	public Material unownedMaterial;
	private float tempValue;

	public List<DiplomaticPosition> relationsList = new List<DiplomaticPosition>();

	void Start()
	{
		SetUpRelationsList ();
	}

	public void SetUpRelationsList()
	{
		for(int i = 0; i < turnInfoScript.allPlayers.Count; ++i)
		{
			DiplomaticPosition relation = new DiplomaticPosition();

			relation.playerOne = turnInfoScript.allPlayers[i];
			relation.playerTwo = playerTurnScript;
			relation.stateCounter = 49;
			relation.firstContact = false;

			relationsList.Add (relation);

			for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j)
			{
				if(turnInfoScript.allPlayers[i].playerRace == turnInfoScript.allPlayers[j].playerRace)
				{
					continue;
				}

				bool skip = false;

				for(int k = 0; k < relationsList.Count; ++k)
				{
					if(relationsList[k].playerOne.playerRace == turnInfoScript.allPlayers[i].playerRace && relationsList[k].playerTwo.playerRace == turnInfoScript.allPlayers[j].playerRace)
					{
						skip = true;
						break;
					}
					if(relationsList[k].playerTwo.playerRace == turnInfoScript.allPlayers[i].playerRace && relationsList[k].playerOne.playerRace == turnInfoScript.allPlayers[j].playerRace)
					{
						skip = true;
						break;
					}
				}

				if(skip == false)
				{
					DiplomaticPosition relationTwo = new DiplomaticPosition();

					relationTwo.playerOne = turnInfoScript.allPlayers[i];
					relationTwo.playerTwo = turnInfoScript.allPlayers[j];
					relationTwo.stateCounter = 49;
					relationTwo.firstContact = false;
					
					relationsList.Add (relationTwo);
				}
			}
		}
	}

	public DiplomaticPosition ReturnDiplomaticRelation(string firstRace, string secondRace)
	{
		for(int i = 0; i < relationsList.Count; ++i)
		{
			if(relationsList[i].playerOne.playerRace == firstRace && relationsList[i].playerTwo.playerRace == secondRace)
			{
				return relationsList[i];
			}
			if(relationsList[i].playerTwo.playerRace == firstRace && relationsList[i].playerOne.playerRace == secondRace)
			{
				return relationsList[i];
			}
		}
		return null;
	}

	public void PeaceTreatyOverride()
	{
		for(int i = 0; i < relationsList.Count; ++i)
		{
			if(relationsList[i].peaceTreatyTimer != 0.0f)
			{
				if(relationsList[i].peaceTreatyTimer + 30.0f < Time.time)
				{
					relationsList[i].peaceTreatyTimer = 0.0f;
					relationsList[i].ceaseFireActive = false;
				}
				else
				{
					relationsList[i].ceaseFireActive = true;
				}
			}
		}
	}

	private void CalculateOffDefModifier(int i) //Off/Def eq: y = (200 / (x + 14.14) ^2) + 0.5
	{
		tempValue = Math.Pow(relationsList[i].stateCounter + 14.14f, 2);
		
		tempValue = (200 / tempValue) + 0.5f;
		
		relationsList[i].offDefModifier = tempValue;
	}

	private void CalculateResourceModifier(int i)
	{
		tempValue = (relationsList[i].stateCounter * 0.005f) + 0.75;

		relationsList [i].resourceModifier = tempValue;
	}

	private void CalculateStealthModifier(int i) //Stealth eq: y = (-1(2)/10000 * (x - 50)^2) + 0.75
	{
		tempValue = Math.Pow (relationsList [i].stateCounter - 50, 2);

		if(relationsList[i].stateCounter < 50)
		{
			tempValue = -0.0001f * tempValue;
		}

		if(relationsList[i].stateCounter >= 50)
		{
			tempValue = -0.0002f * tempValue;
		}

		relationsList [i].stealthModifier = tempValue + 0.75f;
	}

	private void CalculateOwnershipModifier(int i)
	{
		tempValue = Math.Log (relationsList [i].stateCounter + 1);

		tempValue = (tempValue * 0.3742f) + 0.5f;

		relationsList [i].ownershipModifier = tempValue;
	}

	public void DiplomaticStateEffects(DiplomaticPosition tempObject)
	{
		for(int i = 0; i < relationsList.Count; ++i) 
		{
			CalculateOffDefModifier(i);
		}
	}

	private void ClampStateValues()
	{
		for(int i = 0; i < relationsList.Count; ++i)
		{
			if(relationsList[i].stateCounter > 100)
			{
				relationsList[i].stateCounter = 100;
			}

			if(relationsList[i].stateCounter < 0)
			{
				relationsList[i].stateCounter = 0;
			}
		}
	}
}

public class DiplomaticPosition
{
	public TurnInfo playerOne, playerTwo;
	public string diplomaticState;
	public int stateCounter;
	public float timeAtPeace, timeAtColdWar, timeAtWar, peaceTreatyTimer, offDefModifier, resourceModifier, stealthModifier, ownershipModifier;
	public bool ceaseFireActive, firstContact, adjacencyBonus, autoFight, tradeAllowed, peaceTreatyAllowed;
}
