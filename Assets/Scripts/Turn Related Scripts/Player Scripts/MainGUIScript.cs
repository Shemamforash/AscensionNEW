﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGUIScript : MasterScript 
{
	private Rect[] allPlanetsGUI, allButtonsGUI, allImprovementButtonsGUI, allHeroLabels, allHeroButtons; 
	public GUISkin mySkin;
	public bool spendMenu = false, hasColonised = false, openImprovementList = false;
	public string resourceToSpend;
	private string cost, indSpend, monSpend, turnNumber, scienceStr, industryStr, moneyStr, GPString, dataSIMString, techBuildButtonText, tempRace, heroName, diplomaticState;
	public int selectedSystem, selectedPlanet, selectedImprovemen;
	private float xLoc, yLoc;
	private Vector2 scrollPositionA = Vector2.zero, scrollPositionB = Vector2.zero;

	void Start()
	{
		GUIRectBuilder();
	}

	void Update()
	{
		if(playerTurnScript.tempObject != null)
		{
			guiPlanScript = playerTurnScript.tempObject.GetComponent<GUISystemDataScript>();
			techTreeScript = playerTurnScript.tempObject.GetComponent<TechTreeScript>();
			heroScript = playerTurnScript.tempObject.GetComponent<HeroScriptParent>();
		}
	}

	private void GUIRectBuilder() //Setting up rects for planet data
	{
		Rect farLeft = new Rect(Screen.width/2 - 725.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Top left
		Rect buttonFarLeft = new Rect(Screen.width/2 - 725.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		Rect improveButtonFarLeft = new Rect(Screen.width/2 - 725.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);
		
		Rect closeLeft = new Rect(Screen.width/2 - 475.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Top right		
		Rect buttonCloseLeft = new Rect(Screen.width/2 - 475.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		Rect improveButtonCloseLeft = new Rect(Screen.width/2 - 475.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);
		
		Rect middleLeft = new Rect (Screen.width/2 - 225.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Middle left		
		Rect buttonMiddleLeft = new Rect(Screen.width/2 - 225.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		Rect improveButtonMiddleLeft = new Rect(Screen.width/2 - 225.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);
		
		Rect middleRight = new Rect(Screen.width/2 + 25.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Middle right		
		Rect buttonMiddleRight = new Rect(Screen.width/2 + 25.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		Rect improveButtonMiddleRight = new Rect(Screen.width/2 + 25.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);

		Rect closeRight = new Rect(Screen.width/2 + 275.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Bottom left		
		Rect buttonCloseRight = new Rect(Screen.width/2 + 275.0f, Screen.height / 2 +50.0f, 200.0f, 50.0f);
		Rect improveButtonCloseRight = new Rect(Screen.width/2 + 275.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);
		
		Rect farRight = new Rect(Screen.width/2 + 525.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Bottom right		
		Rect buttonFarRight = new Rect(Screen.width/2 + 525.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		Rect improveButtonFarRight = new Rect(Screen.width/2 + 525.0f, Screen.height / 2 - 100.0f, 200.0f, 50.0f);

		Rect heroLabel1 = new Rect(Screen.width / 2 - 450.0f, Screen.height / 2 + 300.0f, 100.0f, 50.0f);//Hero labels
		Rect heroLabel2 = new Rect(Screen.width / 2 - 340.0f, Screen.height / 2 + 300.0f, 100.0f, 50.0f);
		Rect heroLabel3 = new Rect(Screen.width / 2 - 230.0f, Screen.height / 2 + 300.0f, 100.0f, 50.0f);
		
		Rect heroButton1 = new Rect(Screen.width / 2 - 450.0f, Screen.height / 2 + 360.0f, 100.0f, 25.0f);//Hero buttons
		Rect heroButton2 = new Rect(Screen.width / 2 - 340.0f, Screen.height / 2 + 360.0f, 100.0f, 25.0f);
		Rect heroButton3 = new Rect(Screen.width / 2 - 230.0f, Screen.height / 2 + 360.0f, 100.0f, 25.0f);


		allHeroLabels = new Rect[3]{heroLabel1, heroLabel2, heroLabel3};

		allHeroButtons = new Rect[3] {heroButton1, heroButton2, heroButton3};

		allPlanetsGUI = new Rect[6] {farLeft, closeLeft, middleLeft, middleRight, closeRight, farRight};
		
		allButtonsGUI = new Rect[6] {buttonFarLeft, buttonCloseLeft, buttonMiddleLeft, buttonMiddleRight, buttonCloseRight, buttonFarRight};

		allImprovementButtonsGUI = new Rect[6]{improveButtonFarLeft, improveButtonCloseLeft, improveButtonMiddleLeft, improveButtonMiddleRight, improveButtonCloseRight, improveButtonFarRight};
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

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			turnNumber = "Turn: " + turnInfoScript.turn.ToString();
			scienceStr = ((int)playerTurnScript.science).ToString();
			industryStr = ((int)playerTurnScript.industry).ToString ();
			moneyStr = ((int)playerTurnScript.money).ToString ();
			GPString = playerTurnScript.GP.ToString ();
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
			diplomaticState = playerTurnScript.diplomaticState;

			if(cameraFunctionsScript.openMenu == true)
			{
				dataSIMString = "Total SIM: " + guiPlanScript.totalSystemSIM.ToString() + "\n" + "Total Science: " + guiPlanScript.totalSystemScience.ToString() + "\n" 
					+ "Total Industry: " + guiPlanScript.totalSystemIndustry.ToString() + "\n" + "Total Money: " + guiPlanScript.totalSystemMoney.ToString(); 
			}
		}
	}

	private void CheckPlanetImprovement(int i)
	{
		guiPlanScript.improvementNumber = systemListConstructor.systemList[selectedSystem].planetImprovementLevel[i];
		
		guiPlanScript.CheckImprovement();
		
		if(guiPlanScript.canImprove == false)
		{
			cost = "Max Improvement";
		}
		
		if(guiPlanScript.canImprove == true)
		{
			cost = "Improve Cost: " + guiPlanScript.improvementCost;
		}
		
		if(systemListConstructor.systemList[selectedSystem].planetColonised[i] == false)
		{
			cost = "Colonise: 1 GP";
		}
	}

	void OnGUI() //Urgh. Unity demands that all GUI related script should come from one OnGUI to prevent excessive numbers of calls to the CPU
	{
		GUI.skin = mySkin;

		#region playerrace
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
		#endregion

		UpdateVariables ();

		GUI.Label (new Rect (Screen.width / 2, Screen.height / 2 - 400.0f, 100.0f, 50.0f), diplomaticState);

		#region turninfo				
		GUI.Label (new Rect(Screen.width - 80, Screen.height - 50, 50, 20), turnNumber);
		
		if(GUI.Button (new Rect(Screen.width - 80, Screen.height - 30, 70, 20), "End turn") && playerTurnScript.playerRace != null) //Endturnbutton
		{
			EndTurnFunction();
		}
		
		GUI.Box (new Rect(15, 15, 100, 130), ""); //Empire resources box
		
		GUI.Label (new Rect(20, 20, 90, 20), playerTurnScript.playerRace);
				
		GUI.Label (new Rect(20, 45, 90, 20), scienceStr);
				
		GUI.Label (new Rect(20, 70, 90, 20), industryStr);
				
		GUI.Label (new Rect(20, 95, 90, 20), moneyStr);

		GUI.Label (new Rect(20, 120, 90, 20), GPString);
		#endregion
		
		#region colonisebutton
		Rect coloniseButton = new Rect(10, Screen.height - 40, 75, 30); //Colonise button
		
		if(cameraFunctionsScript.coloniseMenu == true)
		{
			if(GUI.Button (coloniseButton, "Colonise") && playerTurnScript.GP > 0 && systemListConstructor.systemList[selectedSystem].systemOwnedBy == null)
			{			
				playerTurnScript.FindSystem (selectedSystem);
			}
		}
		#endregion
		
		#region planetinfomenu
		if(cameraFunctionsScript.openMenu == true)
		{
			#region planetdata			
			GUI.Box (new Rect(0.5f, 0.5f, Screen.width, Screen.height), "Planets in System");
			
			if(guiPlanScript.foundPlanetData == false)
			{
				guiPlanScript.SystemSIMCounter(selectedSystem, playerTurnScript);
				guiPlanScript.foundPlanetData = true;
			}
			#endregion

			#region settingupbutton			
			GUI.Label(new Rect (Screen.width/2 - 500.0f, Screen.height/2 - 350.0f, 100.0f, 100.0f), dataSIMString);
			
			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; i++) //This sections of the function evaluates the improvement level of each system, and improves it if the button is clicked
			{	
				CheckPlanetImprovement(i);

				if(GUI.Button(allButtonsGUI[i], cost) && systemListConstructor.systemList[selectedSystem].planetImprovementLevel[i] < 3)
				{	
					spendMenu = true;
					selectedPlanet = i;
					indSpend = guiPlanScript.improvementCost + " Industry";
					monSpend = guiPlanScript.improvementCost * 2 + " Money";
				}

				if(GUI.Button (allImprovementButtonsGUI[i], "Improvements"))
				{
					openImprovementList = true;
					xLoc = allImprovementButtonsGUI[i].xMax;
					yLoc = allImprovementButtonsGUI[i].yMax;
					selectedPlanet = i;
				}

				GUI.Label (allPlanetsGUI[i], guiPlanScript.allPlanetsInfo[i]);

				GUILayout.BeginArea(new Rect(allImprovementButtonsGUI[i].x, Screen.height / 2 + 100.0f, 200.0f, 400.0f));

				for(int j = 0; j < systemListConstructor.systemList[selectedSystem].improvementSlots[i]; ++j) //Display improvements on system
				{
					GUILayout.Label (techTreeScript.improvementsOnPlanet[i,j], GUILayout.Height(50.0f));
				}

				GUILayout.EndArea();
			}
			#endregion

			#region spendmenu
			if(spendMenu == true && cameraFunctionsScript.openMenu == true)
			{
				GUI.Box (new Rect(Screen.width/2 - 100.0f, Screen.height/2 - 50.0f, 200.0f, 75.0f), "Resource to Spend:");	

				if(systemListConstructor.systemList[selectedSystem].planetColonised[selectedPlanet] == true)
				{
					guiPlanScript.improvementNumber = systemListConstructor.systemList[selectedSystem].planetImprovementLevel[selectedPlanet];

					guiPlanScript.CheckImprovement();

					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 92.5f, 35.0f), indSpend) && playerTurnScript.industry >= guiPlanScript.improvementCost)
					{
						resourceToSpend = "Industry";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(selectedSystem, selectedPlanet);
					}

					if(GUI.Button (new Rect(Screen.width/2 + 2.5f, Screen.height/2 - 15.0f, 92.5f, 35.0f), monSpend) && playerTurnScript.money >= (guiPlanScript.improvementCost * 2))
					{
						resourceToSpend = "Money";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(selectedSystem, selectedPlanet);
					}
				}

				if(systemListConstructor.systemList[selectedSystem].planetColonised[selectedPlanet] == false)
				{
					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 190.0f, 35.0f), "1 GP") && playerTurnScript.GP > 0)
					{
						playerTurnScript.GP -= 1;
						systemListConstructor.systemList[selectedSystem].planetColonised[selectedPlanet] = true;
						++playerTurnScript.planetsColonisedThisTurn;
						hasColonised = true;
						spendMenu = false;
					}
				}

				if(GUI.Button(new Rect(Screen.width/2 + 77.5f, Screen.height/2 - 45.0f, 18.5f, 18.5f), "x"))
				{
					spendMenu = false;
				}

				guiPlanScript.SystemSIMCounter(selectedSystem, playerTurnScript);
			}
			#endregion
			
			#region techtreedata
			if(openImprovementList == true)
			{
				if(GUI.Button (new Rect(xLoc + 200.0f, yLoc, 20.0f, 20.0f), "X"))
				{
					openImprovementList = false;
				}

				GUILayout.BeginArea(new Rect(xLoc, yLoc, 200.0f, 400.0f));

				scrollPositionA = GUILayout.BeginScrollView(scrollPositionA);

				for(int i = 0; i <= techTreeScript.techTier; ++i)
				{
					for(int j = 0; j < 6; ++j)
					{
						if(techTreeScript.techTreeComplete[i,j,0] == null || techTreeScript.techTreeComplete[i,j,1] == "Built")
						{
							continue;
						}

						techBuildButtonText = techTreeScript.techTreeComplete[i,j,0] + "\n" + techTreeScript.techTreeCost[i,j].ToString();

						if(GUILayout.Button(techBuildButtonText, GUILayout.Height(40.0f)))
						{
							techTreeScript.ImproveSystem(i,j);

							if(techTreeScript.techTreeComplete[i,j,1] == "Built")
							{
								for(int k = 0; k < systemListConstructor.systemList[selectedSystem].improvementSlots[selectedPlanet]; k++)
								{									
									if(techTreeScript.improvementsOnPlanet[selectedPlanet, k] == null)
									{
										techTreeScript.improvementsOnPlanet[selectedPlanet, k] = techTreeScript.techTreeComplete[i,j,0];
										break;
									}
								}
							}
						}
					}
				}
				GUILayout.EndScrollView();

				GUILayout.EndArea ();
			}

			#region systembenefits
			
			GUILayout.BeginArea (new Rect(Screen.width / 2 + 750.0f, Screen.height / 2 - 200.0f, 200.0f, 400.0f));

			scrollPositionB = GUILayout.BeginScrollView(scrollPositionB);

			for(int i = 0; i < 23; ++i)
			{
				if(techTreeScript.improvementMessageArray[i] != null || techTreeScript.improvementMessageArray[i] != "")
				{
					GUILayout.Label(techTreeScript.improvementMessageArray[i], GUILayout.Height (40.0f));
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();

			#endregion

			if(GUI.Button (new Rect(Screen.width / 2 - 610.0f, Screen.height / 2 + 300.0f, 150.0f, 50.0f), "Purchase Hero: 1GP"))
			{
				heroGUIScript.CheckIfCanHire(selectedSystem);
			}

			for(int i = 0; i < 3; ++i)
			{
				if(systemListConstructor.systemList[selectedSystem].heroesInSystem[i] == null)
				{
					continue;
				}

				RefreshHeroInfo(i);
			
				tier3HeroScript.FillLinkableSystems();

				if(heroName == "Merchant" && tier3HeroScript.linkableSystemsExist == true)
				{
					if(GUI.Button (allHeroLabels[i], heroName))
					{
						tier3HeroScript.openSystemLinkScreen = true;
						heroGUIScript.selectedHero = i;
						tier3HeroScript.FillLinkableSystems();
					}
				}

				else
				{
					GUI.Label (allHeroLabels[i], heroName);
				}

				if(GUI.Button (allHeroButtons[i], "Level Up"))
				{
					heroScript =  systemListConstructor.systemList[selectedSystem].heroesInSystem[i].GetComponent<HeroScriptParent>();
					heroScript.LevelUp ();
				}
			}

			GUI.Label (new Rect(Screen.width/2 - 700.0f, Screen.height / 2 + 400.0f, 100.0f, 40.0f), diplomacyScript.playerStates[1]);

			#endregion
		}
		#endregion
	}

	public void RefreshHeroInfo(int hero)
	{
		heroName = systemListConstructor.systemList[selectedSystem].heroesInSystem[hero].name;
	}
}
