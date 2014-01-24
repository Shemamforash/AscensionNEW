using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGUIScript : MasterScript 
{
	private Rect[] allPlanetsGUI, allButtonsGUI, allImprovementButtonsGUI, allHeroLabels, allHeroButtons; 
	public GUISkin mySkin;
	public bool spendMenu = false, openImprovementList = false, systemOwnedByPlayer;
	public string resourceToSpend;
	private string cost, indSpend, monSpend, turnNumber, scienceStr, industryStr, moneyStr, GPString, dataSIMString, techBuildButtonText, tempRace, heroName, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public int selectedSystem, selectedPlanet;
	private float xLoc, yLoc;
	private Vector2 scrollPositionA = Vector2.zero, scrollPositionB = Vector2.zero;
	public Texture2D industryTexture, scienceTexture;

	void Start()
	{
		GUIRectBuilder();
	}

	void Update()
	{
		if(playerTurnScript.tempObject != null)
		{
			int system = RefreshCurrentSystem(playerTurnScript.tempObject);

			if(systemListConstructor.systemList[system].systemOwnedBy == playerTurnScript.playerRace)
			{
				systemSIMData = playerTurnScript.tempObject.GetComponent<SystemSIMData>();
				techTreeScript = playerTurnScript.tempObject.GetComponent<TechTreeScript>();
				heroScript = playerTurnScript.tempObject.GetComponent<HeroScriptParent>();
			}
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
			playerEnemyOneDiplomacy = diplomacyScript.playerEnemyOneRelations.diplomaticState + " | " + diplomacyScript.playerEnemyOneRelations.peaceCounter;
			playerEnemyTwoDiplomacy = diplomacyScript.playerEnemyTwoRelations.diplomaticState + " | " + diplomacyScript.playerEnemyTwoRelations.peaceCounter;
			systemSIMData = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<SystemSIMData>();

			if(selectedPlanet != -1)
			{
				systemSIMData.CheckPlanetValues(selectedSystem, selectedPlanet, playerTurnScript);
			}

			systemOwnedByPlayer = false;

			if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == playerTurnScript.playerRace)
			{
				systemOwnedByPlayer = true;
			}

			if(cameraFunctionsScript.openMenu == true)
			{
				dataSIMString = "Total SIM: " + systemSIMData.totalSystemSIM.ToString() + "\n" + "Total Science: " + systemSIMData.totalSystemScience.ToString() + "\n" 
					+ "Total Industry: " + systemSIMData.totalSystemIndustry.ToString() + "\n" + "Total Money: " + systemSIMData.totalSystemMoney.ToString(); 
			}
		}
	}

	private void CheckPlanetImprovement(int i)
	{
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		
		systemSIMData.CheckImprovement(selectedSystem, i);
		
		if(systemSIMData.canImprove == false)
		{
			cost = "Max Improvement";
		}
		
		if(systemSIMData.canImprove == true)
		{
			cost = "Improve Cost: " + systemSIMData.improvementCost;
		}
		
		if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == false)
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

		#region turninfo				
		
		GUI.Label (new Rect(0.0f, Screen.height - 30.0f, Screen.width, 30.0f), ""); //Empire resources box
		
		GUI.Label (new Rect(10.0f, Screen.height - 25.0f, 90.0f, 20.0f), playerTurnScript.playerRace);
				
		GUI.Label (new Rect(110.0f, Screen.height - 25.0f, 50.0f, 20.0f), scienceStr);

		GUI.Label (new Rect(155.0f, Screen.height - 30.0f, 30.0f, 30.0f), scienceTexture);
				
		GUI.Label (new Rect(190.0f, Screen.height - 25.0f, 50.0f, 20.0f), industryStr);

		GUI.Label (new Rect(235.0f, Screen.height - 30.0f, 30.0f, 30.0f), industryTexture);
				
		GUI.Label (new Rect(270.0f, Screen.height - 25.0f, 50.0f, 20.0f), "M " + moneyStr);

		GUI.Label (new Rect(330.0f, Screen.height - 25.0f, 50.0f, 20.0f), "GP " + GPString);

		GUI.Label (new Rect(Screen.width - 160.0f, Screen.height - 22.5f, 70, 20), turnNumber);

		if(GUI.Button (new Rect(Screen.width - 80.0f, Screen.height - 22.5f, 70, 20), "End turn") && playerTurnScript.playerRace != null) //Endturnbutton
		{
			EndTurnFunction();
		}

		#endregion
		
		#region colonisebutton
		Rect coloniseButton = new Rect(10, Screen.height - 40, 75, 30); //Colonise button
		
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

		GUI.Label (new Rect (10, Screen.height - 120.0f, 160.0f, 30.0f), playerEnemyOneDiplomacy);
		GUI.Label (new Rect (10, Screen.height - 80.0f, 160.0f, 30.0f), playerEnemyTwoDiplomacy);
		
		#region planetinfomenu
		if(cameraFunctionsScript.openMenu == true)
		{
			#region planetdata			
			GUI.Box (new Rect(0.5f, 0.5f, Screen.width, Screen.height), "Planets in System");
			
			if(systemSIMData.foundPlanetData == false)
			{
				systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
				systemSIMData.foundPlanetData = true;
			}
			#endregion

			#region settingupbutton			
			GUI.Label(new Rect (Screen.width/2 - 500.0f, Screen.height/2 - 350.0f, 100.0f, 100.0f), dataSIMString);

			int tempInt = 0;

			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; i++) //This sections of the function evaluates the improvement level of each system, and improves it if the button is clicked
			{	
				CheckPlanetImprovement(i);

				if(systemOwnedByPlayer == true)
				{
					if(GUI.Button(allButtonsGUI[i], cost) && systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel < 3)
					{	
						if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true)
						{
							spendMenu = true;
							selectedPlanet = i;
							indSpend = systemSIMData.improvementCost + " Industry";
							monSpend = systemSIMData.improvementCost * 2 + " Money";
						}

						if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == false && playerTurnScript.GP > 0)
						{
							playerTurnScript.GP -= 1;
							systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised = true;
							++playerTurnScript.planetsColonisedThisTurn;
							spendMenu = false;
							systemSIMData.CheckPlanetValues(selectedSystem, selectedPlanet, playerTurnScript);
						}
					}

					if(GUI.Button (allImprovementButtonsGUI[i], "Improvements"))
					{
						openImprovementList = true;
						xLoc = allImprovementButtonsGUI[i].xMax;
						yLoc = allImprovementButtonsGUI[i].yMax;
						selectedPlanet = i;
					}
				}

				GUI.Label (allPlanetsGUI[i], systemSIMData.allPlanetsInfo[i]);

				GUILayout.BeginArea(new Rect(allImprovementButtonsGUI[i].x, Screen.height / 2 + 100.0f, 200.0f, 400.0f));

				for(int j = tempInt; j < systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementSlots; ++j) //Display improvements on system
				{
					GUILayout.Label (systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementsBuilt[j], GUILayout.Height(50.0f));
				}

				GUILayout.EndArea();
			}
			#endregion

			#region spendmenu
			if(spendMenu == true && cameraFunctionsScript.openMenu == true)
			{
				GUI.Box (new Rect(Screen.width/2 - 100.0f, Screen.height/2 - 50.0f, 200.0f, 75.0f), "Resource to Spend:");	

				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
				{
					systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;

					systemSIMData.CheckImprovement(selectedSystem, selectedPlanet);

					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 92.5f, 35.0f), indSpend) && playerTurnScript.industry >= systemSIMData.improvementCost)
					{
						resourceToSpend = "Industry";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(selectedSystem, selectedPlanet);
					}

					if(GUI.Button (new Rect(Screen.width/2 + 2.5f, Screen.height/2 - 15.0f, 92.5f, 35.0f), monSpend) && playerTurnScript.money >= (systemSIMData.improvementCost * 2))
					{
						resourceToSpend = "Money";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(selectedSystem, selectedPlanet);
					}
				}

				if(GUI.Button(new Rect(Screen.width/2 + 77.5f, Screen.height/2 - 45.0f, 18.5f, 18.5f), "x"))
				{
					spendMenu = false;
					selectedPlanet = -1;
				}

				systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
			}
			#endregion
			
			#region techtreedata
			if(openImprovementList == true)
			{
				if(GUI.Button (new Rect(xLoc + 200.0f, yLoc, 20.0f, 20.0f), "X"))
				{
					openImprovementList = false;
					selectedPlanet = -1;
				}

				GUILayout.BeginArea(new Rect(xLoc, yLoc, 200.0f, 400.0f));

				scrollPositionA = GUILayout.BeginScrollView(scrollPositionA);

				for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
				{
					if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true || techTreeScript.listOfImprovements[i].improvementLevel > techTreeScript.techTier 
					   || techTreeScript.listOfImprovements[i].improvementCategory == enemyOneTurnScript.playerRace 
					   || techTreeScript.listOfImprovements[i].improvementCategory == enemyTwoTurnScript.playerRace)
					{
						continue;
					}

					techBuildButtonText = techTreeScript.listOfImprovements[i].improvementName + "\n" + techTreeScript.listOfImprovements[i].improvementCost;

					if(GUILayout.Button(techBuildButtonText, GUILayout.Height(40.0f)))
					{
						for(int j = 0; j < systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
						{
							if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
							{
								if(techTreeScript.ImproveSystem(i) == true)
								{
									systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = techTreeScript.listOfImprovements[i].improvementName;
								}
								break;
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

			for(int i = 0; i < techTreeScript.improvementMessageArray.Count; ++i)
			{
				GUILayout.Label(techTreeScript.improvementMessageArray[i], GUILayout.Height (40.0f));
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();

			#endregion

			if(systemOwnedByPlayer == true)
			{
				if(GUI.Button (new Rect(Screen.width / 2 - 610.0f, Screen.height / 2 + 300.0f, 150.0f, 50.0f), "Purchase Hero: 1GP"))
				{
					heroGUIScript.CheckIfCanHire(selectedSystem);
				}
			}

			for(int i = 0; i < 3; ++i)
			{
				if(systemListConstructor.systemList[selectedSystem].heroesInSystem[i] == null)
				{
					continue;
				}

				RefreshHeroInfo(i);
			
				tier3HeroScript.FillLinkableSystems();

				if(heroName == "Merchant" && tier3HeroScript.linkableSystemsExist == true && systemOwnedByPlayer == true)
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

				if(systemOwnedByPlayer == true)
				{
					if(GUI.Button (allHeroButtons[i], "Level Up"))
					{
						heroScript =  systemListConstructor.systemList[selectedSystem].heroesInSystem[i].GetComponent<HeroScriptParent>();
						heroScript.LevelUp ();
					}
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
