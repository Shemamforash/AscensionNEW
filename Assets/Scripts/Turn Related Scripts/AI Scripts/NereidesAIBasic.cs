using UnityEngine;
using System.Collections;

public class NereidesAIBasic : AIBasicParent
{
	void Awake()
	{
		turnInfoScript = gameObject.GetComponent<TurnInfo>();
		nereidesTurnScript = gameObject.GetComponent<NereidesAIBasic>();

		materialInUse = nereidesMaterial;
		playerRace = "Nereid";
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");

		PickRace();

		nereidesHomeSystem = homeSystem;

		turnInfoScript.systemsInPlay++;

		GP = raceGP;

		lineRenderScript = GameObject.Find(nereidesHomeSystem).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = "Nereides";

		StartSystemPlanetColonise(nereidesMaterial, nereidesHomeSystem, ownedSystems);
	}
}

