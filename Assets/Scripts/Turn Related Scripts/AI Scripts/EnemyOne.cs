using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		enemyOneTurnScript = gameObject.GetComponent<EnemyOne>();
	}
	
	public void SetRace()
	{
		Debug.Log(playerTurnScript.playerRace);

		if(playerTurnScript.playerRace == "Humans" || playerTurnScript.playerRace == "Selkies")
		{
			NereidesStart ();
		}
		if(playerTurnScript.playerRace == "Nereides")
		{
			HumansStart ();
		}
	}
	
	void NereidesStart()
	{
		playerRace = "Nereides";
		materialInUse = nereidesMaterial;
		nereidesHomeSystem = homeSystem;
		RaceStart (playerRace, nereidesHomeSystem, materialInUse);
	}
	
	void HumansStart()
	{
		playerRace = "Humans";
		materialInUse = humansMaterial;
		humansHomeSystem = homeSystem;
		RaceStart (playerRace, humansHomeSystem, materialInUse);
	}
	
	void RaceStart(string thisRace, string thisHome, Material thisMaterial)
	{
		Debug.Log (thisRace + thisHome);

		PickRace();
		
		turnInfoScript.systemsInPlay++;
		
		GP = raceGP;
		
		lineRenderScript = GameObject.Find(thisHome).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = thisRace;
		
		StartSystemPlanetColonise(thisMaterial, thisHome, ownedSystems);
	}
}

