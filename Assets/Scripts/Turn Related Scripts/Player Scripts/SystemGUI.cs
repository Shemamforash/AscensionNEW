using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemGUI : MasterScript 
{
	private Rect[] allPlanetsGUI, allButtonsGUI, allImprovementButtonsGUI, allHeroLabels, allHeroButtons; 
	public GUISkin mySkin;
	public bool spendMenu = false, openImprovementList = false, systemOwnedByPlayer;
	public string resourceToSpend;
	private string cost, indSpend, monSpend, dataSIMString, techBuildButtonText, heroName, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public int selectedSystem, selectedPlanet, numberOfGridChildren;
	private float xLoc, yLoc;
	private Vector2 scrollPositionA = Vector2.zero, scrollPositionB = Vector2.zero;
	public Texture2D industryTexture, scienceTexture, moneyTexture;
	public GameObject gridObject, playerSystemInfoScreen;
	public UIGrid gridList;
	private List<UILabel> planetLabels = new List<UILabel> ();
	private List<Transform> planetSprites = new List<Transform> ();

	void Start()
	{
		GUIRectBuilder();
		foreach(Transform child in gridObject.GetComponentsInChildren<Transform>())
		{
			if(child == gridObject.transform)
			{
				continue;
			}
			planetSprites.Add(child);
		}
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

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
			systemSIMData = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<SystemSIMData>();

			numberOfGridChildren = 0;

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
				planetLabels.Clear();

				Debug.Log (systemListConstructor.systemList[selectedSystem].systemSize);

				for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
				{
					planetLabels.Add(planetSprites[i].gameObject.GetComponent<UILabel>());
						
					++numberOfGridChildren;
				}

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

		UpdateVariables ();

		if(cameraFunctionsScript.openMenu == false)
		{
			NGUITools.SetActive(playerSystemInfoScreen, false);
		}

		#region planetinfomenu
		if(cameraFunctionsScript.openMenu == true)
		{		
			NGUITools.SetActive(playerSystemInfoScreen, true);

			float gridWidth = (numberOfGridChildren * gridList.cellWidth) / 2 - (gridList.cellWidth/2);
			
			gridObject.transform.localPosition = new Vector3(playerSystemInfoScreen.transform.localPosition.x - gridWidth, 
			                                                     gridObject.transform.localPosition.y, gridObject.transform.localPosition.z);

			gridList.repositionNow = true;

			for(int i = 0; i < 6; i++) //This sections of the function evaluates the improvement level of each system, and improves it if the button is clicked
			{	
				if(i < planetLabels.Count)
				{
					NGUITools.SetActive(planetSprites[i].gameObject, true);
					planetLabels[i].text = systemSIMData.allPlanetsInfo[i];
				}
				if(i >= planetLabels.Count)
				{
					NGUITools.SetActive(planetSprites[i].gameObject, false);
				}
			}

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
					heroGUI.CheckIfCanHire(selectedSystem);
				}
			}

			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].heroesInSystem.Count; ++i)
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
						heroGUI.selectedHero.name = heroName;
						tier3HeroScript.FillLinkableSystems();
					}
				}

				else
				{
					GUI.Label (allHeroLabels[i], heroName);
				}
			}
			#endregion
		}
		#endregion
	}

	public void RefreshHeroInfo(int hero)
	{
		heroName = systemListConstructor.systemList[selectedSystem].heroesInSystem[hero].name;
	}
}
