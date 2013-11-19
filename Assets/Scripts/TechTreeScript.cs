using UnityEngine;
using System.Collections;
using System.Xml;

public class TechTreeScript : MonoBehaviour 
{
	private string techToImprove;

	private string[,] allImprovementsTiers = new string[4,6];
	private LineRenderScript lineRenderScript;
	private TurnInfo turnInfoScript;
	public bool systemHasImproved = false;
	protected float sciencePercentBonus = 1.0f, industryPercentBonus = 1.0f, moneyPercentBonus = 1.0f;
	protected int techTier, sciencePointBonus, industryPointBonus, moneyPointBonus;
	private GameObject[] connectedSystems = new GameObject[4];

	private void Start()
	{
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
	}

	private void Update()
	{
		if(systemHasImproved == true)
		{
			//techToImprove = clickedImprovementName;
			//techTier = clickedImprovementTier;

			CheckTier ();
			for(int i = 0; i < 6; i++)
			{
				if(allImprovementsTiers[techTier, i] == null)
				{
					allImprovementsTiers[techTier, i] = clickedImprovementName;
					break;
				}
			}

		}
	}

	private void CheckTier()
	{
		if(techTier == 0)
		{
			TierZero();
		}
		
		if(techTier == 1)
		{
			TierOne ();
		}
		
		if(techTier == 2)
		{
			TierTwo ();
		}
		
		if(techTier == 3)
		{
			TierThree ();
		}
	}

	private void TierZero()
	{
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

	private void TierOne()
	{
		if(techToImprove == "Collaboration")
		{
			//foreach faction at peace sciencePercenBonus += +0.1
		}
	}

	private void TierTwo()
	{
		
	}

	private void TierThree()
	{
		
	}
}
