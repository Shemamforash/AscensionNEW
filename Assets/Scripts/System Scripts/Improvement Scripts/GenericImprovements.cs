using UnityEngine;
using System.Collections;

public class GenericImprovements : MasterScript
{
	public void CheckTierZero(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[0].hasBeenBuilt == true) //Secondary Research
		{
			for(int i = 0; i < improvements.listOfImprovements.Count; ++i)
			{
				if(improvements.listOfImprovements[i].hasBeenBuilt == true)
				{
					improvements.sciencePercentBonus += 0.05f;
					improvements.industryPercentBonus += 0.05f;
					improvements.tempCount += 0.05f;
				}
			}
			
			improvements.listOfImprovements[0].improvementMessage = ("+" + improvements.tempCount * 100f + "% Production from Improvements");
		}
		
		if(improvements.listOfImprovements[1].hasBeenBuilt == true) //Synergy
		{
			improvements.tempCount = 0.0f;
			
			int thisSystem = RefreshCurrentSystem(gameObject);
			
			for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
			{
				int k = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);
				
				if(systemListConstructor.systemList[k].systemOwnedBy == thisPlayer.playerRace)
				{
					improvements.industryPercentBonus += 0.075f;
					improvements.tempCount += 0.075f;
				}
			}
			
			improvements.listOfImprovements[1].improvementMessage = ("+" + improvements.tempCount * 100f + "% Industry from nearby systems");
		}
		
		if(improvements.listOfImprovements[2].hasBeenBuilt == true) //Morale
		{
			thisPlayer.capital += thisPlayer.playerOwnedHeroes.Count * 0.02f;
			
			improvements.listOfImprovements[2].improvementMessage = ("+" + (thisPlayer.playerOwnedHeroes.Count * 2f) + "% Capital from active Heroes");
		}
	}
	
	public void CheckTierOne(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[3].hasBeenBuilt == true) //Capitalism
		{
			improvements.tempCount = 0.0f;
			
			int j = improvements.CheckDiplomaticStateOfAllPlayers(thisPlayer, "Peace");
			
			if(j != 0)
			{
				systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();
				systemSIMData.scienceUnitBonus += (turnInfoScript.turn / 20 * Mathf.Pow (2.0f, j));
				improvements.tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));
			}
			
			improvements.listOfImprovements[3].improvementMessage = ("+" + improvements.tempCount + " Science from Peace");
		}

		if(improvements.listOfImprovements[4].hasBeenBuilt == true)
		{
			int tempCount = CheckNumberOfPlanetsWithImprovement(4, thisPlayer, improvements);

			improvements.industryPercentBonus += (tempCount * 0.05f);
			
			improvements.listOfImprovements[4].improvementMessage = ("+" + improvements.tempCount * 5f + "% Industry from other Systems with this Improvement");
		}
		
		if(improvements.listOfImprovements[5].hasBeenBuilt == true) //Quick Starters
		{
			int j = improvements.CheckDiplomaticStateOfAllPlayers(thisPlayer, "War");

			if(j != 0)
			{
				improvements.maxOwnershipBonus += 20f;
				improvements.tempCount = 20f;
			}

			improvements.listOfImprovements[5].improvementMessage = ("+" + improvements.tempCount + "% Max Ownership on Planets from War");
		}
	}
	
	public void CheckTierTwo(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[6].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.0f;
			
			int i = RefreshCurrentSystem(gameObject);
			
			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
				{
					improvements.sciencePercentBonus += 0.25f;
					improvements.tempCount += 0.25f;
				}
			}
			
			improvements.listOfImprovements[6].improvementMessage = ("+" + improvements.tempCount * 100f + "% Science from uncolonised planets");
		}
		
		if(improvements.listOfImprovements[7].hasBeenBuilt == true) //Unionisation
		{
			improvements.tempCount = 0.0f;
			bool allPlanetsColonised  = true;
			int i = RefreshCurrentSystem(gameObject);
			
			for(int j = 0; j <  systemListConstructor.systemList[i].systemSize; ++j)
			{
				if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == false)
				{
					allPlanetsColonised = false;
				}
			}
			
			if(allPlanetsColonised == true)
			{
				improvements.industryPercentBonus += 0.2f;
				improvements.tempCount += 0.2f;
			}
			
			improvements.industryPercentBonus += 0.1f;
			improvements.tempCount += 0.1f;
			
			improvements.listOfImprovements[7].improvementMessage = ("+" + improvements.tempCount * 100f + "% Industry on System");
		}
		
		if(improvements.listOfImprovements[8].hasBeenBuilt == true) //Familiarity
		{
			improvements.listOfImprovements[8].improvementMessage = ("2x SIM production on Home-Type Planets");
		}
	}
	
	public void CheckTierThree(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[9].hasBeenBuilt == true)
		{
			improvements.tempCount = 0.0f;
			float tempCountB = 0.0f;
			
			systemSIMData.scienceUnitBonus += (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			improvements.tempCount = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			
			systemSIMData.industryUnitBonus -= (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			tempCountB = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			
			improvements.listOfImprovements[9].improvementMessage = ("+" + improvements.tempCount + " Science, -" + tempCountB + " Industry On System");
		}

		if(improvements.listOfImprovements[10].hasBeenBuilt == true)
		{
			//TODO
		}

		if(improvements.listOfImprovements[11].hasBeenBuilt == true)
		{
			int tempCount = CheckNumberOfPlanetsWithImprovement(11, thisPlayer, improvements);
			
			improvements.researchCost += tempCount;
			
			improvements.listOfImprovements[11].improvementMessage = ("-" + improvements.tempCount + " Research cost from other Systems with this Improvement");
		}
	}

	private int CheckNumberOfPlanetsWithImprovement(int improvementNo, TurnInfo thisPlayer, ImprovementsBasic improvements)
	{
		int currentPlanets = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == null || systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				continue;
			}
			
			if(improvements.listOfImprovements[improvementNo].hasBeenBuilt == true)
			{
				++currentPlanets;
			}
		}

		return currentPlanets;
	}
}
