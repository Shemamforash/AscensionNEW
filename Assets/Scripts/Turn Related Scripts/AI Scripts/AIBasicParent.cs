using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, highestSIM;
	private int tempPlanet, tempSystem, tempPlanetB, tempSystemB, currentPlanet, currentSystem, checkHeroTimer = 0;
	private bool saveForHero;
	private TurnInfo thisPlayer;
	private AIHeroBehaviour heroBehaviour;

	public void Expand(TurnInfo player)
	{
		thisPlayer = player;

		for(float i = thisPlayer.capital; i > 0; --i)
		{
			CheckToSaveForHero();

			if(saveForHero == false)
			{
				AIExpansion();
			}

			if(saveForHero == true)
			{
				string temp = heroBehaviour.SetSpecialisation();
				heroGUI.CheckIfCanHire(thisPlayer, temp);
			}
		}

		turnInfoScript.TurnEnd(thisPlayer);

		if(heroBehaviour == null)
		{
			heroBehaviour = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<AIHeroBehaviour> ();
		}

		heroBehaviour.HeroDecisionStart (player);
	}

	private void OptimumTechToBuild(int system)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		float sciRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemScience;
		float indRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemIndustry;

		for(int i = 0; i < systemListConstructor.systemList[system].planetsInSystem.Count; ++i)
		{

		}
	}

	private void CheckToSaveForHero()
	{
		checkHeroTimer++;
		
		if(checkHeroTimer == 6)
		{
			float temp = 0;
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
			{
				if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
				{
					systemSIMData = systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();
					temp += systemSIMData.totalSystemScience + systemSIMData.totalSystemIndustry;
				}
				
				if(temp >= ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = true;
				}

				else if(temp < ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = false;
				}
			}

			checkHeroTimer = 0;
		}
	}

	public void AIExpansion()
	{
		if(thisPlayer.capital > 1)
		{
			turnInfoScript.RefreshPlanetPower();

			currentPlanet = -1;
			currentSystem = -1;

			float planetSIM = CheckThroughPlanets (thisPlayer);
			
			float systemSIM = CheckThroughSystems (thisPlayer);
			
			if(planetSIM > systemSIM && thisPlayer.capital >= systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].capitalValue)
			{
				currentPlanet = tempPlanet;
				
				currentSystem = tempSystem;

				thisPlayer.capital -= systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].capitalValue;

				thisPlayer.capitalModifier += 0.05f;
			}
			
			if(systemSIM > planetSIM && thisPlayer.capital >= 20.0f)
			{
				currentPlanet = tempPlanetB;
				
				currentSystem = tempSystemB;
				
				systemListConstructor.systemList[currentSystem].systemOwnedBy = thisPlayer.playerRace;
				
				lineRenderScript = systemListConstructor.systemList[currentSystem].systemObject.GetComponent<LineRenderScript>();
				
				lineRenderScript.SetRaceLineColour(thisPlayer.playerRace);
				
				systemListConstructor.systemList[currentSystem].systemObject.renderer.material = thisPlayer.materialInUse;

				ambientStarRandomiser.AmbientColourChange(currentSystem);

				++systemsInPlay;

				++thisPlayer.systemsColonisedThisTurn;

				thisPlayer.capital -= 20.0f;

				thisPlayer.capitalModifier += 0.1f;

				empireBoundaries.ModifyBoundaryCircles ();
			}

			if(currentPlanet != -1 && currentSystem != -1)
			{
				systemListConstructor.systemList[currentSystem].planetsInSystem[currentPlanet].planetColonised = true;
				
				++thisPlayer.planetsColonisedThisTurn;
			}

			CheckToImprovePlanet (thisPlayer);
		}
	}

	public float CheckThroughPlanets(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				systemDefence = systemListConstructor.systemList [i].systemObject.GetComponent<SystemDefence> ();
				
				if(systemDefence.underInvasion == true)
				{
					continue;
				}

				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true || systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel == 3)
					{
						continue;
					}

					tempSIM = (systemListConstructor.systemList[i].planetsInSystem[j].planetScience + systemListConstructor.systemList[i].planetsInSystem[j].planetIndustry)
						* (systemListConstructor.systemList[i].planetsInSystem[j].improvementSlots * 1.5f);

					if(tempSIM > highestSIM)
					{
						highestSIM = tempSIM;

						tempPlanet = j;
						
						tempSystem = i;
					}
				}
			}
		}

		return highestSIM;
	}

	public float CheckThroughSystems(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();

				for(int j = 0; j < systemListConstructor.systemList[i].numberOfConnections; ++j)
				{
					tempSIM = 0.0f;

					int k = RefreshCurrentSystem(systemListConstructor.systemList[i].permanentConnections[j]);

					if(systemListConstructor.systemList[k].systemOwnedBy == null)
					{
						float tempPlanetSIM = 0.0f;
						int tempPlanet = -1;
						float tempHighestPlanetSIM = 0.0f;

						for(int l = 0; l < systemListConstructor.systemList[k].systemSize; ++l)
						{
							tempPlanetSIM = (systemListConstructor.systemList[k].planetsInSystem[l].planetScience + systemListConstructor.systemList[k].planetsInSystem[l].planetIndustry)  
								* (systemListConstructor.systemList[k].planetsInSystem[l].improvementSlots);

							if(tempPlanetSIM > tempHighestPlanetSIM)
							{
								tempHighestPlanetSIM = tempPlanetSIM;

								tempPlanet = l;
							}

							tempSIM += tempPlanetSIM;
						}

						tempSIM = (tempSIM / systemListConstructor.systemList[k].systemSize);

						if(tempSIM > highestSIM)
						{
							highestSIM = tempSIM;

							tempSystemB = k;

							tempPlanetB = tempPlanet;
						}
					}
				}
			}
		}

		return highestSIM;
	}

	public void CheckToImprovePlanet(TurnInfo thisPlayer)
	{
		for(int i = 0; i < turnInfoScript.mostPowerfulPlanets.Count - 1; i++)
		{
			if(thisPlayer.industry < 0.8f && thisPlayer.capital < 1.0f)
			{
				break;
			}

			int j = RefreshCurrentSystem(turnInfoScript.mostPowerfulPlanets[i].system);

			if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
			{
				ImprovePlanet(turnInfoScript.mostPowerfulPlanets[i].planetPosition, j, thisPlayer);
			}
		}
	}
	
	public void ImprovePlanet(int planetPosition, int system, TurnInfo thisPlayer)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemSIMData.improvementNumber = systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(system, planetPosition);
		
		if(systemSIMData.canImprove == true && systemDefence.underInvasion == false)
		{
			float industryImprovementCost = systemFunctions.IndustryCost(systemSIMData.improvementNumber, system, planetPosition);

			if(thisPlayer.industry >= industryImprovementCost && thisPlayer.capital >= systemSIMData.improvementCost)
			{
				++systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				thisPlayer.industry -= industryImprovementCost;

				thisPlayer.capital -= systemSIMData.improvementCost;
			}
		}
	}
}

