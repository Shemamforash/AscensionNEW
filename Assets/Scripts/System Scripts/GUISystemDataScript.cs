using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GUISystemDataScript : MonoBehaviour 
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
	public string[,] planNameOwnImprov = new string[6,3]; //Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData, isOkToColonise;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM;
	public float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;
	
	private string text = " ";
	private GameObject[] systemConnections = new GameObject[4];

	private TurnInfo turnInfoScript;
	private PlayerTurn playerTurnScript;
	private EnemyAIBasic enemyTurnScript;
	private LineRenderScript lineRenderScript;
	private CameraFunctions cameraFunctionsScript;
	private TechTreeScript techTreeScript;

	void Awake()
	{
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyAIBasic>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();

		LoadFile();		
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
					}
				
					break;
				}
			}
		}
	}
	
	public void FindSystem(TurnInfo thisPlayer) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{		
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		
		systemConnections = lineRenderScript.connections;

		string ownedByString = null;

		int arrayPosition = 0;

		if(thisPlayer == playerTurnScript)
		{
			ownedByString  = "Player";
			PlayerColoniseSystem(systemConnections);
			lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		}

		if(thisPlayer == enemyTurnScript)
		{
			ownedByString  = "Enemy";
			isOkToColonise = true;
		}

		if(isOkToColonise == true)
		{
			for(int i = 0; i < 60; i ++)
			{
				if(thisPlayer.systemList[i] == gameObject)
				{
					arrayPosition = i;
					break;
				}
			}

			thisPlayer.ownedSystems[arrayPosition] = gameObject;
			
			lineRenderScript.ownedBy = ownedByString;
			
			gameObject.renderer.material = thisPlayer.materialInUse;
			
			thisPlayer.GP -= 1;
			
			cameraFunctionsScript.coloniseMenu = false;
			
			planNameOwnImprov[0,1] = "Yes";
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
			if(lineRenderScript.ownedBy == "Player")
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
						tempSci = float.Parse(turnInfoScript.planetRIM[j,1]) * resourceBonus * playerTurnScript.raceScience; //Need to sort out variable types, too much casting
						tempInd = float.Parse(turnInfoScript.planetRIM[j,2]) * resourceBonus * playerTurnScript.raceIndustry;
						tempMon = float.Parse(turnInfoScript.planetRIM[j,3]) * resourceBonus * playerTurnScript.raceMoney;
						techTreeScript.planetToCheck = planetType;
						techTreeScript.CheckPlanets();
					}

					allPlanetsInfo[n] = gameObject.name + " " + (n+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
						+ ((int)(tempSci)).ToString() + "\n" + ((int)(tempInd)).ToString() + "\n" + ((int)(tempMon)).ToString();
				}

				totalSystemScience += (tempSci * techTreeScript.sciencePercentBonus) + techTreeScript.sciencePointBonus;
				totalSystemIndustry += (tempInd * techTreeScript.industryPercentBonus) + techTreeScript.industryPointBonus;
				totalSystemMoney += (tempMon * techTreeScript.moneyPercentBonus) + techTreeScript.moneyPointBonus;
			}
		}
		
		if(turnInfoScript.endTurn == true) //Checks tech tier unlocked
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
