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
	public string[,] planNameOwnImprov = new string[6,4]; //Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, isOkToColonise;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM;
	public float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;
	
	private string text = " ";
	private GameObject[] systemConnections = new GameObject[4];

	private TurnInfo playerOwnedSystem;

	void Awake()
	{
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();

		LoadFile();		
	}

	public void CheckOwnership()
	{
		if(lineRenderScript.ownedBy != null && playerOwnedSystem == null)
		{
			if(lineRenderScript.ownedBy == playerTurnScript.playerRace)
			{
				playerOwnedSystem = playerTurnScript;
			}
			if(lineRenderScript.ownedBy == enemyOneTurnScript.playerRace)
			{
				playerOwnedSystem = enemyOneTurnScript;
			}
			if(lineRenderScript.ownedBy == enemyTwoTurnScript.playerRace)
			{
				playerOwnedSystem = enemyTwoTurnScript;
			}
		}
	}

	private void LoadFile() //This fills the systems array with all the planets in the system, whether they are owned, and their improvement level
	{
		using(StreamReader reader =  new StreamReader("SystemTypeData.txt"))
		{
			while(text != null)
			{
				text = reader.ReadLine();
			
				if(text == gameObject.name)
				{		
					text = reader.ReadLine();
			
					numPlanets = int.Parse (text);
			
					for(int i = 0; i < numPlanets; i++)
					{
						text = reader.ReadLine();
					
						planNameOwnImprov[i,0] = text;
						
						planNameOwnImprov[i,1] = "No";
						
						planNameOwnImprov[i,2] = "0";

						FindPlanetImprovementSlots(i);
					}
				
					break;
				}
			}
		}
	}

	private void FindPlanetImprovementSlots(int thisInt)
	{
		for(int j = 0; j < 12; ++j)
		{
			if(planNameOwnImprov[thisInt, 0] == turnInfoScript.planetRIM[j, 0])
			{
				planNameOwnImprov[thisInt, 3] = turnInfoScript.planetRIM[j, 4];
			}
		}
	}

	public void FindSystem(TurnInfo thisPlayer) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{		
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		
		systemConnections = lineRenderScript.connections;

		int arrayPosition = 0;

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
				if(turnInfoScript.systemList[i] == gameObject)
				{
					arrayPosition = i;
					break;
				}
			}

			thisPlayer.ownedSystems[arrayPosition] = gameObject;
			
			lineRenderScript.ownedBy = thisPlayer.playerRace;
			
			gameObject.renderer.material = thisPlayer.materialInUse;
			
			thisPlayer.GP -= 1;

			++turnInfoScript.systemsInPlay;
			
			cameraFunctionsScript.coloniseMenu = false;
			
			planNameOwnImprov[0,1] = "Yes";

			CheckOwnership();
		}
	}

	void PlayerColoniseSystem(GameObject[] connections)
	{
		foreach(GameObject connection in connections)
		{
			if(connection == null)
			{
				break;
			}

			lineRenderScript = connection.GetComponent<LineRenderScript>();

			if(lineRenderScript.ownedBy == playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}
			else
			{
				continue;
			}
		}
	}

	public void SystemSIMCounter() //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{				
		totalSystemScience = 0.0f;
		totalSystemIndustry = 0.0f;
		totalSystemMoney = 0.0f;

		for(int n = 0; n < numPlanets; ++n)
		{
			if(planNameOwnImprov[n,0] != null && planNameOwnImprov[n,1] == "Yes")
			{
				string planetType = planNameOwnImprov[n,0];

				improvementNumber = int.Parse (planNameOwnImprov[n,2]);

				CheckImprovement();
				
				for(int j = 0; j < 12; ++j)
				{
					if(turnInfoScript.planetRIM[j,0] == planetType)
					{
						tempSci = float.Parse(turnInfoScript.planetRIM[j,1]); //Need to sort out variable types, too much casting
						tempInd = float.Parse(turnInfoScript.planetRIM[j,2]);
						tempMon = float.Parse(turnInfoScript.planetRIM[j,3]);

						techTreeScript.planetToCheck = planetType;

						techTreeScript.CheckPlanets();
					}

					allPlanetsInfo[n] = gameObject.name + " " + (n+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
						+ ((int)(tempSci * resourceBonus * playerOwnedSystem.raceScience)).ToString() + "\n" 
						+ ((int)(tempInd * resourceBonus * playerOwnedSystem.raceIndustry)).ToString() + "\n" 
						+ ((int)(tempMon * resourceBonus * playerOwnedSystem.raceMoney)).ToString();
				}

				totalSystemScience += (tempSci * techTreeScript.sciencePercentBonus * resourceBonus * playerOwnedSystem.raceScience) + techTreeScript.sciencePointBonus;
				totalSystemIndustry += (tempInd * techTreeScript.industryPercentBonus * resourceBonus * playerOwnedSystem.raceIndustry) + techTreeScript.industryPointBonus;
				totalSystemMoney += (tempMon * techTreeScript.moneyPercentBonus * resourceBonus * playerOwnedSystem.raceMoney) + techTreeScript.moneyPointBonus;

				turnInfoScript.RefreshPlanetPower();
			}
		}
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

	public void UpdatePlanetPowerArray()
	{
		for(int i = 0; i < numPlanets; ++i)
		{
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 0] = gameObject.name;
			
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 1] = i.ToString();
			
			for(int j = 0; j < 12; ++j)
			{
				if(turnInfoScript.planetRIM[j, 0] == planNameOwnImprov[i, 0])
				{								
					improvementNumber = int.Parse (planNameOwnImprov[i, 2]);
					
					CheckImprovement();
					
					float tempSciInt = float.Parse (turnInfoScript.planetRIM[j,1]); 

					float tempIndInt = float.Parse (turnInfoScript.planetRIM[j,2]); 
					float tempMonInt = float.Parse (turnInfoScript.planetRIM[j,3]); 
					
					float tempInt = tempSciInt + tempIndInt + tempMonInt;
					
					turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 2] = tempInt.ToString();
					
					break;
				}
			}

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
