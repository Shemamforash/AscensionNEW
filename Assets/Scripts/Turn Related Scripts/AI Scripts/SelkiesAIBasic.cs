using UnityEngine;
using System.Collections;

public class SelkiesAIBasic : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		selkiesTurnScript = gameObject.GetComponent<SelkiesAIBasic>();

		materialInUse = selkiesMaterial;
		playerRace = "Selkie";
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");

		PickRace();

		selkiesHomeSystem = homeSystem;

		turnInfoScript.systemsInPlay++;

		GP = raceGP;

		lineRenderScript = GameObject.Find(selkiesHomeSystem).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = "Selkies";

		StartSystemPlanetColonise(selkiesMaterial, selkiesHomeSystem, ownedSystems);
	}
}