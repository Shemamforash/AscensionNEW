using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GUISystemDataScript : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber;
	[HideInInspector]
	public float pScience, pIndustry, pMoney, improvementCost, resourceBonus;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public string[] allPlanetsInfo = new string[6];	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, isOkToColonise;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM, tempTotalSci, tempTotalInd, tempTotalMon;
	public float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;

	private GameObject[] systemConnections = new GameObject[4];

	private TurnInfo playerOwnedSystem;

	void Start()
	{
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();
	}

	public void CheckOwnership()
	{
		int i = masterScript.RefreshCurrentSystem(gameObject);

		if(masterScript.systemList[i].systemOwnedBy != null && playerOwnedSystem == null)
		{
			if(masterScript.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
			{
				playerOwnedSystem = playerTurnScript;
			}
			if(masterScript.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				playerOwnedSystem = enemyOneTurnScript;
			}
			if(masterScript.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				playerOwnedSystem = enemyTwoTurnScript;
			}
		}
	}

	public void FindSystem(TurnInfo thisPlayer) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{		
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		
		systemConnections = lineRenderScript.connections;

		if(thisPlayer.playerRace == playerTurnScript.playerRace)
		{
			PlayerColoniseSystem(systemConnections);

			lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		}

		if(thisPlayer.playerRace != playerTurnScript.playerRace)
		{
			isOkToColonise = true;
		}

		if(isOkToColonise == true && thisPlayer.GP > 0)
		{
			for(int i = 0; i < 60; i ++)
			{
				if(masterScript.systemList[i].systemObject == gameObject)
				{
					masterScript.systemList[i].systemOwnedBy = thisPlayer.playerRace;

					lineRenderScript = masterScript.systemList[i].systemObject.GetComponent<LineRenderScript>();

					lineRenderScript.SetRaceLineColour(playerTurnScript.playerRace);

					gameObject.renderer.material = thisPlayer.materialInUse;

					thisPlayer.GP -= 1;
					
					++turnInfoScript.systemsInPlay;
					
					cameraFunctionsScript.coloniseMenu = false;

					break;
				}
			}

			CheckOwnership();
		}
	}

	void PlayerColoniseSystem(GameObject[] connections)
	{
		for(int i = 0; i < 4; ++i)
		{
			if(connections[i] == null)
			{
				break;
			}

			int j = masterScript.RefreshCurrentSystem(connections[i]);

			for(int k = 0; k < 60; ++k)
			{
				if(masterScript.systemList[k].systemName == "Sol")
				{
					Debug.Log(masterScript.systemList[k].systemOwnedBy);
				}
			}

			if(masterScript.systemList[j].systemOwnedBy == playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}

			else
			{
				continue;
			}
		}
	}

	public void SystemSIMCounter(int i) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{				
		tempTotalSci = 0.0f;
		tempTotalInd = 0.0f;
		tempTotalMon = 0.0f;

		for(int j = 0; j < masterScript.systemList[i].systemSize; ++j)
		{
			if(masterScript.systemList[i].planetColonised[j] == true)
			{
				string planetType = masterScript.systemList[i].planetType[j];

				improvementNumber = masterScript.systemList[i].planetImprovementLevel[j];

				CheckImprovement();
				
				tempSci = masterScript.systemList[i].planetScience[j]; //Need to sort out variable types, too much casting
				tempInd = masterScript.systemList[i].planetIndustry[j];
				tempMon = masterScript.systemList[i].planetMoney[j];

				techTreeScript.planetToCheck = planetType;

				techTreeScript.CheckPlanets();

				allPlanetsInfo[i] = gameObject.name + " " + (j+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
					+ ((int)(tempSci * resourceBonus * playerOwnedSystem.raceScience)).ToString() + "\n" 
					+ ((int)(tempInd * resourceBonus * playerOwnedSystem.raceIndustry)).ToString() + "\n" 
					+ ((int)(tempMon * resourceBonus * playerOwnedSystem.raceMoney)).ToString();

				tempTotalSci += tempSci * techTreeScript.sciencePercentBonus * resourceBonus * playerOwnedSystem.raceScience;
				tempTotalInd += tempInd * techTreeScript.industryPercentBonus * resourceBonus * playerOwnedSystem.raceIndustry;
				tempTotalMon += tempMon * techTreeScript.moneyPercentBonus * resourceBonus * playerOwnedSystem.raceMoney;
			}
		}

		//heroGUIScript.CheckHeroesInSystem(); need to include heroscript

		totalSystemScience = tempTotalSci + techTreeScript.sciencePointBonus + heroScript.heroSciBonus;
		totalSystemIndustry = tempTotalInd + techTreeScript.industryPointBonus  + heroScript.heroIndBonus;
		totalSystemMoney = tempTotalMon + techTreeScript.moneyPointBonus  + heroScript.heroMonBonus;
		
		turnInfoScript.RefreshPlanetPower();
	}

	public void CheckUnlockedTier()
	{
		totalSystemSIM += totalSystemScience + totalSystemIndustry + totalSystemMoney;
			
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

	public void UpdatePlanetPowerArray(int k)
	{
		for(int i = 0; i < numPlanets; ++i)
		{
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 0] = gameObject.name;
			
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 1] = i.ToString();
				
			improvementNumber = masterScript.systemList[k].planetImprovementLevel[i];
			
			CheckImprovement();
			
			float tempInt = masterScript.systemList[k].planetScience[i] + masterScript.systemList[k].planetIndustry[i] + masterScript.systemList[k].planetMoney[i];
			
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 2] = tempInt.ToString();

			++turnInfoScript.savedIterator;
		}
	}

	public void CheckImprovement() //Contains data on the quality of planets and the bonuses they receive
	{
		if(improvementNumber == 0)
		{
			improvementLevel = "Poor";
			resourceBonus = 0.5f;
			canImprove = true;
			improvementCost = 10.0f;
		}
		if(improvementNumber == 1)
		{
			improvementLevel = "Normal";
			resourceBonus = 1.0f;
			canImprove = true;
			improvementCost = 20.0f;
		}
		if(improvementNumber == 2)
		{
			improvementLevel = "Good";
			resourceBonus = 2.0f;
			canImprove = true;
			improvementCost = 40.0f;
		}
		if(improvementNumber == 3)
		{
			improvementLevel = "Superb";
			resourceBonus = 3.0f;
			canImprove = false;
		}
	}
}
