using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TurnInfo : MasterScript
{
	[HideInInspector]
	public int planetsColonisedThisTurn, systemsColonisedThisTurn, savedIterator, researchCostModifier = 0, antimatter, blueCarbon, radioisotopes, liquidH2;
	public float raceKnowledge, racePower, knowledge, power, wealth, raceWealth, turn = 0, wealthModifier;
	[HideInInspector]
	public string[,] planetRIM = new string[12,5];
	public List<PlanetWealth> mostWealthfulPlanets = new List<PlanetWealth>();
	[HideInInspector]
	public bool playerHasWon, startSteps, isPlayer;
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

	public void RefreshPlanetWealth()
	{
		mostWealthfulPlanets.Clear ();

		turnInfoScript.savedIterator = 0;

		for(int i = 0; i < systemListConstructor.mapSize; i++)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null)
			{
				continue;
			}

			systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			
			systemSIMData.UpdatePlanetWealthArray();
		}

		SortSystemWealth ();
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
			raceKnowledge = 1 * gameSpeedModifer;
			racePower = 1 * gameSpeedModifer;
			raceWealth = 10.6f;
			homeSystem = "Midgard";
			homePlanetType = "Ocean";
			materialInUse = turnInfoScript.humansMaterial;
		}
		if(playerRace == "Selkies")
		{
			raceKnowledge = 1 * gameSpeedModifer;
			racePower = 1.4f * gameSpeedModifer;
			raceWealth = 10.5f;
			homeSystem = "Samael";
			homePlanetType = "Prairie";
			materialInUse = turnInfoScript.selkiesMaterial;
		}
		if(playerRace == "Nereides")
		{
			raceKnowledge = 2 * gameSpeedModifer;
			racePower = 1 * gameSpeedModifer;
			raceWealth = 0.4f;
			homeSystem = "Nepthys";
			homePlanetType = "Boreal";
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

		diplomacyScript.DiplomaticStateEffects ();

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
			systemSIMData.SystemSIMCounter(selectedPlayer);
			systemFunctions.CheckUnlockedTier(improvementsBasic, i);

			selectedPlayer.knowledge += systemSIMData.totalSystemKnowledge;
			selectedPlayer.power += systemSIMData.totalSystemPower;

			if(selectedPlayer.playerRace == "Selkies")
			{
				racialTraitScript.amber += systemSIMData.totalSystemAmber;
			}

			selectedPlayer.researchCostModifier += improvementsBasic.researchCost;
		}

		for(int j = 0; j < selectedPlayer.playerOwnedHeroes.Count; ++j)
		{				
			heroScript = selectedPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();
			heroScript.HeroEndTurnFunctions(selectedPlayer);
		}
		
		racialTraitScript.RacialBonus (selectedPlayer);

		turnInfoScript.SortSystemWealth();
		
		selectedPlayer.wealth += (selectedPlayer.wealthModifier + 1f) * selectedPlayer.raceWealth;

		if(selectedPlayer.wealth > 100.0f)
		{
			selectedPlayer.wealth = 100.0f;
		}

		selectedPlayer.planetsColonisedThisTurn = 0;

		selectedPlayer.systemsColonisedThisTurn = 0;
	}

	public void SortSystemWealth()
	{
		GameObject tempObject;
		float tempFloat;
		int tempInt;

		for(int i = turnInfoScript.mostWealthfulPlanets.Count - 1; i >= 0; --i)
		{
			bool swaps = false;

			for(int j = 1; j <= i; ++j)
			{
				if(mostWealthfulPlanets[j-1].simOutput < mostWealthfulPlanets[j].simOutput)
				{
					tempObject = mostWealthfulPlanets[j-1].system;
					tempFloat = mostWealthfulPlanets[j-1].simOutput;
					tempInt = mostWealthfulPlanets[j-1].planetPosition;

					mostWealthfulPlanets[j-1].system = mostWealthfulPlanets[j].system;
					mostWealthfulPlanets[j-1].simOutput = mostWealthfulPlanets[j].simOutput;
					mostWealthfulPlanets[j-1].planetPosition = mostWealthfulPlanets[j].planetPosition;

					mostWealthfulPlanets[j].system = tempObject;
					mostWealthfulPlanets[j].simOutput = tempFloat;
					mostWealthfulPlanets[j].planetPosition = tempInt;

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

public class PlanetWealth
{
	public GameObject system;
	public float simOutput;
	public int planetPosition;
}