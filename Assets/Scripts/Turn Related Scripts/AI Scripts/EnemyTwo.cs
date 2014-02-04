using UnityEngine;
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
		
		capital = 50;
		
		for(int i = 0;  i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemName == homeSystem)
			{
				systemListConstructor.systemList[i].systemOwnedBy = playerRace;

				systemListConstructor.systemList[i].systemObject.renderer.material = materialInUse;

				lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();
				
				lineRenderScript.SetRaceLineColour(playerRace);
				
				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetType == homePlanetType)
					{
						systemListConstructor.systemList[i].planetsInSystem[j].planetColonised = true;
						break;
					}
				}
				
				break;
			}
		}
	}
}