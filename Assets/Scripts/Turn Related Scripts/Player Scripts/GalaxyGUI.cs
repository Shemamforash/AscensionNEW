using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyGUI : MasterScript 
{
	public GameObject coloniseButton, planetSelectionWindow;
	private List<GameObject> planetSelectionList = new List<GameObject>();
	private int selectedSystem;
	private string tempRace, scienceString, industryString, capitalString, turnNumber, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public UILabel scienceLabel, industryLabel, capitalLabel, raceLabel, turnLabel, diplomacyLabelOne, diplomacyLabelTwo;

	void Start()
	{
		foreach(Transform child in planetSelectionWindow.transform)
		{
			planetSelectionList.Add (child.gameObject);
		}
	}

	void Update()
	{
		UpdateVariables();
		UpdateLabels ();
	}

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			scienceString = ((int)playerTurnScript.science).ToString();
			industryString = ((int)playerTurnScript.industry).ToString ();
			capitalString = ((int)playerTurnScript.capital).ToString ();
			turnNumber = "Year: " + (2200 + (turnInfoScript.turn * 4)).ToString();
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

			if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == null)
			{
				NGUITools.SetActive(coloniseButton, true);
			}

			playerEnemyOneDiplomacy = diplomacyScript.playerEnemyOneRelations.diplomaticState + " | " + diplomacyScript.playerEnemyOneRelations.peaceCounter;
			playerEnemyTwoDiplomacy = diplomacyScript.playerEnemyTwoRelations.diplomaticState + " | " + diplomacyScript.playerEnemyTwoRelations.peaceCounter;
		}
	}

	private void SelectRace(string thisRace)
	{
		playerTurnScript.playerRace = thisRace;
		playerTurnScript.StartTurn();
		enemyOneTurnScript.SetRace();
		enemyTwoTurnScript.SetRace();
		raceLabel.text = playerTurnScript.playerRace;
	}

	private void UpdateLabels()
	{
		scienceLabel.text = scienceString;
		industryLabel.text = industryString;
		capitalLabel.text = capitalString;
		turnLabel.text = turnNumber;
		diplomacyLabelOne.text = playerEnemyOneDiplomacy;
		diplomacyLabelTwo.text = playerEnemyTwoDiplomacy;
	}

	public void EndTurnFunction()
	{
		turnInfoScript.turn++;
		playerTurnScript.TurnEnd (playerTurnScript);
		enemyOneTurnScript.Expand(enemyOneTurnScript);
		enemyTwoTurnScript.Expand(enemyTwoTurnScript);
	}

	public void CheckToColoniseSystem()
	{
		if(playerTurnScript.capital >= 10)
		{
			playerTurnScript.FindSystem (selectedSystem);
			SelectFirstPlanet();
			NGUITools.SetActive(coloniseButton, false);
		}
	}

	public void ColonisePlanet()
	{
		int planet = planetSelectionList.IndexOf (UIButton.current.gameObject);

		systemListConstructor.systemList[selectedSystem].planetsInSystem[planet].planetColonised = true;
		
		++playerTurnScript.planetsColonisedThisTurn;
		
		++playerTurnScript.systemsColonisedThisTurn;
		
		playerTurnScript.systemHasBeenColonised = false;

		NGUITools.SetActive (planetSelectionWindow, false);
	}

	private void SelectFirstPlanet()
	{
		NGUITools.SetActive (planetSelectionWindow, true);

		for(int i = 0; i < planetSelectionList.Count; ++i)
		{
			if(i < systemListConstructor.systemList[selectedSystem].systemSize)
			{
				float planetSIM = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetScience + systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetIndustry;
				
				string planetInfo = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType + " " + planetSIM.ToString() + " SIM";

				planetSelectionList[i].transform.Find ("Label").gameObject.GetComponent<UILabel>().text = planetInfo;

				NGUITools.SetActive(planetSelectionList[i], true);
			}

			if(i >= systemListConstructor.systemList[selectedSystem].systemSize)
			{
				NGUITools.SetActive(planetSelectionList[i], false);
			}
		}

		planetSelectionWindow.GetComponent<UIScrollView> ().ResetPosition ();
		planetSelectionWindow.GetComponent<UIGrid> ().repositionNow = true;
	}

	void OnGUI()
	{
		GUI.skin = systemGUI.mySkin;

		if(playerTurnScript.playerRace == null)
		{
			GUI.Box (new Rect(Screen.width/2 - 150, Screen.height/2 - 40, 300, 80), "\nSelect Race");
			
			if(GUI.Button (new Rect(Screen.width/2 - 140, Screen.height/2, 90, 20), "Humans"))
			{
				tempRace = "Humans";
			}
			
			if(GUI.Button (new Rect(Screen.width/2 -45, Screen.height/2, 90, 20), "Selkies"))
			{
				tempRace = "Selkies";
			}
			
			if(GUI.Button (new Rect(Screen.width/2 + 50, Screen.height/2, 90, 20), "Nereides"))
			{
				tempRace = "Nereides";
			}
			
			if(tempRace != null)
			{
				SelectRace(tempRace);
				turnInfoScript.RefreshPlanetPower();
			}
		}
	}
}
