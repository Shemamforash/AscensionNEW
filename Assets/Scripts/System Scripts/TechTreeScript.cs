using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TechTreeScript : MasterScript 
{
	public List<string> improvementMessageArray = new List<string>();

	public float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	public float sciencePointBonus, industryPointBonus, moneyPointBonus;
	public int techTier = 0;
	private int currentPlanetsWithHyperNet = 0;

	public List<ImprovementClass> listOfImprovements = new List<ImprovementClass>();

	void Start()
	{
		guiPlanScript = gameObject.GetComponent<GUISystemDataScript>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();

		LoadTechTree();
	}

	public void ImproveSystem(int improvement) //Occurs if button of tech is clicked.
	{
		if(playerTurnScript.industry >= listOfImprovements[improvement].improvementCost) //Checks cost of tech and current industry
		{
			playerTurnScript.industry -= listOfImprovements[improvement].improvementCost;
			listOfImprovements[improvement].hasBeenBuilt = true;;
		}
	}

	private void LoadTechTree() //Loads tech tree into two arrays (whether tech has been built, and the cost of each tech)
	{		
		using(StreamReader reader =  new StreamReader("ImprovementList.txt"))
		{
			for(int i = 0; i < 20; ++i)
			{
				ImprovementClass improvement = new ImprovementClass();

				improvement.improvementName = reader.ReadLine();
				improvement.improvementCategory = reader.ReadLine ();
				improvement.improvementLevel = int.Parse (reader.ReadLine ());
				improvement.improvementCost = float.Parse(reader.ReadLine ());
				improvement.hasBeenBuilt = false;

				listOfImprovements.Add (improvement);
			}
		}
	}

	public void ActiveTechnologies(int system, TurnInfo selectedPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		sciencePercentBonus = 1.0f; //Resets the percentage modifier for SIM. Is there an easier way?
		industryPercentBonus = 1.0f;
		moneyPercentBonus = 1.0f;

		sciencePointBonus = 0.0f;
		industryPointBonus = 0.0f;
		moneyPointBonus = 0.0f;

		float tempCount = 0.0f;

		if(listOfImprovements[0].hasBeenBuilt == true) //Secondary Research
		{
			for(int i = 0; i < listOfImprovements.Count; ++i)
			{
				if(listOfImprovements[i].hasBeenBuilt == true)
				{
					sciencePercentBonus += 0.05f;
					tempCount += 0.05f;
				}
			}

			improvementMessageArray.Add("+" + tempCount * 100 + "% Science from Improvements");
		}

		if(listOfImprovements[1].hasBeenBuilt == true) //Synergy
		{
			tempCount = 0.0f;

			int thisSystem = RefreshCurrentSystem(gameObject);

			for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
			{
				int k = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].connectedSystems[i]);

				if(systemListConstructor.systemList[k].systemOwnedBy == selectedPlayer.playerRace)
				{
					industryPercentBonus += 0.075f;
					tempCount += 0.075f;
				}
			}

			improvementMessageArray.Add("+" + tempCount * 100 + "% Industry from nearby systems");
		}

		if(listOfImprovements[2].hasBeenBuilt == true) //Morale
		{
			tempCount = 0.0f;

			int i = RefreshCurrentSystem(gameObject);

			for(int j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
				{
					continue;
				}

				heroScript = systemListConstructor.systemList[i].heroesInSystem[j].GetComponent<HeroScriptParent>();

				moneyPercentBonus += (heroScript.currentLevel * 5.0f);
				tempCount += (heroScript.currentLevel * 5.0f);
			}

			improvementMessageArray.Add("+" + tempCount * 100 + "% from Hero levels");
		}

		if(listOfImprovements[3].hasBeenBuilt == true) //Capitalism
		{
			tempCount = 0.0f;

			int j = CheckDiplomaticStateOfAllPlayers(selectedPlayer, "Peace");

			sciencePointBonus += (turnInfoScript.turn * Mathf.Pow (2.0f, j));
			tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));

			improvementMessageArray.Add("+" + tempCount + " Science from Peace");
		}

		if(listOfImprovements[4].hasBeenBuilt == true) //Leadership
		{
			tempCount = 0.0f;

			for(int i = 0; i < 60; ++i)
			{
				if(systemListConstructor.systemList[i].systemOwnedBy != selectedPlayer.playerRace)
				{
					continue;
				}

				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
					{
						industryPointBonus += 1;
						tempCount += 1;
					}
				}
			}

			improvementMessageArray.Add("+" + tempCount + " Industry from colonisation");
		}

		if(listOfImprovements[5].hasBeenBuilt == true) //Quick Starters
		{
			tempCount = 0.0f;

			int j = CheckDiplomaticStateOfAllPlayers(selectedPlayer, "War");

			industryPercentBonus += (j * 0.25f);
			tempCount += (j * 0.25f);
			improvementMessageArray.Add("+" + tempCount * 100 + "% Industry from War");
		}

		if(listOfImprovements[6].hasBeenBuilt == true)
		{
			tempCount = 0.0f;

			int i = RefreshCurrentSystem(gameObject);

			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
				{
					sciencePercentBonus += 0.25f;
					tempCount += 0.25f;
				}
			}

			improvementMessageArray.Add("+" + tempCount * 100 + "% Science from uncolonised planets");
		}

		if(listOfImprovements[7].hasBeenBuilt == true) //Unionisation
		{
			tempCount = 0.0f;
			bool allPlanetsColonised  = true;
			int i = RefreshCurrentSystem(gameObject);
			
			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
				{
					allPlanetsColonised = false;
				}
			}

			if(allPlanetsColonised == true)
			{
				industryPercentBonus += 0.2f;
				tempCount += 0.2f;
			}

			industryPercentBonus += 0.1f;
			tempCount += 0.1f;

			improvementMessageArray.Add("+" + tempCount * 100 + "% Industry on System");
		}

		if(listOfImprovements[8].hasBeenBuilt == true) //Familiarity
		{
			improvementMessageArray.Add("2x SIM production on Home-Type Planets");
		}

		if(listOfImprovements[9].hasBeenBuilt == true) //Hypernet
		{
			tempCount = 0.0f;
			float tempCountB = 0.0f;
			float tempCountC = 0.0f;

			sciencePointBonus += guiPlanScript.totalSystemScience;
			tempCount = guiPlanScript.totalSystemScience;

			industryPointBonus -= (0.5f * turnInfoScript.turn) * guiPlanScript.totalSystemIndustry;
			tempCountB = (0.5f * turnInfoScript.turn) * guiPlanScript.totalSystemIndustry;

			moneyPointBonus -= (0.5f * turnInfoScript.turn) * guiPlanScript.totalSystemMoney;
			tempCountC = (0.5f * turnInfoScript.turn) * guiPlanScript.totalSystemMoney;

			improvementMessageArray.Add("+" + tempCount + " Science, -" + tempCountB + " Industry, -" + tempCountC + "Money on System");
		}

		if(listOfImprovements[11].hasBeenBuilt == true)
		{
			tempCount = 0.0f;
			int i = HyperNet(selectedPlayer);

			sciencePercentBonus += (i * 0.05f);
			industryPercentBonus += (i * 0.05f);
			moneyPercentBonus += (i * 0.05f);
			tempCount = (i * 0.05f);

			improvementMessageArray.Add ("+" + tempCount + "% SIM from systems with Hypernet");
		}

		if(listOfImprovements[12].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
				{
					++systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
				}
			}

			improvementMessageArray.Add("+1 Ownership per turn");
		}

		if(listOfImprovements[13].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetCategory == "Terran")
				{
					systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership += 5;
				}
			}

			improvementMessageArray.Add("+5 Ownership on Terran");
		}

		if(listOfImprovements[14].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel == 3)
				{
					systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership = 120;
				}
			}

			improvementMessageArray.Add("+20% Max Ownership on Fully Improved Systems");
		}

		if(listOfImprovements[15].hasBeenBuilt == true)
		{
			selectedPlayer.money -= guiPlanScript.totalSystemSIM * 0.3f;

			sciencePercentBonus -= 0.3f;
			industryPercentBonus -= 0.3f;
			moneyPercentBonus -= 0.3f;

			++racialTraitScript.ambitionCounter;

			improvementMessageArray.Add("30% Money Converted to Ambition");
		}

		if(listOfImprovements[16].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership < 33)
				{
					systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership = 33;
				}
			}

			improvementMessageArray.Add("Minimum Ownership of 33%");
		}

		if(listOfImprovements[17].hasBeenBuilt == true)
		{
			tempCount = 0.0f;

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetCategory == "Terran")
				{
					racialTraitScript.ambitionCounter += 2;
					tempCount = 2.0f;
					break;
				}
			}

			improvementMessageArray.Add("+" + tempCount + " Ambition from Terran Planet");
		}

		if(listOfImprovements[18].hasBeenBuilt == true)
		{
			improvementMessageArray.Add("Ambition has no effect on planet Ownership");
		}

		if(listOfImprovements[19].hasBeenBuilt == true)
		{
			tempCount = 0.0f;

			string tempString = null;

			if(racialTraitScript.ambitionCounter > 75)
			{
				tempCount = (racialTraitScript.ambitionCounter - 75) / 100.0f;

				tempString = ("+" + tempCount + "% SIM from Renaissance");
			}
			if(racialTraitScript.ambitionCounter < -75)
			{
				tempCount = (racialTraitScript.ambitionCounter + 75) / 100.0f;
				 
				tempString = (tempCount + "% SIM from Depression");
			}

			sciencePercentBonus += tempCount;
			industryPercentBonus += tempCount;
			moneyPercentBonus += tempCount;

			improvementMessageArray.Add(tempString);
		}
	}

	private int CheckDiplomaticStateOfAllPlayers(TurnInfo selectedPlayer, string state)
	{
		int noOfPlayersInState = 0;

		if(selectedPlayer == playerTurnScript)
		{
			if(diplomacyScript.playerEnemyOneRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
			
			if(diplomacyScript.playerEnemyTwoRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
		}
		
		if(selectedPlayer == enemyOneTurnScript)
		{
			if(diplomacyScript.playerEnemyOneRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
			
			if(diplomacyScript.enemyOneEnemyTwoRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
		}
		
		if(selectedPlayer == enemyTwoTurnScript)
		{
			if(diplomacyScript.playerEnemyTwoRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
			
			if(diplomacyScript.enemyOneEnemyTwoRelations.diplomaticState == state)
			{
				++noOfPlayersInState;
			}
		}

		return noOfPlayersInState;
	}

	private int HyperNet(TurnInfo selectedPlayer) //Tier 3 tech. Bonus SIM for each connected planet. This function is good.
	{		
		currentPlanetsWithHyperNet = 0;
		
		for(int i = 0; i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null || systemListConstructor.systemList[i].systemOwnedBy == selectedPlayer.playerRace)
			{
				continue;
			}

			techTreeScript = systemListConstructor.systemList[i].systemObject.GetComponent<TechTreeScript>();
			
			if(listOfImprovements[11].hasBeenBuilt == true)
			{
				++currentPlanetsWithHyperNet;
			}
		}

		return currentPlanetsWithHyperNet;
	}
}

public class ImprovementClass
{
	public string improvementName, improvementCategory;
	public float improvementCost;
	public int improvementLevel;
	public bool hasBeenBuilt;
}


