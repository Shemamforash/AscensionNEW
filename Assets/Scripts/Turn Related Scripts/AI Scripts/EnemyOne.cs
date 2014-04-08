using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{	
	public void RaceStart(string thisRace)
	{		
		isPlayer = false;

		playerRace = thisRace;

		PickRace ();

		turnInfoScript.systemsInPlay++;

		GameObject home = GameObject.Find (homeSystem);

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject == home)
			{
				systemListConstructor.systemList[i].systemOwnedBy = playerRace;

				systemListConstructor.systemList[i].systemObject.renderer.material = materialInUse;

				ambientStarRandomiser.AmbientColourChange(i);

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

		empireBoundaries.ModifyBoundaryCircles ();
	}
}

