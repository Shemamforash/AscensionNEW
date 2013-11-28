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

	public void Expand(GameObject activeSystem)
	{
		lineRenderScript = activeSystem.GetComponent<LineRenderScript>();

		bool selectPlanet = false;

		int noOfSystemsIteratedThrough = 0;

		foreach(GameObject system in lineRenderScript.connections)
		{
			if(system == null)
			{
				break;
			}

			thisLineRenderScript = system.GetComponent<LineRenderScript>();

			if(thisLineRenderScript.ownedBy == "Enemy" || thisLineRenderScript.ownedBy == "Player")
			{
				continue;
			}

			if(thisLineRenderScript.ownedBy == "" && GP > 0)
			{
				guiPlanScript = system.GetComponent<GUISystemDataScript>(); //Wtf is happening here!?
				guiPlanScript.FindSystem(enemyTurnScript);
				guiPlanScript = activeSystem.GetComponent<GUISystemDataScript>();
				++noOfSystemsIteratedThrough;
			}
		}

		if(noOfSystemsIteratedThrough == 0)
		{
			selectPlanet = true;
		}

		if(selectPlanet == true)
		{
			foreach(GameObject system in lineRenderScript.connections)
			{
				if(system != null)
				{
					CalculateSIMRatio(system);
				}
			}

			Expand (mostValuableSystem);
		}

		TurnEnd(ownedSystems);
	}

	void TestConnections(GameObject system)
	{
		lineRenderScript = system.GetComponent<LineRenderScript>();

		foreach(GameObject connection in lineRenderScript.connections)
		{
			if(connection == null)
			{
				break;
			}

			thisLineRenderScript = connection.GetComponent<LineRenderScript>();

			if(thisLineRenderScript.ownedBy != null || thisLineRenderScript.ownedBy != "")
			{

			}
		}
	}

	void CalculateSIMRatio(GameObject thisSystem)
	{
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

		if(tempRatio > systemRatio)
		{
			systemRatio = tempRatio;
			mostValuableSystem = thisSystem;
		}
	}
}
