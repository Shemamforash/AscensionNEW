using UnityEngine;
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
	public float scienceBonus, industryBonus, improvementCost, ownershipBonus, adjacencyBonus;
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
	}

	private void GetModifierValues(TurnInfo thisPlayer, int system, int planet)
	{
		ownershipBonus = systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership / 66.6666f;
		scienceModifier = (ownershipBonus + thisPlayer.raceScience + improvementsBasic.sciencePercentBonus);
		industryModifier = (ownershipBonus + thisPlayer.raceIndustry + improvementsBasic.industryPercentBonus + racialTraitScript.NereidesIndustryModifer(thisPlayer));

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
		
		CheckImprovement(system, planet);

		tempSci = systemListConstructor.systemList [system].planetsInSystem [planet].planetScience * scienceModifier;
		tempInd = systemListConstructor.systemList [system].planetsInSystem [planet].planetIndustry * industryModifier;

		if(improvementsBasic.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}

		PlanetUIInfo planetInfo = new PlanetUIInfo();

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true)
		{
			planetInfo.generalInfo = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership + "% Owned\n";
			planetInfo.scienceOutput = Math.Round(tempSci, 1).ToString();
			planetInfo.industryOutput = Math.Round (tempInd,1).ToString();
		}

		allPlanetsInfo.Add (planetInfo);
	}

	public void SystemSIMCounter(int i, TurnInfo thisPlayer) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{	
		allPlanetsInfo.Clear ();

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
				adjacencyBonus = adjacencyBonus + 1.5f;
			}

			totalSystemScience = totalSystemScience * adjacencyBonus;
			totalSystemIndustry = totalSystemIndustry * adjacencyBonus;
		}

		if(isEmbargoed == true)
		{
			totalSystemScience = 0;
			totalSystemIndustry = 0;
		}

		if(thisPlayer.playerRace == "Selkies")
		{
			IncreaseAmber(i);
		}
	}

	private void IncreaseAmber (int system)
	{
		if(improvementsBasic.listOfImprovements[28].hasBeenBuilt == true)
		{
			float tempMod = 1.0f;
			
			if(improvementsBasic.IsBuiltOnPlanetType(system, 28, "Molten") == true)
			{
				tempMod = 1.5f;
			}
			
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Molten" || tempString == "Desert" || tempString == "Rocky")
				{
					totalSystemAmber += (tempMod * 2f) * improvementsBasic.amberPercentBonus;
				}
				else
				{
					totalSystemAmber += tempMod * improvementsBasic.amberPercentBonus;
				}
			}
		}

		totalSystemAmber += improvementsBasic.amberPointBonus;

		racialTraitScript.amber += totalSystemAmber;
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

	public void CheckUnlockedTier()
	{
		totalSystemSIM += totalSystemScience + totalSystemIndustry;
			
		if(totalSystemSIM >= 1600.0f && totalSystemSIM < 3200)
		{
			improvementsBasic.techTier = 1;
		}
		if(totalSystemSIM >= 3200.0f && totalSystemSIM < 6400)
		{
			improvementsBasic.techTier = 2;
		}
		if(totalSystemSIM >= 6400.0f)
		{
			improvementsBasic.techTier = 3;
		}
	}

	public void UpdatePlanetPowerArray(int system)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			PlanetPower planet = new PlanetPower();

			planet.system = gameObject;

			improvementNumber = systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel;
			
			CheckImprovement(system, i);

			float tempSIM = (systemListConstructor.systemList[system].planetsInSystem[i].planetScience + systemListConstructor.systemList[system].planetsInSystem[i].planetIndustry)
							* systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership / 66.6666f;

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
			systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 100;
			canImprove = false;
		}
	}
}

public class PlanetUIInfo
{
	public string generalInfo, scienceOutput, industryOutput;
}
