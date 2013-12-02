using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public string playerHomeSystem;
	public GameObject tempObject;
	private MainGUIScript mainGUIScript;

	void Awake()
	{
		playerRace = null;
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();
		turnInfoScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<TurnInfo>();
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

	public void ImproveButtonClick(int i)
	{
		int q = int.Parse(guiPlanScript.planNameOwnImprov[i,2]);
		
		q++;
		
		guiPlanScript.planNameOwnImprov[i,2] = (q).ToString ();
		
		if(mainGUIScript.resourceToSpend == "Industry")
		{
			industry -= (int)guiPlanScript.improvementCost;
		}
		
		if(mainGUIScript.resourceToSpend == "Money")
		{
			money -= (int)(guiPlanScript.improvementCost * 2);
		}
	}

	public void StartTurn()
	{
		PickRace ();

		playerHomeSystem = homeSystem;

		turnInfoScript.systemsInPlay++;

		lineRenderScript = GameObject.Find(playerHomeSystem).GetComponent<LineRenderScript>();
		lineRenderScript.ownedBy = "Player";

		StartSystemPlanetColonise(playerMaterial, playerHomeSystem, ownedSystems);

		GP = raceGP;

		cameraFunctionsScript.selectedSystem = playerHomeSystem; //Set the selected system
	}
}
