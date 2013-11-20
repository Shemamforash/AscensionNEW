using UnityEngine;
using System.Collections;
using System.IO;

public class TechTreeScript : MonoBehaviour 
{
	public int techToBuildTier, techToBuildPosition;
	public string[,,] techTreeComplete = new string[4,6,2];
	public float[,] techTreeCost = new float[4,6];
	public string planetToCheck;

	private LineRenderScript lineRenderScript;
	private TurnInfo turnInfoScript;
	private GUISystemDataScript systemDataScript;
	private TechTreeScript techTreeScript;
	
	public float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	public float sciencePointBonus, industryPointBonus, moneyPointBonus;
	public int techTier = 0;
	private GameObject[] connectedSystems = new GameObject[4];

	private int currentPlanetsWithHyperNet = 0;

	private bool synergy = false, capitalism = false, unionisation = false, familiarity = false, secondaryResearch = false;

	void Start()
	{
		systemDataScript = gameObject.GetComponent<GUISystemDataScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();

		LoadTechTree();
	}

	public void ImproveSystem()
	{
		if(turnInfoScript.industry >= techTreeCost[techToBuildTier, techToBuildPosition])
		{
			turnInfoScript.industry -= (int)techTreeCost[techToBuildTier, techToBuildPosition];
			techTreeComplete[techToBuildTier, techToBuildPosition, 1] = "Built";
			secondaryResearch = true;
		}
	}

	private void LoadTechTree()
	{
		string text = " ";
		
		using(StreamReader reader =  new StreamReader("HumanTechTree.txt"))
		{
			for(int i = 0; i < 4; i++)
			{
				for(int j = 0; j < 6; j++)
				{
					if((i == 0 && j > 4) || (i == 1 && j > 4))
					{
						break;
					}

					text = reader.ReadLine();
					techTreeComplete[i,j,0] = text;
					techTreeComplete[i,j,1] = "Not Built";

					text = reader.ReadLine();
					techTreeCost[i,j] = float.Parse(text);
				}
			}			
		}
	}

	public void ActiveTechnologies()
	{
		if(techTreeComplete[0,0,1] == "Built" && secondaryResearch == true) //Secondary Research
		{
			turnInfoScript.science += 50;
			secondaryResearch = false;
		}

		if(techTreeComplete[0,1,1] == "Built" && synergy == false) //Synergy
		{
			Synergy();
			
			synergy = true;
		}

		if(techTreeComplete[0,3,1] == "Built" && capitalism == false) //Capitalism
		{
			capitalism = true;
		}

		if(techTreeComplete[2,1,1] == "Built" && unionisation == false) //Unionisation
		{
			Unionisation();

			unionisation = true;
		}

		if(techTreeComplete[2,2,1] == "Built" && familiarity == false) //Familiarity
		{
			familiarity = true;
		}

		if(techTreeComplete[2,5,1] == "Built") //Hypernet
		{
			HyperNet();
		}
	}

	private void Unionisation()
	{
		industryPercentBonus += 0.1f;
		
		if(systemDataScript.totalSystemMoney == 0)
		{
			industryPercentBonus += 0.1f;
		}
	}

	private void Synergy()
	{
		for(int i = 0; i < 4; ++i)
		{
			connectedSystems[i] = lineRenderScript.connections[i];
		}
		
		foreach (GameObject system in connectedSystems)
		{
			for(int i = 0; i < 60; ++i)
			{
				if(system == null)
				{
					break;
				}
				
				if(system == turnInfoScript.ownedSystems[i])
				{
					industryPercentBonus += 0.05f;
				}
			}
		}
	}

	private void HyperNet()
	{
		sciencePercentBonus -= currentPlanetsWithHyperNet * 0.005f;
		industryPercentBonus -= currentPlanetsWithHyperNet * 0.005f;
		moneyPercentBonus -= currentPlanetsWithHyperNet * 0.005f;
		
		currentPlanetsWithHyperNet = 0;
		
		foreach(GameObject system in turnInfoScript.ownedSystems)
		{
			if(system == null)
			{
				continue;
			}
			
			techTreeScript = system.GetComponent<TechTreeScript>();
			
			if(techTreeScript.techTreeComplete[2,5,1] == "Built")
			{
				++currentPlanetsWithHyperNet;
			}
		}
		
		sciencePercentBonus += currentPlanetsWithHyperNet * 0.005f;
		industryPercentBonus += currentPlanetsWithHyperNet * 0.005f;
		moneyPercentBonus += currentPlanetsWithHyperNet * 0.005f;
	}

	public void CheckPlanets()
	{
		if(capitalism == true && planetToCheck == "Plains" || planetToCheck == "Ocean" || planetToCheck == "Forest")
		{
			systemDataScript.tempMon += 10;
		}
		
		if(familiarity == true && planetToCheck == turnInfoScript.homePlanet)
		{
			systemDataScript.tempMon += systemDataScript.tempMon * 0.5f;
		}
	}
}


