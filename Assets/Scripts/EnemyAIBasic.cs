using UnityEngine;
using System.Collections;

public class EnemyAIBasic : TurnInfo 
{
	public string enemyHomeSystem;
	private EnemyAIBasic enemyTurnScript;
	private LineRenderScript thisLineRenderScript;

	void Awake()
	{
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

	void Update()
	{
		if(endTurn == true)
		{
			Expand ();
		}
	}

	public void Expand()
	{
		foreach(GameObject system in lineRenderScript.connections)
		{
			thisLineRenderScript = system.GetComponent<LineRenderScript>();

			if(thisLineRenderScript.ownedBy == "" && GP > 0)
			{
				guiPlanScript = system.GetComponent<GUISystemDataScript>();
				guiPlanScript.FindSystem(enemyTurnScript);
			}
		}
	}
}
