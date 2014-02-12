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

		for(int i = 0;  i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject== GameObject.Find (homeSystem))
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

