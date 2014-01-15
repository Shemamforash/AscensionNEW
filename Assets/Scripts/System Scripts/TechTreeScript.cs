using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TechTreeScript : MasterScript 
{
	public string[] improvementMessageArray = new string[24];

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
			for(int i = 0; i < 12; ++i)
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

	public void AddImprovementMessage(string message, int tech)
	{
		improvementMessageArray[tech] = message;
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

			AddImprovementMessage("+" + tempCount + "% Science from Improvements", 0);
		}

		if(listOfImprovements[1].hasBeenBuilt == true) //Synergy
		{
			tempCount = 0.0f;

			for(int i = 0; i < 4; ++i)
			{
				if(lineRenderScript.connections[i] == null)
				{
					break;
				}

				int k = RefreshCurrentSystem(lineRenderScript.connections[i]);

				if(systemListConstructor.systemList[k].systemOwnedBy == selectedPlayer.playerRace)
				{
					industryPercentBonus += 0.075f;
					tempCount += 0.075f;
				}
			}

			AddImprovementMessage("+" + tempCount + "% Industry from nearby systems", 1);
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

			AddImprovementMessage("+" + tempCount + "% from Hero levels", 2);
		}

		if(listOfImprovements[3].hasBeenBuilt == true) //Capitalism
		{
			tempCount = 0.0f;

			int j = CheckDiplomaticStateOfAllPlayers(selectedPlayer, "Peace");

			sciencePointBonus += (turnInfoScript.turn * Mathf.Pow (2.0f, j));
			tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));

			AddImprovementMessage("+" + tempCount + " Science from Peace", 3);
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
					if(systemListConstructor.systemList[i].planetColonised[j] == true)
					{
						industryPointBonus += 1;
						tempCount += 1;
					}
				}
			}

			AddImprovementMessage("+" + tempCount + " Industry from colonisation", 4);
		}

		if(listOfImprovements[5].hasBeenBuilt == true) //Quick Starters
		{
			tempCount = 0.0f;

			int j = CheckDiplomaticStateOfAllPlayers(selectedPlayer, "War");

			industryPercentBonus += (j * 0.25f);
			tempCount += (j * 0.25f);
			AddImprovementMessage("+" + tempCount + "% Industry from War", 5);
		}

		if(listOfImprovements[6].hasBeenBuilt == true)
		{
			tempCount = 0.0f;

			int i = RefreshCurrentSystem(gameObject);

			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetColonised[j] == false)
				{
					sciencePercentBonus += 0.25f;
					tempCount += 0.25f;
				}
			}

			AddImprovementMessage("+" + tempCount + "% Science from uncolonised planets", 6);
		}

		if(listOfImprovements[7].hasBeenBuilt == true) //Unionisation
		{
			tempCount = 0.0f;
			bool allPlanetsColonised  = true;
			int i = RefreshCurrentSystem(gameObject);
			
			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetColonised[j] == false)
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

			AddImprovementMessage("+" + tempCount + "% Industry on System", 7);
		}

		if(listOfImprovements[8].hasBeenBuilt == true) //Familiarity
		{
			int i = RefreshCurrentSystem(gameObject);
			
			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetType[j] == selectedPlayer.homePlanetType)
				{
					sciencePointBonus += systemListConstructor.systemList[i].planetScience[j];
					industryPointBonus += systemListConstructor.systemList[i].planetIndustry[j];
					moneyPointBonus += systemListConstructor.systemList[i].planetMoney[j];
				}
			}

			AddImprovementMessage("2x SIM production on Home-Type Planets", 8);
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

			AddImprovementMessage("+" + tempCount + " Science, -" + tempCountB + " Industry, -" + tempCountC + "Money on System", 9);
		}

		if(listOfImprovements[11].hasBeenBuilt == true)
		{
			tempCount = 0.0f;
			int i = HyperNet(selectedPlayer);

			sciencePercentBonus += (i * 0.05f);
			industryPercentBonus += (i * 0.05f);
			moneyPercentBonus += (i * 0.05f);
			tempCount = (i * 0.05f);

			AddImprovementMessage ("+" + tempCount + "% SIM from systems with Hypernet", 14);
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


