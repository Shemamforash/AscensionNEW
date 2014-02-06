using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemGUI : MasterScript 
{ 
	private SystemScrollviews systemScrollviews;

	public GUISkin mySkin;
	private string dataSIMString;
	public int selectedSystem, selectedPlanet, numberOfGridChildren, numberOfHeroes;
	public List<PlanetUIElements> planetElementList = new List<PlanetUIElements>();
	public GameObject planetListGrid, playerSystemInfoScreen;

	void Start()
	{
		systemScrollviews = gameObject.GetComponent<SystemScrollviews> ();
		SetUpPlanets ();
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

		UpdateVariables ();
		
		if(cameraFunctionsScript.openMenu == false)
		{
			NGUITools.SetActive(playerSystemInfoScreen, false);
		}
		
		if(cameraFunctionsScript.openMenu == true)
		{		
			NGUITools.SetActive(playerSystemInfoScreen, true);
			
			float gridWidth = (numberOfGridChildren * planetListGrid.GetComponent<UIGrid>().cellWidth) / 2 - (planetListGrid.GetComponent<UIGrid>().cellWidth/2);
			
			planetListGrid.transform.localPosition = new Vector3(playerSystemInfoScreen.transform.localPosition.x - gridWidth, 
			                                                     planetListGrid.transform.localPosition.y, planetListGrid.transform.localPosition.z);
			
			planetListGrid.GetComponent<UIGrid>().repositionNow = true;
			
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
		}
	}

	public void HireHero()
	{
		if(numberOfHeroes <= 6)
		{
			heroGUI.CheckIfCanHire();
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

	private void SetUpPlanets()
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
			NGUITools.SetActive(systemScrollviews.improvementsToBuildScrollView, true);
			systemScrollviews.UpdateScrollviewContents();
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
		systemSIMData.CheckPlanetValues (selectedSystem, i, playerTurnScript);
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
}

public class PlanetUIElements
{
	public GameObject spriteObject;
	public UILabel infoLabel, industryCost, capitalCost;
	public UIButton improveButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}