using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public string playerHomeSystem;

	void Awake()
	{
		playerRace = null;
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		materialInUse = playerMaterial;
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0) && cameraFunctionsScript.selectedSystem != null) //Assigns scripts to selected system.
		{			
			tempObject = GameObject.Find (cameraFunctionsScript.selectedSystem);
			
			if(tempObject.tag == "StarSystem")
			{
				guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
				techTreeScript = tempObject.GetComponent<TechTreeScript>();
				heroScript = tempObject.GetComponent<HeroScript>();
			}
		}
		
		cameraFunctionsScript.CentreCamera(); //Checks if camera needs centreing
	}

	public void StartTurn()
	{
		PickRace ();

		playerHomeSystem = homeSystem;

		Debug.Log (playerHomeSystem);
		
		StartSystemPlanetColonise(playerMaterial, playerHomeSystem);

		cameraFunctionsScript.selectedSystem = playerHomeSystem; //Set the selected system
	}
}
