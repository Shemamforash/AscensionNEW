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
	private HeroScript heroScript;
	
	public float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	public float sciencePointBonus, industryPointBonus, moneyPointBonus;

	public int techTier = 0;
	private GameObject[] connectedSystems = new GameObject[4];

	private int currentPlanetsWithHyperNet = 0;

	private bool capitalism = false, familiarity = false, secondaryResearch = false;
	public bool leadership = false;

	void Start()
	{
		systemDataScript = gameObject.GetComponent<GUISystemDataScript>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScript>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();

		LoadTechTree();
	}

	public void ImproveSystem() //Occurs if button of tech is clicked.
	{
		if(turnInfoScript.industry >= techTreeCost[techToBuildTier, techToBuildPosition]) //Checks cost of tech and current industry
		{
			turnInfoScript.industry -= (int)techTreeCost[techToBuildTier, techToBuildPosition];
			techTreeComplete[techToBuildTier, techToBuildPosition, 1] = "Built";
			secondaryResearch = true; //Activates tech ability
		}
	}

	private void LoadTechTree() //Loads tech tree into two arrays (whether tech has been built, and the cost of each tech)
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

	public void ActiveTechnologies() //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		sciencePercentBonus = 1.0f; //Resets the percentage modifier for SIM. Is there an easier way?
		industryPercentBonus = 1.0f;
		moneyPercentBonus = 1.0f;

		if(techTreeComplete[0,0,1] == "Built" && secondaryResearch == true) //Secondary Research
		{
			turnInfoScript.science += 50;
			secondaryResearch = false;
		}

		if(techTreeComplete[0,1,1] == "Built") //Synergy
		{
			Synergy();
		}

		if(techTreeComplete[0,2,1] == "Built") //Morale
		{
			foreach(string hero in heroScript.heroesInSystem)
			{
				if(hero == null)
				{
					break;
				}
				else
				{
					moneyPointBonus += 10.0f;
				}
			}
		}

		if(techTreeComplete[0,3,1] == "Built" && capitalism == false) //Capitalism
		{
			capitalism = true;
		}

		if(techTreeComplete[0,4,1] == "Built" && leadership == false) //Leadership
		{
			leadership = true;
		}

		if(techTreeComplete[1,1,1] == "Built") //Quick Starters
		{
			industryPointBonus += turnInfoScript.planetsColonisedThisTurn * 50.0f;
		}

		if(techTreeComplete[2,1,1] == "Built") //Unionisation
		{
			Unionisation();
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

	private void HyperNet() //Tier 3 tech. Bonus SIM for each connected planet. This function is good.
	{		
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

	public void CheckPlanets() //This function contains effects caused by tech, but cannot be activated within this script.
	{
		if(capitalism == true && planetToCheck == "Plains" || planetToCheck == "Ocean" || planetToCheck == "Forest")
		{
			systemDataScript.tempMon += 10;
		}
		
		//if(familiarity == true && planetToCheck == turnInfoScript.homePlanet)
		//{
			//systemDataScript.tempMon += systemDataScript.tempMon * 0.5f;
		//}
	}
}


