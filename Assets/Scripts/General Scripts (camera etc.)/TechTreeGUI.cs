using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechTreeGUI : MasterScript
{
	public GameObject techTree;
	public List<TechLabels> techLabels = new List<TechLabels>();
	
	void Start()
	{
		NGUITools.SetActive(techTree, true);

		GameObject[] tempArray = GameObject.FindGameObjectsWithTag ("TechLabel");

		for(int i = 0; i < tempArray.Length; ++i)
		{
			TechLabels techLabel = new TechLabels();

			techLabel.label = tempArray[i].GetComponent<UILabel>();
			techLabel.button = tempArray[i].GetComponent<UIButton>();

			techLabels.Add (techLabel);
		}

		for(int i = 0; i < techLabels.Count; ++i)
		{
			techLabels[i].label.text = techLabels[i].label.gameObject.name;
			techLabels[i].button.enabled = false;
		}

		NGUITools.SetActive(techTree, false);
	}

	public void ShowTechTree ()
	{
		CheckActiveTech ();

		NGUITools.SetActive(techTree, true);
	}

	private void CheckActiveTech()
	{
		for(int j = 0; j < techLabels.Count; ++j)
		{
			for(int i = 0; i < heroTechTree.heroTechList.Count; ++i)
			{
				if(techLabels[i].label.text == heroTechTree.heroTechList[i].techName)
				{
					if(heroTechTree.heroTechList[i].isActive == true)
					{
						techLabels[i].button.enabled = false;
					}

					for(int k = 0; k < heroTechTree.heroTechList.Count; ++k)
					{
						if(heroTechTree.heroTechList[i].prerequisite == null)
						{
							continue;
						}

						if(heroTechTree.heroTechList[k].techName == heroTechTree.heroTechList[i].prerequisite)
						{
							if(heroTechTree.heroTechList[k].isActive == true)
							{
								techLabels[j].button.enabled = true;
							}
						}
					}
				}
			}
		}
	}

	public void ActivateTech()
	{
		for(int i = 0; i < heroTechTree.heroTechList.Count; ++i)
		{
			if(heroTechTree.heroTechList[i].techName == "bacon" && playerTurnScript.science >= heroTechTree.heroTechList[i].scienceCost)
			{
				playerTurnScript.science -= heroTechTree.heroTechList[i].scienceCost;
				heroTechTree.heroTechList[i].isActive = true;
				CheckActiveTech();
			}
		}
	}
}

public class TechLabels
{
	public UILabel label;
	public UIButton button;
}
