using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo 
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, systemRatio, compensator;
	private GameObject mostValuableSystem;
	
	public void Expand(AIBasicParent thisPlayer)
	{
		planetsColonisedThisTurn = 0;

		for(int i = GP; i > 0; --i)
		{
			mostValuableSystem = null;
			compensator = 0.00f;
			SearchThroughArrays(thisPlayer);
		}

		TurnEnd(ownedSystems);
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

			if(mostValuableSystem != null)
			{
				guiPlanScript = mostValuableSystem.GetComponent<GUISystemDataScript>();
				guiPlanScript.FindSystem(thisPlayer);
			}
		}
	}

	public void CheckForSuitableSystem(AIBasicParent thisPlayer)
	{
		for(int i = 0; i < 60; ++i)
		{
			lineRenderScript = masterScript.systemList[i].systemObject.GetComponent<LineRenderScript>();
			
			if(masterScript.systemList[i].systemOwnedBy == "Selkies" || masterScript.systemList[i].systemOwnedBy == "Humans" || masterScript.systemList[i].systemOwnedBy == "Nereides")
			{
				continue;
			}
			
			foreach(GameObject connection in lineRenderScript.connections)
			{
				if(connection == null)
				{
					continue;
				}
				
				if(masterScript.systemList[i].systemOwnedBy == thisPlayer.playerRace)
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
		
		guiPlanScript = thisSystem.GetComponent<GUISystemDataScript>();
		
		for(int i = 0; i < masterScript.systemList[system].systemSize; ++i)
		{
			float tempSci = float.Parse (masterScript.systemList[system].planetScience[i]);
			float tempInd = float.Parse (masterScript.systemList[system].planetIndustry[i]);
			float tempMon = float.Parse (masterScript.systemList[system].planetMoney[i]);
				
			tempSIM = (tempSci + tempInd + tempMon);
			
			break;
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
			if(turnInfoScript.mostPowerfulPlanets[i,0] == masterScript.systemList[system].systemName)
			{
				ImprovePlanet(int.Parse (turnInfoScript.mostPowerfulPlanets[i,1]), system);
			}
		}
	}

	public void CheckForSuitablePlanet(float ratio, AIBasicParent thisPlayer)
	{
		int tempPlanet = 0, tempSystem = 0;
		
		for(int i = 0; i < 60; ++i)
		{
			if(masterScript.systemList[i].systemOwnedBy != thisPlayer)
			{
				continue;
			}

			CheckToImprovePlanet(i);
			
			for(int j = 0; j < masterScript.systemList[j].systemSize; ++j)
			{
				float tempSci = float.Parse (masterScript.systemList[i].planetScience[j]);
				float tempInd = float.Parse (masterScript.systemList[i].planetIndustry[j]);
				float tempMon = float.Parse (masterScript.systemList[i].planetMoney[j]);
						
				tempSIM = (tempSci + tempInd + tempMon);
						
				float tempRatio = (1.0f/16.7f) * (tempSIM/2.0f);
						
				if(tempRatio > ratio)
				{
					tempPlanet = j;
					tempSystem = i;
				}
						
				break;
			}
		}
		
		if(tempSystem != null && GP > 0)
		{
			ColonisePlanet(tempSystem, tempPlanet, thisPlayer);
		}
	}

	private void RandomNumber(int system)
	{
		if(systemRatio < Random.Range (compensator, 1.00f) || compensator > 0.9f)
		{
			mostValuableSystem = masterScript.systemList[system].systemObject;
		}
	}

	public void ColonisePlanet(int system, int planet, AIBasicParent thisPlayer)
	{
		
		GP -= 1;
		
		masterScript.systemList[system].systemOwnedBy = thisPlayer;
		
		++planetsColonisedThisTurn;
		
	}

	public void ImprovePlanet(int planetPosition, int system)
	{
		guiPlanScript.improvementNumber = masterScript.systemList[system].planetImprovementLevel[planetPosition];

		guiPlanScript.CheckImprovement();

		if(guiPlanScript.canImprove == true)
		{
			if(industry >= guiPlanScript.improvementCost)
			{
				++masterScript.systemList[system].planetImprovementLevel[planetPosition];

				industry -= (int)guiPlanScript.improvementCost;
			}

			else if(money >= guiPlanScript.improvementCost * 2)
			{
				++masterScript.systemList[system].planetImprovementLevel[planetPosition];

				money -= ((int)guiPlanScript.improvementCost * 2);
			}
		}
	}
}

