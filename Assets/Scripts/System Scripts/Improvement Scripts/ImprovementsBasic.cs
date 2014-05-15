using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ImprovementsBasic : MasterScript 
{
	public float knowledgePercentBonus, powerPercentBonus, amberPenalty, amberProductionBonus, amberPointBonus, knowledgeBonusModifier, populationModifier, maxPopulationBonus;
	public float tempKnwlBonus, tempPowBonus, tempPopulationBonus, tempWealth, tempKnwlUnitBonus, tempPowUnitBonus, tempResearchCostReduction, tempImprovementCostReduction, 
			tempPopulationUnitBonus, tempCount, tempBonusAmbition, tempAmberProductionBonus, tempAmberPointBonus, tempImprovementSlots, tempAmberPenalty;
	public List<string> planetToBuildOn;
	public GameObject tooltip;
	public int techTier = 0, improvementCostModifier = 0, researchCost, system;
	private GenericImprovements genericImprovements;

	public List<ImprovementClass> listOfImprovements = new List<ImprovementClass>();

	void Start()
	{
		planetToBuildOn = new List<string>();

		knowledgePercentBonus = 0; powerPercentBonus = 0;

		systemSIMData = gameObject.GetComponent<SystemSIMData>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();
		genericImprovements = GameObject.Find ("ScriptsContainer").GetComponent<GenericImprovements> ();

		LoadNewTechTree();
	}

	public bool ImproveSystem(int improvement) //Occurs if button of tech is clicked.
	{
		if(playerTurnScript.power >= (listOfImprovements[improvement].improvementCost - improvementCostModifier)) //Checks cost of tech and current power
		{
			playerTurnScript.power -= (listOfImprovements[improvement].improvementCost - improvementCostModifier);
			listOfImprovements[improvement].hasBeenBuilt = true;
			return true;
		}
		else
		{
			return false;
		}
	}

	private void LoadNewTechTree() //Loads tech tree into two arrays (whether tech has been built, and the cost of each tech)
	{		
		for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
		{
			ImprovementClass newImprovement = new ImprovementClass();

			newImprovement.improvementName = systemListConstructor.basicImprovementsList[i].name;
			newImprovement.improvementCategory = systemListConstructor.basicImprovementsList[i].category;
			newImprovement.improvementCost = systemListConstructor.basicImprovementsList[i].cost;
			newImprovement.improvementLevel = systemListConstructor.basicImprovementsList[i].level;
			newImprovement.improvementMessage = "";
			newImprovement.hasBeenBuilt = false;

			listOfImprovements.Add(newImprovement);
		}
	}

	public void ActiveTechnologies(int curSystem, TurnInfo thisPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		knowledgePercentBonus = 1.0f; //Resets the percentage modifier for SIM. Is there an easier way?
		powerPercentBonus = 1.0f;
		improvementCostModifier = 0;
		knowledgeBonusModifier = 1.0f;
		populationModifier = 1.0f;
		amberPenalty = 1f;
		amberPointBonus = 0f;
		amberProductionBonus = 1f;
		researchCost = 0;
		maxPopulationBonus = 0f;

		tempCount = 0.0f;
		system = curSystem;

		for(int i = 0; i < listOfImprovements.Count; ++i)
		{
			if(listOfImprovements[i].hasBeenBuilt == true)
			{
				genericImprovements.TechSwitch(i, this, thisPlayer, false);
			}
		}

		knowledgePercentBonus = knowledgePercentBonus * knowledgeBonusModifier;
	}
	
	public int CheckDiplomaticStateOfAllPlayers(TurnInfo thisPlayer, string state)
	{
		int noOfPlayersInState = 0;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == thisPlayer.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == thisPlayer.playerRace)
			{
				diplomacyScript.relationsList[i].diplomaticState = state;
				++noOfPlayersInState;
			}
		}

		return noOfPlayersInState;
	}

	public bool IsBuiltOnPlanetType(int system, int improvementNo, string planetType)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt[j] == listOfImprovements[improvementNo].improvementName)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[i].planetType == planetType)
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}

public class ImprovementClass
{
	public string improvementName, improvementCategory;
	public float improvementCost;
	public int improvementLevel;
	public string improvementMessage;
	public bool hasBeenBuilt;
}


