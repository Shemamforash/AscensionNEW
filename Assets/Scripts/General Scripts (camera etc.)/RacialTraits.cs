﻿using UnityEngine;
using System.Collections;
using System;

public class RacialTraits : MasterScript 
{
	public float ambitionCounter, ambitiongrowthModifier, amber;
	public int nereidesStacks;
	public UILabel racialLabel;
	
	void Start()
	{
		ambitionCounter = 40f;
	}
	
	public void Purge() //Nereides function to produce elation
	{
		while(playerTurnScript.knowledge >= 100 && playerTurnScript.power >= 100)
		{
			playerTurnScript.knowledge -= 100;
			playerTurnScript.power -= 100;
			++nereidesStacks;
		}
	}
	
	public float NereidesPowerModifer (TurnInfo thisPlayer) //Returns power modifier based on elation
	{
		if(thisPlayer.playerRace == "Nereides")
		{
			return (nereidesStacks / 10) + 1;
		}
		
		return 1;
	}
	
	public float HumanTrait(TurnInfo thisPlayer, ImprovementsBasic improvements) //Returns ambition modifier
	{
		if(thisPlayer.playerRace == "Humans")
		{
			if(improvements.listOfImprovements[18].hasBeenBuilt == false)
			{
				ambitiongrowthModifier = ambitionCounter / 40.0f;
				
				return ambitiongrowthModifier;
			}
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
				
				if(tempString == "Molten" || tempString == "Chasm" || tempString == "Waste")
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
			racialLabel.text = Math.Round(amber, 2) + " Amber";
		}
	}
}