using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public GameObject tempObject;
	public bool isOkToColonise, systemHasBeenColonised;

	void Start()
	{
		playerRace = null;
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0) && cameraFunctionsScript.selectedSystem != null) //Assigns scripts to selected system.
		{			
			tempObject = cameraFunctionsScript.selectedSystem;
			
			if(tempObject != null && tempObject.tag == "StarSystem")
			{
				systemSIMData = tempObject.GetComponent<SystemSIMData>();
				techTreeScript = tempObject.GetComponent<TechTreeScript>();
				heroScript = tempObject.GetComponent<HeroScriptParent>();
			}
		}
		
		cameraFunctionsScript.CentreCamera(); //Checks if camera needs centreing
	}

	public void FindSystem(int system) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{		
		lineRenderScript = systemListConstructor.systemList[system].systemObject.GetComponent<LineRenderScript>();
		
		for(int i = 0; i < systemListConstructor.systemList[system].numberOfConnections; ++i)
		{			
			int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);
			
			if(systemListConstructor.systemList[j].systemOwnedBy == playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}
			
			else
			{
				continue;
			}
		}
		
		if(isOkToColonise == true && capital > 10.0f)
		{
			systemListConstructor.systemList[system].systemOwnedBy = playerRace;

			lineRenderScript.SetRaceLineColour(playerRace);
			
			systemListConstructor.systemList[system].systemObject.renderer.material = materialInUse;
			
			capital -= 10.0f;
			
			++turnInfoScript.systemsInPlay;

			++systemsColonisedThisTurn;
			
			cameraFunctionsScript.coloniseMenu = false;
			
			isOkToColonise = false;

			systemHasBeenColonised = true;
		}
	}

	public void StartTurn()
	{
		PickRace ();

		cameraFunctionsScript.selectedSystem = GameObject.Find (homeSystem); //Set the selected system

		turnInfoScript.systemsInPlay++;
		
		int i = RefreshCurrentSystem(GameObject.Find(homeSystem));

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

		capital = 50.0f;
	}
}
