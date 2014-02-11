using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo 
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, highestSIM, systemRatio;
	private int tempPlanet, tempSystem, tempPlanetB, tempSystemB, currentPlanet, currentSystem;
	
	public void Expand(AIBasicParent thisPlayer)
	{
		for(float i = thisPlayer.capital; i > 0; --i)
		{
			AIExpansion(thisPlayer);
		}

		TurnEnd(thisPlayer);
	}

	public void AIExpansion(TurnInfo thisPlayer)
	{
		currentPlanet = -1;
		currentSystem = -1;

		float planetSIM = CheckThroughPlanets (thisPlayer);
		
		float systemSIM = CheckThroughSystems (thisPlayer);
		
		if(planetSIM > systemSIM && thisPlayer.capital >= 5.0f)
		{
			currentPlanet = tempPlanet;
			
			currentSystem = tempSystem;

			thisPlayer.capital -= 5.0f;
		}
		
		if(systemSIM > planetSIM && thisPlayer.capital >= 10.0f)
		{
			currentPlanet = tempPlanetB;
			
			currentSystem = tempSystemB;
			
			systemListConstructor.systemList[currentSystem].systemOwnedBy = thisPlayer.playerRace;
			
			lineRenderScript = systemListConstructor.systemList[currentSystem].systemObject.GetComponent<LineRenderScript>();
			
			lineRenderScript.SetRaceLineColour(thisPlayer.playerRace);
			
			systemListConstructor.systemList[currentSystem].systemObject.renderer.material = thisPlayer.materialInUse;
			
			++systemsInPlay;

			++thisPlayer.systemsColonisedThisTurn;

			thisPlayer.capital -= 10.0f;
		}

		if(currentPlanet != -1 && currentSystem != -1)
		{
			systemListConstructor.systemList[currentSystem].planetsInSystem[currentPlanet].planetColonised = true;
			
			++thisPlayer.planetsColonisedThisTurn;

			CheckToImprovePlanet (thisPlayer);
		}
	}

	public float CheckThroughPlanets(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < 60; ++i)
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
						* (systemListConstructor.systemList[i].planetsInSystem[j].improvementSlots * 3.0f);

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

		for(int i = 0; i < 60; ++i)
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
								* (systemListConstructor.systemList[k].planetsInSystem[l].improvementSlots * 1.0f);

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
			if(thisPlayer.industry < 10.0f && thisPlayer.capital < 1.0f)
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
		
		systemSIMData.CheckImprovement(system, planetPosition);
		
		if(systemSIMData.canImprove == true && systemDefence.underInvasion == false)
		{
			if(industry >= systemSIMData.improvementCost && thisPlayer.capital >= systemSIMData.improvementNumber + 1)
			{
				++systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				industry -= (int)systemSIMData.improvementCost;

				thisPlayer.capital -= systemSIMData.improvementNumber + 1;
			}
		}
	}
}

