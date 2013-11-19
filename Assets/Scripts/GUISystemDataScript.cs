using UnityEngine;
using System.Collections;
using System.IO;

public class GUISystemDataScript : MonoBehaviour 
{
	//THIS IS A PROTOTYPE ONLY CLASS. THIS WILL BE USED TO STORE PLANET DATA AND DISPLAY IT IN A GUI UNTIL A TRUE UI AND PLANET SCREEN CAN BE CREATED

	[HideInInspector]
	public int numPlanets, improvementNumber;
	[HideInInspector]
	public float pScience, pIndustry, pMoney, improvementCost, resourceBonus;
	[HideInInspector]
	public string improvementLevel;
	[HideInInspector]
	public string[] allPlanetsInfo = new string[6];	//Unique to object
	[HideInInspector]
	public string[,] planNameOwnImprov = new string[6,3]; //Unique to object
	[HideInInspector]
	public Rect[] allPlanetsGUI, allButtonsGUI;
	[HideInInspector]
	public bool canImprove, foundPlanetData;

	public float totalSystemScience, totalSystemIndustry, totalSystemMoney, totalSystemSIM;
	private float tempSci = 0.0f, tempInd = 0.0f, tempMon = 0.0f;
	
	private string text = " ";
	private GameObject[] systemConnections = new GameObject[4];

	private TurnInfo turnInfoScript;
	private LineRenderScript lineRenderScript;
	private CameraFunctions cameraFunctionsScript;
	private TechTreeScript techTreeScript;

	void Awake()
	{
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		techTreeScript = gameObject.GetComponent<TechTreeScript>();

		LoadFile();		
		GUIRectBuilder();
	}

	private void LoadFile() //This fills the systems array with all the planets in the system, whether they are owned, and their improvement level
	{
		using(StreamReader reader =  new StreamReader("SystemTypeData.txt"))
		{
			while(text != null)
			{
				text = reader.ReadLine();
			
				if(text == gameObject.name)
				{		
					text = reader.ReadLine();
			
					numPlanets = int.Parse (text);
			
					for(int i = 0; i < numPlanets; i++)
					{
						text = reader.ReadLine();
					
						planNameOwnImprov[i,0] = text;
						
						planNameOwnImprov[i,1] = "no";
						
						planNameOwnImprov[i,2] = "0";
					}
				
					break;
				}
			}
		}
	}
	
	public void FindSystem() //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{		
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		
		systemConnections = lineRenderScript.connections;
		
		for(int i = 0; i < 4; i++)
		{
			if(systemConnections[i] == null)
			{
				break;
			}
			
			for(int j = 0; j < turnInfoScript.arrayIterator; ++j)
			{
				if(turnInfoScript.ownedSystems[j] == null)
				{
					break;
				}
				
				if(systemConnections[i] == turnInfoScript.ownedSystems[j])
				{					
					turnInfoScript.ownedSystems[turnInfoScript.arrayIterator] = gameObject;
					
					turnInfoScript.arrayIterator++;
					
					gameObject.renderer.material = turnInfoScript.ownedMaterial;
						
					turnInfoScript.GP -= 1;
						
					cameraFunctionsScript.coloniseMenu = false;
						
					return;
				}
			}
		}
	}

	public void SystemSIMCounter() //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{				
		totalSystemScience = 0.0f;
		totalSystemIndustry = 0.0f;
		totalSystemMoney = 0.0f;

		for(int n = 0; n < numPlanets; ++n)
		{
			if(planNameOwnImprov[n,0] != null)
			{
				string planetType = planNameOwnImprov[n,0];

				improvementNumber = int.Parse (planNameOwnImprov[n,2]);

				CheckImprovement();
				
				for(int j = 0; j < 12; ++j)
				{
					if(turnInfoScript.planetRIM[j,0] == planetType)
					{
						tempSci = float.Parse(turnInfoScript.planetRIM[j,1]) * resourceBonus * turnInfoScript.raceScience;
						tempInd = float.Parse(turnInfoScript.planetRIM[j,2]) * resourceBonus * turnInfoScript.raceIndustry;
						tempMon = float.Parse(turnInfoScript.planetRIM[j,2]) * resourceBonus * turnInfoScript.raceMoney;
					}

					allPlanetsInfo[n] = gameObject.name + " " + (n+1) + "\n" + planetType + "\n" + improvementLevel + "\n" 
						+ tempSci.ToString() + "\n" + tempInd.ToString() + "\n" + tempMon.ToString();
				}

				totalSystemScience += (tempSci * techTreeScript.sciencePercentBonus) + techTreeScript.sciencePointBonus;
				totalSystemIndustry += (tempInd * techTreeScript.industryPercentBonus) + techTreeScript.industryPointBonus;
				totalSystemMoney += (tempMon * techTreeScript.moneyPercentBonus) + techTreeScript.moneyPointBonus;
			}
		}
		
		if(turnInfoScript.endTurn == true)
		{
			totalSystemSIM += totalSystemScience + totalSystemIndustry + totalSystemMoney;
		}
	}

	private void GUIRectBuilder()
	{
		Rect topLeft = new Rect(Screen.width/2 - 320.0f, Screen.height / 2 - 200.0f, 200.0f, 200.0f); //Top Left
		
		Rect buttonTopLeft = new Rect(Screen.width/2 - 320.0f, Screen.height / 2 - 75.0f, 200.0f, 50.0f);
		
		Rect topMiddle = new Rect(Screen.width/2 - 100.0f, Screen.height / 2 - 200.0f, 200.0f, 200.0f); //Top middle
		
		Rect buttonTopMiddle = new Rect(Screen.width/2 - 100.0f, Screen.height / 2 - 75.0f, 200.0f, 50.0f);
		
		Rect topRight = new Rect (Screen.width/2 + 120.0f, Screen.height / 2 - 200.0f, 200.0f, 200.0f); //Top right
		
		Rect buttonTopRight = new Rect(Screen.width/2 + 120.0f, Screen.height / 2 - 75.0f, 200.0f, 50.0f);
		
		Rect bottomLeft = new Rect(Screen.width/2 - 320.0f, Screen.height / 2, 200.0f, 200.0f); //Bottom left
		
		Rect buttonBottomLeft = new Rect(Screen.width/2 - 320.0f, Screen.height / 2 + 125.0f, 200.0f, 50.0f);
		
		Rect bottomMiddle = new Rect(Screen.width/2 -100.0f, Screen.height / 2, 200.0f, 200.0f); //Bottom middle
		
		Rect buttonBottomMiddle = new Rect(Screen.width/2 - 100.0f, Screen.height / 2 + 125.0f, 200.0f, 50.0f);
		
		Rect bottomRight = new Rect(Screen.width/2 + 120.0f, Screen.height / 2, 200.0f, 200.0f); //Bottom right
		
		Rect buttonBottomRight = new Rect(Screen.width/2 + 120.0f, Screen.height / 2 + 125.0f, 200.0f, 50.0f);
		
		allPlanetsGUI = new Rect[6] {topLeft, topMiddle, topRight, bottomLeft, bottomMiddle, bottomRight};
		
		allButtonsGUI = new Rect[6] {buttonTopLeft, buttonTopMiddle, buttonTopRight, buttonBottomLeft, buttonBottomMiddle, buttonBottomRight};	
	}
	
	public void CheckImprovement()
	{
		if(improvementNumber == 0)
		{
			improvementLevel = "Poor";
			resourceBonus = 0.5f;
			canImprove = true;
			improvementCost = 10.0f;
		}
		if(improvementNumber == 1)
		{
			improvementLevel = "Normal";
			resourceBonus = 1.0f;
			canImprove = true;
			improvementCost = 20.0f;
		}
		if(improvementNumber == 2)
		{
			improvementLevel = "Good";
			resourceBonus = 2.0f;
			canImprove = true;
			improvementCost = 40.0f;
		}
		if(improvementNumber == 3)
		{
			improvementLevel = "Superb";
			resourceBonus = 3.0f;
			canImprove = false;
		}
	}
}
