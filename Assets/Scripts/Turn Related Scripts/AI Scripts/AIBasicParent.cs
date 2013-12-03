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
		while(GP > 0)
		{
			mostValuableSystem = null;
			compensator = 0.00f;
			SearchThroughArrays(thisPlayer);
		}

		GP += raceGP;

		//TurnEnd(ownedSystems);
	}

	public void SearchThroughArrays(AIBasicParent thisPlayer)
	{
		while(mostValuableSystem == null)
		{
			CheckForSuitableSystem(thisPlayer);
			compensator += 0.1f;

			if(mostValuableSystem == null || compensator > 1.0f)
			{
				break;
			}

			guiPlanScript = mostValuableSystem.GetComponent<GUISystemDataScript>();
			guiPlanScript.FindSystem(thisPlayer);
		}
	}

	public void CheckForSuitableSystem(AIBasicParent thisPlayer)
	{
		foreach(GameObject system in turnInfoScript.systemList)
		{
			if(system == null)
			{
				continue;
			}

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

				if(thisLineRenderScript.ownedBy == thisPlayer.name)
				{
					CalculateSIMRatio(system);
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
		if(systemRatio < Random.Range (compensator, 1.00f))
		{
			if(compensator == 1.0f)
			{
				mostValuableSystem = thisSystem;
			}

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
