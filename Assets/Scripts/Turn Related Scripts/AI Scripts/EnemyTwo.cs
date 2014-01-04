﻿using UnityEngine;
using System.Collections;

public class EnemyTwo : AIBasicParent
{
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
		
		for(int i = 0;  i < 60; ++i)
		{
			if(masterScript.systemList[i].systemName == homeSystem)
			{
				masterScript.systemList[i].systemOwnedBy = playerRace;
			}
		}
		
		StartSystemPlanetColonise(materialInUse, homeSystem, ownedSystems);
	}
}