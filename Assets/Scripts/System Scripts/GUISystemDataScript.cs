using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GUISystemDataScript : MasterScript
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber;
	[HideInInspector]
	public float pScience, pIndustry, pMoney, improvementCost, resourceBonus;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public string[] allPlanetsInfo = new string[6];	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM, tempTotalSci, tempTotalInd, tempTotalMon;
	public float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;

	void Start()
	{
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();
	}

	public void SystemSIMCounter(int i, TurnInfo thisPlayer) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{				
		tempTotalSci = 0.0f;
		tempTotalInd = 0.0f;
		tempTotalMon = 0.0f;

		for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(systemListConstructor.systemList[i].planetColonised[j] == true)
			{
				string planetType = systemListConstructor.systemList[i].planetType[j];

				improvementNumber = systemListConstructor.systemList[i].planetImprovementLevel[j];

				CheckImprovement();
				
				tempSci = systemListConstructor.systemList[i].planetScience[j]; //Need to sort out variable types, too much casting
				tempInd = systemListConstructor.systemList[i].planetIndustry[j];
				tempMon = systemListConstructor.systemList[i].planetMoney[j];

				techTreeScript.planetToCheck = planetType;

				techTreeScript.CheckPlanets();

				allPlanetsInfo[j] = gameObject.name + " " + (j+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
					+ ((int)(tempSci * resourceBonus * thisPlayer.raceScience)).ToString() + "\n" 
					+ ((int)(tempInd * resourceBonus * thisPlayer.raceIndustry)).ToString() + "\n" 
					+ ((int)(tempMon * resourceBonus * thisPlayer.raceMoney)).ToString();

				tempTotalSci += tempSci * techTreeScript.sciencePercentBonus * resourceBonus * thisPlayer.raceScience;
				tempTotalInd += tempInd * techTreeScript.industryPercentBonus * resourceBonus * thisPlayer.raceIndustry;
				tempTotalMon += tempMon * techTreeScript.moneyPercentBonus * resourceBonus * thisPlayer.raceMoney;
			}
		}
		//Debug.Log(tempTotalSci + " " + techTreeScript.sciencePointBonus + " " + heroScript.heroSciBonus);

		Debug.Log (heroScript.heroSciBonus);

		totalSystemScience = tempTotalSci + techTreeScript.sciencePointBonus + heroScript.heroSciBonus;
		totalSystemIndustry = tempTotalInd + techTreeScript.industryPointBonus  + heroScript.heroIndBonus;
		totalSystemMoney = tempTotalMon + techTreeScript.moneyPointBonus  + heroScript.heroMonBonus;
		
		turnInfoScript.RefreshPlanetPower();
	}

	public void CheckUnlockedTier()
	{
		totalSystemSIM += totalSystemScience + totalSystemIndustry + totalSystemMoney;
			
		if(totalSystemSIM >= 1600.0f && totalSystemSIM < 3200)
		{
			techTreeScript.techTier = 1;
		}
		if(totalSystemSIM >= 3200.0f && totalSystemSIM < 6400)
		{
			techTreeScript.techTier = 2;
		}
		if(totalSystemSIM >= 6400.0f)
		{
			techTreeScript.techTier = 3;
		}
	}

	public void UpdatePlanetPowerArray(int k)
	{
		for(int i = 0; i < numPlanets; ++i)
		{
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 0] = gameObject.name;
			
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 1] = i.ToString();
				
			improvementNumber = systemListConstructor.systemList[k].planetImprovementLevel[i];
			
			CheckImprovement();
			
			float tempInt = systemListConstructor.systemList[k].planetScience[i] + systemListConstructor.systemList[k].planetIndustry[i] + systemListConstructor.systemList[k].planetMoney[i];
			
			turnInfoScript.mostPowerfulPlanets[turnInfoScript.savedIterator, 2] = tempInt.ToString();

			++turnInfoScript.savedIterator;
		}
	}

	public void CheckImprovement() //Contains data on the quality of planets and the bonuses they receive
	{
		if(improvementNumber == 0)
		{
			improvementLevel = "Poor";
			resourceBonus = 0.5f;
			canImprove = true;
			improvementCost = 10.0f;
		}
		if(improvementNumber == 1)
		{
			improvementLevel = "Normal";
			resourceBonus = 1.0f;
			canImprove = true;
			improvementCost = 20.0f;
		}
		if(improvementNumber == 2)
		{
			improvementLevel = "Good";
			resourceBonus = 2.0f;
			canImprove = true;
			improvementCost = 40.0f;
		}
		if(improvementNumber == 3)
		{
			improvementLevel = "Superb";
			resourceBonus = 3.0f;
			canImprove = false;
		}
	}
}
