using UnityEngine;
using System.Collections;

public class NereidesImprovements : MasterScript 
{
	public void CheckNereidesImprovements(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[20].hasBeenBuilt == true)
		{
			improvements.improvementCostModifier += racialTraitScript.nereidesStacks;
			improvements.listOfImprovements[20].improvementMessage = ("-" + racialTraitScript.nereidesStacks + " Industry Cost for Improvements");
		}
		
		if(improvements.listOfImprovements[21].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.1f * (float)racialTraitScript.nereidesStacks;
			improvements.sciencePercentBonus += improvements.tempCount;
			improvements.listOfImprovements[21].improvementMessage = ("+" + improvements.tempCount + "% Science from Elation");
		}
		
		if(improvements.listOfImprovements[22].hasBeenBuilt == true)
		{
			improvements.tempCount = 0f;
			
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Icy" || tempString == "Tundra" || tempString == "Dead")
				{
					improvements.tempCount += 1f;
				}
			}

			if(improvements.IsBuiltOnPlanetType(system, 22, "Icy") == true)
			{
				improvements.tempCount = improvements.tempCount * 2f;
			}
			
			thisPlayer.capital += (int)improvements.tempCount;
			improvements.listOfImprovements[22].improvementMessage = ("+" + improvements.tempCount + " Capital from Cold Planets");
		}
		
		if(improvements.listOfImprovements[23].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Icy")
				{
					systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots = 3;
					systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
				
				if(tempString == "Tundra")
				{
					systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots = 4;
					systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
			
			improvements.listOfImprovements[23].improvementMessage = ("+1 Improvement Slot on Tundra and Icy Planets");
		}
		
		if(improvements.listOfImprovements[24].hasBeenBuilt == true)
		{
			improvements.listOfImprovements[24].improvementMessage = ("+50% Industry and 0% Science on Hot Planets");
		}
		
		if(improvements.listOfImprovements[25].hasBeenBuilt == true)
		{
			improvements.scienceBonusModifier += 1.0f;
			improvements.tempCount = 100f;

			if(improvements.IsBuiltOnPlanetType(system, 25, "Icy") == true)
			{
				improvements.scienceBonusModifier += 1.5f;
				improvements.tempCount = 150f;
			}
			
			improvements.listOfImprovements[25].improvementMessage = ("+" + improvements.tempCount + "% Effect from Science Improvements");
		}
		
		if(improvements.listOfImprovements[26].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.1f;

			if(improvements.IsBuiltOnPlanetType(system, 26, "Icy") == true || improvements.IsBuiltOnPlanetType(system, 26, "Tundra") == true || improvements.IsBuiltOnPlanetType(system, 26, "Dead") == true)
			{
				improvements.tempCount = 0.15f;
			}
			
			improvements.ownershipModifier += improvements.tempCount * racialTraitScript.nereidesStacks;
			improvements.listOfImprovements[26].improvementMessage = ("+" + improvements.tempCount * racialTraitScript.nereidesStacks + "Ownership from Elation");
		}
		
		if(improvements.listOfImprovements[27].hasBeenBuilt == true)
		{
			if(systemListConstructor.systemList[system].systemDefence < systemDefence.maxSystemDefence)
			{
				improvements.sciencePercentBonus += 1f;
				improvements.industryPercentBonus += 1f;
				
				improvements.listOfImprovements[27].improvementMessage = ("+100% Resource Production from Invasion");
			}
		}
	}

}
