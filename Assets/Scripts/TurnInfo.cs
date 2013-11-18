using UnityEngine;
using System.Collections;
using System.IO;

public class TurnInfo : MonoBehaviour 
{
	[HideInInspector]
	public int arrayIterator, GP, raceGP;
	[HideInInspector]
	public float science, industry, money, raceScience, raceIndustry, raceMoney;
	[HideInInspector]
	public string[,] planetRIM = new string[12,4];
	[HideInInspector]
	public GameObject[] systemList = new GameObject[60];
	[HideInInspector]
	public GameObject[] ownedSystems = new GameObject[60];
	public Material ownedMaterial;
	public Camera mainCamera;
	
	private int turn = 0;
	private float timer = 0.0f;
	private string playerRace, homePlanet, cost;
	private bool moveCamera = false;
	private GameObject tempObject;
	private GUISystemDataScript guiPlanScript;
	private CameraFunctions cameraFunctionsScript;
	
	void Awake()
	{	
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");
		
		Time.timeScale = 0;
		
		LoadPlanetData();
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0) && cameraFunctionsScript.selectedSystem != null)
		{			
			GameObject tempObject = GameObject.Find (cameraFunctionsScript.selectedSystem);
			
			guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
		}

		CentreCamera();
	}

	void CentreCamera()
	{
		GameObject planetObject = GameObject.Find (cameraFunctionsScript.selectedSystem);

		if(Input.GetKeyDown("f"))
		{
			moveCamera = true;

			timer = Time.time;

			tempObject = planetObject;
		}

		if(moveCamera == true)
		{
			Vector3 homingPlanetPosition = new Vector3(tempObject.transform.position.x, tempObject.transform.position.y, -30.0f);
			
			Vector3 currentPosition = mainCamera.transform.position;

			if(mainCamera.transform.position == homingPlanetPosition || Time.time > timer + 1.0f)
			{
				homingPlanetPosition = mainCamera.transform.position;

				moveCamera = false;

				timer = 0.0f;
			}

			mainCamera.transform.position = Vector3.Lerp (currentPosition, homingPlanetPosition, 0.1f);
		}
	}
	
	void LoadPlanetData()
	{
		string text = " ";
		
		using(StreamReader reader =  new StreamReader("PlanetRIMData.txt"))
		{
			for(int i = 0; i < 12; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					text = reader.ReadLine();
					planetRIM[i,j] = text;
				}
			}			
		}
	}
	
	void OnGUI()
	{
		#region playerrace
		if(playerRace == null)
		{
			GUI.Box (new Rect(Screen.width/2 - 150, Screen.height/2 - 40, 300, 80), "Select Race");
			
			if(GUI.Button (new Rect(Screen.width/2 - 130, Screen.height/2, 80, 20), "Humans"))
			{
				playerRace = "Human";
				raceScience = 1;
				raceIndustry = 1;
				raceMoney = 2;
				raceGP = 3;
				homePlanet = "Sol";
			}
			if(GUI.Button (new Rect(Screen.width/2 -40, Screen.height/2, 80, 20), "Selkies"))
			{
				playerRace = "Selkie";
				raceScience = 1;
				raceIndustry = 3;
				raceMoney = 2;
				raceGP = 2;
				homePlanet = "Heracles";
			}
			if(GUI.Button (new Rect(Screen.width/2 + 50, Screen.height/2, 80, 20), "Nereides"))
			{
				playerRace = "Nereid";
				raceScience = 6;
				raceIndustry = 2;
				raceMoney = 4;
				raceGP = 1;
				homePlanet = "Nepthys";
			}
			
			for(int i = 0; i < 60; i++)
			{
				if(systemList[i].name == homePlanet)
				{
					ownedSystems[arrayIterator] = systemList[i];
					ownedSystems[arrayIterator].renderer.material = ownedMaterial;
					arrayIterator++;
					break;
				}
			}

			cameraFunctionsScript.selectedSystem = homePlanet;

			GP = raceGP;
			
			Time.timeScale = 1;
		}
		#endregion
		
		#region turninfo
		Rect bottomRight = new Rect(Screen.width - 80, Screen.height - 30, 70, 20);
		
		string turnNumber = "Turn: " + turn.ToString();
		
		GUI.Label (new Rect(Screen.width - 80, Screen.height - 50, 50, 20), turnNumber);
		
		if(GUI.Button (bottomRight, "End turn") && playerRace != null)
		{
			turn++;
			TurnEnd ();
		}
		
		GUI.Box (new Rect(15, 15, 100, 130), "");
		
		GUI.Label (new Rect(20, 20, 60, 20), playerRace);
		
		string scienceStr = science.ToString();
		
		GUI.Label (new Rect(20, 45, 60, 20), scienceStr);
		
		string industryStr = industry.ToString ();
		
		GUI.Label (new Rect(20, 70, 60, 20), industryStr);
		
		string moneyStr = money.ToString ();
		
		GUI.Label (new Rect(20, 95, 60, 20), moneyStr);
		
		string GPString = GP.ToString ();
		
		GUI.Label (new Rect(20, 120, 60, 20), GPString);
		#endregion
		
		#region colonisebutton
		Rect coloniseButton = new Rect(10, Screen.height - 40, 75, 30);
		
		if(cameraFunctionsScript.coloniseMenu == true)
		{
			if(GUI.Button (coloniseButton, "Colonise") && GP > 0)
			{			
				guiPlanScript.FindSystem ();
			}
		}
		#endregion

		if(cameraFunctionsScript.openMenu == true)
		{
			GUI.enabled = true;
			
			Rect fullScreenMenu = new Rect(0.5f, 0.5f, Screen.width, Screen.height);
			
			GUI.Box (fullScreenMenu, "Planets in System");
			
			if(guiPlanScript.foundPlanetData == false)
			{
				guiPlanScript.FindPlanetData();
				guiPlanScript.foundPlanetData = true;
			}

			for(int i = 0; i < guiPlanScript.numPlanets; i++)
			{
				guiPlanScript.improvementNumber = int.Parse (guiPlanScript.planNameOwnImprov[i,2]);
				
				guiPlanScript.CheckImprovement();

				cost = "Improve Cost: " + guiPlanScript.improvementCost;

				if(GUI.Button(guiPlanScript.allButtonsGUI[i], cost) && guiPlanScript.canImprove == true && industry >= guiPlanScript.improvementCost)
				{	
					int q = int.Parse(guiPlanScript.planNameOwnImprov[i,2]);

					q++;
						
					guiPlanScript.planNameOwnImprov[i,2] = (q).ToString ();

					industry -= guiPlanScript.improvementCost;
					
					guiPlanScript.FindPlanetData();
				}

				GUI.Label (guiPlanScript.allPlanetsGUI[i], guiPlanScript.allPlanetsInfo[i]);
			}	
		}
		
	}
	
	void TurnEnd()
	{			
		string selectedPlanet;
		
		foreach(GameObject system in ownedSystems)
		{
			if(system != null)
			{
				guiPlanScript = system.GetComponent<GUISystemDataScript>();
			
				for(int i = 0; i < 6; i++)
				{
					selectedPlanet = guiPlanScript.planNameOwnImprov[i,0];

					if(selectedPlanet == null)
					{
						break;
					}
					
					guiPlanScript.improvementNumber = int.Parse (guiPlanScript.planNameOwnImprov[i,2]);
					
					guiPlanScript.CheckImprovement();
				
					for(int j = 0; j < 12; j++)
					{
						if(selectedPlanet == planetRIM[j,0])
						{
							science += (float.Parse (planetRIM[j,1])) * raceScience * guiPlanScript.resourceBonus;
							industry += (float.Parse (planetRIM[j,2])) * raceIndustry * guiPlanScript.resourceBonus;
							money += (float.Parse (planetRIM[j,3])) * raceMoney * guiPlanScript.resourceBonus;
						}
					}
				}
			}
		}
		
		GP += raceGP;
	}
}
