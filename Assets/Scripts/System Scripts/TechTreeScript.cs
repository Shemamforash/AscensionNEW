using UnityEngine;
using System.Collections;
using System.IO;

public class TechTreeScript : MasterScript 
{
	public int techToBuildTier, techToBuildPosition;
	public string[,,] techTreeComplete = new string[4,6,2];
	public string[,] improvementsOnPlanet = new string[6,4];
	public float[,] techTreeCost = new float[4,6];
	public string planetToCheck;
	public string[] improvementMessageArray = new string[24];

	public float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	public float sciencePointBonus, industryPointBonus, moneyPointBonus;

	public int techTier = 0;

	private int currentPlanetsWithHyperNet = 0;

	private bool capitalism = false, familiarity = false, secondaryResearch = false;
	public bool leadership = false;
	private float tempValue;

	void Start()
	{
		guiPlanScript = gameObject.GetComponent<GUISystemDataScript>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();

		LoadTechTree();
	}

	public void ImproveSystem(int tier, int position) //Occurs if button of tech is clicked.
	{
		if(playerTurnScript.industry >= techTreeCost[tier, position]) //Checks cost of tech and current industry
		{
			playerTurnScript.industry -= (int)techTreeCost[tier, position];
			techTreeComplete[tier, position, 1] = "Built";
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

	public void AddImprovementMessage(string message, int tech)
	{
		improvementMessageArray[tech] = message;
	}

	public void ActiveTechnologies(TurnInfo selectedPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		sciencePercentBonus = 1.0f; //Resets the percentage modifier for SIM. Is there an easier way?
		industryPercentBonus = 1.0f;
		moneyPercentBonus = 1.0f;

		if(techTreeComplete[0,0,1] == "Built" && secondaryResearch == true) //Secondary Research
		{
			playerTurnScript.science += 50;
			AddImprovementMessage("+50 Science on Improvement", 0);
			secondaryResearch = false;
		}

		if(techTreeComplete[0,1,1] == "Built") //Synergy
		{
			Synergy(selectedPlayer);
			AddImprovementMessage ("+" + tempValue.ToString () + "% Industry on system", 1);
		}

		if(techTreeComplete[0,2,1] == "Built") //Morale
		{
			tempValue = 0.0f;

			int i = masterScript.RefreshCurrentSystem(gameObject);

			for(int j = 0; j < 3; ++j)
			{
				if(masterScript.systemList[i].heroesInSystem[j] == null)
				{
					continue;
				}

				else
				{
					moneyPointBonus += 10.0f;
					tempValue += 10.0f;
				}
			}

			AddImprovementMessage("+" + tempValue.ToString() + " Money from heroes in System", 2);
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
			industryPointBonus += playerTurnScript.planetsColonisedThisTurn * 50.0f;
			tempValue = playerTurnScript.planetsColonisedThisTurn * 50.0f;
			AddImprovementMessage("+" + tempValue + " Industry from planets colonised this turn", 5);
		}

		if(techTreeComplete[1,4,1] == "Built")
		{
			bool industryBonus = false;
			bool scienceBonus = false;

			for(int i = 0; i < 4; ++i)
			{
				if(lineRenderScript.connections[i] == null)
				{
					break;
				}

				int j = masterScript.RefreshCurrentSystem(lineRenderScript.connections[i]);

				if(masterScript.systemList[j].systemOwnedBy == "Selkies" && industryBonus == false)
				{
					industryPercentBonus += 0.5f;
					AddImprovementMessage ("+ 50% Industry from Selkies adjacency", 8);
					industryBonus = true;
				}

				if(masterScript.systemList[j].systemOwnedBy == "Nereides" && scienceBonus == false)
				{
					industryPercentBonus += 0.5f;
					AddImprovementMessage ("+ 50% Science from Nereides adjacency", 9);
					scienceBonus = true;
				}
			}
		}

		if(techTreeComplete[2,1,1] == "Built") //Unionisation
		{
			Unionisation();
			AddImprovementMessage ("+" + tempValue + "% Industry from Economic status", 10);
		}

		if(techTreeComplete[2,2,1] == "Built" && familiarity == false) //Familiarity
		{
			familiarity = true;
		}

		if(techTreeComplete[2,5,1] == "Built") //Hypernet
		{
			HyperNet();
			AddImprovementMessage ("+" + tempValue + "% SIM from systems with Hypernet", 14);
		}
	}

	private void Unionisation()
	{
		tempValue = 0.0f;

		industryPercentBonus += 0.1f;
		tempValue += 0.1f;
		
		if(guiPlanScript.totalSystemMoney == 0)
		{
			industryPercentBonus += 0.1f;
			tempValue += 0.1f;
		}
	}

	private void Synergy(TurnInfo selectedPlayer)
	{
		float tempFloat = 0.0f;

		for(int i = 0; i < 4; ++i)
		{
			if(lineRenderScript.connections[i] = null)
			{
				break;
			}

			int j = masterScript.RefreshCurrentSystem(lineRenderScript.connections[i]);

			if(masterScript.systemList[j].systemOwnedBy == selectedPlayer.playerRace)
			{
				industryPercentBonus += 0.05f;
				tempFloat += 0.05f;
			}
		}
	}

	private void HyperNet() //Tier 3 tech. Bonus SIM for each connected planet. This function is good.
	{		
		currentPlanetsWithHyperNet = 0;
		
		foreach(GameObject system in playerTurnScript.ownedSystems)
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

		tempValue = currentPlanetsWithHyperNet * 0.005f;
	}

	public void CheckPlanets() //This function contains effects caused by tech, but cannot be activated within this script.
	{
		if(capitalism == true && planetToCheck == "Plains" || planetToCheck == "Ocean" || planetToCheck == "Forest")
		{
			guiPlanScript.tempMon += 10;
		}
		
		//if(familiarity == true && planetToCheck == turnInfoScript.homePlanet)
		//{
			//guiPlanScript.tempMon += guiPlanScript.tempMon * 0.5f;
		//}
	}
}


