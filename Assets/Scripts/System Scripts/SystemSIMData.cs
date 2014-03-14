using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemSIMData : MasterScript
{
	[HideInInspector]
	public int improvementNumber, antiStealthPower, thisSystem;
	[HideInInspector]
	public float scienceUnitBonus, industryUnitBonus, improvementCost, baseResourceBonus, adjacencyBonus, industryBuffModifier, scienceBuffModifier, embargoTimer, promotionTimer;
	[HideInInspector]
	public string improvementLevel, promotedBy = null, embargoedBy = null;
	[HideInInspector]
	public List<PlanetUIInfo> allPlanetsInfo = new List<PlanetUIInfo>();	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData;

	public float totalSystemScience, totalSystemIndustry, totalSystemSIM, totalSystemAmber;
	public float flResourceModifier, flOwnershipModifier, flOffDefModifier;
	public float planetScienceModifier, planetIndustryModifier;
	public float systemScienceModifier, systemIndustryModifier, systemOwnershipModifier;
	private TurnInfo thisPlayer;

	void Start()
	{
		systemDefence = gameObject.GetComponent<SystemDefence> ();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		improvementsBasic = gameObject.GetComponent<ImprovementsBasic>();

		thisSystem = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].systemSize; ++i)
		{
			PlanetUIInfo planetInfo = new PlanetUIInfo();

			planetInfo.generalInfo = null;
			planetInfo.scienceOutput = null;
			planetInfo.industryOutput = null;

			allPlanetsInfo.Add(planetInfo);
		}

		embargoedBy = null;
		promotedBy = null;
	}

	public void SystemSIMCounter(TurnInfo player) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{
		float tempTotalSci = 0.0f, tempTotalInd = 0.0f;
		thisPlayer = player;
		CheckFrontLineBonus ();

		for(int j = 0; j < systemListConstructor.systemList[thisSystem].systemSize; ++j)
		{
			if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true)
			{
				tempTotalSci += CheckPlanetValues(j, "Science");
				tempTotalInd += CheckPlanetValues(j, "Industry");
			}
		}

		CalculateSystemModifierValues ();

		totalSystemScience = (tempTotalSci + scienceUnitBonus) * systemScienceModifier;
		totalSystemIndustry = (tempTotalInd + industryUnitBonus) * systemIndustryModifier;

		if(thisPlayer.playerRace == "Selkies")
		{
			racialTraitScript.IncreaseAmber(thisSystem);
		}

		IncreaseOwnership ();
	}

	private void CalculateSystemModifierValues()
	{
		systemScienceModifier =  improvementsBasic.sciencePercentBonus * EmbargoPenalty() * PromoteBonus() * improvementsBasic.amberPenalty * flResourceModifier;
		systemIndustryModifier =  improvementsBasic.industryPercentBonus * racialTraitScript.NereidesIndustryModifer (thisPlayer) * EmbargoPenalty () * PromoteBonus () * improvementsBasic.amberPenalty * flResourceModifier;
		systemOwnershipModifier = racialTraitScript.HumanTrait (thisPlayer) * improvementsBasic.amberPenalty * flOwnershipModifier * improvementsBasic.ownershipModifier;
	}

	public float CheckPlanetValues(int planet, string resource)
	{
		CalculatePlanetModifierValues (planet);
		
		float tempSci = 0, tempInd = 0;
		
		string planetType = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
		
		improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(thisSystem, planet);
		
		tempSci = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetScience * planetScienceModifier;
		tempInd = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetIndustry * planetIndustryModifier;
		
		if(improvementsBasic.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}
		
		if(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetColonised == true)
		{
			string sOut = Math.Round(tempSci, 1) + "\n(" + Math.Round (planetScienceModifier, 1) + ")";
			string iOut = Math.Round (tempInd,1) + "\n(" + Math.Round (planetIndustryModifier, 1) + ")";

			allPlanetsInfo[planet].generalInfo = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ Math.Round(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership, 1) + "% Owned\n";
			allPlanetsInfo[planet].scienceOutput = sOut;
			allPlanetsInfo[planet].industryOutput = iOut;
		}
		
		switch(resource)
		{
		case "Science":
			return tempSci;
		case "Industry":
			return tempInd;
		default:
			return 0;
		}
	}

	private void CalculatePlanetModifierValues(int planet)
	{
		systemDefence.CheckStatusEffects(planet);
		
		baseResourceBonus = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership / 66.6666f;
		planetScienceModifier = thisPlayer.raceScience * baseResourceBonus * scienceBuffModifier;
		planetIndustryModifier = thisPlayer.raceIndustry * baseResourceBonus * industryBuffModifier;
		
		if(improvementsBasic.listOfImprovements[24].hasBeenBuilt == true)
		{
			string tempString = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
			
			if(tempString == "Molten" || tempString == "Desert" || tempString == "Rocky")
			{
				planetScienceModifier = 0f;
				planetIndustryModifier += planetIndustryModifier * 0.5f;
			}
		}
	}	

	public void IncreaseOwnership()
	{
		for(int j = 0; j < systemListConstructor.systemList[thisSystem].systemSize; ++j)
		{
			if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true)
			{
				improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetImprovementLevel;
				
				systemFunctions.CheckImprovement(thisSystem, j);

				if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership < (systemListConstructor.systemList[thisSystem].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus)
				   && systemDefence.underInvasion == false)
				{
					float ownershipToAdd = systemOwnershipModifier;

					if(ownershipToAdd > (systemListConstructor.systemList[thisSystem].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus) - systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership)
					{
						ownershipToAdd = (systemListConstructor.systemList[thisSystem].planetsInSystem[j].maxOwnership + improvementsBasic.maxOwnershipBonus)
							- systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership;
					}

					systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership += ownershipToAdd;

					if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership < 0)
					{
						systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetOwnership = 0;
						WipePlanetInfo(thisSystem, j);
					}
				}
			}
		}
	}

	private void CheckFrontLineBonus()
	{
		flResourceModifier = 1f;
		flOwnershipModifier = 1f;
		flOffDefModifier = 1f;

		int noSystems = 0;

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
		{
			int neighbour = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);

			if(systemListConstructor.systemList[neighbour].systemOwnedBy != null && systemListConstructor.systemList[neighbour].systemOwnedBy != thisPlayer.playerRace)
			{
				DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation(systemListConstructor.systemList[thisSystem].systemOwnedBy, systemListConstructor.systemList[neighbour].systemOwnedBy);

				flResourceModifier += temp.resourceModifier;
				flOwnershipModifier += temp.ownershipModifier;
				flOffDefModifier += temp.offDefModifier;
				++noSystems;
			}
		}

		if(noSystems != 0)
		{
			flResourceModifier = flResourceModifier / noSystems;
			flOwnershipModifier = flOwnershipModifier / noSystems;
			flOffDefModifier = flOffDefModifier / noSystems;
		}
	}

	private float PromoteBonus() //Calculates resource bonus from promotions on enemy systems
	{
		float totalAdjacencyBonus = 1f;

		if(promotedBy == null)
		{
			for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
			{
				int j = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);

				systemSIMData = systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();

				if(systemSIMData.promotedBy != null)
				{
					if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
					{
						totalAdjacencyBonus += 0.05f;
					}

					else
					{
						totalAdjacencyBonus += 0.1f;
					}
				}
			}
		}
		
		else if(promotedBy != null)
		{
			if(promotionTimer + 30.0f < Time.time)
			{
				promotedBy = null;
			}

			DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (thisPlayer.playerRace, promotedBy);

			++temp.stateCounter;

			totalAdjacencyBonus = 1.5f;
		}

		return totalAdjacencyBonus;
	}

	private float EmbargoPenalty() //Calculates penalties from Embargoes
	{
		if(embargoedBy != null)
		{
			DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (thisPlayer.playerRace, embargoedBy);

			--temp.stateCounter;

			if(embargoTimer + 20.0f < Time.time)
			{
				embargoedBy = null;
			}

			float embargoPenalty = 1 - temp.resourceModifier;

			return embargoPenalty;
		}

		return 1;
	}

	public void UpdatePlanetPowerArray()
	{
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].systemSize; ++i)
		{
			PlanetPower planet = new PlanetPower();

			planet.system = gameObject;

			improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetImprovementLevel;
			
			systemFunctions.CheckImprovement(thisSystem, i);

			float tempSIM = (systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetScience + systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetIndustry)
							* systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOwnership / 66.6666f;

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
