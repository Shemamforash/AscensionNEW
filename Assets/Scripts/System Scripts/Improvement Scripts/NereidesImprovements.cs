using UnityEngine;
using System.Collections;

public class NereidesImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo thisPlayer;
	
	public void TechSwitch(int tech, ImprovementsBasic tempImprov, TurnInfo player, bool check)
	{
		systemSIMData = systemListConstructor.systemList [improvements.system].systemObject.GetComponent<SystemSIMData> ();

		improvements = tempImprov;
		checkValue = check;
		thisPlayer = player;
		
		switch (tech)
		{
		case 20:
			TN1I1();
			break;
		case 21:
			TN1I2();
			break;
		case 22:
			TN2I1();
			break;
		case 23:
			TN2I2();
			break;
		case 24:
			TN3I1();
			break;
		case 25:
			TN3I2();
			break;
		case 26:
			TN4I1();
			break;
		case 27:
			TN4I2();
			break;
		default:
			break;
		}
	}

	private void TN1I1()
	{
		improvements.tempImprovementCostReduction = racialTraitScript.nereidesStacks;

		if(checkValue == false)
		{
			improvements.improvementCostModifier += (int)improvements.tempImprovementCostReduction;
			improvements.listOfImprovements[20].improvementMessage = ("-" + racialTraitScript.nereidesStacks + " Power Cost for Improvements");
		}
	}

	private void TN1I2()
	{
		improvements.tempKnwlBonus = 0.1f * (float)racialTraitScript.nereidesStacks;
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += improvements.tempKnwlBonus;
			improvements.listOfImprovements[21].improvementMessage = ("+" + improvements.tempCount + "% Knowledge from Elation");
		}
	}

	private void TN2I1()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			string tempString = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetType;
			
			if(tempString == "Boreal" || tempString == "Tundra" || tempString == "Desolate")
			{
				improvements.tempCount += 1f;
			}
		}
		
		if(improvements.IsBuiltOnPlanetType(improvements.system, 22, "Boreal") == true)
		{
			improvements.tempCount = improvements.tempCount * 2f;
		}

		improvements.planetToBuildOn.Add ("Boreal");
		improvements.planetToBuildOn.Add ("Tundra");
		improvements.planetToBuildOn.Add ("Desolate");

		improvements.tempWealth = improvements.tempCount;

		if(checkValue == false)
		{
			thisPlayer.wealth += (int)improvements.tempWealth;
			improvements.listOfImprovements[22].improvementMessage = ("+" + improvements.tempCount + " Wealth from Cold Planets");
		}
	}

	private void TN2I2() //TODO this needs a value
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			string tempString = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetType;
			
			if(tempString == "Boreal" && checkValue == false)
			{
				++improvements.tempImprovementSlots;
				
				if(checkValue == false)
				{
					++systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementSlots;
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
			
			if(tempString == "Tundra" && checkValue == false)
			{
				++improvements.tempImprovementSlots;
				
				if(checkValue == false)
				{
					++systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementSlots;
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
		}

		improvements.planetToBuildOn.Add ("Boreal");
		improvements.planetToBuildOn.Add ("Tundra");

		if(checkValue == false)
		{
			improvements.listOfImprovements[23].improvementMessage = ("+1 Improvement Slot on Tundra and Boreal Planets");
		}
	}

	private void TN3I1()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].planetsInSystem.Count; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetCategory == "Hot")
			{
				improvements.tempKnwlUnitBonus = -systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetKnowledge;
				improvements.tempPowUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPower * 0.5f;
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[24].improvementMessage = ("+50% Power and 0% Knowledge on Hot Planets");
		}
	}

	private void TN3I2()
	{
		improvements.tempKnwlBonus += 1.0f;
		improvements.tempCount = 100f;
		
		if(improvements.IsBuiltOnPlanetType(improvements.system, 25, "Boreal") == true)
		{
			improvements.tempKnwlBonus += 0.5f;
			improvements.tempCount += 50f;
		}

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;
		improvements.planetToBuildOn.Add ("Boreal");

		if(checkValue == false)
		{
			improvements.knowledgeBonusModifier += improvements.tempKnwlBonus;
			improvements.listOfImprovements[25].improvementMessage = ("+" + improvements.tempCount + "% Effect from Knowledge Improvements");
		}
	}

	private void TN4I1()
	{
		if(improvements.IsBuiltOnPlanetType(improvements.system, 26, "Boreal") == true 
		   || improvements.IsBuiltOnPlanetType(improvements.system, 26, "Tundra") == true 
		   || improvements.IsBuiltOnPlanetType(improvements.system, 26, "Desolate") == true)
		{
			improvements.tempCount = 0.15f;
		}

		improvements.tempOwnershipBonus = improvements.tempCount * racialTraitScript.nereidesStacks;
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempOwnershipBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempOwnershipBonus / 66.666f);

		improvements.planetToBuildOn.Add ("Boreal");
		improvements.planetToBuildOn.Add ("Tundra");
		improvements.planetToBuildOn.Add ("Desolate");

		if(checkValue == false)
		{
			improvements.ownershipModifier += improvements.tempOwnershipBonus;
			improvements.listOfImprovements[26].improvementMessage = ("+" + improvements.tempCount * racialTraitScript.nereidesStacks + "Ownership from Elation");
		}
	}

	private void TN4I2()
	{
		if(systemListConstructor.systemList[improvements.system].systemDefence < systemDefence.maxSystemDefence)
		{
			improvements.tempKnwlBonus = 1f;
			improvements.tempPowBonus = 1f;
		}

		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += 1f;
			improvements.powerPercentBonus += 1f;
			improvements.listOfImprovements[27].improvementMessage = ("+100% Resource Production from Invasion");
		}
	}
}
