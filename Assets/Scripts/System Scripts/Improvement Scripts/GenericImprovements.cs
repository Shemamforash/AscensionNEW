using UnityEngine;
using System.Collections;

public class GenericImprovements : MasterScript
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo thisPlayer;
	private HumanImprovements humanImprovements;
	private NereidesImprovements nereidesImprovements;
	private SelkiesImprovements selkiesImprovements;

	public void Start()
	{
		humanImprovements = GameObject.Find ("ScriptsContainer").GetComponent<HumanImprovements> ();
		nereidesImprovements = GameObject.Find ("ScriptsContainer").GetComponent<NereidesImprovements> ();
		selkiesImprovements = GameObject.Find ("ScriptsContainer").GetComponent<SelkiesImprovements> ();
	}

	public void TechSwitch(int tech, ImprovementsBasic tempImprov, TurnInfo player, bool check)
	{
		improvements = tempImprov;

		systemSIMData = systemListConstructor.systemList [improvements.system].systemObject.GetComponent<SystemSIMData> ();

		checkValue = check;
		thisPlayer = player;

		improvements.planetToBuildOn = null;

		improvements.tempKnwlUnitBonus = 0f;  
		improvements.tempPowUnitBonus = 0f; 

		improvements.tempKnwlBonus = 0f; //To Unit
		improvements.tempPowBonus = 0f; //To Unit
		improvements.tempOwnershipBonus = 0f; //To Unit
		improvements.tempAmberPenalty = 0f; //To Unit
		improvements.tempOwnershipUnitBonus = 0f; //To Unit

		improvements.tempBonusAmbition = 0f; 
		improvements.tempAmberProductionBonus = 0f; 
		improvements.tempAmberPointBonus = 0f;
		improvements.tempImprovementSlots = 0f;
		improvements.tempWealth = 0f; 
		
		improvements.tempResearchCostReduction = 0f;  
		improvements.tempImprovementCostReduction = 0f; 
		
		improvements.tempCount = 0f;		
		
		switch (tech)
		{
		case 0:
			T0I1();
			break;
		case 1:
			T0I2();
			break;
		case 2:
			T0I3();
			break;
		case 3:
			T1I1();
			break;
		case 4:
			T1I2();
			break;
		case 5:
			T1I3();
			break;
		case 6:
			T2I1();
			break;
		case 7:
			T2I2();
			break;
		case 8:
			T2I3();
			break;
		case 9:
			T3I1();
			break;
		case 10:
			T3I2();
			break;
		case 11:
			T3I3();
			break;
		default:
			break;
		}
		
		if(thisPlayer.playerRace == "Humans")
		{
			humanImprovements.TechSwitch(tech, tempImprov, thisPlayer, checkValue);
		}
		if(thisPlayer.playerRace == "Nereides")
		{
			nereidesImprovements.TechSwitch(tech, tempImprov, thisPlayer, checkValue);
		}
		if(thisPlayer.playerRace == "Selkies")
		{
			selkiesImprovements.TechSwitch(tech, tempImprov, thisPlayer, checkValue);
		}
	}

	private void T0I1()
	{
		for(int i = 0; i < improvements.listOfImprovements.Count; ++i)
		{
			if(improvements.listOfImprovements[i].hasBeenBuilt == true)
			{
				improvements.tempKnwlBonus += 0.05f;
				improvements.tempPowBonus += 0.05f;
				improvements.tempCount += 0.05f;
			}
		}

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += improvements.tempKnwlBonus;
			improvements.powerPercentBonus += improvements.tempPowBonus;
			improvements.listOfImprovements[0].improvementMessage = ("+" + improvements.tempCount * 100f + "% Production from Improvements");
		}
	}

	private void T0I2()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].permanentConnections.Count; ++i)
		{
			int k = RefreshCurrentSystem(systemListConstructor.systemList[improvements.system].permanentConnections[i]);
			
			if(systemListConstructor.systemList[k].systemOwnedBy == thisPlayer.playerRace)
			{
				improvements.tempPowBonus += 0.075f;
				improvements.tempCount += 0.075f;
			}
		}

		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;

		if(checkValue == false)
		{
			improvements.powerPercentBonus += improvements.tempPowBonus;
			improvements.listOfImprovements[1].improvementMessage = ("+" + improvements.tempCount * 100f + "% Power from nearby systems");
		}
	}

	private void T0I3()
	{
		improvements.tempWealth += thisPlayer.playerOwnedHeroes.Count * 0.02f;

		if(checkValue == false)
		{
			thisPlayer.wealth += improvements.tempWealth;
			improvements.listOfImprovements[2].improvementMessage = ("+" + (thisPlayer.playerOwnedHeroes.Count * 2f) + "% Wealth from active Heroes");
		}
	}

	private void T1I1()
	{
		int j = improvements.CheckDiplomaticStateOfAllPlayers(thisPlayer, "Peace");
		
		if(j != 0)
		{
			systemSIMData = systemListConstructor.systemList[improvements.system].systemObject.GetComponent<SystemSIMData>();
		}

		improvements.tempKnwlUnitBonus = (turnInfoScript.turn / 20 * Mathf.Pow (2.0f, j));
		improvements.tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));

		if(checkValue == false)
		{
			systemSIMData.knowledgeUnitBonus += improvements.tempKnwlUnitBonus;
			improvements.listOfImprovements[3].improvementMessage = ("+" + improvements.tempCount + " Knowledge from Peace");
		}
	}

	private void T1I2()
	{
		int tempCount = CheckNumberOfPlanetsWithImprovement(4, thisPlayer, improvements);
		
		improvements.tempPowBonus = (tempCount * 0.05f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;

		if(checkValue == false)
		{
			improvements.powerPercentBonus += improvements.tempPowBonus;
			improvements.listOfImprovements[4].improvementMessage = ("+" + improvements.tempCount * 5f + "% Power from other Systems with this Improvement");
		}
	}

	private void T1I3()
	{
		int j = improvements.CheckDiplomaticStateOfAllPlayers(thisPlayer, "War");
		
		if(j != 0)
		{
			improvements.tempOwnershipBonus += 20f;
			improvements.tempCount = 20f;
		}

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempOwnershipBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempOwnershipBonus / 66.666f);

		if(checkValue == false)
		{
			improvements.maxOwnershipBonus += improvements.tempOwnershipBonus;
			improvements.listOfImprovements[5].improvementMessage = ("+" + improvements.tempCount + "% Max Ownership on Planets from War");
		}
	}

	private void T2I1()
	{
		for(int j = 0; j <  systemListConstructor.systemList[improvements.system].systemSize; ++j)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetColonised == false)
			{
				improvements.tempKnwlBonus += 0.25f;
				improvements.tempCount += 0.25f;
			}
		}

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += improvements.tempKnwlBonus;
			improvements.listOfImprovements[6].improvementMessage = ("+" + improvements.tempCount * 100f + "% Knowledge from uncolonised planets");
		}
	}

	private void T2I2()
	{
		bool allPlanetsColonised  = true;
		
		for(int j = 0; j <  systemListConstructor.systemList[improvements.system].systemSize; ++j)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetColonised == false)
			{
				allPlanetsColonised = false;
			}
		}
		
		if(allPlanetsColonised == true)
		{
			improvements.tempPowUnitBonus += 0.2f;
			improvements.tempCount += 0.2f;
		}
		
		improvements.tempPowBonus += 0.1f;
		improvements.tempCount += 0.1f;
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;

		if(checkValue == false)
		{
			improvements.powerPercentBonus += improvements.tempPowBonus;
			improvements.listOfImprovements[7].improvementMessage = ("+" + improvements.tempCount * 100f + "% Power on System");
		}
	}

	private void T2I3()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].planetsInSystem.Count; ++i)
		{
			string cat = null;

			switch(thisPlayer.playerRace)
			{
			case "Selkies":
				cat = "Hot";
				break;
			case "Humans":
				cat = "Terran";
				break;
			case "Nereides":
				cat = "Cold";
				break;
			default:
				break;
			}

			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetCategory == cat)
			{
				improvements.tempKnwlUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetKnowledge;
				improvements.tempPowUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPower;
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[8].improvementMessage = ("2x SI production on Home-Type Planets");
		}
	}

	private void T3I1()
	{
		float tempCountB = 0.0f;
		
		improvements.tempKnwlUnitBonus += (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemKnowledge;
		improvements.tempCount = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemKnowledge;
		
		improvements.tempPowUnitBonus += (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemPower;
		tempCountB = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemPower;

		if(checkValue == false)
		{
			systemSIMData.knowledgeUnitBonus += improvements.tempKnwlUnitBonus;
			systemSIMData.powerUnitBonus -= improvements.tempPowUnitBonus;
			improvements.listOfImprovements[9].improvementMessage = ("+" + improvements.tempCount + " Knowledge, -" + tempCountB + " Power On System");
		}
	}

	private void T3I2()
	{
		//TODO
	}

	private void T3I3()
	{
		improvements.tempResearchCostReduction = CheckNumberOfPlanetsWithImprovement(11, thisPlayer, improvements);

		if(checkValue == false)
		{
			improvements.researchCost += (int)improvements.tempResearchCostReduction;
			improvements.listOfImprovements[11].improvementMessage = ("-" + improvements.tempCount + " Research cost from other Systems with this Improvement");
		}
	}

	private int CheckNumberOfPlanetsWithImprovement(int improvementNo, TurnInfo thisPlayer, ImprovementsBasic improvements)
	{
		int currentPlanets = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null || systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				continue;
			}
			
			if(improvements.listOfImprovements[improvementNo].hasBeenBuilt == true)
			{
				++currentPlanets;
			}
		}

		return currentPlanets;
	}
}
