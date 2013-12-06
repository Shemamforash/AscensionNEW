using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		enemyOneTurnScript = gameObject.GetComponent<EnemyOne>();
		playerTurnScript = gameObject.GetComponent<PlayerTurn>();
	}
	
	public void SetRace()
	{
		if(playerTurnScript.playerRace == "Humans" || playerTurnScript.playerRace == "Selkies")
		{
			RaceStart ("Nereides");
		}
		if(playerTurnScript.playerRace == "Nereides")
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

