using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GUISystemDataScript : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber, maxOwnership;
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

	public void SystemSIMCounter(int i, TurnInfo thisPlayer) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{	
		tempTotalSci = 0.0f;
		tempTotalInd = 0.0f;
		tempTotalMon = 0.0f;

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == true)
			{
				string planetType = systemListConstructor.systemList[i].planetType[j];

				improvementNumber = systemListConstructor.systemList[i].planetImprovementLevel[j];

				CheckImprovement();
				
				tempSci = systemListConstructor.systemList[i].planetScience[j]; //Need to sort out variable types, too much casting
				tempInd = systemListConstructor.systemList[i].planetIndustry[j];
				tempMon = systemListConstructor.systemList[i].planetMoney[j];

				techTreeScript.planetToCheck = planetType;

				techTreeScript.CheckPlanets();

				allPlanetsInfo[j] = gameObject.name + " " + (j+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
					+ systemListConstructor.systemList[i].planetOwnership[j] + "% Owned\n"
					+ ((int)(tempSci * resourceBonus * thisPlayer.raceScience)).ToString() + "\n" 
					+ ((int)(tempInd * resourceBonus * thisPlayer.raceIndustry)).ToString() + "\n" 
					+ ((int)(tempMon * resourceBonus * thisPlayer.raceMoney)).ToString();

				tempTotalSci += tempSci * techTreeScript.sciencePercentBonus * resourceBonus * thisPlayer.raceScience;
				tempTotalInd += tempInd * techTreeScript.industryPercentBonus * resourceBonus * thisPlayer.raceIndustry;
				tempTotalMon += tempMon * techTreeScript.moneyPercentBonus * resourceBonus * thisPlayer.raceMoney;

				if(systemListConstructor.systemList[i].planetOwnership[j] < maxOwnership && underInvasion == false)
				{
					++systemListConstructor.systemList[i].planetOwnership[j];
				}
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

	private float FindAdjacencyBonuses()
	{
		float totalAdjacencyBonus = 0.0f;

		for(int i = 0; i < 4; ++i)
		{
			if(lineRenderScript.connections[i] == null)
			{
				continue;
			}

			int j = RefreshCurrentSystem(lineRenderScript.connections[i]);

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

			improvementNumber = systemListConstructor.systemList[k].planetImprovementLevel[i];
			
			CheckImprovement();

			float tempSIM = (systemListConstructor.systemList[k].planetScience[i] + systemListConstructor.systemList[k].planetIndustry[i] + systemListConstructor.systemList[k].planetMoney[i]) * resourceBonus;

			planet.simOutput = tempSIM;

			planet.planetPosition = i;
				
			turnInfoScript.mostPowerfulPlanets.Add (planet);

			++turnInfoScript.savedIterator;
		}
	}

	public void CheckImprovement() //Contains data on the quality of planets and the bonuses they receive
	{
		if(improvementNumber == 0)
		{
			improvementLevel = "Poor";
			maxOwnership = 33;
			canImprove = true;
			improvementCost = 10.0f;
		}
		if(improvementNumber == 1)
		{
			improvementLevel = "Normal";
			maxOwnership = 50;
			canImprove = true;
			improvementCost = 20.0f;
		}
		if(improvementNumber == 2)
		{
			improvementLevel = "Good";
			maxOwnership = 66;
			canImprove = true;
			improvementCost = 40.0f;
		}
		if(improvementNumber == 3)
		{
			improvementLevel = "Superb";
			maxOwnership = 100;
			canImprove = false;
		}

		resourceBonus = (float)maxOwnership / 66.6666f;
	}
}
