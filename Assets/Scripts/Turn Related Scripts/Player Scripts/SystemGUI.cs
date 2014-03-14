using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemGUI : MasterScript 
{ 
	private SystemScrollviews systemScrollviews;

	public GUISkin mySkin;
	public UILabel systemIndustry, systemScience;
	public GameObject planetPrefab;
	public int selectedSystem, selectedPlanet, numberOfHeroes;
	public List<PlanetUIElements> planetElementList = new List<PlanetUIElements>();
	public GameObject planetListGrid, playerSystemInfoScreen;

	void Start()
	{
		systemScrollviews = gameObject.GetComponent<SystemScrollviews> ();
		SetUpPlanets ();
		selectedPlanet = -1;
	}

	void Update()
	{
		if(playerTurnScript.tempObject != null)
		{
			selectedSystem = RefreshCurrentSystem(playerTurnScript.tempObject);

			systemSIMData = playerTurnScript.tempObject.GetComponent<SystemSIMData>();
			improvementsBasic = playerTurnScript.tempObject.GetComponent<ImprovementsBasic>();
		}
		
		if(cameraFunctionsScript.openMenu == false)
		{
			if(playerSystemInfoScreen.activeInHierarchy == true)
			{
				NGUITools.SetActive(playerSystemInfoScreen, false);
				selectedPlanet = -1;
				systemScrollviews.selectedPlanet = -1;
				systemScrollviews.tabContainer.GetComponent<UILabel>().text = null;
			}
		}
		
		if(cameraFunctionsScript.openMenu == true)
		{		
			UpdateVariables ();

			NGUITools.SetActive(playerSystemInfoScreen, true);

			PositionGrid(planetListGrid, systemListConstructor.systemList[selectedSystem].systemSize);
			
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

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
			systemSIMData = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<SystemSIMData>();
			
			if(selectedPlanet != -1)
			{
				systemSIMData.CheckPlanetValues(selectedPlanet, "None");
			}
			
			if(cameraFunctionsScript.openMenu == true)
			{
				systemIndustry.text = Math.Round (systemSIMData.totalSystemIndustry, 1) + " (" + Math.Round (systemSIMData.systemIndustryModifier,1) + ")";
				systemScience.text = Math.Round (systemSIMData.totalSystemScience, 1) + " (" + Math.Round (systemSIMData.systemScienceModifier,1) + ")";  
			}
		}
	}

	public void PositionGrid(GameObject grid, int size)
	{
		float gridWidth = (size * grid.GetComponent<UIGrid>().cellWidth) / 2 - (grid.GetComponent<UIGrid>().cellWidth/2);
		
		grid.transform.localPosition = new Vector3(playerSystemInfoScreen.transform.localPosition.x - gridWidth, 
		                                                     grid.transform.localPosition.y, grid.transform.localPosition.z);
		
		grid.GetComponent<UIGrid>().repositionNow = true;
	}

	public void HireHero()
	{
		if(numberOfHeroes <= 6)
		{
			heroGUI.CheckIfCanHire();
		}
	}

	private void SetUpPlanets()
	{
		string[] tempString = new string[6] {"Planet 1", "Planet 2", "Planet 3", "Planet 4", "Planet 5", "Planet 6"};
		
		for(int i = 0; i < 6; ++i)
		{
			PlanetUIElements planet = new PlanetUIElements();
			
			planet.spriteObject = NGUITools.AddChild(planetListGrid, planetPrefab);
			planet.spriteObject.name = tempString[i];
			EventDelegate.Add (planet.spriteObject.GetComponent<UIButton>().onClick, PlanetInterfaceClick);
			planet.infoLabel = planet.spriteObject.transform.Find ("General Output Label").gameObject.GetComponent<UILabel>();
			EventDelegate.Add (planet.spriteObject.transform.Find ("Improve Button").gameObject.GetComponent<UIButton>().onClick, ImprovePlanet);
			planet.scienceProductionSprite = planet.spriteObject.transform.Find ("Science Output").gameObject;
			planet.scienceProduction = planet.scienceProductionSprite.transform.Find("Science Label").gameObject.GetComponent<UILabel>();
			planet.industryProductionSprite = planet.spriteObject.transform.Find("Industry Output").gameObject;
			planet.industryProduction = planet.industryProductionSprite.transform.Find("Industry Label").gameObject.GetComponent<UILabel>();
			planet.improveButton = planet.spriteObject.transform.Find("Improve Button").gameObject.GetComponent<UIButton>();
			planet.capitalCost = planet.improveButton.transform.Find("Capital Cost").gameObject.GetComponent<UILabel>();
			planet.industryCost = planet.improveButton.transform.Find("Industry Cost").gameObject.GetComponent<UILabel>();
			
			Transform[] tempTransform = planet.spriteObject.GetComponentsInChildren<Transform>();
			
			for(int j = 0; j < tempTransform.Length; ++j)
			{
				if(tempTransform[j].gameObject.tag == "Improvement Slot")
				{
					planet.improvementSlots.Add(tempTransform[j].gameObject);
					tempTransform[j].GetComponent<UILabel>().depth = 1;
					NGUITools.SetActive(tempTransform[j].gameObject, false);
				}
			}
			
			planetElementList.Add (planet);
		}
	}

	private string CheckPlanetImprovement(int i)
	{
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(selectedSystem, i);
		
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

		if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
			{
				NGUITools.SetActive(systemScrollviews.improvementsToBuildScrollView, true);
				NGUITools.SetActive(systemScrollviews.tabContainer, true);
				systemScrollviews.techTierToShow = 0;
				systemScrollviews.tabContainer.GetComponent<UILabel>().text = "Build Improvements On " + systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetName;
				systemScrollviews.UpdateScrollviewContents();
				systemScrollviews.selectedPlanet = selectedPlanet;
			}

			if(playerTurnScript.capital >= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].capitalValue)
			{
				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == false)
				{
					systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised = true;
					++playerTurnScript.planetsColonisedThisTurn;
					systemSIMData.CheckPlanetValues(selectedPlanet, "None");
					playerTurnScript.capital -= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].capitalValue;
					playerTurnScript.capitalModifier += 0.1f;
				}
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

		if(systemSIMData.improvementNumber < 3)
		{
			systemFunctions.CheckImprovement(selectedSystem, selectedPlanet);

			float industryImprovementCost = systemFunctions.IndustryCost(systemSIMData.improvementNumber, selectedSystem, selectedPlanet);

			if(playerTurnScript.industry >= industryImprovementCost && playerTurnScript.capital >= systemSIMData.improvementCost)
			{
				playerTurnScript.industry -= industryImprovementCost;
				playerTurnScript.capital -= systemSIMData.improvementCost;
				++systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;
				UpdateColonisedPlanetDetails(selectedPlanet);
			}

			selectedPlanet = -1;
		}
	}

	private void UpdateColonisedPlanetDetails(int i)
	{
		if(planetElementList[i].scienceProductionSprite.activeInHierarchy == false)
		{
			NGUITools.SetActive (planetElementList [i].scienceProductionSprite, true);
		}

		if(planetElementList[i].industryProductionSprite.activeInHierarchy == false)
		{
			NGUITools.SetActive (planetElementList [i].industryProductionSprite, true);
		}

		planetElementList[i].infoLabel.text = systemSIMData.allPlanetsInfo[i].generalInfo;
		planetElementList [i].industryProduction.text = systemSIMData.allPlanetsInfo [i].industryOutput;
		planetElementList [i].scienceProduction.text = systemSIMData.allPlanetsInfo [i].scienceOutput;
		
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		systemFunctions.CheckImprovement(selectedSystem, i);
		
		NGUITools.SetActive(planetElementList[i].spriteObject, true);
		NGUITools.SetActive(planetElementList[i].improveButton.gameObject, true);
		
		if(systemSIMData.improvementNumber < 3)
		{
			planetElementList[i].improveButton.isEnabled = true;
			float temp = systemFunctions.IndustryCost(systemSIMData.improvementNumber, selectedSystem, i);
			planetElementList[i].industryCost.text = (Math.Round (temp, 1)).ToString();
			planetElementList[i].capitalCost.text = systemSIMData.improvementCost.ToString();
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
		NGUITools.SetActive (planetElementList [i].scienceProductionSprite, false);
		NGUITools.SetActive (planetElementList [i].industryProductionSprite, false);
		planetElementList[i].infoLabel.text = "Uncolonised\nClick to Colonise";
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
				planetElementList[i].improvementSlots[j].gameObject.name = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].improvementsBuilt[j];
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
	public GameObject spriteObject, scienceProductionSprite, industryProductionSprite;
	public UILabel infoLabel, industryProduction, scienceProduction, industryCost, capitalCost;
	public UIButton improveButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}