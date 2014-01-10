﻿using UnityEngine;
using System.Collections;
using System.IO;

public class TurnInfo : MasterScript
{
	[HideInInspector]
	public int GP, raceGP, planetsColonisedThisTurn, savedIterator;
	public float raceScience, raceIndustry, raceMoney, science, industry, money;
	[HideInInspector]
	public string[,] planetRIM = new string[12,5];
	public string[,] mostPowerfulPlanets = new string[211,3];
	[HideInInspector]
	public bool endTurn;
	public Camera mainCamera;
	public Material nereidesMaterial, humansMaterial, selkiesMaterial, materialInUse;

	public string playerRace, homeSystem, homePlanetType;
	public int turn = 0, systemsInPlay = 0;

	void Start()
	{			
		RefreshPlanetPower();
	}

	public void RefreshPlanetPower()
	{
		for(int i = 0; i < 60; i++)
		{
			guiPlanScript = systemListConstructor.systemList[i].systemObject.GetComponent<GUISystemDataScript>();
			
			guiPlanScript.UpdatePlanetPowerArray(i);
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
			homePlanetType = "Ocean";
			materialInUse = turnInfoScript.humansMaterial;
		}
		if(playerRace == "Selkies")
		{
			raceScience = 1;
			raceIndustry = 3;
			raceMoney = 2;
			raceGP = 2;
			homeSystem = "Heracles";
			homePlanetType = "Plains";
			materialInUse = turnInfoScript.selkiesMaterial;
		}
		if(playerRace == "Nereides")
		{
			raceScience = 6;
			raceIndustry = 2;
			raceMoney = 4;
			raceGP = 1;
			homeSystem = "Nepthys";
			homePlanetType = "Icy";
			materialInUse = turnInfoScript.nereidesMaterial;
		}
	}

	public void TurnEnd(TurnInfo selectedPlayer) //This function accumulates all the SIM generated by each system to give an empire SIM value
	{		
		endTurn = true;

		for(int i = 0; i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy != selectedPlayer.playerRace)
			{
				continue;
			}

			for(int j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
				{
					continue;
				}
				
				heroScript = systemListConstructor.systemList[i].heroesInSystem[j].GetComponent<HeroScriptParent>();
				heroScript.HeroEndTurnFunctions();
			}

			guiPlanScript = systemListConstructor.systemList[i].systemObject.GetComponent<GUISystemDataScript>();
			techTreeScript = systemListConstructor.systemList[i].systemObject.GetComponent<TechTreeScript>();

			techTreeScript.ActiveTechnologies(selectedPlayer);
			guiPlanScript.SystemSIMCounter(i, selectedPlayer);
			guiPlanScript.CheckUnlockedTier();

			selectedPlayer.science += guiPlanScript.totalSystemScience;
			selectedPlayer.industry += guiPlanScript.totalSystemIndustry;
			selectedPlayer.money += guiPlanScript.totalSystemMoney;
		}

		turnInfoScript.SortSystemPower();
		
		selectedPlayer.GP += selectedPlayer.raceGP;

		selectedPlayer.planetsColonisedThisTurn = 0;

		endTurn = false;
	}

	public void SortSystemPower()
	{
		int i, j; 
		string[] temp = new string[3];

		for(i = 210; i >= 0; --i)
		{
			if(mostPowerfulPlanets[i, 0] == "" || mostPowerfulPlanets[i,0] == null)
			{
				continue;
			}

			Debug.Log (mostPowerfulPlanets[i,0]);

			bool swaps = false;

			for(j = 1; j <= i; ++j)
			{
				if(float.Parse (mostPowerfulPlanets[j-1, 2]) < float.Parse (mostPowerfulPlanets[j, 2]))
				{
					temp[0] = mostPowerfulPlanets[j-1, 0].ToString ();
					temp[1] = mostPowerfulPlanets[j-1, 1].ToString();
					temp[2] = mostPowerfulPlanets[j-1, 2].ToString();

					mostPowerfulPlanets[j-1, 0] = mostPowerfulPlanets[j, 0];
					mostPowerfulPlanets[j-1, 1] = mostPowerfulPlanets[j, 1];
					mostPowerfulPlanets[j-1, 2] = mostPowerfulPlanets[j, 2];

					mostPowerfulPlanets[j, 0] = temp[0];
					mostPowerfulPlanets[j, 1] = temp[1];
					mostPowerfulPlanets[j, 2] = temp[2];

					swaps = true;
				}
			}

			if(swaps == false)
			{
				break;
			}
		}
	}
}
