using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo 
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, highestSIM, systemRatio;
	private int tempPlanet, tempSystem, tempPlanetB, tempSystemB, currentPlanet, currentSystem;
	
	public void Expand(AIBasicParent thisPlayer)
	{
		for(int i = GP; i > 0; --i)
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
		
		if(planetSIM > systemSIM)
		{
			currentPlanet = tempPlanet;
			
			currentSystem = tempSystem;
		}
		
		if(systemSIM > planetSIM)
		{
			currentPlanet = tempPlanetB;
			
			currentSystem = tempSystemB;
			
			systemListConstructor.systemList[currentSystem].systemOwnedBy = thisPlayer.playerRace;
			
			lineRenderScript = systemListConstructor.systemList[currentSystem].systemObject.GetComponent<LineRenderScript>();
			
			lineRenderScript.SetRaceLineColour(thisPlayer.playerRace);
			
			systemListConstructor.systemList[currentSystem].systemObject.renderer.material = thisPlayer.materialInUse;
			
			++systemsInPlay;

			++thisPlayer.systemsColonisedThisTurn;
		}

		if(currentPlanet != -1 && currentPlanet != -1)
		{
			systemListConstructor.systemList[currentSystem].planetsInSystem[currentPlanet].planetColonised = true;
			
			++thisPlayer.planetsColonisedThisTurn;
			
			--GP;

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
				guiPlanScript = systemListConstructor.systemList [i].systemObject.GetComponent<GUISystemDataScript> ();
				
				if(guiPlanScript.underInvasion == true)
				{
					continue;
				}

				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true || systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel == 3)
					{
						continue;
					}

					tempSIM = systemListConstructor.systemList[i].planetsInSystem[j].planetScience + systemListConstructor.systemList[i].planetsInSystem[j].planetIndustry + systemListConstructor.systemList[i].planetsInSystem[j].planetMoney
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

					int k = RefreshCurrentSystem(systemListConstructor.systemList[i].connectedSystems[j]);

					if(systemListConstructor.systemList[k].systemOwnedBy == null)
					{
						float tempPlanetSIM = 0.0f;
						float tempHighestPlanetSIM = 0.0f;

						for(int l = 0; l < systemListConstructor.systemList[k].systemSize; ++l)
						{
							tempPlanetSIM = systemListConstructor.systemList[k].planetsInSystem[l].planetScience + systemListConstructor.systemList[k].planetsInSystem[l].planetIndustry + systemListConstructor.systemList[k].planetsInSystem[l].planetMoney 
								* (systemListConstructor.systemList[k].planetsInSystem[l].improvementSlots * 1.5f);

							if(tempPlanetSIM > tempHighestPlanetSIM)
							{
								tempHighestPlanetSIM = tempPlanetSIM;

								tempPlanetB = l;
							}

							tempSIM += tempPlanetSIM;
						}

						tempSIM = (tempSIM / systemListConstructor.systemList[k].systemSize);

						if(tempSIM > highestSIM)
						{
							highestSIM = tempSIM;

							tempSystemB = k;
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
			if(thisPlayer.industry < 10.0f && thisPlayer.money < 20.0f)
			{
				break;
			}

			int j = RefreshCurrentSystem(turnInfoScript.mostPowerfulPlanets[i].system);

			if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
			{
				ImprovePlanet(turnInfoScript.mostPowerfulPlanets[i].planetPosition, j);
			}
		}
	}
	
	public void ImprovePlanet(int planetPosition, int system)
	{
		guiPlanScript.improvementNumber = systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
		
		guiPlanScript.CheckImprovement(planetPosition, system);
		
		if(guiPlanScript.canImprove == true && guiPlanScript.underInvasion == false)
		{
			if(industry >= guiPlanScript.improvementCost)
			{
				++systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				industry -= (int)guiPlanScript.improvementCost;
			}
			
			else if(money >= guiPlanScript.improvementCost * 2)
			{
				++systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				money -= ((int)guiPlanScript.improvementCost * 2);
			}
		}
	}
}

