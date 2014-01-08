using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo 
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, systemRatio, compensator, tempRatio;
	private GameObject mostValuableSystem;
	private int tempPlanet, tempSystem, planetToColonise;
	
	public void Expand(AIBasicParent thisPlayer)
	{
		planetsColonisedThisTurn = 0;

		for(int i = GP; i > 0; --i)
		{
			mostValuableSystem = null;
			compensator = 0.00f;
			SearchThroughArrays(thisPlayer);
		}

		TurnEnd(thisPlayer);
	}

	public void SearchThroughArrays(AIBasicParent thisPlayer)
	{
		while(mostValuableSystem == null)
		{
			CheckForSuitableSystem(thisPlayer);
			compensator += 0.25f;

			if(compensator > 1.0f)
			{
				break;
			}
		}

		if(mostValuableSystem != null)
		{
			int i = RefreshCurrentSystem(mostValuableSystem);
			guiPlanScript = mostValuableSystem.GetComponent<GUISystemDataScript>();
			ColoniseSystem(i, thisPlayer);
		}
	}

	public void ColoniseSystem(int system, AIBasicParent thisPlayer)
	{
		systemListConstructor.systemList [system].systemOwnedBy = playerRace;

		lineRenderScript = systemListConstructor.systemList[system].systemObject.GetComponent<LineRenderScript>();
		
		lineRenderScript.SetRaceLineColour(thisPlayer.playerRace);
		
		systemListConstructor.systemList[system].systemObject.renderer.material = thisPlayer.materialInUse;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			CalculateSIMRatioOfPlanetsInSystem(system, 0.0f);

			float tempRatio2 = tempRatio;

			if(tempRatio2 > tempRatio)
			{
				tempRatio = tempRatio2;
				planetToColonise = i;
			}
		}

		ColonisePlanet (system, planetToColonise);

		++turnInfoScript.systemsInPlay;
		
		cameraFunctionsScript.coloniseMenu = false;
	}

	public void CheckForSuitableSystem(AIBasicParent thisPlayer)
	{
		for(int i = 0; i < 60; ++i)
		{
			lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();
			
			if(systemListConstructor.systemList[i].systemOwnedBy == "Selkies" || systemListConstructor.systemList[i].systemOwnedBy == "Humans" || systemListConstructor.systemList[i].systemOwnedBy == "Nereides")
			{
				continue;
			}
			
			for(int j = 0; j < 4; ++j)
			{
				if(lineRenderScript.connections[j] == null)
				{
					continue;
				}

				int k = RefreshCurrentSystem(lineRenderScript.connections[j]);
				
				if(systemListConstructor.systemList[k].systemOwnedBy == thisPlayer.playerRace)
				{
					CalculateSIMRatio(i);
					CheckForSuitablePlanet (systemRatio, thisPlayer);
					RandomNumber(i);
					
					if(mostValuableSystem != null)
					{
						break;
					}
				}
			}
		}
	}

	void CalculateSIMRatio(int system)
	{
		tempSIM = 0.0f;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			float tempSci = systemListConstructor.systemList[system].planetScience[i];
			float tempInd = systemListConstructor.systemList[system].planetIndustry[i];
			float tempMon = systemListConstructor.systemList[system].planetMoney[i];
				
			tempSIM = (tempSci + tempInd + tempMon);
		}
		
		float tempRatio = tempSIM / guiPlanScript.numPlanets;
		
		systemRatio = (1.0f/16.7f) * (tempRatio/2.0f);
		
		if(systemRatio >= 1.0f)
		{
			systemRatio = 0.9f;
		}
	}

	public void CheckToImprovePlanet(int system)
	{
		for(int i = 0; i < 211; i++)
		{
			if(turnInfoScript.mostPowerfulPlanets[i,0] == systemListConstructor.systemList[system].systemName)
			{
				ImprovePlanet(int.Parse (turnInfoScript.mostPowerfulPlanets[i,1]), system);
			}
		}
	}

	public void CheckForSuitablePlanet(float ratio, AIBasicParent thisPlayer)
	{
		tempPlanet = -1; 
		tempSystem = -1;

		for(int i = 0; i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy != thisPlayer.playerRace)
			{
				continue;
			}

			CheckToImprovePlanet(i);
			CalculateSIMRatioOfPlanetsInSystem(i, ratio);
		}
		
		if(tempSystem > 0 && GP > 0)
		{
			ColonisePlanet(tempSystem, tempPlanet);
		}
	}

	private void CalculateSIMRatioOfPlanetsInSystem(int system, float ratio)
	{
		for(int j = 0; j < systemListConstructor.systemList[system].systemSize; ++j)
		{
			if(systemListConstructor.systemList[system].planetColonised[j] == false)
			{
				continue;
			}
			
			float tempSci = systemListConstructor.systemList[system].planetScience[j];
			float tempInd = systemListConstructor.systemList[system].planetIndustry[j];
			float tempMon = systemListConstructor.systemList[system].planetMoney[j];
			
			tempSIM = (tempSci + tempInd + tempMon);
			
			tempRatio = (1.0f/16.7f) * (tempSIM/2.0f);
			
			if(tempRatio > ratio)
			{
				tempPlanet = j;
				tempSystem = system;
			}
		}
	}

	private void RandomNumber(int system)
	{
		if(systemRatio < Random.Range (compensator, 1.00f) || compensator > 0.9f)
		{
			mostValuableSystem = systemListConstructor.systemList[system].systemObject;
		}
	}

	public void ColonisePlanet(int system, int planet)
	{
		systemListConstructor.systemList[system].planetColonised[planet] = true;

		++planetsColonisedThisTurn;

		--GP;
	}

	public void ImprovePlanet(int planetPosition, int system)
	{
		guiPlanScript.improvementNumber = systemListConstructor.systemList[system].planetImprovementLevel[planetPosition];

		guiPlanScript.CheckImprovement();

		if(guiPlanScript.canImprove == true)
		{
			if(industry >= guiPlanScript.improvementCost)
			{
				++systemListConstructor.systemList[system].planetImprovementLevel[planetPosition];

				industry -= (int)guiPlanScript.improvementCost;
			}

			else if(money >= guiPlanScript.improvementCost * 2)
			{
				++systemListConstructor.systemList[system].planetImprovementLevel[planetPosition];

				money -= ((int)guiPlanScript.improvementCost * 2);
			}
		}
	}
}

