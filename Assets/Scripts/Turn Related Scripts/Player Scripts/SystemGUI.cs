using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemGUI : MasterScript 
{ 
	public GUISkin mySkin;
	private string dataSIMString;
	public int selectedSystem, selectedPlanet, numberOfGridChildren;
	public Texture2D industryTexture, scienceTexture;
	public GameObject gridObject, playerSystemInfoScreen, builtImprovementLabel, builtImprovementScrollview;
	private List<GameObject> builtImprovementList = new List<GameObject>();
	public UIGrid gridList;
	private List<PlanetUIElements> planetElementList = new List<PlanetUIElements>();
	private List<GameObject> improvementsList = new List<GameObject>();
	public GameObject scrollviewButton, scrollviewWindow;
	private Vector3 improvementListPosition = new Vector3();

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

		techTreeScript = systemListConstructor.systemList[0].systemObject.GetComponent<TechTreeScript>();

		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{			
			GameObject message = NGUITools.AddChild(builtImprovementScrollview, builtImprovementLabel);

			NGUITools.SetActive(message, false);

			message.GetComponent<UIDragScrollView>().scrollView = builtImprovementScrollview.GetComponent<UIScrollView>();

			builtImprovementList.Add (message);

			GameObject improvement = NGUITools.AddChild(scrollviewWindow, scrollviewButton); //Scrollviewwindow is gameobject containing scrollview, scrollviewbutton is the button prefab

			improvement.transform.Find ("Sprite").GetComponent<UISprite>().depth = 1; //Depth set to 20 to ensure I can see it, will be changed when scrollview actually works

			improvement.transform.Find ("Label").GetComponent<UILabel>().depth = 2;

			improvement.GetComponent<UIDragScrollView>().scrollView = scrollviewWindow.GetComponent<UIScrollView>(); //Assigning scrollview variable of prefab

			improvement.name = techTreeScript.listOfImprovements[i].improvementName; //Just naming the object in the hierarchy

			EventDelegate.Add(improvement.GetComponent<UIButton>().onClick, BuildImprovement);

			improvement.transform.Find ("Label").GetComponent<UILabel>().text = techTreeScript.listOfImprovements[i].improvementName + "\n" + techTreeScript.listOfImprovements[i].improvementCost; //Add label text

			improvementsList.Add (improvement); //Add improvement into a list so I can enable/disable improvements as needed

			NGUITools.SetActive(improvement, false); //Default set improvement to false so it won't be shown in scrollview unless needed

			scrollviewWindow.GetComponent<UIGrid> ().Reposition (); //Reposition in grid
			scrollviewWindow.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview
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

	private void UpdateScrollviewContents()
	{
		NGUITools.SetActive(scrollviewWindow, true); //Display the scrollview
		
		scrollviewWindow.transform.position = planetElementList[selectedPlanet].spriteObject.transform.position;
		
		improvementListPosition = new Vector3(scrollviewWindow.transform.localPosition.x, 
		                          scrollviewWindow.transform.localPosition.y + 240.0f, 
		                          scrollviewWindow.transform.localPosition.z);
		
		scrollviewWindow.transform.localPosition = improvementListPosition;
		
		scrollviewWindow.GetComponent<UIScrollView> ().Scroll (Time.deltaTime); //How does this even work?
		
		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{			
			if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true || techTreeScript.listOfImprovements[i].improvementLevel > techTreeScript.techTier 
			   || techTreeScript.listOfImprovements[i].improvementCategory == enemyOneTurnScript.playerRace 
			   || techTreeScript.listOfImprovements[i].improvementCategory == enemyTwoTurnScript.playerRace) 
			{
				if(improvementsList[i].activeInHierarchy == true)
				{
					NGUITools.SetActive(improvementsList[i], false); //If tech has been built it doesnt need to be shown in the scrollview
				}
				continue;
			}
			
			NGUITools.SetActive(improvementsList[i], true); //If tech has not been built, set it to active so it can be shown in the scrollview
			scrollviewWindow.GetComponent<UIGrid> ().Reposition (); //Reposition grid contents
			scrollviewWindow.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview position

			scrollviewWindow.GetComponent<UIGrid>().repositionNow = true;
		}
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
			UpdateScrollviewContents();
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

	public void BuildImprovement()
	{
		GameObject improvement = UIButton.current.gameObject;

		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{
			if(techTreeScript.listOfImprovements[i].improvementName == improvement.name)
			{
				for(int j = 0; j < systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
				{
					if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(techTreeScript.ImproveSystem(i) == true)
						{
							systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = techTreeScript.listOfImprovements[i].improvementName;
							UpdateScrollviewContents();
						}
						break;
					}
				}
			}
		}
	}

	public void UpdateBuiltImprovements()
	{
		for(int i = 0; i < techTreeScript.improvementMessageArray.Count; ++i)
		{
			NGUITools.SetActive(builtImprovementList[i], true);
			builtImprovementList[i].transform.Find("Sprite").GetComponent<UISprite>().depth = 1;
			builtImprovementList[i].GetComponent<UILabel>().depth = 2;
			builtImprovementList[i].GetComponent<UILabel>().text = techTreeScript.improvementMessageArray[i];
			builtImprovementScrollview.GetComponent<UIScrollView>().ResetPosition();
			builtImprovementScrollview.GetComponent<UIGrid>().Reposition();
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

			UpdateBuiltImprovements();
								
			if(systemSIMData.foundPlanetData == false)
			{
				systemSIMData.SystemSIMCounter(selectedSystem, playerTurnScript);
				systemSIMData.foundPlanetData = true;
			}
		}
	}
}

public class PlanetUIElements
{
	public GameObject spriteObject;
	public UILabel infoLabel, industryCost, capitalCost;
	public UIButton improveButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}