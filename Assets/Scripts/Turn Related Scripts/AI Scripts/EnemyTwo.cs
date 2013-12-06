using UnityEngine;
using System.Collections;

public class EnemyTwo : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		enemyTwoTurnScript = gameObject.GetComponent<EnemyTwo>();
		playerTurnScript = gameObject.GetComponent<PlayerTurn>();
	}

	public void SetRace()
	{
		if(playerTurnScript.playerRace == "Humans" || playerTurnScript.playerRace == "Nereides")
		{
			RaceStart ("Selkies");
		}
		if(playerTurnScript.playerRace == "Selkies")
		{
			RaceStart ("Humans");
		}
	}
	
	void RaceStart(string thisRace)
	{		
		playerRace = thisRace;

		PickRace ();

		turnInfoScript.systemsInPlay++;
		
		GP = raceGP;
		
		lineRenderScript = GameObject.Find(homeSystem).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = playerRace;
		
		StartSystemPlanetColonise(materialInUse, homeSystem, ownedSystems);
	}
}