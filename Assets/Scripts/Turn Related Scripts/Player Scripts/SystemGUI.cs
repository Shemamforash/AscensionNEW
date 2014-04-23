using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemGUI : MasterScript 
{ 
	private SystemScrollviews systemScrollviews;

	public GUISkin mySkin;
	public UILabel systemPower, systemKnowledge;
	public GameObject planetPrefab;
	public int selectedSystem, selectedPlanet, numberOfHeroes;
	public List<PlanetUIElements> planetElementList = new List<PlanetUIElements>();
	public GameObject planetListGrid, playerSystemInfoScreen, heroChooseScreen;

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
						UpdateColonisedPlanetDetails(i, selectedSystem);
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
				systemPower.text = Math.Round (systemSIMData.totalSystemPower, 1) + " (" + Math.Round (systemSIMData.systemPowerModifier,1) + ")";
				systemKnowledge.text = Math.Round (systemSIMData.totalSystemKnowledge, 1) + " (" + Math.Round (systemSIMData.systemKnowledgeModifier,1) + ")";  
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
		NGUITools.SetActive (heroChooseScreen, true);

	}

	public void ChooseHeroSpecialisation()
	{
		string heroToHire = UIButton.current.transform.parent.gameObject.name;
		turnInfoScript.CheckIfCanHire(playerTurnScript, heroToHire);
		NGUITools.SetActive (heroChooseScreen, false);
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
			planet.knowledgeProductionSprite = planet.spriteObject.transform.Find ("Knowledge Output").gameObject;
			planet.knowledgeProduction = planet.knowledgeProductionSprite.transform.Find("Knowledge Label").gameObject.GetComponent<UILabel>();
			planet.powerProductionSprite = planet.spriteObject.transform.Find("Power Output").gameObject;
			planet.powerProduction = planet.powerProductionSprite.transform.Find("Power Label").gameObject.GetComponent<UILabel>();
			planet.improveButton = planet.spriteObject.transform.Find("Improve Button").gameObject.GetComponent<UIButton>();
			planet.wealthCost = planet.improveButton.transform.Find("Wealth Cost").gameObject.GetComponent<UILabel>();
			planet.powerCost = planet.improveButton.transform.Find("Power Cost").gameObject.GetComponent<UILabel>();
			planet.rareResourceLabel = planet.spriteObject.transform.Find("Rare Resource Output").gameObject.GetComponent<UILabel>();
			planet.sabotageButton = planet.spriteObject.transform.Find("Sabotage").gameObject.GetComponent<UIButton>();

			float scale = (Screen.width * 0.0893f) + 28.557f;

			scale = 1f/200f * (float)Math.Round((double)scale, 2);

			planet.spriteObject.GetComponent<Transform>().localScale = new Vector3(scale, 1f, 1f);

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

			if(playerTurnScript.wealth >= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].wealthValue)
			{
				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == false)
				{
					systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised = true;
					++playerTurnScript.planetsColonisedThisTurn;
					systemSIMData.CheckPlanetValues(selectedPlanet, "None");
					playerTurnScript.wealth -= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].wealthValue;
					playerTurnScript.wealthModifier += 0.1f;
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

			float powerImprovementCost = systemFunctions.PowerCost(systemSIMData.improvementNumber, selectedSystem, selectedPlanet);

			if(playerTurnScript.power >= powerImprovementCost && playerTurnScript.wealth >= systemSIMData.improvementCost)
			{
				playerTurnScript.power -= powerImprovementCost;
				playerTurnScript.wealth -= systemSIMData.improvementCost;
				++systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;
				UpdateColonisedPlanetDetails(selectedPlanet, selectedSystem);
			}

			selectedPlanet = -1;
		}
	}

	private void UpdateColonisedPlanetDetails(int i, int system)
	{
		if(planetElementList[i].knowledgeProductionSprite.activeInHierarchy == false)
		{
			NGUITools.SetActive (planetElementList [i].knowledgeProductionSprite, true);
		}

		if(planetElementList[i].powerProductionSprite.activeInHierarchy == false)
		{
			NGUITools.SetActive (planetElementList [i].powerProductionSprite, true);
		}

		planetElementList[i].infoLabel.text = systemSIMData.allPlanetsInfo[i].generalInfo;
		planetElementList [i].powerProduction.text = systemSIMData.allPlanetsInfo [i].powerOutput;
		planetElementList [i].knowledgeProduction.text = systemSIMData.allPlanetsInfo [i].knowledgeOutput;

		
		if(systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType != null)
		{
			planetElementList[i].rareResourceLabel.text = systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType;
			NGUITools.SetActive(planetElementList[i].rareResourceLabel.gameObject, true);
		}
		else
		{
			NGUITools.SetActive(planetElementList[i].rareResourceLabel.gameObject, false);
		}
		
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetImprovementLevel;
		systemFunctions.CheckImprovement(selectedSystem, i);
		
		NGUITools.SetActive(planetElementList[i].spriteObject, true);
		NGUITools.SetActive(planetElementList[i].improveButton.gameObject, true);
		
		if(systemSIMData.improvementNumber < 3)
		{
			planetElementList[i].improveButton.isEnabled = true;
			float temp = systemFunctions.PowerCost(systemSIMData.improvementNumber, selectedSystem, i);
			planetElementList[i].powerCost.text = (Math.Round (temp, 1)).ToString();
			planetElementList[i].wealthCost.text = systemSIMData.improvementCost.ToString();
		}
		
		if(systemSIMData.improvementNumber == 3)
		{
			planetElementList[i].improveButton.isEnabled = false;
		}
		
		planetElementList[i].improveButton.gameObject.GetComponent<UILabel>().text = CheckPlanetImprovement(i);

		UpdateImprovementGrid (i, system);
	}

	private void UpdateUncolonisedPlanetDetails(int i)
	{
		NGUITools.SetActive (planetElementList [i].knowledgeProductionSprite, false);
		NGUITools.SetActive (planetElementList [i].powerProductionSprite, false);
		planetElementList[i].infoLabel.text = "Uncolonised\nClick to Colonise";
		NGUITools.SetActive(planetElementList[i].improveButton.gameObject, false);
		NGUITools.SetActive(planetElementList[i].rareResourceLabel.gameObject, false);
	}

	private void UpdateImprovementGrid(int i, int system)
	{
		for(int j = 0; j < 4; ++j)
		{
			if(j < systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots)
			{
				NGUITools.SetActive(planetElementList[i].improvementSlots[j], true);
				planetElementList[i].improvementSlots[j].gameObject.GetComponent<UILabel>().text = systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt[j];
				planetElementList[i].improvementSlots[j].gameObject.name = systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt[j];
			}
			if(j >= systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots)
			{
				NGUITools.SetActive(planetElementList[i].improvementSlots[j], false);
			}
		}
	}

	private void SabotageButton()
	{
		for(int i = 0; i < planetElementList.Count; ++i)
		{
			if(UIButton.current == planetElementList[i].sabotageButton)
			{

			}
		}
	}
}

public class PlanetUIElements
{
	public GameObject spriteObject, knowledgeProductionSprite, powerProductionSprite;
	public UILabel infoLabel, powerProduction, knowledgeProduction, powerCost, wealthCost, rareResourceLabel;
	public UIButton improveButton, sabotageButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}