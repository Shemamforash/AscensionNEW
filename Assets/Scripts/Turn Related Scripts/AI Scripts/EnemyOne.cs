using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{	
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

		for(int i = 0;  i < 60; ++i)
		{
			if(masterScript.systemList[i].systemName == homeSystem)
			{
				masterScript.systemList[i].systemOwnedBy = playerRace;

				lineRenderScript = masterScript.systemList[i].systemObject.GetComponent<LineRenderScript>();

				lineRenderScript.SetRaceLineColour(playerRace);
			}
		}

		StartSystemPlanetColonise(materialInUse, homeSystem);
	}
}

