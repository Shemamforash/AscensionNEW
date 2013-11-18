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
	public string[] allPlanetsInfo = new string[6];	
	[HideInInspector]
	public string[,] planNameOwnImprov = new string[6,3];
	[HideInInspector]
	public Rect[] allPlanetsGUI, allButtonsGUI;
	[HideInInspector]
	public bool canImprove, foundPlanetData;
	
	private string text = " ";
	private GameObject[] systemConnections = new GameObject[4];
	private TurnInfo turnInfoScript;
	private LineRenderScript lineRenderScript;
	private CameraFunctions cameraFunctionsScript;

	void Awake()
	{
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();

		LoadFile();		
		GUIRectBuilder();
	}
	
	void LoadFile()
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
	
	public void FindSystem()
	{		
		GameObject tempObject = GameObject.Find (cameraFunctionsScript.selectedSystem);

		lineRenderScript = tempObject.GetComponent<LineRenderScript>();
		
		systemConnections = lineRenderScript.connections;
		
		for(int i = 0; i < 4; i++)
		{
			if(systemConnections[i] == null)
			{
				break;
			}
			
			for(int j = 0; j < 60; j++)
			{
				if(turnInfoScript.ownedSystems[j] == null)
				{
					break;
				}
				
				if(systemConnections[i] == turnInfoScript.ownedSystems[j])
				{					
					turnInfoScript.ownedSystems[turnInfoScript.arrayIterator] = tempObject;
					
					turnInfoScript.arrayIterator++;
					
					tempObject.renderer.material = turnInfoScript.ownedMaterial;
						
					turnInfoScript.GP -= 1;
						
					cameraFunctionsScript.coloniseMenu = false;
						
					return;
				}
			}
		}
	}

	public void FindPlanetData()
	{
		for(int i = 0; i < 6; i++)
		{				
			for(int n = 0; n < 6; n++)
			{
				string planetType = planNameOwnImprov[n,0];
				
				if(planetType != null)
				{
					improvementNumber = int.Parse(planNameOwnImprov[n,2]);
					
					CheckImprovement();
					
					for(int j = 0; j < 12; j++)
					{
						if(planetType == turnInfoScript.planetRIM[j,0])
						{
							pScience = (float.Parse (turnInfoScript.planetRIM[j,1])) * (float)turnInfoScript.raceScience * resourceBonus;
							pIndustry = (float.Parse (turnInfoScript.planetRIM[j,2])) * (float)turnInfoScript.raceIndustry * resourceBonus;
							pMoney = (float.Parse (turnInfoScript.planetRIM[j,3])) * (float)turnInfoScript.raceMoney * resourceBonus;
						}
					}
					
					if(planetType != null)
					{
						allPlanetsInfo[n] = gameObject.name + " " + (n+1) + "\n" + planetType + "\n" + improvementLevel + "\n" + pScience.ToString() + "\n" + pIndustry.ToString() + "\n" + pMoney.ToString();
					}
				}
			}	
		}	
	}
	
	void GUIRectBuilder()
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
