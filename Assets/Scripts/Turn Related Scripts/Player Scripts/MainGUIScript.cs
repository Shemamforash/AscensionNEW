using UnityEngine;
using System.Collections;

public class MainGUIScript : MonoBehaviour 
{
	private Rect[] allPlanetsGUI, allButtonsGUI; 
	private Rect[,] tierButtonsGUI;

	private TurnInfo turnInfoScript;
	private TechTreeScript techTreeScript;
	private CameraFunctions cameraFunctionsScript;
	private HeroScript heroScript;
	private GUISystemDataScript guiPlanScript;
	private PlayerTurn playerTurnScript;
	private SelkiesAIBasic selkiesTurnScript;
	
	public bool spendMenu = false, hasColonised = false;
	public string resourceToSpend;
	private string cost, indSpend, monSpend;
	private int thisPlanet;

	void Awake()
	{
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		selkiesTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SelkiesAIBasic>();

		turnInfoScript.playerRace = null;

		GUIRectBuilder();
		GUITechTreeRectBuilder();
	}

	void Update()
	{
		if(playerTurnScript.tempObject != null)
		{
			guiPlanScript = playerTurnScript.tempObject.GetComponent<GUISystemDataScript>();
			techTreeScript = playerTurnScript.tempObject.GetComponent<TechTreeScript>();
			heroScript = playerTurnScript.tempObject.GetComponent<HeroScript>();
		}
	}

	private void GUIRectBuilder() //Setting up rects for planet data
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
	
	private void GUITechTreeRectBuilder() //Setting up rects for tech tree
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

	void OnGUI() //Urgh. Unity demands that all GUI related script should come from one OnGUI to prevent excessive numbers of calls to the CPU
	{
		#region playerrace
		if(playerTurnScript.playerRace == null)
		{
			GUI.Box (new Rect(Screen.width/2 - 150, Screen.height/2 - 40, 300, 80), "Select Race");
			
			if(GUI.Button (new Rect(Screen.width/2 - 130, Screen.height/2, 80, 20), "Humans"))
			{
				playerTurnScript.playerRace = "Human";
			}
			
			if(GUI.Button (new Rect(Screen.width/2 -40, Screen.height/2, 80, 20), "Selkies"))
			{
				playerTurnScript.playerRace = "Selkie";
			}

			if(GUI.Button (new Rect(Screen.width/2 + 50, Screen.height/2, 80, 20), "Nereides"))
			{
				playerTurnScript.playerRace = "Nereid";
			}

			if(playerTurnScript.playerRace != null)
			{
				playerTurnScript.StartTurn();
			}
		}
		#endregion
		
		#region turninfo		
		string turnNumber = "Turn: " + turnInfoScript.turn.ToString();
		
		GUI.Label (new Rect(Screen.width - 80, Screen.height - 50, 50, 20), turnNumber);
		
		if(GUI.Button (new Rect(Screen.width - 80, Screen.height - 30, 70, 20), "End turn") && playerTurnScript.playerRace != null)
		{
			turnInfoScript.turn++;
			playerTurnScript.TurnEnd (playerTurnScript.ownedSystems);
			selkiesTurnScript.Expand();
		}
		
		GUI.Box (new Rect(15, 15, 100, 130), "");
		
		GUI.Label (new Rect(20, 20, 60, 20), playerTurnScript.playerRace);
		
		string scienceStr = playerTurnScript.science.ToString();
		
		GUI.Label (new Rect(20, 45, 60, 20), scienceStr);
		
		string industryStr = playerTurnScript.industry.ToString ();
		
		GUI.Label (new Rect(20, 70, 60, 20), industryStr);
		
		string moneyStr = playerTurnScript.money.ToString ();
		
		GUI.Label (new Rect(20, 95, 60, 20), moneyStr);
		
		string GPString = playerTurnScript.GP.ToString ();
		
		GUI.Label (new Rect(20, 120, 60, 20), GPString);
		#endregion
		
		#region colonisebutton
		Rect coloniseButton = new Rect(10, Screen.height - 40, 75, 30);
		
		if(cameraFunctionsScript.coloniseMenu == true)
		{
			if(GUI.Button (coloniseButton, "Colonise") && playerTurnScript.GP > 0)
			{			
				guiPlanScript.FindSystem (playerTurnScript);
			}
		}
		#endregion
		
		#region planetinfomenu
		if(cameraFunctionsScript.openMenu == true)
		{
			#region planetdata			
			GUI.Box (new Rect(0.5f, 0.5f, Screen.width, Screen.height), "Planets in System");
			
			if(guiPlanScript.foundPlanetData == false)
			{
				guiPlanScript.SystemSIMCounter();
				guiPlanScript.foundPlanetData = true;
			}
			#endregion

			#region settingupbutton
			string dataSIMString = "Total SIM: " + guiPlanScript.totalSystemSIM.ToString() + "\n" + "Total Science: " + guiPlanScript.totalSystemScience.ToString() + "\n" 
				+ "Total Industry: " + guiPlanScript.totalSystemIndustry.ToString() + "\n" + "Total Money: " + guiPlanScript.totalSystemMoney.ToString(); 
			
			GUI.Label(new Rect (Screen.width/2 - 500.0f, Screen.height/2 - 350.0f, 100.0f, 100.0f), dataSIMString);
			
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
				
				if(guiPlanScript.planNameOwnImprov[i,1] == "No")
				{
					cost = "Colonise: 1 GP";
				}
				
				if(GUI.Button(allButtonsGUI[i], cost) && guiPlanScript.canImprove == true )
				{	
					spendMenu = true;
					thisPlanet = i;
					indSpend = guiPlanScript.improvementCost + " Industry";
					monSpend = guiPlanScript.improvementCost * 2 + " Money";
				}
				
				GUI.Label (allPlanetsGUI[i], guiPlanScript.allPlanetsInfo[i]);
			}
			#endregion

			#region spendmenu
			if(spendMenu == true && cameraFunctionsScript.openMenu == true)
			{
				GUI.Box (new Rect(Screen.width/2 - 100.0f, Screen.height/2 - 50.0f, 200.0f, 75.0f), "Resource to Spend:");	

				if(guiPlanScript.planNameOwnImprov[thisPlanet, 1] == "Yes")
				{
					guiPlanScript.improvementNumber = int.Parse (guiPlanScript.planNameOwnImprov[thisPlanet,2]);

					guiPlanScript.CheckImprovement();

					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 92.5f, 35.0f), indSpend) && playerTurnScript.industry >= guiPlanScript.improvementCost)
					{
						resourceToSpend = "Industry";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(thisPlanet);
					}

					if(GUI.Button (new Rect(Screen.width/2 + 2.5f, Screen.height/2 - 15.0f, 92.5f, 35.0f), monSpend) && playerTurnScript.money >= (guiPlanScript.improvementCost * 2))
					{
						resourceToSpend = "Money";
						spendMenu = false;
						playerTurnScript.ImproveButtonClick(thisPlanet);
					}
				}

				if(guiPlanScript.planNameOwnImprov[thisPlanet, 1] == "No")
				{
					
					if(GUI.Button (new Rect(Screen.width/2 - 95.0f, Screen.height/2 - 15.0f, 190.0f, 35.0f), "1 GP") && playerTurnScript.GP > 0)
					{
						playerTurnScript.GP -= 1;
						guiPlanScript.planNameOwnImprov[thisPlanet, 1] = "Yes";
						++playerTurnScript.planetsColonisedThisTurn;
						hasColonised = true;
						spendMenu = false;
					}
				}

				if(GUI.Button(new Rect(Screen.width/2 + 77.5f, Screen.height/2 - 45.0f, 18.5f, 18.5f), "x"))
				{
					spendMenu = false;
				}

				guiPlanScript.SystemSIMCounter();
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
			
			if(GUI.Button (new Rect(Screen.width / 2 - 800.0f, Screen.height/2, 150.0f, 50.0f), "President"))
			{
				heroScript.heroesInSystem[1] = "President";
			}
			if(heroScript.heroesInSystem[1] == "President")
			{
				GUI.Label(new Rect(Screen.width/2 - 750.0f, Screen.height/2 +30.0f,140.0f, 20.0f), "Is Present");
			}
		}
		#endregion
	}
}
