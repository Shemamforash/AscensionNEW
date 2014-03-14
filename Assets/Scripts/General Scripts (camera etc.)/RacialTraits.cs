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

	public void Purge() //Nereides function to produce elation
	{
		while(playerTurnScript.science >= 100 && playerTurnScript.industry >= 100)
		{
			playerTurnScript.science -= 100;
			playerTurnScript.industry -= 100;
			++nereidesStacks;
		}
	}

	public float NereidesIndustryModifer (TurnInfo thisPlayer) //Returns industry modifier based on elation
	{
		if(thisPlayer.playerRace == "Nereides")
		{
			return (nereidesStacks / 10) + 1;
		}

		return 1;
	}

	public float HumanTrait(TurnInfo thisPlayer) //Returns ambition modifier
	{
		if(thisPlayer.playerRace == "Humans" && improvementsBasic.listOfImprovements[18].hasBeenBuilt == false)
		{
			ambitionOwnershipModifier = ambitionCounter / 40.0f;

			return ambitionOwnershipModifier;
		}

		return 1f;
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
				ambitionCounter -= 0.25f;
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

	public void IncreaseAmber (int system)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();
		
		systemSIMData.totalSystemAmber = 0f;
		
		if(improvementsBasic.listOfImprovements[28].hasBeenBuilt == true)
		{
			float tempMod = 0.1f;
			
			if(improvementsBasic.IsBuiltOnPlanetType(system, 28, "Molten") == true)
			{
				tempMod = 0.15f;
			}
			
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Molten" || tempString == "Desert" || tempString == "Rocky")
				{
					systemSIMData.totalSystemAmber += (tempMod * 2f) * improvementsBasic.amberProductionBonus;
				}
				else
				{
					systemSIMData.totalSystemAmber += tempMod * improvementsBasic.amberProductionBonus;
				}
			}
		}
		
		systemSIMData.totalSystemAmber += improvementsBasic.amberPointBonus;
		
		racialTraitScript.amber += systemSIMData.totalSystemAmber;
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
