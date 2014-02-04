using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemGUI : MasterScript 
{
	private Rect[] allPlanetsGUI, allButtonsGUI, allImprovementButtonsGUI, allHeroLabels, allHeroButtons; 
	public GUISkin mySkin;
	public bool spendMenu = false, openImprovementList = false, systemOwnedByPlayer;
	public string resourceToSpend;
	private string cost, indSpend, dataSIMString, techBuildButtonText, heroName, playerEnemyOneDiplomacy, playerEnemyTwoDiplomacy;
	public int selectedSystem, selectedPlanet, numberOfGridChildren;
	private float xLoc, yLoc;
	private Vector2 scrollPositionA = Vector2.zero, scrollPositionB = Vector2.zero;
	public Texture2D industryTexture, scienceTexture;
	public GameObject gridObject, playerSystemInfoScreen;
	public UIGrid gridList;
	private List<PlanetUIElements> planetElementList = new List<PlanetUIElements>();
	private List<GameObject> unbuiltImprovements = new List<GameObject>();
	public GameObject scrollviewButton, scrollviewWindow;

	void Start()
	{
		string[] tempString = new string[6] {"Planet 1", "Planet 2", "Planet 3", "Planet 4", "Planet 5", "Planet 6"};

		for(int i = 0; i < 6; ++i)
		{
			PlanetUIElements planet = new PlanetUIElements();

			planet.spriteObject = GameObject.Find (tempString[i]);
			planet.infoLabel = planet.spriteObject.GetComponent<UILabel>();
			planet.improveButton = planet.spriteObject.transform.Find("Improve Button").gameObject.GetComponent<UIButton>();
			planet.capitalCost = planet.improveButton.transform.Find("Capital Cost").gameObject.GetComponent<UILabel>();
			planet.industryCost = planet.improveButton.transform.Find("Industry Cost").gameObject.GetComponent<UILabel>();

			Transform[] tempTransform = planet.spriteObject.GetComponentsInChildren<Transform>();

			for(int j = 0; j < tempTransform.Length; ++j)
			{
				if(tempTransform[j].gameObject.tag == "Improvement Slot")
				{
					planet.improvementSlots.Add(tempTransform[j].gameObject);
					NGUITools.SetActive(tempTransform[j].gameObject, false);
				}
			}

			planetElementList.Add (planet);
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
				for(int i = 0; i < 6; ++i)
				{
					if(i < systemListConstructor.systemList[selectedSystem].systemSize)
					{
						NGUITools.SetActive(planetElementList[i].spriteObject, true);
						++numberOfGridChildren;
					}
					if(i >= systemListConstructor.systemList[selectedSystem].systemSize)
					{
						NGUITools.SetActive(planetElementList[i].spriteObject, false);
					}
				}

				dataSIMString = "Total SIM: " + systemSIMData.totalSystemSIM.ToString() + "\n" + "Total Science: " + systemSIMData.totalSystemScience.ToString() + "\n" 
					+ "Total Industry: " + systemSIMData.totalSystemIndustry.ToString(); 
			}
		}
	}

	private string CheckPlanetImprovement(int i)
	{
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		
		systemSIMData.CheckImprovement(selectedSystem, i);
		
		if(systemSIMData.canImprove == false)
		{
			return "Max Improvement";
		}
		
		if(systemSIMData.canImprove == true)
		{
			return "Improve Cost: ";
		}

		return null;
	}

	public void PlanetInterfaceClick()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(planetElementList[i].spriteObject == UIButton.current.gameObject)
			{
				selectedPlanet = i;
				break;
			}
		}

		if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
		{
			NGUITools.SetActive(scrollviewWindow, true);

			scrollviewWindow.transform.position = planetElementList[selectedPlanet].spriteObject.transform.position;

			scrollviewWindow.GetComponent<UIScrollView> ().Scroll (Time.deltaTime);

			for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
			{
				scrollviewWindow.GetComponent<UIScrollView>().ResetPosition ();

				if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true || techTreeScript.listOfImprovements[i].improvementLevel > techTreeScript.techTier 
				   || techTreeScript.listOfImprovements[i].improvementCategory == enemyOneTurnScript.playerRace 
				   || techTreeScript.listOfImprovements[i].improvementCategory == enemyTwoTurnScript.playerRace)
				{
					continue;
				}

				GameObject improvement = NGUITools.AddChild(scrollviewWindow, scrollviewButton);
				improvement.GetComponent<UISprite>().depth = 20;
				improvement.GetComponent<UIDragScrollView>().scrollView = scrollviewWindow.GetComponent<UIScrollView>();
				improvement.name = techTreeScript.listOfImprovements[i].improvementName;
				improvement.GetComponent<UILabel>().text = techTreeScript.listOfImprovements[i].improvementName + "\n" + techTreeScript.listOfImprovements[i].improvementCost;
				unbuiltImprovements.Add (improvement);

				scrollviewWindow.GetComponent<UIScrollView>().ResetPosition ();
			}
		}

		if(playerTurnScript.capital >= 5)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == false)
			{
				systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised = true;
				++playerTurnScript.planetsColonisedThisTurn;
				systemSIMData.CheckPlanetValues(selectedSystem, selectedPlanet, playerTurnScript);
				playerTurnScript.capital -= 5;
			}
		}
	}

	public void ImprovePlanet()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(planetElementList[i].improveButton.gameObject == UIButton.current.gameObject)
			{
				selectedPlanet = i;
				break;
			}
		}

		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;
		
		systemSIMData.CheckImprovement(selectedSystem, selectedPlanet);

		if(playerTurnScript.industry >= systemSIMData.improvementCost && playerTurnScript.capital >= systemSIMData.improvementNumber + 1)
		{
			playerTurnScript.industry -= systemSIMData.improvementCost;
			playerTurnScript.capital -= systemSIMData.improvementNumber + 1;
			++systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;
		}

		systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
	}

	private void UpdateColonisedPlanetDetails(int i)
	{
		planetElementList[i].infoLabel.text = systemSIMData.allPlanetsInfo[i];
		
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		systemSIMData.CheckImprovement(selectedSystem, i);
		
		NGUITools.SetActive(planetElementList[i].spriteObject, true);
		NGUITools.SetActive(planetElementList[i].improveButton.gameObject, true);
		
		if(systemSIMData.improvementNumber < 3)
		{
			planetElementList[i].improveButton.isEnabled = true;
			planetElementList[i].industryCost.text = systemSIMData.improvementCost.ToString();
			planetElementList[i].capitalCost.text = (systemSIMData.improvementNumber + 1).ToString();
		}
		
		if(systemSIMData.improvementNumber == 3)
		{
			planetElementList[i].improveButton.isEnabled = false;
			NGUITools.SetActiveChildren(planetElementList[i].spriteObject, false);
		}
		
		planetElementList[i].improveButton.gameObject.GetComponent<UILabel>().text = CheckPlanetImprovement(i);

		UpdateImprovementGrid (i);
	}

	private void UpdateUncolonisedPlanetDetails(int i)
	{
		planetElementList[i].infoLabel.text = systemSIMData.allPlanetsInfo[i];
		NGUITools.SetActive(planetElementList[i].improveButton.gameObject, false);
	}

	private void UpdateImprovementGrid(int i)
	{
		for(int j = 0; j < 4; ++j)
		{
			if(j < systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementSlots)
			{
				NGUITools.SetActive(planetElementList[i].improvementSlots[j], true);
				planetElementList[i].improvementSlots[j].gameObject.GetComponent<UILabel>().text = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementsBuilt[j];
			}
			if(j >= systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementSlots)
			{
				NGUITools.SetActive(planetElementList[i].improvementSlots[j], false);
			}
		}
	}

	void OnGUI()
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
				if(i < systemListConstructor.systemList[selectedSystem].systemSize)
				{
					NGUITools.SetActive(planetElementList[i].spriteObject, true);

					if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true)
					{
						UpdateColonisedPlanetDetails(i);
					}

					if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == false)
					{
						UpdateUncolonisedPlanetDetails(i);
					}
				}

				if(i >= systemListConstructor.systemList[selectedSystem].systemSize)
				{
					NGUITools.SetActive(planetElementList[i].spriteObject, false);
				}
			}

			#region planetdata						
			if(systemSIMData.foundPlanetData == false)
			{
				systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
				systemSIMData.foundPlanetData = true;
			}
			#endregion



			/*
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
				}

				if(GUI.Button(new Rect(Screen.width/2 + 77.5f, Screen.height/2 - 45.0f, 18.5f, 18.5f), "x"))
				{
					spendMenu = false;
					selectedPlanet = -1;
				}

				systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
			}
			#endregion
			*/
			
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
				if(GUI.Button (new Rect(Screen.width / 2 - 610.0f, Screen.height / 2 + 300.0f, 150.0f, 50.0f), "Purchase Hero: 50 Capital"))
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

public class PlanetUIElements
{
	public GameObject spriteObject;
	public UILabel infoLabel, industryCost, capitalCost;
	public UIButton improveButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}