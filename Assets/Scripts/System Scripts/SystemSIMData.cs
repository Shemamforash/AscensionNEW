using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SystemSIMData : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

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
	public float systemScienceModifier, systemIndustryModifier;
	public float finalScienceModifier, finalIndustryModifier, finalOwnershipModifier;
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
		CalculateBaseSystemModifiers ();

		for(int j = 0; j < systemListConstructor.systemList[thisSystem].systemSize; ++j)
		{
			if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true)
			{
				tempTotalSci += CheckPlanetValues(j, "Science");
				tempTotalInd += CheckPlanetValues(j, "Industry");
			}
		}

		totalSystemScience = tempTotalSci + scienceUnitBonus;
		totalSystemIndustry = tempTotalInd + industryUnitBonus;

		if(thisPlayer.playerRace == "Selkies")
		{
			racialTraitScript.IncreaseAmber(thisSystem);
		}
	}

	private void CalculateBaseSystemModifiers()
	{
		systemIndustryModifier = 0f; 
		systemScienceModifier = 0f;
		
		systemIndustryModifier += thisPlayer.raceScience * improvementsBasic.sciencePercentBonus * EmbargoPenalty () * PromoteBonus () * flResourceModifier;
		systemScienceModifier += thisPlayer.raceScience * improvementsBasic.industryPercentBonus * racialTraitScript.NereidesIndustryModifer (thisPlayer) * EmbargoPenalty() * PromoteBonus() * flResourceModifier;
	}

	public float CheckPlanetValues(int planet, string resource)
	{
		GetModifierValues (planet);
		
		float tempSci = 0, tempInd = 0;
		
		string planetType = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
		
		improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(thisSystem, planet);
		
		tempSci = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetScience * finalScienceModifier;
		tempInd = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetIndustry * finalIndustryModifier;
		
		if(improvementsBasic.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}
		
		if(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetColonised == true)
		{
			allPlanetsInfo[planet].generalInfo = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ Math.Round(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership, 1) + "% Owned\n";
			allPlanetsInfo[planet].scienceOutput = Math.Round(tempSci, 1).ToString();
			allPlanetsInfo[planet].industryOutput = Math.Round (tempInd,1).ToString();
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

	private void GetModifierValues(int planet)
	{
		systemDefence.CheckStatusEffects(planet);
		
		baseResourceBonus = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership / 66.6666f;
		finalScienceModifier = baseResourceBonus * scienceBuffModifier * systemScienceModifier;
		finalIndustryModifier = baseResourceBonus * industryBuffModifier * systemIndustryModifier;
		
		if(improvementsBasic.listOfImprovements[24].hasBeenBuilt == true)
		{
			string tempString = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
			
			if(tempString == "Molten" || tempString == "Desert" || tempString == "Rocky")
			{
				finalScienceModifier = 0f;
				finalIndustryModifier += finalIndustryModifier * 0.5f;
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
					float additionalOwnership = 0;

					if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Humans")
					{
						additionalOwnership = (int)racialTraitScript.HumanTrait();
					}

					if(improvementsBasic.listOfImprovements[18].hasBeenBuilt == true)
					{
						additionalOwnership = 0;
					}

					float ownershipToAdd = (additionalOwnership + 1) * improvementsBasic.ownershipModifier;

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
