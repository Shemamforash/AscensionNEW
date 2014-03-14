using UnityEngine;
using System.Collections;

public class HumanImprovements : MasterScript 
{
	public void CheckHumanImprovements(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[12].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership < systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership)
					{
						++systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
					}
				}
			}
			
			improvements.listOfImprovements[12].improvementMessage = ("+1 Ownership per turn");
		}
		
		if(improvements.listOfImprovements[13].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetCategory == "Terran")
				{
					if(5 > systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership - systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership)
					{
						systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership += systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership - 
							systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
					}
					else
					{
						systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership += 5;
					}
				}
			}
			
			improvements.listOfImprovements[13].improvementMessage = ("+5 Ownership on Terran");
		}
		
		if(improvements.listOfImprovements[14].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel == 3)
				{
					improvements.maxOwnershipBonus += 20;
				}
			}
			
			improvements.listOfImprovements[14].improvementMessage = ("+20% Max Ownership on Fully Improved Systems");
		}
		
		if(improvements.listOfImprovements[15].hasBeenBuilt == true)
		{			
			improvements.sciencePercentBonus -= 0.3f;
			improvements.industryPercentBonus -= 0.3f;
			
			++racialTraitScript.ambitionCounter;
			
			improvements.listOfImprovements[15].improvementMessage = ("-30% SIM Converted to Ambition");
		}
		
		if(improvements.listOfImprovements[16].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership < 33)
				{
					systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership = 33;
				}
			}
			
			improvements.listOfImprovements[16].improvementMessage = ("Minimum Ownership of 33%");
		}
		
		if(improvements.listOfImprovements[17].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.0f;
			
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetCategory == "Terran")
				{
					racialTraitScript.ambitionCounter += 2;
					improvements.tempCount = 2.0f;
					break;
				}
			}
			
			improvements.listOfImprovements[17].improvementMessage = ("+" + improvements.tempCount + " Ambition from Terran Planet");
		}
		
		if(improvements.listOfImprovements[18].hasBeenBuilt == true)
		{
			improvements.listOfImprovements[18].improvementMessage = ("Ambition has no effect on planet Ownership");
		}
		
		if(improvements.listOfImprovements[19].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.0f;
			
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
			
			improvements.sciencePercentBonus += improvements.tempCount;
			improvements.industryPercentBonus += improvements.tempCount;
			
			improvements.listOfImprovements[19].improvementMessage = (tempString);
		}
	}
}
