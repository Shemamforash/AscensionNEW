using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public GameObject tempObject;
	

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
				guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
				techTreeScript = tempObject.GetComponent<TechTreeScript>();
				heroScript = tempObject.GetComponent<HeroScriptParent>();
			}
		}
		
		cameraFunctionsScript.CentreCamera(); //Checks if camera needs centreing
	}

	public void ImproveButtonClick(int i, int j)
	{
		++masterScript.systemList[i].planetImprovementLevel[j];

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

		turnInfoScript.systemsInPlay++;
		
		int i = masterScript.RefreshCurrentSystem(GameObject.Find(homeSystem));

		masterScript.systemList[i].systemOwnedBy = playerRace;

		lineRenderScript = masterScript.systemList[i].systemObject.GetComponent<LineRenderScript>();

		lineRenderScript.SetRaceLineColour(playerRace);

		StartSystemPlanetColonise(materialInUse, homeSystem);

		GP = raceGP;

		cameraFunctionsScript.selectedSystem = GameObject.Find (homeSystem); //Set the selected system

		foreach(GameObject system in ownedSystems)
		{
			if(system == null)
			{
				continue;
			}
		}
	}
}
