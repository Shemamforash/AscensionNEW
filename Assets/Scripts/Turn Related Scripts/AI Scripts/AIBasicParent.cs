using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo 
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, systemRatio, compensator;
	private GameObject mostValuableSystem;
	private LineRenderScript thisLineRenderScript;
	
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
			compensator += 0.1f;

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

	public void CheckToImprovePlanet()
	{
		for(int i = 0; i < 211; i ++)
		{
			foreach(GameObject system in ownedSystems)
			{
				if(system == null)
				{
					continue;
				}

				if(turnInfoScript.mostPowerfulPlanets[i,0] == system.name)
				{
					ImprovePlanet(int.Parse (turnInfoScript.mostPowerfulPlanets[i,1]), system);
				}
			}
		}
	}

	public void ImprovePlanet(int planetPosition, GameObject thisSystem)
	{
		guiPlanScript = thisSystem.GetComponent<GUISystemDataScript>();

		guiPlanScript.improvementNumber = int.Parse (guiPlanScript.planNameOwnImprov[planetPosition, 2]);

		guiPlanScript.CheckImprovement();

		if(guiPlanScript.canImprove == true)
		{
			int q = int.Parse(guiPlanScript.planNameOwnImprov[planetPosition,2]);
			
			q++;

			if(industry >= guiPlanScript.improvementCost)
			{
				guiPlanScript.planNameOwnImprov[planetPosition,2] = (q).ToString ();

				industry -= (int)guiPlanScript.improvementCost;
			}

			else if(money >= guiPlanScript.improvementCost * 2)
			{
				guiPlanScript.planNameOwnImprov[planetPosition,2] = (q).ToString ();

				money -= ((int)guiPlanScript.improvementCost * 2);
			}
		}
	}

	public void CheckForSuitablePlanet(float ratio)
	{
		int tempPlanet = 0;
		GameObject tempSystem = null;

		foreach(GameObject system in ownedSystems)
		{
			if(system == null)
			{
				continue;
			}

			guiPlanScript = system.GetComponent<GUISystemDataScript>();

			for(int i = 0; i < guiPlanScript.numPlanets; i++)
			{
				for(int f = 0; f < 12; f++)
				{
					if(guiPlanScript.planNameOwnImprov[i, 0] == turnInfoScript.planetRIM[f, 0] && guiPlanScript.planNameOwnImprov[i, 1] == "No")
					{
						float tempSci = float.Parse (turnInfoScript.planetRIM[f,1]);
						float tempInd = float.Parse (turnInfoScript.planetRIM[f,2]);
						float tempMon = float.Parse (turnInfoScript.planetRIM[f,3]);
						
						tempSIM = (tempSci + tempInd + tempMon);

						float tempRatio = (1.0f/16.7f) * (tempSIM/2.0f);

						if(tempRatio > ratio)
						{
							tempPlanet = i;
							tempSystem = system;
						}
						
						break;
					}
				}
			}
		}

		if(tempSystem != null)
		{
			ColonisePlanet(tempSystem, tempPlanet);
		}
	}

	public void ColonisePlanet(GameObject system, int planet)
	{
		guiPlanScript = system.GetComponent<GUISystemDataScript>();

		GP -= 1;
		guiPlanScript.planNameOwnImprov[planet, 1] = "Yes";
		++planetsColonisedThisTurn;

	}

	public void CheckForSuitableSystem(AIBasicParent thisPlayer)
	{
		foreach(GameObject system in turnInfoScript.systemList)
		{
			lineRenderScript = system.GetComponent<LineRenderScript>();

			if(lineRenderScript.ownedBy == "Player" || lineRenderScript.ownedBy == "EnemyOne" || lineRenderScript.ownedBy == "EnemyTwo")
			{
				continue;
			}

			foreach(GameObject connection in lineRenderScript.connections)
			{
				if(connection == null)
				{
					continue;
				}

				thisLineRenderScript = connection.GetComponent<LineRenderScript>();

				if(thisLineRenderScript.ownedBy == thisPlayer.playerRace)
				{
					CalculateSIMRatio(system);
					CheckToImprovePlanet();
					CheckForSuitablePlanet (systemRatio);
					RandomNumber(system);

					if(mostValuableSystem != null)
					{
						break;
					}
				}
			}
		}
	}

	private void RandomNumber(GameObject thisSystem)
	{
		if(systemRatio < Random.Range (compensator, 1.00f) || compensator > 0.9f)
		{
			mostValuableSystem = thisSystem;
		}
	}

	void CalculateSIMRatio(GameObject thisSystem)
	{
		tempSIM = 0.0f;

		guiPlanScript = thisSystem.GetComponent<GUISystemDataScript>();

		for(int i = 0; i < guiPlanScript.numPlanets; i++)
		{
			for(int f = 0; f < 12; f++)
			{
				if(guiPlanScript.planNameOwnImprov[i, 0] == turnInfoScript.planetRIM[f, 0])
				{
					float tempSci = float.Parse (turnInfoScript.planetRIM[f,1]);
					float tempInd = float.Parse (turnInfoScript.planetRIM[f,2]);
					float tempMon = float.Parse (turnInfoScript.planetRIM[f,3]);

					tempSIM += (tempSci + tempInd + tempMon);

					break;
				}
			}
		}


		float tempRatio = tempSIM / guiPlanScript.numPlanets;

		systemRatio = (1.0f/16.7f) * (tempRatio/2.0f);

		if(systemRatio >= 1.0f)
		{
			systemRatio = 0.9f;
		}
	}
}
