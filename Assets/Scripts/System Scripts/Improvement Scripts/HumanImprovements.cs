using UnityEngine;
using System.Collections;

public class HumanImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;

	public void TechSwitch(int tech, ImprovementsBasic tempImprov, TurnInfo player, bool check)
	{
		systemSIMData = systemListConstructor.systemList [tempImprov.system].systemObject.GetComponent<SystemSIMData> ();

		improvements = tempImprov;
		checkValue = check;
		
		switch (tech)
		{
		case 12:
			TH1I1();
			break;
		case 13:
			TH1I2();
			break;
		case 14:
			TH2I1();
			break;
		case 15:
			TH2I2();
			break;
		case 16:
			TH3I1();
			break;
		case 17:
			TH3I2();
			break;
		case 18:
			TH4I1();
			break;
		case 19:
			TH4I2();
			break;
		default:
			break;
		}
	}

	private void TH1I1()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetColonised == true)
			{
				if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation < systemListConstructor.systemList[improvements.system].planetsInSystem[i].maxPopulation)
				{
					float tempFloat = systemListConstructor.systemList[improvements.system].planetsInSystem[i].maxPopulation - systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation;

					improvements.tempKnwlUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetKnowledge * ((tempFloat * 2f) / 66.666f);
					improvements.tempPowUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPower * ((tempFloat * 2f) / 66.666f);

					if(checkValue == false)
					{
						++improvements.tempPopulationUnitBonus;
						++systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation;
					}
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[12].improvementMessage = ("+1 Population per turn");
		}
	}

	private void TH1I2()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetCategory == "Terran")
			{
				float oToAdd = 0;

				if(5 > systemListConstructor.systemList[improvements.system].planetsInSystem[i].maxPopulation - systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation)
				{
					oToAdd = systemListConstructor.systemList[improvements.system].planetsInSystem[i].maxPopulation - 
						systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation;
				}
				else
				{
					oToAdd = 5;
				}

				improvements.tempPopulationUnitBonus += oToAdd;

				if(checkValue == false)
				{
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation += oToAdd;
				}
			}
		}

		improvements.planetToBuildOn.Add ("Forest");
		improvements.planetToBuildOn.Add ("Ocean");
		improvements.planetToBuildOn.Add ("Prairie");
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempPopulationBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempPopulationBonus / 66.666f);

		if(checkValue == false)
		{
			improvements.listOfImprovements[13].improvementMessage = ("+5 Population on Terran");
		}
	}

	private void TH2I1()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetImprovementLevel == 3)
			{
				improvements.tempKnwlUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetKnowledge * (20f / 66.666f);
				improvements.tempPowUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPower * (20f / 66.666f);

				if(checkValue == false)
				{
					improvements.maxPopulationBonus = 20;
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[14].improvementMessage = ("+20% Max Population on Fully Improved Systems");
		}
	}

	private void TH2I2()
	{
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * -0.3f;
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * -0.3f;
		
		++improvements.tempBonusAmbition;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus -= 0.3f;
			improvements.powerPercentBonus -= 0.3f;

			++racialTraitScript.ambitionCounter;

			improvements.listOfImprovements[15].improvementMessage = ("-30% SIM Converted to Ambition");
		}
	}

	private void TH3I1()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation < 33)
			{
				improvements.tempKnwlUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetKnowledge * (33f / 66.666f);
				improvements.tempPowUnitBonus = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPower * (33f / 66.666f);

				if(checkValue == false)
				{
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetPopulation = 33;
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[16].improvementMessage = ("Minimum Population of 33%");
		}
	}

	private void TH3I2()
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetCategory == "Terran")
			{
				improvements.tempBonusAmbition = 2;
				break;
			}
		}

		improvements.planetToBuildOn.Add ("Forest");
		improvements.planetToBuildOn.Add ("Ocean");
		improvements.planetToBuildOn.Add ("Prairie");

		if(checkValue == false)
		{
			racialTraitScript.ambitionCounter += improvements.tempBonusAmbition;
			improvements.listOfImprovements[17].improvementMessage = ("+" + improvements.tempBonusAmbition + " Ambition from Terran Planet");
		}
	}

	private void TH4I1()
	{
		improvements.tempPopulationBonus = racialTraitScript.ambitionCounter / 40.0f;

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempPopulationBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempPopulationBonus / 66.666f);

		if(checkValue == false)
		{
			improvements.listOfImprovements[18].improvementMessage = ("Ambition has no effect on planet Population");
		}
	}

	private void TH4I2()
	{		
		string tempString = null;
		
		if(racialTraitScript.ambitionCounter > 75)
		{
			improvements.tempCount = (racialTraitScript.ambitionCounter - 75) / 100.0f;
			
			tempString = ("+" + improvements.tempCount + "% SIM from Renaissance");
		}
		if(racialTraitScript.ambitionCounter < -75)
		{
			improvements.tempCount = (racialTraitScript.ambitionCounter + 75) / 100.0f;
			
			tempString = (improvements.tempCount + "% SIM from Depression");
		}

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempCount;
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempCount;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += improvements.tempCount;
			improvements.powerPercentBonus += improvements.tempCount;			
			improvements.listOfImprovements[19].improvementMessage = (tempString);
		}
	}
}
