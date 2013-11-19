using UnityEngine;
using System.Collections;
using System.IO;

public class TechTreeScript : MonoBehaviour 
{
	public int techToBuildTier, techToBuildPosition;
	public string[,,] techTreeComplete = new string[4,6,2];
	public float[,] techTreeCost = new float[4,6];

	private LineRenderScript lineRenderScript;
	private TurnInfo turnInfoScript;
	private GUISystemDataScript systemDataScript;
	
	public float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	public float sciencePointBonus, industryPointBonus, moneyPointBonus;
	public int techTier = 0;
	private GameObject[] connectedSystems = new GameObject[4];

	private bool activeTech = false, builtThisTurn = false;

	void Start()
	{
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
			builtThisTurn = true;
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
		if(techTreeComplete[0,0,1] == "Built" && builtThisTurn == true) //Secondary Research
		{
			turnInfoScript.science += 50;
			builtThisTurn = false;
		}

		if(techTreeComplete[0,1,1] == "Built" && activeTech == false) //Synergy
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

			activeTech = true;
		}

		if(techTreeComplete[0,3,1] == "Built")
		{
			foreach(GameObject system in turnInfoScript.ownedSystems)
			{
				systemDataScript = system.GetComponent<GUISystemDataScript>();

				for(int i = 0; i < systemDataScript.numPlanets; ++i)
				{
					string tempString = systemDataScript.planNameOwnImprov[i,0];

					if(tempString == "Plains" || tempString == "Ocean" || tempString == "Forest")
					{
						turnInfoScript.money += 10;
					}
				}
			}
		}
	}
}
