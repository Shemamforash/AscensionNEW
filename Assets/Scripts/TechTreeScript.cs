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
			turnInfoScript.industry -= techTreeCost[techToBuildTier, techToBuildPosition];
			techTreeComplete[techToBuildTier, techToBuildPosition, 1] = "Built";
			//TechnologyList();
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



	private void TechnologyList()
	{
		string techToImprove = ""; //************ edit this asap ************

		if(techToImprove == "Secondary Research")
		{
			//+50 science on system improvement
		}

		if(techToImprove == "Synergy")
		{
			for(int i = 0; i < 4; ++i)
			{
				connectedSystems[i] = lineRenderScript.connections[i];
			}

			foreach (GameObject system in connectedSystems)
			{
				for(int i = 0; i < 60; ++i)
				{
					if(system == turnInfoScript.ownedSystems[i])
					{
						industryPercentBonus += 0.05f;
					}
				}
			}
		}

		if(techToImprove == "Morale")
		{
			//foreach hero in system +10 money
		}

	}
}
