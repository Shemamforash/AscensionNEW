﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SystemSIMData : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber, antiStealthPower;
	[HideInInspector]
	public float scienceBonus, industryBonus, improvementCost, ownershipBonus, adjacencyBonus, industrySEModifier, scienceSEModifier, embargoTimer, promotionTimer;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public List<PlanetUIInfo> allPlanetsInfo = new List<PlanetUIInfo>();	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, isEmbargoed, isPromoted;

	public float totalSystemScience, totalSystemIndustry, totalSystemSIM, totalSystemAmber, tempTotalSci, tempTotalInd;
	public float scienceModifier, industryModifier;
	public float tempSci = 0.0f, tempInd = 0.0f;

	void Start()
	{
		systemDefence = gameObject.GetComponent<SystemDefence> ();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		improvementsBasic = gameObject.GetComponent<ImprovementsBasic>();

		int planetNumber = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[planetNumber].systemSize; ++i)
		{
			PlanetUIInfo planetInfo = new PlanetUIInfo();

			planetInfo.generalInfo = null;
			planetInfo.scienceOutput = null;
			planetInfo.industryOutput = null;

			allPlanetsInfo.Add(planetInfo);
		}
	}

	private void GetModifierValues(TurnInfo thisPlayer, int system, int planet)
	{
		systemDefence.CheckStatusEffects(planet);

		ownershipBonus = systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership / 66.6666f;
		scienceModifier = ((ownershipBonus * thisPlayer.raceScience) + improvementsBasic.sciencePercentBonus) * scienceSEModifier;
		industryModifier = (ownershipBonus + thisPlayer.raceIndustry + improvementsBasic.industryPercentBonus + racialTraitScript.NereidesIndustryModifer(thisPlayer)) * industrySEModifier;

		if(improvementsBasic.listOfImprovements[24].hasBeenBuilt == true)
		{
			string tempString = systemListConstructor.systemList[system].planetsInSystem[planet].planetType;
			
			if(tempString == "Molten" || tempString == "Desert" || tempString == "Rocky")
			{
				scienceModifier = 0f;
				industryModifier += 50.0f;
			}
		}
	}

	public void CheckPlanetValues(int system, int planet, TurnInfo thisPlayer)
	{
		GetModifierValues (thisPlayer, system, planet);

		string planetType = systemListConstructor.systemList[system].planetsInSystem[planet].planetType;
		
		improvementNumber = systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(system, planet);

		tempSci = systemListConstructor.systemList [system].planetsInSystem [planet].planetScience * scienceModifier;
		tempInd = systemListConstructor.systemList [system].planetsInSystem [planet].planetIndustry * industryModifier;

		if(improvementsBasic.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true)
		{
			allPlanetsInfo[planet].generalInfo = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ Math.Round(systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership, 1) + "% Owned\n";
			allPlanetsInfo[planet].scienceOutput = Math.Round(tempSci, 1).ToString();
			allPlanetsInfo[planet].industryOutput = Math.Round (tempInd,1).ToString();
		}
	}

	public void SystemSIMCounter(int i, TurnInfo thisPlayer) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{
		tempTotalSci = 0.0f;
		tempTotalInd = 0.0f;

		if(isEmbargoed == false)
		{
			for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
				{
					CheckPlanetValues(i, j, thisPlayer);

					tempTotalSci += tempSci;
					tempTotalInd += tempInd;
				}
			}

			totalSystemScience = tempTotalSci + scienceBonus;
			totalSystemIndustry = tempTotalInd + industryBonus;

			scienceBonus = 0f;
			industryBonus = 0f;
		
			if(isPromoted == false)
			{
				adjacencyBonus = FindAdjacencyBonuses (thisPlayer);
			}

			if(isPromoted == true)
			{
				if(promotionTimer + 30.0f < Time.time)
				{
					isPromoted = false;
				}

				adjacencyBonus = adjacencyBonus + 1.5f;
			}

			totalSystemScience = totalSystemScience * adjacencyBonus;
			totalSystemIndustry = totalSystemIndustry * adjacencyBonus;
		}

		if(isEmbargoed == true)
		{
			if(embargoTimer + 20.0f < Time.time)
			{
				isEmbargoed = false;
			}

			totalSystemScience = 0;
			totalSystemIndustry = 0;
		}

		if(thisPlayer.playerRace == "Selkies")
		{
			racialTraitScript.IncreaseAmber(i);
		}
	}

	public void IncreaseOwnership()
	{
		int i = RefreshCurrentSystem (gameObject);

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
			{
				improvementNumber = systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel;
				
				systemFunctions.CheckImprovement(i, j);

				if(systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership < (systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus)
				   && systemDefence.underInvasion == false)
				{
					float additionalOwnership = CheckOwnershipBonus(systemListConstructor.systemList[i].systemOwnedBy);

					if(improvementsBasic.listOfImprovements[18].hasBeenBuilt == true)
					{
						additionalOwnership = 0;
					}

					float ownershipToAdd = (additionalOwnership + 1) * improvementsBasic.ownershipModifier;

					if(ownershipToAdd > (systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus) - systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership)
					{
						ownershipToAdd = (systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus)
							- systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership;
					}

					systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership += ownershipToAdd;

					if(systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership < 0)
					{
						systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership = 0;
						WipePlanetInfo(i, j);
					}
				}
			}
		}
	}

	public float CheckOwnershipBonus(string owner)
	{
		if(owner == "Humans")
		{
			return (int)racialTraitScript.HumanTrait();
		}

		return 0;
	}

	private float FindAdjacencyBonuses(TurnInfo thisPlayer)
	{
		float totalAdjacencyBonus = 1f;

		int thisSystem = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
		{
			int j = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);

			for(int k = 0; k < thisPlayer.playerOwnedHeroes.Count; ++k)
			{
				heroScript = thisPlayer.playerOwnedHeroes[k].GetComponent<HeroScriptParent>();

				if(heroScript.heroLocation == systemListConstructor.systemList[j].systemObject)
				{
					if(heroScript.heroTier3 == "Ambassador")
					{
						totalAdjacencyBonus += 0.1f;
					}
				}
			}
		}

		return totalAdjacencyBonus;
	}

	public void UpdatePlanetPowerArray(int system)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			PlanetPower planet = new PlanetPower();

			planet.system = gameObject;

			improvementNumber = systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel;
			
			systemFunctions.CheckImprovement(system, i);

			float tempSIM = (systemListConstructor.systemList[system].planetsInSystem[i].planetScience + systemListConstructor.systemList[system].planetsInSystem[i].planetIndustry)
							* systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership / 66.6666f;

			planet.simOutput = tempSIM;

			planet.planetPosition = i;
				
			turnInfoScript.mostPowerfulPlanets.Add (planet);

			++turnInfoScript.savedIterator;
		}
	}
}

public class PlanetUIInfo
{
	public string generalInfo, scienceOutput, industryOutput;
}
