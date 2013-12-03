using UnityEngine;
using System.Collections;

public class EnemyTwo : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		enemyTwoTurnScript = gameObject.GetComponent<EnemyTwo>();
	}

	public void SetRace()
	{
		if(playerTurnScript.playerRace == "Humans" || playerTurnScript.playerRace == "Nereides")
		{
			SelkiesStart ();
		}

		if(playerTurnScript.playerRace == "Selkies")
		{
			HumansStart ();
		}
	}

	void SelkiesStart()
	{
		playerRace = "Selkies";
		materialInUse = selkiesMaterial;
		selkiesHomeSystem = homeSystem;
		RaceStart (playerRace, selkiesHomeSystem, materialInUse);
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
		PickRace();
		
		turnInfoScript.systemsInPlay++;
		
		GP = raceGP;
		
		lineRenderScript = GameObject.Find(thisHome).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = thisRace;
		
		StartSystemPlanetColonise(thisMaterial, thisHome, ownedSystems);
	}
}