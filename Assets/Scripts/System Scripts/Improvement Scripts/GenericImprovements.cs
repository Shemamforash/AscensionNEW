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

		systemSIMData = systemListConstructor.systemList [tempImprov.system].systemObject.GetComponent<SystemSIMData> ();

		checkValue = check;
		thisPlayer = player;

		improvements.planetToBuildOn.Clear ();

		improvements.tempKnwlUnitBonus = 0f;  
		improvements.tempPowUnitBonus = 0f; 

		improvements.tempKnwlBonus = 0f; //To Unit
		improvements.tempPowBonus = 0f; //To Unit
		improvements.tempPopulationBonus = 0f; //To Unit
		improvements.tempAmberPenalty = 0f; //To Unit
		improvements.tempPopulationUnitBonus = 0f; //To Unit

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

	private void T0I1() //Amplification
	{
		for(int i = 0; i < improvements.listOfImprovements.Count; ++i)
		{
			if(improvements.listOfImprovements[i].hasBeenBuilt == true)
			{
				improvements.tempKnwlBonus += 0.025f;
				improvements.tempPowBonus += 0.025f;
				improvements.tempCount += 0.025f;
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

	private void T0I2() //Fertile Link
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

	private void T0I3() //Fortune
	{
		if(checkValue == false)
		{
			improvements.resourceYieldBonus += 0.5f;
			improvements.listOfImprovements[2].improvementMessage = ("+50% Yield on Secondary Resources");
		}
	}

	private void T1I1() //Injection
	{
		if(checkValue == false)
		{
			improvements.listOfImprovements[3].improvementMessage = ("Resource Bonus on Secondary Resource Generation");
		}
	}

	private void T1I2() //Custodians
	{
		int tempCount = CheckNumberOfPlanetsWithImprovement(4, thisPlayer, improvements);
		
		improvements.tempPowBonus = (tempCount * 0.005f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;

		if(checkValue == false)
		{
			improvements.powerPercentBonus += improvements.tempPowBonus;
			improvements.listOfImprovements[4].improvementMessage = ("+" + improvements.tempCount * 0.5f + "% Power from other Systems with this Improvement");
		}
	}

	private void T1I3() //Isolation
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].planetsInSystem.Count; ++i)
		{
			float basePop = (systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetImprovementLevel + 1) * 25f;

			systemListConstructor.systemList[improvements.system].planetsInSystem[i].maxPopulation = basePop + systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetImprovementLevel * 10f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[5].improvementMessage = ("+10% * Planet Quality Max Population on Planets");
		}
	}

	private void T2I1() //Inertia
	{
		improvements.tempCount = 0f;

		for(int i = 0; i <  systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt[j] == "")
				{
					improvements.tempKnwlBonus += 0.05f;
					improvements.tempCount += 0.05f;
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[6].improvementMessage = ("+" + improvements.tempCount * 100f + "% Knowledge from uncolonised planets");
		}
	}

	private void T2I2() //Nostalgia
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
