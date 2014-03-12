using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyGUI : MasterScript 
{
	public GameObject coloniseButton, planetSelectionWindow, purgeButton;
	private List<GameObject> planetSelectionList = new List<GameObject>();
	private int selectedSystem;
	private string tempRace, scienceString, industryString, capitalString, turnNumber;
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
			turnNumber = "Year: " + (2200 + (int)(turnInfoScript.turn / 2f)).ToString();
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

			if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == null)
			{
				NGUITools.SetActive(coloniseButton, true);
			}
		}
	}

	public void SelectRace(string thisRace)
	{
		playerTurnScript.playerRace = thisRace;
		playerTurnScript.StartTurn();
		turnInfoScript.CreateEnemyAI ();

		for(int i = 0; i < turnInfoScript.allPlayers.Count; ++i)
		{
			turnInfoScript.allPlayers[i].RaceStart(turnInfoScript.allPlayers[i].playerRace);
		}

		raceLabel.text = playerTurnScript.playerRace;

		if(playerTurnScript.playerRace == "Nereides")
		{
			NGUITools.SetActive(purgeButton, true);
		}

		turnInfoScript.StartGame ();
	}

	private void UpdateLabels()
	{
		scienceLabel.text = scienceString;
		industryLabel.text = industryString;
		capitalLabel.text = capitalString;
		turnLabel.text = turnNumber;

		string tempString = null;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == playerTurnScript.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == playerTurnScript.playerRace)
			{
				tempString = tempString + diplomacyScript.relationsList[i].diplomaticState + " | " + diplomacyScript.relationsList[i].stateCounter;

				if(i != diplomacyScript.relationsList.Count - 1)
				{
					tempString = tempString + "\n";
				}
			}
		}

		diplomacyLabelOne.text = tempString;
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
}
