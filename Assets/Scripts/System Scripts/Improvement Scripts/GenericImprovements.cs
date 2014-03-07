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
					improvements.tempCount += 0.05f;
				}
			}
			
			improvements.listOfImprovements[0].improvementMessage = ("+" + improvements.tempCount * 100 + "% Science from ");
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
			
			improvements.listOfImprovements[1].improvementMessage = ("+" + improvements.tempCount * 100 + "% Industry from nearby systems");
		}
		
		if(improvements.listOfImprovements[2].hasBeenBuilt == true) //Morale
		{
			improvements.tempCount = 0.0f;
			
			for(int j = 0; j < thisPlayer.playerOwnedHeroes.Count; ++j)
			{				
				heroScript = thisPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();
				
				if(heroScript.heroLocation == gameObject)
				{
					//moneyPercentBonus += (heroScript.currentLevel * 5.0f); //TODO
					improvements.tempCount += (heroScript.currentLevel * 5.0f);
				}
			}
			
			improvements.listOfImprovements[2].improvementMessage = ("+" + improvements.tempCount + "% from Hero levels");
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
				systemSIMData.scienceBonus += (turnInfoScript.turn / 20 * Mathf.Pow (2.0f, j));
				improvements.tempCount = (turnInfoScript.turn * Mathf.Pow (2.0f, j));
			}
			
			improvements.listOfImprovements[3].improvementMessage = ("+" + improvements.tempCount + " Science from Peace");
		}
		
		if(improvements.listOfImprovements[4].hasBeenBuilt == true) //Leadership
		{
			improvements.tempCount = 0.0f;
			
			for(int i = 0; i < systemListConstructor.mapSize; ++i)
			{
				if(systemListConstructor.systemList[i].systemOwnedBy != thisPlayer.playerRace)
				{
					continue;
				}
				
				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true)
					{
						systemSIMData.industryBonus += 1;
						improvements.tempCount += 1;
					}
				}
			}
			
			improvements.listOfImprovements[4].improvementMessage = ("+" + improvements.tempCount + " Industry from colonisation");
		}
		
		if(improvements.listOfImprovements[5].hasBeenBuilt == true) //Quick Starters
		{
			improvements.tempCount = 0.0f;
			
			int j = improvements.CheckDiplomaticStateOfAllPlayers(thisPlayer, "War");
			
			improvements.industryPercentBonus += (j * 0.25f);
			improvements.tempCount += (j * 0.25f);
			improvements.listOfImprovements[5].improvementMessage = ("+" + improvements.tempCount * 100 + "% Industry from War");
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
			
			improvements.listOfImprovements[6].improvementMessage = ("+" + improvements.tempCount * 100 + "% Science from uncolonised planets");
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
			
			improvements.listOfImprovements[7].improvementMessage = ("+" + improvements.tempCount * 100 + "% Industry on System");
		}
		
		if(improvements.listOfImprovements[8].hasBeenBuilt == true) //Familiarity
		{
			improvements.listOfImprovements[8].improvementMessage = ("2x SIM production on Home-Type Planets");
		}
	}
	
	public void CheckTierThree(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		if(improvements.listOfImprovements[9].hasBeenBuilt == true) //Hypernet
		{
			improvements.tempCount = 0.0f;
			float tempCountB = 0.0f;
			
			systemSIMData.scienceBonus += (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			improvements.tempCount = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemScience;
			
			systemSIMData.industryBonus -= (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			tempCountB = (0.025f * turnInfoScript.turn) * systemSIMData.totalSystemIndustry;
			
			improvements.listOfImprovements[9].improvementMessage = ("+" + improvements.tempCount + " Science, -" + tempCountB + " Industry On System");
		}
		
		if(improvements.listOfImprovements[11].hasBeenBuilt == true)
		{
			int currentPlanetsWithHyperNet = 0;
			
			for(int i = 0; i < systemListConstructor.mapSize; ++i)
			{
				if(systemListConstructor.systemList[i].systemOwnedBy == null || systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
				{
					continue;
				}
				
				if(improvements.listOfImprovements[11].hasBeenBuilt == true)
				{
					++currentPlanetsWithHyperNet;
				}
			}
			
			improvements.sciencePercentBonus += (currentPlanetsWithHyperNet * 0.05f);
			improvements.industryPercentBonus += (currentPlanetsWithHyperNet * 0.05f);
			improvements.tempCount = (currentPlanetsWithHyperNet * 0.05f);
			
			improvements.listOfImprovements[11].improvementMessage = ("+" + improvements.tempCount + "% SIM from systems with Hypernet");
		}
	}
}
