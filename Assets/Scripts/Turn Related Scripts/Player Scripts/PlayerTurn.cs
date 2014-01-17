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
				guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
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
			int j = RefreshCurrentSystem(systemListConstructor.systemList[system].connectedSystems[i]);
			
			if(systemListConstructor.systemList[j].systemOwnedBy == playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}
			
			else
			{
				continue;
			}
		}
		
		if(isOkToColonise == true && GP > 0)
		{
			systemListConstructor.systemList[system].systemOwnedBy = playerRace;

			Debug.Log (systemListConstructor.systemList[system].systemName);
			
			lineRenderScript.SetRaceLineColour(playerRace);
			
			systemListConstructor.systemList[system].systemObject.renderer.material = materialInUse;
			
			--GP;
			
			++turnInfoScript.systemsInPlay;
			
			cameraFunctionsScript.coloniseMenu = false;
			
			isOkToColonise = false;

			systemHasBeenColonised = true;
		}
	}

	public void OnGUI()
	{
		GUI.skin = mainGUIScript.mySkin;

		if(systemHasBeenColonised == true)
		{
			int selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 185.0f, Screen.height / 2 - 60.0f, 120.0f, 370.0f));

			GUILayout.Box("Select Planet");

			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
			{
				float planetSIM = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetScience + systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetIndustry +
					systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetMoney;

				string planetInfo = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType + " " + planetSIM.ToString() + " SIM";

				if(GUILayout.Button(planetInfo, GUILayout.Height (50.0f)))
				{
					systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised = true;

					++playerTurnScript.planetsColonisedThisTurn;

					++playerTurnScript.systemsColonisedThisTurn;

					systemHasBeenColonised = false;
				}
			}

			GUILayout.EndArea();
		}
	}

	public void ImproveButtonClick(int i, int j)
	{
		++systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel;

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

		GP = raceGP;
	}
}
