﻿using UnityEngine;
using System.Collections;
using System.IO;

public class TurnInfo : MonoBehaviour 
{
	[HideInInspector]
	public int GP, raceGP, science, industry, money, planetsColonisedThisTurn;
	public float raceScience, raceIndustry, raceMoney;
	[HideInInspector]
	public string[,] planetRIM = new string[12,4];
	[HideInInspector]
	public GameObject[] systemList = new GameObject[60];
	[HideInInspector]
	public GameObject[] ownedSystems = new GameObject[60];
	[HideInInspector]
	public bool endTurn;
	public Camera mainCamera;
	public Material selkiesMaterial;
	public Material playerMaterial, humansMaterial;
	public Material nereidesMaterial;
	public Material materialInUse;
	
	public string playerRace, homeSystem;
	public int turn = 0, systemsInPlay = 0;
	
	public GUISystemDataScript guiPlanScript;
	public TechTreeScript techTreeScript;
	public HeroScript heroScript;
	public LineRenderScript lineRenderScript;
	public TurnInfo turnInfoScript;

	public PlayerTurn playerTurnScript;
	public EnemyOne enemyOneTurnScript;
	public EnemyTwo enemyTwoTurnScript;

	void Awake()
	{			
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();

		LoadPlanetData();

		systemList = GameObject.FindGameObjectsWithTag("StarSystem");
	}
	
	void LoadPlanetData() //Loads up planet stats into array
	{
		string text = " ";
		
		using(StreamReader reader =  new StreamReader("PlanetRIMData.txt"))
		{
			for(int i = 0; i < 12; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					text = reader.ReadLine();
					planetRIM[i,j] = text;
				}
			}			
		}
	}

	public void PickRace() //Start of turn function. Race choice dictates starting planet and inherent bonuses as well as racial technologies.
	{
		if(playerRace == "Humans")
		{
			raceScience = 1;
			raceIndustry = 1;
			raceMoney = 2;
			raceGP = 3;
			homeSystem = "Sol";
		}
		if(playerRace == "Selkies")
		{
			raceScience = 1;
			raceIndustry = 3;
			raceMoney = 2;
			raceGP = 2;
			homeSystem = "Heracles";
		}
		if(playerRace == "Nereides")
		{
			raceScience = 6;
			raceIndustry = 2;
			raceMoney = 4;
			raceGP = 1;
			homeSystem = "Nepthys";
		}
	}

	public void StartSystemPlanetColonise(Material playerMaterial, string homeSystem, GameObject[] thisSystemArray)
	{
		guiPlanScript = GameObject.Find (homeSystem).GetComponent<GUISystemDataScript>();
		
		for(int i = 0; i < 60; i++) //Find selected system and set it to owned
		{
			if(turnInfoScript.systemList[i] == GameObject.Find (homeSystem))
			{
				thisSystemArray[i] = turnInfoScript.systemList[i];

				thisSystemArray[i].renderer.material = playerMaterial;

				turnInfoScript.systemList[i] = null;

				++systemsInPlay;

				break;
			}
		}
		
		for(int i = 0; i < guiPlanScript.numPlanets; ++i) //Colonise ocean planet (earth).
		{
			if(guiPlanScript.planNameOwnImprov[i,0] == "Ocean")
			{
				guiPlanScript.planNameOwnImprov[i,1] = "Yes";
				break;
			}
		}
	}

	public void TurnEnd(GameObject[] systems) //This function accumulates all the SIM generated by each system to give an empire SIM value
	{		
		endTurn = true;
		
		foreach(GameObject system in systems)
		{
			if(system == null)
			{
				continue;
			}

			Debug.Log (system.name);

			if(system != null)
			{
				guiPlanScript = system.GetComponent<GUISystemDataScript>();
				techTreeScript = system.GetComponent<TechTreeScript>();
				heroScript = system.GetComponent<HeroScript>();

				techTreeScript.ActiveTechnologies();
				heroScript.CheckHeroesInSystem();
				guiPlanScript.SystemSIMCounter();

				science += (int)guiPlanScript.totalSystemScience;
				industry += (int)guiPlanScript.totalSystemIndustry;
				money += (int)guiPlanScript.totalSystemMoney;
			}
		}
		
		GP += raceGP;

		planetsColonisedThisTurn = 0;

		endTurn = false;

		//guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
	}
}
