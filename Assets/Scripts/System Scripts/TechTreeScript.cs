using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class TechTreeScript : MasterScript 
{
	public float sciencePercentBonus, industryPercentBonus;
	public float sciencePointBonus, industryPointBonus, tempCount;
	public GameObject tooltip;
	public int techTier = 0;
	private int currentPlanetsWithHyperNet = 0;

	public List<ImprovementClass> listOfImprovements = new List<ImprovementClass>();

	void Start()
	{
		sciencePercentBonus = 0; industryPercentBonus = 0;

		systemSIMData = gameObject.GetComponent<SystemSIMData>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();

		LoadNewTechTree();
	}

	public bool ImproveSystem(int improvement) //Occurs if button of tech is clicked.
	{
		if(playerTurnScript.industry >= listOfImprovements[improvement].improvementCost) //Checks cost of tech and current industry
		{
			playerTurnScript.industry -= listOfImprovements[improvement].improvementCost;
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

	public void ActiveTechnologies(int system, TurnInfo thisPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		sciencePercentBonus = 0.0f; //Resets the percentage modifier for SIM. Is there an easier way?
		industryPercentBonus = 0.0f;

		sciencePointBonus = 0.0f;
		industryPointBonus = 0.0f;

		tempCount = 0.0f;

		if(techTier >= 0)
		{
			CheckTierZero(system, thisPlayer);
		}
		if(techTier >= 1)
		{
			CheckTierOne(system, thisPlayer);
		}
		if(techTier >= 2)
		{
			CheckTierTwo(system, thisPlayer);
		}
		if(techTier == 3)
		{
			CheckTierThree(system, thisPlayer);
		}
		if(thisPlayer.playerRace == "Humans")
		{
			CheckHumanImprovements(system, thisPlayer);
		}
	}

	public void CheckTierZero(int system, TurnInfo thisPlayer)
	{
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

			listOfImprovements[0].improvementMessage = ("+" + tempCount * 100 + "% Science from Improvements");
		}
		
		if(listOfImprovements[1].hasBeenBuilt == true) //Synergy
		{
			tempCount = 0.0f;
			
			int thisSystem = RefreshCurrentSystem(gameObject);
			
			for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
			{
				int k = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);
				
				if(systemListConstructor.systemList[k].systemOwnedBy == thisPlayer.playerRace)
				{
					industryPercentBonus += 0.075f;
					tempCount += 0.075f;
				}
			}
			
			listOfImprovements[1].improvementMessage = ("+" + tempCount * 100 + "% Industry from nearby systems");
		}
		
		if(listOfImprovements[2].hasBeenBuilt == true) //Morale
		{
			tempCount = 0.0f;
			
			for(int j = 0; j < thisPlayer.playerOwnedHeroes.Count; ++j)
			{				
				heroScript = thisPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();

				if(heroScript.heroLocation == gameObject)
				{
					//moneyPercentBonus += (heroScript.currentLevel * 5.0f); //TODO
					tempCount += (heroScript.currentLevel * 5.0f);
				}
			}
			
			listOfImprovements[2].improvementMessage = ("+" + tempCount + "% from Hero levels");
		}
	}
	
	public void CheckTierOne(int system, TurnInfo thisPlayer)
	{
		if(listOfImprovements[3].hasBeenBuilt == true) //Capitalism
		{
			tempCount = 0.0f;
			
			int j = CheckDiplomaticStateOfAllPlayers(thisPlayer, "Peace");

			if(j != 0)
			{
				sciencePointBonus += (turnInfoScript.turn / 20 * Mathf.Pow (2.0f, j));
				tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));
			}
			
			listOfImprovements[3].improvementMessage = ("+" + tempCount + " Science from Peace");
		}
		
		if(listOfImprovements[4].hasBeenBuilt == true) //Leadership
		{
			tempCount = 0.0f;
			
			for(int i = 0; i < systemListConstructor.mapSize; ++i)
			{
				if(systemListConstructor.systemList[i].systemOwnedBy != thisPlayer.playerRace)
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
			
			listOfImprovements[4].improvementMessage = ("+" + tempCount + " Industry from colonisation");
		}
		
		if(listOfImprovements[5].hasBeenBuilt == true) //Quick Starters
		{
			tempCount = 0.0f;
			
			int j = CheckDiplomaticStateOfAllPlayers(thisPlayer, "War");
			
			industryPercentBonus += (j * 0.25f);
			tempCount += (j * 0.25f);
			listOfImprovements[5].improvementMessage = ("+" + tempCount * 100 + "% Industry from War");
		}
	}
	
	public void CheckTierTwo(int system, TurnInfo thisPlayer)
	{
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
			
			listOfImprovements[6].improvementMessage = ("+" + tempCount * 100 + "% Science from uncolonised planets");
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
			
			listOfImprovements[7].improvementMessage = ("+" + tempCount * 100 + "% Industry on System");
		}
		
		if(listOfImprovements[8].hasBeenBuilt == true) //Familiarity
		{
			listOfImprovements[8].improvementMessage = ("2x SIM production on Home-Type Planets");
		}
	}
	
	public void CheckTierThree(int system, TurnInfo thisPlayer)
	{
		if(listOfImprovements[9].hasBeenBuilt == true) //Hypernet
		{
			tempCount = 0.0f;
			float tempCountB = 0.0f;
			
			sciencePointBonus += (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			tempCount = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			
			industryPointBonus -= (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			tempCountB = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			
			listOfImprovements[9].improvementMessage = ("+" + tempCount + " Science, -" + tempCountB + " Industry On System");
		}
		
		if(listOfImprovements[11].hasBeenBuilt == true)
		{
			tempCount = 0.0f;
			int i = HyperNet(thisPlayer);
			
			sciencePercentBonus += (i * 0.05f);
			industryPercentBonus += (i * 0.05f);
			tempCount = (i * 0.05f);
			
			listOfImprovements[11].improvementMessage = ("+" + tempCount + "% SIM from systems with Hypernet");
		}
	}

	public void CheckHumanImprovements(int system, TurnInfo thisPlayer)
	{
		if(listOfImprovements[12].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership < systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership)
					{
						++systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
					}
				}
			}
			
			listOfImprovements[12].improvementMessage = ("+1 Ownership per turn");
		}
		
		if(listOfImprovements[13].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetCategory == "Terran")
				{
					if(5 > systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership - systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership)
					{
						systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership += systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership - 
							systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
					}
					else
					{
						systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership += 5;
					}
				}
			}
			
			listOfImprovements[13].improvementMessage = ("+5 Ownership on Terran");
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
			
			listOfImprovements[14].improvementMessage = ("+20% Max Ownership on Fully Improved Systems");
		}
		
		if(listOfImprovements[15].hasBeenBuilt == true)
		{			
			sciencePercentBonus -= 0.3f;
			industryPercentBonus -= 0.3f;
			
			++racialTraitScript.ambitionCounter;
			
			listOfImprovements[15].improvementMessage = ("-30% SIM Converted to Ambition");
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
			
			listOfImprovements[16].improvementMessage = ("Minimum Ownership of 33%");
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
			
			listOfImprovements[17].improvementMessage = ("+" + tempCount + " Ambition from Terran Planet");
		}
		
		if(listOfImprovements[18].hasBeenBuilt == true)
		{
			listOfImprovements[18].improvementMessage = ("Ambition has no effect on planet Ownership");
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
			
			listOfImprovements[19].improvementMessage = (tempString);
		}
	}

	private int CheckDiplomaticStateOfAllPlayers(TurnInfo thisPlayer, string state)
	{
		int noOfPlayersInState = 0;

		if(thisPlayer == playerTurnScript)
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
		
		if(thisPlayer == enemyOneTurnScript)
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
		
		if(thisPlayer == enemyTwoTurnScript)
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

	private int HyperNet(TurnInfo thisPlayer) //Tier 3 tech. Bonus SIM for each connected planet. This function is good.
	{		
		currentPlanetsWithHyperNet = 0;
		
		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null || systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
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
	public string improvementMessage;
	public bool hasBeenBuilt;
}


