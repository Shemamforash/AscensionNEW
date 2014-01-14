﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TurnInfo : MasterScript
{
	[HideInInspector]
	public int GP, raceGP, planetsColonisedThisTurn, savedIterator;
	public float raceScience, raceIndustry, raceMoney, science, industry, money;
	[HideInInspector]
	public string[,] planetRIM = new string[12,5];
	public List<PlanetPower> mostPowerfulPlanets = new List<PlanetPower>();
	[HideInInspector]
	public bool endTurn, playerHasWon;
	public Camera mainCamera;
	public Material nereidesMaterial, humansMaterial, selkiesMaterial, materialInUse;

	public string playerRace, homeSystem, homePlanetType, playerHasWonRace;
	public int turn = 0, systemsInPlay = 0;

	public void RefreshPlanetPower()
	{
		mostPowerfulPlanets.Clear ();

		turnInfoScript.savedIterator = 0;

		for(int i = 0; i < 60; i++)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null)
			{
				continue;
			}

			guiPlanScript = systemListConstructor.systemList[i].systemObject.GetComponent<GUISystemDataScript>();
			
			guiPlanScript.UpdatePlanetPowerArray(i);
		}

		SortSystemPower ();
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
			if(systemListConstructor.systemList[i].systemOwnedBy == selectedPlayer.playerRace || systemListConstructor.systemList[i].systemOwnedBy == null)
			{
				playerHasWon = true;
				playerHasWonRace = selectedPlayer.playerRace;
			}

			playerHasWon = false;

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
			guiPlanScript.IncreaseOwnership();

			selectedPlayer.science += guiPlanScript.totalSystemScience;
			selectedPlayer.industry += guiPlanScript.totalSystemIndustry;
			selectedPlayer.money += guiPlanScript.totalSystemMoney;
		}

		SelectedPlayerDiplomacyChangeCheck (selectedPlayer);

		turnInfoScript.SortSystemPower();
		
		selectedPlayer.GP += selectedPlayer.raceGP;

		selectedPlayer.planetsColonisedThisTurn = 0;

		endTurn = false;
	}

	private void SelectedPlayerDiplomacyChangeCheck(TurnInfo selectedPlayer)
	{
		if(selectedPlayer == playerTurnScript || selectedPlayer == enemyOneTurnScript)
		{
			diplomacyScript.CheckForDiplomaticStateChange (diplomacyScript.playerEnemyOneRelations);
		}
		if(selectedPlayer == playerTurnScript || selectedPlayer == enemyTwoTurnScript)
		{
			diplomacyScript.CheckForDiplomaticStateChange (diplomacyScript.playerEnemyTwoRelations);
		}
		if(selectedPlayer == enemyOneTurnScript || selectedPlayer == enemyTwoTurnScript)
		{
			diplomacyScript.CheckForDiplomaticStateChange (diplomacyScript.enemyOneEnemyTwoRelations);
		}
	}

	public void SortSystemPower()
	{
		GameObject tempObject;
		float tempFloat;
		int tempInt;

		for(int i = turnInfoScript.mostPowerfulPlanets.Count - 1; i >= 0; --i)
		{
			bool swaps = false;

			for(int j = 1; j <= i; ++j)
			{
				if(mostPowerfulPlanets[j-1].simOutput < mostPowerfulPlanets[j].simOutput)
				{
					tempObject = mostPowerfulPlanets[j-1].system;
					tempFloat = mostPowerfulPlanets[j-1].simOutput;
					tempInt = mostPowerfulPlanets[j-1].planetPosition;

					mostPowerfulPlanets[j-1].system = mostPowerfulPlanets[j].system;
					mostPowerfulPlanets[j-1].simOutput = mostPowerfulPlanets[j].simOutput;
					mostPowerfulPlanets[j-1].planetPosition = mostPowerfulPlanets[j].planetPosition;

					mostPowerfulPlanets[j].system = tempObject;
					mostPowerfulPlanets[j].simOutput = tempFloat;
					mostPowerfulPlanets[j].planetPosition = tempInt;

					swaps = true;
				}
			}

			if(swaps == false)
			{
				break;
			}
		}
	}

	void OnGUI()
	{
		GUI.skin = mainGUIScript.mySkin;

		if(playerHasWon == true)
		{
			GUI.Label (new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 100.0f, 200.0f, 200.0f), playerHasWonRace);
			Time.timeScale = 0;
		}
	}
}

public class PlanetPower
{
	public GameObject system;
	public float simOutput;
	public int planetPosition;
}