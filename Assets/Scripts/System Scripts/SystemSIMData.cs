using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SystemSIMData : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber, antiStealthPower;
	[HideInInspector]
	public float pScience, pIndustry, improvementCost, ownershipBonus, adjacencyBonus;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public string[] allPlanetsInfo = new string[6];	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, underInvasion, isEmbargoed, isPromoted;

	public float totalSystemScience, totalSystemIndustry, totalSystemSIM, tempTotalSci, tempTotalInd;
	public float scienceModifier, industryModifier;
	public float tempSci = 0.0f, tempInd = 0.0f;

	void Start()
	{
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
	}

	private void GetModifierValues(TurnInfo thisPlayer, int system, int planet)
	{
		ownershipBonus = systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership / 66.6666f;
		scienceModifier = (ownershipBonus + thisPlayer.raceScience + techTreeScript.sciencePercentBonus + racialTraitScript.IncomeModifier(thisPlayer, "Science"));
		industryModifier = (ownershipBonus + thisPlayer.raceIndustry + techTreeScript.industryPercentBonus + racialTraitScript.IncomeModifier(thisPlayer, "Industry"));
	}

	public void CheckPlanetValues(int system, int planet, TurnInfo thisPlayer)
	{
		GetModifierValues (thisPlayer, system, planet);

		string planetType = systemListConstructor.systemList[system].planetsInSystem[planet].planetType;
		
		improvementNumber = systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel;
		
		CheckImprovement(system, planet);

		tempSci = systemListConstructor.systemList [system].planetsInSystem [planet].planetScience * scienceModifier;
		tempInd = systemListConstructor.systemList [system].planetsInSystem [planet].planetIndustry * industryModifier;

		if(techTreeScript.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true)
		{
			allPlanetsInfo[planet] = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership + "% Owned\n"
					+ ((int)tempSci).ToString() + "\n" 
					+ ((int)tempInd).ToString() + "\n";
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == false)
		{
			allPlanetsInfo[planet] = "Uncolonised Planet\n Click to Colonise";
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

			totalSystemScience = tempTotalSci + techTreeScript.sciencePointBonus;
			totalSystemIndustry = tempTotalInd + techTreeScript.industryPointBonus;

			for(int j = 0; j < thisPlayer.playerOwnedHeroes.Count; ++j)
			{
				heroScript = thisPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();
				
				totalSystemScience += heroScript.heroSciBonus;
				totalSystemIndustry += heroScript.heroIndBonus;
			}

			if(isPromoted == false)
			{
				adjacencyBonus = FindAdjacencyBonuses (thisPlayer);
			}

			if(isPromoted == true)
			{
				adjacencyBonus = 1.5f;
			}

			totalSystemScience += totalSystemScience * adjacencyBonus;
			totalSystemIndustry += totalSystemIndustry * adjacencyBonus;
		}

		if(isEmbargoed == true)
		{
			totalSystemScience = 0;
			totalSystemIndustry = 0;
		}

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

					int ownershipToAdd = additionalOwnership + 1;

					if(ownershipToAdd > systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership - systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership)
					{
						ownershipToAdd = systemListConstructor.systemList[i].planetsInSystem[j].maxOwnership - systemListConstructor.systemList[i].planetsInSystem[j].planetOwnership;
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

	public int CheckOwnershipBonus(string owner)
	{
		if(owner == "Humans")
		{
			return racialTraitScript.HumanTrait();
		}

		return 0;
	}

	private float FindAdjacencyBonuses(TurnInfo thisPlayer)
	{
		float totalAdjacencyBonus = 0.0f;

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

			if(systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership <= 100)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].maxOwnership = 100;
			}

			canImprove = false;
		}
	}
}
