using UnityEngine;
using System.Collections;

public class EnemyAIBasic : TurnInfo 
{
	public string enemyHomeSystem;
	private float tempSIM, systemRatio;
	private GameObject mostValuableSystem;
	private EnemyAIBasic enemyTurnScript;
	private LineRenderScript thisLineRenderScript;
	private TurnInfo turnInfoScript;

	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		enemyTurnScript = gameObject.GetComponent<EnemyAIBasic>();

		materialInUse = enemyMaterial;
		playerRace = "Selkie";
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");

		PickRace();

		enemyHomeSystem = homeSystem;

		GP = raceGP;

		lineRenderScript = GameObject.Find(enemyHomeSystem).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = "Enemy";

		StartSystemPlanetColonise(enemyMaterial, enemyHomeSystem, ownedSystems);
	}

	public void Expand()
	{
		while(GP > 0 && turnInfoScript.systemsInPlay < 60)
		{
			mostValuableSystem = null;

			if(turnInfoScript.systemsInPlay == 55)
			{
				foreach(GameObject system in systemList)
				{
					if(system == null)
					{
						continue;
					}
					Debug.Log (system.name);
				}
			}

			while(mostValuableSystem == null)
			{
				CheckForSuitableSystem();

			}

			guiPlanScript = mostValuableSystem.GetComponent<GUISystemDataScript>();
			guiPlanScript.FindSystem(enemyTurnScript);
		}

		GP += raceGP;
	}

	public void CheckForSuitableSystem()
	{
		foreach(GameObject system in systemList)
		{
			if(system == null)
			{
				continue;
			}

			lineRenderScript = system.GetComponent<LineRenderScript>();

			if(lineRenderScript.ownedBy == "Enemy" || lineRenderScript.ownedBy == "Player")
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

				if(thisLineRenderScript.ownedBy == "Enemy")
				{
					CalculateSIMRatio(system);
					RandomNumber(system);
				}
			}
		}
	}

	private void RandomNumber(GameObject thisSystem)
	{
		if(systemRatio < Random.Range (0.00f, 1.00f))
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
	}
}
