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
			RaceStart ("Nereides", nereidesHomeSystem, nereidesMaterial);
		}
		if(playerTurnScript.playerRace == "Nereides")
		{
			RaceStart ("Humans", humansHomeSystem, humansMaterial);
		}
	}
	
	void RaceStart(string thisRace, string thisHome, Material thisMaterial)
	{		
		playerRace = thisRace;
		PickRace ();
		materialInUse = thisMaterial;
		thisHome = homeSystem;

		turnInfoScript.systemsInPlay++;
		
		GP = raceGP;
		
		lineRenderScript = GameObject.Find(thisHome).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = "EnemyOne";

		StartSystemPlanetColonise(thisMaterial, thisHome, ownedSystems);
	}
}

