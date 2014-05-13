﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemGUI : MasterScript 
{ 
	private SystemScrollviews systemScrollviews;
	public UILabel systemPower, systemKnowledge, systemName, systemSize, planetHeaderName, planetHeaderClass, planetHeaderOwner, improveButtonPower, improveButtonWealth, rareResourceLabel, population, growth, defence, offence;
	
	public int selectedSystem, selectedPlanet, numberOfHeroes;
	private List<PlanetElementDetails> planetElementList = new List<PlanetElementDetails>();
	public GameObject playerSystemInfoScreen, heroChooseScreen, planetInfoWindow, improveButton;
	public GameObject[] planetObjects = new GameObject[6];

	void Start()
	{
		systemScrollviews = gameObject.GetComponent<SystemScrollviews> ();
		SetUpPlanets ();
		selectedPlanet = -1;
	}

	private void CheckActiveElements()
	{
		if(playerSystemInfoScreen.activeInHierarchy == false)
		{
			NGUITools.SetActive(playerSystemInfoScreen, true);
		}
		
		if(selectedPlanet == -1)
		{
			NGUITools.SetActive(planetInfoWindow, false);
		}

		else if(selectedPlanet != -1)
		{
			NGUITools.SetActive(planetInfoWindow, true);
		}
	}

	private void CheckSystemSize()
	{
		for(int i = 0; i < 6; i++) //This sections of the function evaluates the improvement level of each system, and improves it if the button is clicked
		{	
			if(i < systemListConstructor.systemList[selectedSystem].systemSize)
			{
				NGUITools.SetActive(planetObjects[i], true);
				
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
				NGUITools.SetActive(planetObjects[i], false);
			}
		}
	}

	private void Update()
	{
		if(cameraFunctionsScript.openMenu == true)
		{
			if(playerTurnScript.tempObject != null)
			{
				selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
				systemSIMData = playerTurnScript.tempObject.GetComponent<SystemSIMData>();
				improvementsBasic = playerTurnScript.tempObject.GetComponent<ImprovementsBasic>();
			}

			CheckActiveElements();
			UpdateOverview();
			CheckSystemSize();
		}

		if(cameraFunctionsScript.openMenu == false)
		{
			if(playerSystemInfoScreen.activeInHierarchy == true)
			{
				NGUITools.SetActive(playerSystemInfoScreen, false);
				selectedPlanet = -1;
				systemScrollviews.selectedPlanet = -1;
			}
		}
	}

	private string ReturnSystemSize(int size)
	{
		string temp = null;

		switch(size)
		{
		case 1:
			temp = "Tiny";
			break;
		case 2:
			temp = "Small";
			break;
		case 3:
			temp = "Medium";
			break;
		case 4:
			temp = "Large";
			break;
		case 5:
			temp = "Very Large";
			break;
		case 6:
			temp = "Massive";
			break;
		default:
			break;
		}

		int tempInt = 0;
		
		for(int i = 0; i < size; ++i)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true)
			{
				++tempInt;
			}
		}

		temp = temp + " (" + tempInt + "/" + size + ")";

		return temp;
	}

	private void UpdateOverview()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			systemName.text = systemListConstructor.systemList[selectedSystem].systemName.ToUpper();

			systemSize.text = ReturnSystemSize(systemListConstructor.systemList[selectedSystem].systemSize).ToUpper();

			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

			systemSIMData = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<SystemSIMData>();
			
			if(selectedPlanet != -1)
			{
				systemSIMData.CheckPlanetValues(selectedPlanet, "None");

				string headerTemp = systemListConstructor.systemList[selectedSystem].systemName + " " + (selectedPlanet + 1);

				planetHeaderName.text = headerTemp.ToUpper();
				planetHeaderClass.text = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetCategory.ToUpper();
				planetHeaderOwner.text = systemListConstructor.systemList[selectedSystem].systemOwnedBy;
			}
			
			if(cameraFunctionsScript.openMenu == true)
			{
				systemPower.text = Math.Round (systemSIMData.totalSystemPower, 1).ToString();
				systemKnowledge.text = Math.Round (systemSIMData.totalSystemKnowledge, 1).ToString();  
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

	public void PlanetInterfaceClick()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(planetObjects[i] == UIButton.current.gameObject)
			{
				selectedPlanet = i;
				break;
			}
		}

		if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
			{
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
		}
	}

	private void UpdateColonisedPlanetDetails(int i, int system)
	{
		NGUITools.SetActive (planetElementList [i].knowledge.gameObject, true);
		NGUITools.SetActive (planetElementList [i].power.gameObject, true);
		NGUITools.SetActive (planetElementList [i].population.gameObject, true);
		NGUITools.SetActive (planetElementList [i].quality.gameObject, true);
		NGUITools.SetActive (planetElementList [i].name.gameObject, true);
		
		NGUITools.SetActive (planetElementList [i].uncolonised.gameObject, false);

		planetElementList [i].name.text = systemListConstructor.systemList [selectedSystem].planetsInSystem [i].planetType.ToUpper();

		planetElementList [i].quality.text = systemSIMData.allPlanetsInfo [i].generalInfo.ToUpper();
		planetElementList [i].powerOP.text = systemSIMData.allPlanetsInfo [i].powerOutput;
		planetElementList [i].knowledgeOP.text = systemSIMData.allPlanetsInfo [i].knowledgeOutput;
		planetElementList [i].population.text = systemSIMData.allPlanetsInfo [i].population;

		population.text = Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetOwnership, 1).ToString () + "%/" + 
			Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].maxOwnership, 1).ToString () + "% COLONISED";
		growth.text = Math.Round (systemSIMData.ownershipToAdd, 1).ToString() + "% GROWTH";

		defence.text = Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetCurrentDefence, 0).ToString () + "/" +
						Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetMaxDefence, 0).ToString () + " DEFENCE (+" + 
						Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].defenceRegeneration, 1).ToString () + "/S)";

		if(rareResourceLabel.text == "2NDRY RESOURCE")
		{
			string tempRes = systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType;

			if(tempRes == null)
			{
				tempRes = "NO RESOURCE";
			}

			rareResourceLabel.text = tempRes;

			NGUITools.SetActive(rareResourceLabel.gameObject, true);
		}
				
		if(systemSIMData.improvementNumber < 3)
		{
			improveButton.GetComponent<UIButton>().isEnabled = true;
			float temp = systemFunctions.PowerCost(systemSIMData.improvementNumber, selectedSystem, i);
			improveButtonPower.text = Math.Round (temp, 1).ToString();
			improveButtonWealth.text = systemSIMData.improvementCost.ToString();
		}
		
		if(systemSIMData.improvementNumber == 3 && improveButton.GetComponent<UIButton>().isEnabled == true)
		{
			improveButtonPower.text = "-";
			improveButtonWealth.text = "-";
			improveButton.GetComponent<UIButton>().isEnabled = false;
		}
	}

	private void UpdateUncolonisedPlanetDetails(int i)
	{
		NGUITools.SetActive (planetElementList [i].knowledge.gameObject, false);
		NGUITools.SetActive (planetElementList [i].power.gameObject, false);
		NGUITools.SetActive (planetElementList [i].population.gameObject, false);
		NGUITools.SetActive (planetElementList [i].quality.gameObject, false);
		NGUITools.SetActive (planetElementList [i].name.gameObject, false);

		NGUITools.SetActive (planetElementList [i].uncolonised.gameObject, true);

		planetElementList[i].uncolonised.text = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType.ToUpper() + " (UNCOLONISED)";
		planetElementList [i].knowledgeOP.text = "";
		planetElementList [i].powerOP.text = "";
		planetElementList [i].population.text = "";
	}

	/*
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
	*/

	private void SabotageButton()
	{
		for(int i = 0; i < planetElementList.Count; ++i)
		{
			//TODO
		}
	}

	private void SetUpPlanets()
	{
		for(int i = 0; i < planetObjects.Length; ++i)
		{
			PlanetElementDetails temp = new PlanetElementDetails();

			temp.knowledge = planetObjects[i].transform.Find ("Knowledge").gameObject.GetComponent<UISprite>();
			temp.power = planetObjects[i].transform.Find ("Power").gameObject.GetComponent<UISprite>();
			temp.knowledgeOP = planetObjects[i].transform.Find ("Knowledge").transform.Find ("Knowledge Output").gameObject.GetComponent<UILabel>();
			temp.powerOP = planetObjects[i].transform.Find ("Power").transform.Find ("Power Output").gameObject.GetComponent<UILabel>();
			temp.quality = planetObjects[i].transform.Find ("Quality").gameObject.GetComponent<UILabel>();
			temp.name = planetObjects[i].transform.Find ("Name").gameObject.GetComponent<UILabel>();
			temp.population = planetObjects[i].transform.Find ("Population").gameObject.GetComponent<UILabel>(); 
			temp.uncolonised = planetObjects[i].transform.Find ("Uncolonised Label").gameObject.GetComponent<UILabel>();

			planetElementList.Add (temp);
		}
	}

	private class PlanetElementDetails
	{
		public UILabel knowledgeOP, powerOP, quality, name, population, uncolonised;
		public UISprite power, knowledge;
	}

	/*
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
	*/
}

public class PlanetUIElements
{
	public GameObject spriteObject, knowledgeProductionSprite, powerProductionSprite;
	public UILabel infoLabel, powerProduction, knowledgeProduction, powerCost, wealthCost, rareResourceLabel, populationLabel;
	public UIButton improveButton, sabotageButton;
	public List<GameObject> improvementSlots = new List<GameObject>();
}