using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemSIMData : MasterScript
{
	[HideInInspector]
	public int improvementNumber, antiStealthWealth, thisSystem;
	[HideInInspector]
	public float knowledgeUnitBonus, powerUnitBonus, improvementCost, baseResourceBonus, adjacencyBonus, powerBuffModifier, knowledgeBuffModifier, embargoTimer, promotionTimer;
	[HideInInspector]
	public string improvementLevel, promotedBy = null, embargoedBy = null;
	[HideInInspector]
	public List<PlanetUIInfo> allPlanetsInfo = new List<PlanetUIInfo>();	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData;

	public float totalSystemKnowledge, totalSystemPower, totalSystemSIM, totalSystemAmber;
	public float flResourceModifier, flOwnershipModifier, flOffDefModifier;
	public float secRecPowerMod, secRecKnowledgeMod, secRecOwnershipMod;
	public float planetKnowledgeModifier, planetPowerModifier;
	public float systemKnowledgeModifier, systemPowerModifier, systemOwnershipModifier;
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
			planetInfo.knowledgeOutput = null;
			planetInfo.powerOutput = null;

			allPlanetsInfo.Add(planetInfo);
		}

		embargoedBy = null;
		promotedBy = null;
	}

	private void CheckSecRecBonus(int system)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType != null)
			{
				switch(systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType)
				{
				case "Antimatter":
					break;
				case "Liquid Hydrogen":
					secRecKnowledgeMod += 0.01f * thisPlayer.liquidH2;
					break;
				case "Blue Carbon":
					secRecOwnershipMod += 0.01f * thisPlayer.blueCarbon;
					break;
				case "Radioisotopes":
					secRecPowerMod += 0.01f * thisPlayer.radioisotopes;
					break;
				default:
					break;
				}
			}
		}
	}

	public void SystemSIMCounter(TurnInfo player) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{
		float tempTotalSci = 0.0f, tempTotalInd = 0.0f;
		secRecKnowledgeMod = 1f; secRecKnowledgeMod = 1f; secRecOwnershipMod = 1f;
		thisPlayer = player;
		CheckFrontLineBonus ();
		CheckSecRecBonus(thisSystem);

		for(int j = 0; j < systemListConstructor.systemList[thisSystem].systemSize; ++j)
		{
			if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true)
			{
				if(systemListConstructor.systemList[thisSystem].planetsInSystem[j].rareResourceType != null)
				{
					int rnd = UnityEngine.Random.Range (0, 25);
					
					if(rnd == 0)
					{
						switch(systemListConstructor.systemList[thisSystem].planetsInSystem[j].rareResourceType)
						{
						case "Antimatter":
							++player.antimatter;
							break;
						case "Liquid Hydrogen":
							++player.liquidH2;
							break;
						case "Blue Carbon":
							++player.blueCarbon;
							break;
						case "Radioisotopes":
							++player.radioisotopes;
							break;
						default:
							break;
						}
					}
				}

				tempTotalSci += CheckPlanetValues(j, "Knowledge");
				tempTotalInd += CheckPlanetValues(j, "Power");
			}
		}

		CalculateSystemModifierValues ();

		totalSystemKnowledge = (tempTotalSci + knowledgeUnitBonus) * systemKnowledgeModifier;
		totalSystemPower = (tempTotalInd + powerUnitBonus) * systemPowerModifier;

		if(thisPlayer.playerRace == "Selkies")
		{
			racialTraitScript.IncreaseAmber(thisSystem);
		}

		IncreaseOwnership ();
	}

	private void CalculateSystemModifierValues()
	{
		systemKnowledgeModifier =  improvementsBasic.knowledgePercentBonus * EmbargoPenalty() * PromoteBonus() * improvementsBasic.amberPenalty * flResourceModifier;
		systemPowerModifier =  improvementsBasic.powerPercentBonus * racialTraitScript.NereidesPowerModifer (thisPlayer) * EmbargoPenalty () * PromoteBonus () * improvementsBasic.amberPenalty * flResourceModifier;
		systemOwnershipModifier = racialTraitScript.HumanTrait (thisPlayer, improvementsBasic) * improvementsBasic.amberPenalty * flOwnershipModifier * improvementsBasic.ownershipModifier;
	}

	public float CheckPlanetValues(int planet, string resource)
	{
		CalculatePlanetModifierValues (planet);
		
		float tempSci = 0, tempInd = 0;
		
		string planetType = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
		
		improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(thisSystem, planet);
		
		tempSci = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetKnowledge * planetKnowledgeModifier;
		tempInd = systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetPower * planetPowerModifier;
		
		if(improvementsBasic.listOfImprovements[8].hasBeenBuilt == true && systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			tempSci = tempSci * 2;
			tempInd = tempInd * 2;
		}
		
		if(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetColonised == true)
		{
			string sOut = Math.Round(tempSci, 1) + "\n(" + Math.Round (planetKnowledgeModifier, 1) + ")";
			string iOut = Math.Round (tempInd,1) + "\n(" + Math.Round (planetPowerModifier, 1) + ")";

			allPlanetsInfo[planet].generalInfo = gameObject.name + " " + (planet+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
				+ Math.Round(systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership, 1) + "% Owned\n";
			allPlanetsInfo[planet].knowledgeOutput = sOut;
			allPlanetsInfo[planet].powerOutput = iOut;
		}
		
		switch(resource)
		{
		case "Knowledge":
			return tempSci;
		case "Power":
			return tempInd;
		default:
			return 0;
		}
	}

	private void CalculatePlanetModifierValues(int planet)
	{
		systemDefence.CheckStatusEffects(planet);
		
		baseResourceBonus = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetOwnership / 66.6666f;
		planetKnowledgeModifier = (thisPlayer.raceKnowledge + secRecKnowledgeMod) * baseResourceBonus * knowledgeBuffModifier;
		planetPowerModifier = (thisPlayer.racePower + secRecPowerMod) * baseResourceBonus * powerBuffModifier;
		
		if(improvementsBasic.listOfImprovements[24].hasBeenBuilt == true)
		{
			string tempString = systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetType;
			
			if(tempString == "Molten" || tempString == "Chasm" || tempString == "Waste")
			{
				planetKnowledgeModifier = 0f;
				planetPowerModifier += planetPowerModifier * 0.5f;
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
					float ownershipToAdd = systemOwnershipModifier * secRecOwnershipMod;

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

	public void UpdatePlanetWealthArray()
	{
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].systemSize; ++i)
		{
			PlanetWealth planet = new PlanetWealth();

			planet.system = gameObject;

			improvementNumber = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetImprovementLevel;
			
			systemFunctions.CheckImprovement(thisSystem, i);

			float tempSIM = (systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetKnowledge + systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetPower)
							* systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOwnership / 66.6666f;

			planet.simOutput = tempSIM;

			planet.planetPosition = i;
				
			turnInfoScript.mostWealthfulPlanets.Add (planet);

			++turnInfoScript.savedIterator;
		}
	}
}

public class PlanetUIInfo
{
	public string generalInfo, knowledgeOutput, powerOutput;
}
