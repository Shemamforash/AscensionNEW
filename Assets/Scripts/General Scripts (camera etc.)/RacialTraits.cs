using UnityEngine;
using System.Collections;

public class RacialTraits : MasterScript 
{
	public float ambitionCounter, ambitionOwnershipModifier, amber;
	public int nereidesStacks;
	public UILabel racialLabel;

	void Start()
	{
		ambitionCounter = 0;
	}

	public void Purge()
	{
		while(playerTurnScript.science >= 100 && playerTurnScript.industry >= 100)
		{
			playerTurnScript.science -= 100;
			playerTurnScript.industry -= 100;
			++nereidesStacks;
		}
	}

	public float NereidesIndustryModifer (TurnInfo thisPlayer)
	{
		if(thisPlayer.playerRace == "Nereides")
		{
			return nereidesStacks / 10;
		}

		return 0;
	}

	public float HumanTrait()
	{
		ambitionOwnershipModifier = ambitionCounter / 25.0f;

		return ambitionOwnershipModifier;
	}

	public void RacialBonus(TurnInfo player)
	{
		if(player.playerRace == "Humans")
		{
			if(player.systemsColonisedThisTurn > 0f)
			{
				ambitionCounter += player.systemsColonisedThisTurn * 4f * (60 / systemListConstructor.mapSize);
			}
			if(player.planetsColonisedThisTurn > 0f)
			{
				ambitionCounter += (player.planetsColonisedThisTurn - player.systemsColonisedThisTurn) * 2f * (60 / systemListConstructor.mapSize);
			}
			if(player.systemsColonisedThisTurn == 0 && player.planetsColonisedThisTurn == 0)
			{
				ambitionCounter -= 0.5f;
			}
			if(ambitionCounter < -100f)
			{
				ambitionCounter = -100f;
			}
			if(ambitionCounter > 100f)
			{
				ambitionCounter = 100f;
			}
		}
		if(player.playerRace == "Nereides")
		{
			player.researchCostModifier += nereidesStacks;
		}
	}

	void Update()
	{
		if(playerTurnScript.playerRace == "Humans")
		{
			racialLabel.text = ("Ambition: " + ((int)ambitionCounter).ToString());
		}

		if(playerTurnScript.playerRace == "Nereides")
		{
			racialLabel.text = nereidesStacks + " stacks";
		}

		if(playerTurnScript.playerRace == "Selkies")
		{
			racialLabel.text = amber + " Amber";
		}
	}
}
