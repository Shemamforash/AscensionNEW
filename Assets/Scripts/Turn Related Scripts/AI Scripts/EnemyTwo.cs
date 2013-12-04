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
			RaceStart ("Selkies", selkiesHomeSystem, selkiesMaterial);
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
		lineRenderScript.ownedBy = "EnemyTwo";
		
		StartSystemPlanetColonise(thisMaterial, thisHome, ownedSystems);
	}
}