using UnityEngine;
using System.Collections;

public class RacialTraits : MasterScript 
{
	public int ambitionCounter, ambitionOwnershipModifier;

	void Start()
	{
		ambitionCounter = 0;
	}

	public int HumanTrait()
	{
		if(ambitionCounter < -100)
		{
			ambitionCounter = -100;
		}
		if(ambitionCounter > 100)
		{
			ambitionCounter = 100;
		}

		ambitionOwnershipModifier = ambitionCounter / 25;

		return ambitionOwnershipModifier;
	}

	public void RacialBonus(TurnInfo player)
	{
		if(player.playerRace == "Humans")
		{
			if(player.systemsColonisedThisTurn > 0)
			{
				ambitionCounter += player.systemsColonisedThisTurn * 2;
			}
			if(player.planetsColonisedThisTurn > 0)
			{
				ambitionCounter += (player.planetsColonisedThisTurn - player.systemsColonisedThisTurn);
			}
			if(player.systemsColonisedThisTurn == 0 && player.planetsColonisedThisTurn == 0)
			{
				ambitionCounter -= 2;
			}
		}
	}

	void OnGUI()
	{
		GUI.skin = mainGUIScript.mySkin;

		if(playerTurnScript.playerRace == "Humans")
		{
			GUI.Label (new Rect(10.0f, Screen.height - 160.0f, 100.0f, 30.0f), ambitionCounter.ToString());
		}
	}
}
