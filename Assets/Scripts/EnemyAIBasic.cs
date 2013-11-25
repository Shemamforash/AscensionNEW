using UnityEngine;
using System.Collections;

public class EnemyAIBasic : TurnInfo 
{
	public string enemyHomeSystem;

	void Awake()
	{
		materialInUse = enemyMaterial;
		playerRace = "Selkie";
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");
		PickRace();

		enemyHomeSystem = homeSystem;

		StartSystemPlanetColonise(enemyMaterial, enemyHomeSystem);
	}
}
