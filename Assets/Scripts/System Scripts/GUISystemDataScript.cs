﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GUISystemDataScript : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber;
	[HideInInspector]
	public float pScience, pIndustry, pMoney, improvementCost, resourceBonus, adjacencyBonus;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public string[] allPlanetsInfo = new string[6];	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, underInvasion;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM, tempTotalSci, tempTotalInd, tempTotalMon;
	public float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;

	void Start()
	{
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
	}

	public void CheckPlanetValues(int i, int j, TurnInfo thisPlayer)
	{
		string planetType = systemListConstructor.systemList[i].planetsInSystem[j].planetType;
		
		improvementNumber = systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel;
		
		CheckImprovement(i, j);

		resourceBonus = systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership / 66.6666f;

		tempSci = systemListConstructor.systemList[i].planetsInSystem[j].planetScience * resourceBonus * thisPlayer.raceScience * techTreeScript.sciencePercentBonus; //Need to sort out variable types, too much casting
		tempInd = systemListConstructor.systemList[i].planetsInSystem[j].planetIndustry * resourceBonus * thisPlayer.raceScience * techTreeScript.industryPercentBonus;
		tempMon = systemListConstructor.systemList[i].planetsInSystem[j].planetMoney * resourceBonus * thisPlayer.raceScience * techTreeScript.moneyPercentBonus;

		if(techTreeScript.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[i].planetsInSystem[j].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
			tempMon = tempMon * 2;
		}

		allPlanetsInfo[j] = gameObject.name + " " + (j+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
			+ systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership + "% Owned\n"
				+ ((int)tempSci).ToString() + "\n" 
				+ ((int)tempInd).ToString() + "\n" 
				+ ((int)tempMon).ToString();
	}

	public void SystemSIMCounter(int i, TurnInfo thisPlayer) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{	
		tempTotalSci = 0.0f;
		tempTotalInd = 0.0f;
		tempTotalMon = 0.0f;

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
			{
				CheckPlanetValues(i, j, thisPlayer);

				tempTotalSci += tempSci;
				tempTotalInd += tempInd;
				tempTotalMon += tempMon;
			}

			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
			{
				allPlanetsInfo[j] = null;
			}
		}

		totalSystemScience = tempTotalSci + techTreeScript.sciencePointBonus;
		totalSystemIndustry = tempTotalInd + techTreeScript.industryPointBonus;
		totalSystemMoney = tempTotalMon + techTreeScript.moneyPointBonus;

		for(int j = 0; j < 3; ++j)
		{
			int k = RefreshCurrentSystem(gameObject);
			
			if(systemListConstructor.systemList[k].heroesInSystem[j] == null)
			{
				continue;
			}
			
			heroScript = systemListConstructor.systemList[k].heroesInSystem[j].GetComponent<HeroScriptParent>();
			
			totalSystemScience += heroScript.heroSciBonus;
			totalSystemIndustry += heroScript.heroIndBonus;
			totalSystemMoney += heroScript.heroMonBonus;
		}

		adjacencyBonus = FindAdjacencyBonuses ();

		totalSystemScience += totalSystemScience * adjacencyBonus;
		totalSystemIndustry += totalSystemIndustry * adjacencyBonus;
		totalSystemMoney += totalSystemMoney * adjacencyBonus;

		turnInfoScript.RefreshPlanetPower();
	}

	public void IncreaseOwnership()
	{
		int i = RefreshCurrentSystem (gameObject);

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
			{
				improvementNumber = systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel;
				
				CheckImprovement(i, j);

				if(systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership < systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership && underInvasion == false)
				{
					int additionalOwnership = CheckOwnershipBonus(systemListConstructor.systemList[i].systemOwnedBy);

					if(techTreeScript.listOfImprovements[18].hasBeenBuilt == true)
					{
						additionalOwnership = 0;
					}

					systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership += (1 + additionalOwnership);

					if(systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership < 0)
					{
						systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership = 0;
						WipePlanetInfo(i, j);
					}
				}
			}
		}
	}

	public int CheckOwnershipBonus(string owner)
	{
		if(owner == "Humans")
		{
			return racialTraitScript.HumanTrait();
		}

		return 0;
	}

	private float FindAdjacencyBonuses()
	{
		float totalAdjacencyBonus = 0.0f;

		int thisSystem = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
		{
			int j = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].connectedSystems[i]);

			for(int k = 0; k < 3; ++k)
			{
				if(systemListConstructor.systemList[j].heroesInSystem[k] == null)
				{
					continue;
				}

				if(systemListConstructor.systemList[j].heroesInSystem[k].name == "President")
				{
					totalAdjacencyBonus += 0.1f;
				}
			}
		}

		return totalAdjacencyBonus;
	}

	public void CheckUnlockedTier()
	{
		totalSystemSIM += totalSystemScience + totalSystemIndustry + totalSystemMoney;
			
		if(totalSystemSIM >= 1600.0f && totalSystemSIM < 3200)
		{
			techTreeScript.techTier = 1;
		}
		if(totalSystemSIM >= 3200.0f && totalSystemSIM < 6400)
		{
			techTreeScript.techTier = 2;
		}
		if(totalSystemSIM >= 6400.0f)
		{
			techTreeScript.techTier = 3;
		}
	}

	public void UpdatePlanetPowerArray(int k)
	{
		for(int i = 0; i < systemListConstructor.systemList[k].systemSize; ++i)
		{
			PlanetPower planet = new PlanetPower();

			planet.system = gameObject;

			improvementNumber = systemListConstructor.systemList[k].planetsInSystem[i].planetImprovementLevel;
			
			CheckImprovement(k, i);

			float tempSIM = (systemListConstructor.systemList[k].planetsInSystem[i].planetScience + systemListConstructor.systemList[k].planetsInSystem[i].planetIndustry + systemListConstructor.systemList[k].planetsInSystem[i].planetMoney) * resourceBonus;

			planet.simOutput = tempSIM;

			planet.planetPosition = i;
				
			turnInfoScript.mostPowerfulPlanets.Add (planet);

			++turnInfoScript.savedIterator;
		}
	}

	public void CheckImprovement(int system, int planet) //Contains data on the quality of planets and the bonuses they receive
	{
		if(improvementNumber == 0)
		{
			improvementLevel = "Poor";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 33;
			canImprove = true;
			improvementCost = 10.0f;
		}
		if(improvementNumber == 1)
		{
			improvementLevel = "Normal";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 50;
			canImprove = true;
			improvementCost = 20.0f;
		}
		if(improvementNumber == 2)
		{
			improvementLevel = "Good";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 66;
			canImprove = true;
			improvementCost = 40.0f;
		}
		if(improvementNumber == 3)
		{
			improvementLevel = "Superb";

			if(systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership <= 100)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 100;
			}

			canImprove = false;
		}
	}
}
