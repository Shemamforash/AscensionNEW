﻿using UnityEngine;
using System.Collections;

public class GalaxyGUI : MasterScript 
{
	private int selectedSystem;
	public Texture2D industryTexture, scienceTexture, moneyTexture;
	private string tempRace, scienceString, industryString, moneyString, gpString, turnNumber, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public GameObject mouseOverSystem;

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			scienceString = ((int)playerTurnScript.science).ToString();
			industryString = ((int)playerTurnScript.industry).ToString ();
			moneyString = ((int)playerTurnScript.money).ToString ();
			gpString = playerTurnScript.GP.ToString ();
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

	private void EndTurnFunction()
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

		if (mouseOverSystem != null) 
		{
			systemSIMData = mouseOverSystem.GetComponent<SystemSIMData>();

			int colonisedPlanets = 0;

			int system = RefreshCurrentSystem(mouseOverSystem);

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
				{
					++colonisedPlanets;
				}
			}

			string systemSummaryString = "Colonised: " + colonisedPlanets + "/" + systemListConstructor.systemList[system].systemSize;

			GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - (Input.mousePosition.y + 60), 180.0f, 60.0f));

			GUILayout.BeginHorizontal();

			GUILayout.Label (mouseOverSystem.name, GUILayout.Width(80.0f), GUILayout.Height(30.0f));
			
			GUILayout.Label (systemSummaryString, GUILayout.Width(100.0f), GUILayout.Height(30.0f));

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			GUILayout.Label (scienceTexture, GUILayout.Width(30.0f), GUILayout.Height(30.0f));

			GUILayout.Label (((int)systemSIMData.totalSystemScience).ToString(), GUILayout.Width (30.0f), GUILayout.Height(30.0f));

			GUILayout.Label (industryTexture, GUILayout.Width (30.0f), GUILayout.Height(30.0f));
			
			GUILayout.Label (((int)systemSIMData.totalSystemIndustry).ToString(), GUILayout.Width (30.0f), GUILayout.Height(30.0f));

			GUILayout.Label (moneyTexture, GUILayout.Width (30.0f), GUILayout.Height(30.0f));
			
			GUILayout.Label (((int)systemSIMData.totalSystemMoney).ToString(), GUILayout.Width (30.0f), GUILayout.Height(30.0f));

			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}

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

		GUI.Label (new Rect(0.0f, Screen.height - 30.0f, Screen.width, 30.0f), ""); //Empire resources box
		
		GUI.Label (new Rect(10.0f, Screen.height - 25.0f, 90.0f, 20.0f), playerTurnScript.playerRace);
		
		GUI.Label (new Rect(110.0f, Screen.height - 25.0f, 50.0f, 20.0f), scienceString);
		
		GUI.Label (new Rect(155.0f, Screen.height - 30.0f, 30.0f, 30.0f), scienceTexture);
		
		GUI.Label (new Rect(190.0f, Screen.height - 25.0f, 50.0f, 20.0f), industryString);
		
		GUI.Label (new Rect(245.0f, Screen.height - 30.0f, 30.0f, 30.0f), industryTexture);
		
		GUI.Label (new Rect(280.0f, Screen.height - 25.0f, 50.0f, 20.0f), moneyString);

		GUI.Label (new Rect(335.0f, Screen.height - 30.0f, 30.0f, 30.0f), moneyTexture);
		
		GUI.Label (new Rect(370.0f, Screen.height - 25.0f, 50.0f, 20.0f), "GP " + gpString);
		
		GUI.Label (new Rect(Screen.width - 160.0f, Screen.height - 25.0f, 70, 20), turnNumber);
		
		if(GUI.Button (new Rect(Screen.width - 80.0f, Screen.height - 25.0f, 70, 20), "End turn") && playerTurnScript.playerRace != null) //Endturnbutton
		{
			EndTurnFunction();
		}

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
				if(GUI.Button (coloniseButton, "Colonise") && playerTurnScript.GP > 0)
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
				float planetSIM = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetScience + systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetIndustry +
					systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetMoney;
				
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
