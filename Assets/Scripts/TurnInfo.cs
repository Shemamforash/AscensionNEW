using UnityEngine;
using System.Collections;
using System.IO;

public class TurnInfo : MonoBehaviour 
{
	[HideInInspector]
	public int GP, raceGP;
	[HideInInspector]
	public int science, industry, money;
	public float raceScience, raceIndustry, raceMoney;
	[HideInInspector]
	public string[,] planetRIM = new string[12,4];
	[HideInInspector]
	public GameObject[] systemList = new GameObject[60];
	[HideInInspector]
	public GameObject[] ownedSystems = new GameObject[60];
	[HideInInspector]
	public Rect[] allPlanetsGUI, allButtonsGUI; 
	public Rect[,] tierButtonsGUI;
	public Material ownedMaterial;
	public Camera mainCamera;
	public bool endTurn;
	public string homePlanet;

	private bool spendMenu = false;
	private string resourceToSpend;
	private int turn = 0;
	private float timer = 0.0f;
	private string playerRace, cost;
	private bool moveCamera = false;
	private GameObject tempObject;
	private GUISystemDataScript guiPlanScript;
	private CameraFunctions cameraFunctionsScript;
	private TechTreeScript techTreeScript;
	
	void Awake()
	{	
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		
		systemList = GameObject.FindGameObjectsWithTag("StarSystem");
		
		Time.timeScale = 0;
		
		LoadPlanetData();

		GUIRectBuilder();

		GUITechTreeRectBuilder();
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0) && cameraFunctionsScript.selectedSystem != null)
		{			
			GameObject tempObject = GameObject.Find (cameraFunctionsScript.selectedSystem);

			if(tempObject.tag == "StarSystem")
			{
				guiPlanScript = tempObject.GetComponent<GUISystemDataScript>();
				techTreeScript = tempObject.GetComponent<TechTreeScript>();
			}
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

	private void GUIRectBuilder()
	{
		Rect topLeft = new Rect(Screen.width/2 - 500.0f, Screen.height / 2 - 225.0f, 200.0f, 100.0f); //Top left
		
		Rect buttonTopLeft = new Rect(Screen.width/2 - 500.0f, Screen.height / 2 - 125.0f, 200.0f, 50.0f);
		
		Rect topRight = new Rect(Screen.width/2 - 250.0f, Screen.height / 2 - 225.0f, 200.0f, 100.0f); //Top right
		
		Rect buttonTopRight = new Rect(Screen.width/2 - 250.0f, Screen.height / 2 - 125.0f, 200.0f, 50.0f);

		
		Rect middleLeft = new Rect (Screen.width/2 - 500.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Middle left
		
		Rect buttonMiddleLeft = new Rect(Screen.width/2 - 500.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);
		
		Rect middleRight = new Rect(Screen.width/2 - 250.0f, Screen.height / 2 - 50.0f, 200.0f, 100.0f); //Middle right
		
		Rect buttonMiddleRight = new Rect(Screen.width/2 - 250.0f, Screen.height / 2 + 50.0f, 200.0f, 50.0f);

		
		Rect bottomLeft = new Rect(Screen.width/2 -500.0f, Screen.height / 2 + 125.0f, 200.0f, 100.0f); //Bottom left
		
		Rect buttonBottomLeft = new Rect(Screen.width/2 - 500.0f, Screen.height / 2 + 225.0f, 200.0f, 50.0f);
		
		Rect bottomRight = new Rect(Screen.width/2 -250.0f, Screen.height / 2 + 125.0f, 200.0f, 100.0f); //Bottom right
		
		Rect buttonBottomRight = new Rect(Screen.width/2 -250.0f, Screen.height / 2 + 225.0f, 200.0f, 50.0f);

		
		allPlanetsGUI = new Rect[6] {topLeft, topRight, middleLeft, middleRight, bottomLeft, bottomRight};
		
		allButtonsGUI = new Rect[6] {buttonTopLeft, buttonTopRight, buttonMiddleLeft, buttonMiddleRight, buttonBottomLeft, buttonBottomRight};	
	}

	private void GUITechTreeRectBuilder()
	{
		Rect top1 = new Rect(Screen.width / 2 + 50.0f, Screen.height/2 - 225.0f, 150.0f, 100.0f);
		Rect top2 = new Rect(Screen.width / 2 + 205.0f, Screen.height/2 - 225.0f, 150.0f, 100.0f);
		Rect top3 = new Rect(Screen.width / 2 + 360.0f, Screen.height/2 - 225.0f, 150.0f, 100.0f);
		Rect top4 = new Rect(Screen.width / 2 + 515.0f, Screen.height/2 - 225.0f, 150.0f, 100.0f);
		Rect top5 = new Rect(Screen.width / 2 + 670.0f, Screen.height/2 - 225.0f, 150.0f, 100.0f);
		Rect ignore1 =  new Rect(100.0f, 100.0f, 100.0f, 100.0f);

		Rect middleTop1 = new Rect(Screen.width / 2 + 50.0f, Screen.height/2 - 100.0f, 150.0f, 100.0f);
		Rect middleTop2 = new Rect(Screen.width / 2 + 205.0f, Screen.height/2 - 100.0f, 150.0f, 100.0f);
		Rect middleTop3 = new Rect(Screen.width / 2 + 360.0f, Screen.height/2 - 100.0f, 150.0f, 100.0f);
		Rect middleTop4 = new Rect(Screen.width / 2 + 515.0f, Screen.height/2 - 100.0f, 150.0f, 100.0f);
		Rect middleTop5 = new Rect(Screen.width / 2 + 670.0f, Screen.height/2 - 100.0f, 150.0f, 100.0f);
		Rect ignore2 =  new Rect(100.0f, 100.0f, 100.0f, 100.0f);

		Rect middleBottom1 = new Rect(Screen.width / 2 + 50.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);
		Rect middleBottom2 = new Rect(Screen.width / 2 + 205.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);
		Rect middleBottom3 = new Rect(Screen.width / 2 + 360.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);
		Rect middleBottom4 = new Rect(Screen.width / 2 + 515.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);
		Rect middleBottom5 = new Rect(Screen.width / 2 + 670.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);
		Rect middleBottom6 = new Rect(Screen.width / 2 + 825.0f, Screen.height/2 + 25.0f, 150.0f, 100.0f);

		Rect bottom1 = new Rect(Screen.width / 2 + 50.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);
		Rect bottom2 = new Rect(Screen.width / 2 + 205.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);
		Rect bottom3 = new Rect(Screen.width / 2 + 360.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);
		Rect bottom4 = new Rect(Screen.width / 2 + 515.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);
		Rect bottom5 = new Rect(Screen.width / 2 + 670.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);
		Rect bottom6 = new Rect(Screen.width / 2 + 825.0f, Screen.height/2 + 150.0f, 150.0f, 100.0f);

		tierButtonsGUI = new Rect[4,6] {{top1, top2, top3, top4, top5, ignore1},{middleTop1, middleTop2, middleTop3, middleTop4, middleTop5, ignore2},
			{middleBottom1, middleBottom2, middleBottom3, middleBottom4, middleBottom5, middleBottom6},{bottom1, bottom2, bottom3, bottom4, bottom5, bottom6}};
	}

	private void PickRace()
	{
		if(playerRace == "Human")
		{
			raceScience = 1;
			raceIndustry = 1;
			raceMoney = 2;
			raceGP = 3;
			homePlanet = "Sol";
		}
		if(playerRace == "Selkie")
		{
			raceScience = 1;
			raceIndustry = 3;
			raceMoney = 2;
			raceGP = 2;
			homePlanet = "Heracles";
		}
		if(playerRace == "Nereid")
		{
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
				ownedSystems[i] = systemList[i];
				ownedSystems[i].renderer.material = ownedMaterial;
				break;
			}
		}
		
		cameraFunctionsScript.selectedSystem = homePlanet;
		
		GP = raceGP;
		
		Time.timeScale = 1;
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
			}

			if(GUI.Button (new Rect(Screen.width/2 -40, Screen.height/2, 80, 20), "Selkies"))
			{
				playerRace = "Selkie";
			}

			if(GUI.Button (new Rect(Screen.width/2 + 50, Screen.height/2, 80, 20), "Nereides"))
			{
				playerRace = "Nereid";
			}

			PickRace ();
		}
		#endregion
		
		#region turninfo		
		string turnNumber = "Turn: " + turn.ToString();
		
		GUI.Label (new Rect(Screen.width - 80, Screen.height - 50, 50, 20), turnNumber);
		
		if(GUI.Button (new Rect(Screen.width - 80, Screen.height - 30, 70, 20), "End turn") && playerRace != null)
		{
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

		#region planetinfomenu
		if(cameraFunctionsScript.openMenu == true)
		{
			#region planetdata
			GUI.enabled = true;
			
			Rect fullScreenMenu = new Rect(0.5f, 0.5f, Screen.width, Screen.height);
			
			GUI.Box (fullScreenMenu, "Planets in System");
			
			if(guiPlanScript.foundPlanetData == false)
			{
				guiPlanScript.SystemSIMCounter();
				guiPlanScript.foundPlanetData = true;
			}

			Rect dataSIM = new Rect (Screen.width/2 - 500.0f, Screen.height/2 - 350.0f, 100.0f, 100.0f);

			string dataSIMString = "Total SIM: " + guiPlanScript.totalSystemSIM.ToString() + "\n" + "Total Science: " + guiPlanScript.totalSystemScience.ToString() + "\n" 
				+ "Total Industry: " + guiPlanScript.totalSystemIndustry.ToString() + "\n" + "Total Money: " + guiPlanScript.totalSystemMoney.ToString(); 

			GUI.Label(dataSIM, dataSIMString);

			for(int i = 0; i < guiPlanScript.numPlanets; i++) //This sections of the function evaluates the improvement level of each system, and improves it if the button is clicked
			{	
				guiPlanScript.improvementNumber = int.Parse (guiPlanScript.planNameOwnImprov[i,2]);
				
				guiPlanScript.CheckImprovement();

				if(guiPlanScript.canImprove == false)
				{
					cost = "Max Improvement";
				}

				if(guiPlanScript.canImprove == true)
				{
					cost = "Improve Cost: " + guiPlanScript.improvementCost;
				}

				if(GUI.Button(allButtonsGUI[i], cost) && guiPlanScript.canImprove == true )
				{	
					spendMenu = true;
				}

				string indSpend = guiPlanScript.improvementCost + " Industry";

				string monSpend = guiPlanScript.improvementCost * 2 + " Money";

				if(spendMenu == true)
				{
					GUI.Box (new Rect(Screen.width/2 - 100.0f, Screen.height/2 - 50.0f, 200.0f, 75.0f), "Resource to Spend:");			

					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 92.5f, 35.0f), indSpend) && industry >= guiPlanScript.improvementCost)
					{
						resourceToSpend = "Industry";
						ImproveButtonClick(i);
						spendMenu = false;
					}

					if(GUI.Button (new Rect(Screen.width/2 + 2.5f, Screen.height/2 - 15.0f, 92.5f, 35.0f), monSpend) && money >= guiPlanScript.improvementCost * 2)
					{
						resourceToSpend = "Money";
						ImproveButtonClick(i);
						spendMenu = false;
					}
				}

				GUI.Label (allPlanetsGUI[i], guiPlanScript.allPlanetsInfo[i]);
			}
			#endregion

			#region techtreedata

			string techBuildButtonText;

			for(int f = 0; f <= techTreeScript.techTier; ++f)
			{
				for(int i = 0; i < 6; ++i)
				{
					if((techTreeScript.techTreeComplete[f,i,0] == null) || (f == 0 && i == 5) || (f == 1 && i == 5))
					{
						break;
					}

					techBuildButtonText = techTreeScript.techTreeComplete[f,i,0].ToString () + "\n" + techTreeScript.techTreeComplete[f,i,1].ToString();

					if(GUI.Button (tierButtonsGUI[f,i], techBuildButtonText))
					{
						techTreeScript.techToBuildPosition = i;
						techTreeScript.techToBuildTier = f;
						techTreeScript.ImproveSystem();
					}
				}
			}

			#endregion
		}
		#endregion
	}

	void ImproveButtonClick(int i)
	{
		int q = int.Parse(guiPlanScript.planNameOwnImprov[i,2]);
		
		q++;
		
		guiPlanScript.planNameOwnImprov[i,2] = (q).ToString ();

		if(resourceToSpend == "Industry")
		{
			industry -= (int)guiPlanScript.improvementCost;
		}

		if(resourceToSpend == "Money")
		{
			money -= (int)(guiPlanScript.improvementCost * 2);
		}
		
		guiPlanScript.SystemSIMCounter();
	}
	
	void TurnEnd() //This function accumulates all the SIM generated by each system to give an empire SIM value
	{			
		endTurn = true;
		
		foreach(GameObject system in ownedSystems)
		{
			if(system != null)
			{
				guiPlanScript = system.GetComponent<GUISystemDataScript>();
				techTreeScript = system.GetComponent<TechTreeScript>();

				guiPlanScript.SystemSIMCounter();
				techTreeScript.ActiveTechnologies();

				science += (int)guiPlanScript.totalSystemScience;
				industry += (int)guiPlanScript.totalSystemIndustry;
				money += (int)guiPlanScript.totalSystemMoney;
			}
		}
		
		GP += raceGP;
		++turn;
		endTurn = false;
	}
}
