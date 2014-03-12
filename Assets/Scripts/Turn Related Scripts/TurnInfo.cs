using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TurnInfo : MasterScript
{
	[HideInInspector]
	public int planetsColonisedThisTurn, systemsColonisedThisTurn, savedIterator, researchCostModifier = 0;
	public float raceScience, raceIndustry, science, industry, capital, raceCapital, turn = 0, capitalModifier;
	[HideInInspector]
	public string[,] planetRIM = new string[12,5];
	public List<PlanetPower> mostPowerfulPlanets = new List<PlanetPower>();
	[HideInInspector]
	public bool playerHasWon, startSteps;
	public Camera mainCamera;
	public Material nereidesMaterial, humansMaterial, selkiesMaterial, materialInUse, emptyMaterial;
	public string playerRace, homePlanetType, playerHasWonRace, homeSystem;
	public int systemsInPlay = 0;
	public List<GameObject> playerOwnedHeroes = new List<GameObject> ();
	public List<EnemyOne> allPlayers = new List<EnemyOne>();

	public void StartGame()
	{
		if(turnInfoScript.startSteps == false && playerTurnScript.playerRace != null)
		{
			InvokeRepeating("EndTurnFunction", 0.0001f, 0.5f);
			turnInfoScript.startSteps = true;
		}
	}

	public void RefreshPlanetPower()
	{
		mostPowerfulPlanets.Clear ();

		turnInfoScript.savedIterator = 0;

		for(int i = 0; i < systemListConstructor.mapSize; i++)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null)
			{
				continue;
			}

			systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			
			systemSIMData.UpdatePlanetPowerArray(i);
		}

		SortSystemPower ();
	}

	public void CreateEnemyAI()
	{
		List<string> enemyRaces = new List<string> ();

		enemyRaces.Add (PlayerPrefs.GetString ("AI One"));

		if(PlayerPrefs.GetString ("AI Two") != "None")
		{
			enemyRaces.Add (PlayerPrefs.GetString("AI Two"));
		}

		for(int i = 0; i < enemyRaces.Count; ++i)
		{
			EnemyOne enemy = gameObject.AddComponent("EnemyOne") as EnemyOne;

			enemy.playerRace = enemyRaces[i];

			allPlayers.Add (enemy);
		}
	}

	public void PickRace() //Start of turn function. Race choice dictates starting planet and inherent bonuses as well as racial technologies.
	{
		float gameSpeedModifer = 60 / systemListConstructor.mapSize;

		if(playerRace == "Humans")
		{
			raceScience = 1 * gameSpeedModifer;
			raceIndustry = 1 * gameSpeedModifer;
			raceCapital = 0.6f;
			homeSystem = "Midgard";
			homePlanetType = "Ocean";
			materialInUse = turnInfoScript.humansMaterial;
		}
		if(playerRace == "Selkies")
		{
			raceScience = 1 * gameSpeedModifer;
			raceIndustry = 1.4f * gameSpeedModifer;
			raceCapital = 10.5f;
			homeSystem = "Samael";
			homePlanetType = "Plains";
			materialInUse = turnInfoScript.selkiesMaterial;
		}
		if(playerRace == "Nereides")
		{
			raceScience = 2 * gameSpeedModifer;
			raceIndustry = 1 * gameSpeedModifer;
			raceCapital = 0.4f;
			homeSystem = "Nepthys";
			homePlanetType = "Icy";
			materialInUse = turnInfoScript.nereidesMaterial;
		}
	}

	public void EndTurnFunction()
	{
		if(playerTurnScript.playerRace != null)
		{
			diplomacyScript.DiplomaticStateEffects();
			turnInfoScript.turn += 1.0f;
			turnInfoScript.TurnEnd (playerTurnScript);
			winConditions.CheckWin(playerTurnScript);

			for(int i = 0; i < allPlayers.Count; ++i)
			{
				allPlayers[i].Expand(allPlayers[i]);
				winConditions.CheckWin(allPlayers[i]);
			}
		}
	}

	public void TurnEnd(TurnInfo selectedPlayer) //This function accumulates all the SIM generated by each system to give an empire SIM value
	{		
		selectedPlayer.researchCostModifier = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy != selectedPlayer.playerRace)
			{
				continue;
			}

			systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			improvementsBasic = systemListConstructor.systemList[i].systemObject.GetComponent<ImprovementsBasic>();
			systemDefence = systemListConstructor.systemList[i].systemObject.GetComponent<SystemDefence>();

			systemDefence.CalculateSystemDefence();

			improvementsBasic.ActiveTechnologies(i, selectedPlayer);
			systemSIMData.SystemSIMCounter(i, selectedPlayer);
			systemFunctions.CheckUnlockedTier(improvementsBasic, i);
			systemSIMData.IncreaseOwnership();

			selectedPlayer.science += systemSIMData.totalSystemScience;
			selectedPlayer.industry += systemSIMData.totalSystemIndustry;

			if(selectedPlayer.playerRace == "Selkies")
			{
				racialTraitScript.amber += systemSIMData.totalSystemAmber;
			}

			selectedPlayer.researchCostModifier += improvementsBasic.researchCost;
		}

		for(int j = 0; j < selectedPlayer.playerOwnedHeroes.Count; ++j)
		{				
			heroScript = selectedPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();
			heroScript.HeroEndTurnFunctions();
		}
		
		racialTraitScript.RacialBonus (selectedPlayer);

		turnInfoScript.SortSystemPower();
		
		selectedPlayer.capital += (selectedPlayer.capitalModifier + 1f) * selectedPlayer.raceCapital;

		if(selectedPlayer.capital > 100.0f)
		{
			selectedPlayer.capital = 100.0f;
		}

		selectedPlayer.planetsColonisedThisTurn = 0;

		selectedPlayer.systemsColonisedThisTurn = 0;
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
}

public class PlanetPower
{
	public GameObject system;
	public float simOutput;
	public int planetPosition;
}