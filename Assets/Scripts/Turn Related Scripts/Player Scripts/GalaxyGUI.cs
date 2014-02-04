using UnityEngine;
using System.Collections;

public class GalaxyGUI : MasterScript 
{
	private int selectedSystem;
	private string tempRace, scienceString, industryString, capitalString, turnNumber, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public UILabel scienceLabel, industryLabel, capitalLabel, raceLabel, turnLabel;

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			scienceString = ((int)playerTurnScript.science).ToString();
			industryString = ((int)playerTurnScript.industry).ToString ();
			capitalString = ((int)playerTurnScript.capital).ToString ();
			turnNumber = "Year: " + (2200 + (turnInfoScript.turn * 4)).ToString();
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
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
	}

	public void EndTurnFunction()
	{
		turnInfoScript.turn++;
		playerTurnScript.TurnEnd (playerTurnScript);
		enemyOneTurnScript.Expand(enemyOneTurnScript);
		enemyTwoTurnScript.Expand(enemyTwoTurnScript);
	}

	void OnGUI()
	{
		GUI.skin = systemGUI.mySkin;

		UpdateVariables ();

		GUI.Label (new Rect (10, Screen.height - 120.0f, 160.0f, 30.0f), playerEnemyOneDiplomacy);
		GUI.Label (new Rect (10, Screen.height - 80.0f, 160.0f, 30.0f), playerEnemyTwoDiplomacy);

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
				
		raceLabel.text = playerTurnScript.playerRace;

		scienceLabel.text = scienceString;

		industryLabel.text = industryString;

		capitalLabel.text = capitalString;
		
		turnLabel.text = turnNumber;

		#region colonisebutton
		Rect coloniseButton = new Rect(Screen.width - 85, Screen.height - 70, 75, 30); //Colonise button
		
		if(cameraFunctionsScript.coloniseMenu == true)
		{
			bool isConnected = false;
			
			lineRenderScript = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<LineRenderScript>();
			
			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].numberOfConnections; ++i)
			{
				int j = RefreshCurrentSystem(systemListConstructor.systemList[selectedSystem].permanentConnections[i]);
				
				if(systemListConstructor.systemList[j].systemOwnedBy == playerTurnScript.playerRace)
				{
					isConnected = true;
					break;
				}
			}
			
			if(isConnected == true)
			{
				if(GUI.Button (coloniseButton, "Colonise") && playerTurnScript.capital >= 10)
				{	
					playerTurnScript.FindSystem (selectedSystem);
				}
			}
		}
		#endregion

		if(playerTurnScript.systemHasBeenColonised == true)
		{
			int selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 - 185.0f, Screen.height / 2 - 60.0f, 120.0f, 370.0f));
			
			GUILayout.Box("Select Planet");
			
			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
			{
				float planetSIM = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetScience + systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetIndustry;
				
				string planetInfo = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType + " " + planetSIM.ToString() + " SIM";
				
				if(GUILayout.Button(planetInfo, GUILayout.Height (50.0f)))
				{
					systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised = true;
					
					++playerTurnScript.planetsColonisedThisTurn;
					
					++playerTurnScript.systemsColonisedThisTurn;
					
					playerTurnScript.systemHasBeenColonised = false;
				}
			}
			
			GUILayout.EndArea();
		}
	}
}
