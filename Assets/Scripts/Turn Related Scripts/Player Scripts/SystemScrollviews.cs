using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SystemScrollviews : MasterScript 
{
	public GameObject builtImprovementLabel, improvementsToBuildScrollView, improvementMessageLabel, improvementMessageScrollview, tabContainer;
	private List<GameObject> builtImprovementList = new List<GameObject>();
	private List<GameObject> improvementsList = new List<GameObject>();
	private List<UIButton> tabList = new List<UIButton> ();
	public int techTierToShow, selectedPlanet;

	void Start()
	{		
		SetUpImprovementLabels ();
		SetUpTabButtons ();
		selectedPlanet = -1;
	}

	void Update()
	{
		if(cameraFunctionsScript.openMenu == false)
		{
			if(improvementsToBuildScrollView.activeInHierarchy == true)
			{
				NGUITools.SetActive(improvementsToBuildScrollView, false);

				foreach(Transform child in improvementMessageScrollview.transform)
				{
					NGUITools.SetActive(child.gameObject, false);
				}

				for(int i = 0; i < builtImprovementList.Count; ++i)
				{
					NGUITools.SetActive(builtImprovementList[i], false);
				}

				Debug.Log (selectedPlanet);
			}
		}

		if(selectedPlanet == -1)
		{
			for(int i = 0; i < tabList.Count; ++i)
			{
				if(tabList[i].enabled == true)
				{
					tabList[i].enabled = false;
				}
			}
		}

		if(cameraFunctionsScript.openMenu == true)
		{
			UpdateScrollviewContents();
			UpdateBuiltImprovements();
		}

		if(Input.GetKeyDown("c"))
		{
			NGUITools.SetActive(improvementsToBuildScrollView, false);
		}
	}

	private void SetUpTabButtons()
	{
		string[] tempArr = new string[4] {"T1 Tab", "T2 Tab", "T3 Tab", "T4 Tab"};

		for(int i = 0; i < 4; ++i)
		{
			tabList.Add (tabContainer.transform.Find (tempArr[i]).transform.Find ("Label").GetComponent<UIButton> ());
		}
	}

	private void SetUpImprovementLabels()
	{		
		for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
		{
			GameObject message = NGUITools.AddChild(improvementMessageScrollview, improvementMessageLabel);
			
			NGUITools.SetActive(message, false);
			
			message.GetComponent<UIDragScrollView>().scrollView = improvementMessageScrollview.GetComponent<UIScrollView>();
			
			builtImprovementList.Add (message);
			
			GameObject improvement = NGUITools.AddChild(improvementsToBuildScrollView, builtImprovementLabel); //Scrollviewwindow is gameobject containing scrollview, scrollviewbutton is the button prefab
			
			improvement.transform.Find ("Sprite").GetComponent<UISprite>().depth = 1; //Depth set to 20 to ensure I can see it, will be changed when scrollview actually works
			
			improvement.transform.Find ("Label").GetComponent<UILabel>().depth = 2;
			
			improvement.GetComponent<UIDragScrollView>().scrollView = improvementsToBuildScrollView.GetComponent<UIScrollView>(); //Assigning scrollview variable of prefab
			
			improvement.name = systemListConstructor.basicImprovementsList[i].name; //Just naming the object in the hierarchy
			
			EventDelegate.Add(improvement.GetComponent<UIButton>().onClick, BuildImprovement);
			
			improvement.transform.Find ("Label").GetComponent<UILabel>().text = improvement.name + "\n" + systemListConstructor.basicImprovementsList[i].cost; //Add label text

			improvement.tag = "Improvement";
			
			improvementsList.Add (improvement); //Add improvement into a list so I can enable/disable improvements as needed
			
			NGUITools.SetActive(improvement, false); //Default set improvement to false so it won't be shown in scrollview unless needed
		}
	}

	public void BuildImprovement()
	{
		GameObject improvement = UIButton.current.gameObject;

		improvementsBasic = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<ImprovementsBasic>();
		
		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementName == improvement.name)
			{
				for(int j = 0; j < systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
				{
					if(systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(improvementsBasic.ImproveSystem(i) == true)
						{
							NGUITools.SetActive (improvement, false);
							systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = improvementsBasic.listOfImprovements[i].improvementName;
							UpdateBuiltImprovements();
							UpdateScrollviewContents();
							improvementsBasic.ActiveTechnologies(systemGUI.selectedSystem, playerTurnScript);
							break;
						}
					}
				}
			}
		}
	}

	public void UpdateBuiltImprovements()
	{
		improvementsBasic = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<ImprovementsBasic>();

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == true && improvementsBasic.listOfImprovements[i].improvementMessage != "")
			{
				if(builtImprovementList[i].activeInHierarchy == false)
				{
					builtImprovementList[i].transform.Find("Sprite").GetComponent<UISprite>().depth = 1;
					builtImprovementList[i].GetComponent<UILabel>().depth = 2;
					NGUITools.SetActive(builtImprovementList[i], true);
					improvementMessageScrollview.GetComponent<UIScrollView>().ResetPosition();
					improvementMessageScrollview.GetComponent<UIGrid>().repositionNow = true;
				}


				builtImprovementList[i].GetComponent<UILabel>().text = improvementsBasic.listOfImprovements[i].improvementMessage;
			}

			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == false)
			{
				if(builtImprovementList[i].activeInHierarchy == true)
				{
					NGUITools.SetActive(builtImprovementList[i], false);
				}
			}
		}
	}

	public void TabClick()
	{
		for(int i = 0; i < tabList.Count; ++i)
		{
			if(tabList[i] == UIButton.current)
			{
				techTierToShow = i;
				UpdateScrollviewContents();
				break;
			}
		}
	}

	private void CheckForTierUnlock()
	{
		for(int i = 0; i < 4; ++i)
		{
			if(improvementsBasic.techTier >= i && tabList[i].enabled == false)
			{
				tabList[i].enabled = true;
			}
			if(improvementsBasic.techTier < i && tabList[i].enabled == true)
			{
				tabList[i].enabled = false;
			}
		}
	}

	public void UpdateScrollviewContents()
	{
		improvementsBasic = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<ImprovementsBasic>();

		CheckForTierUnlock ();
						
		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{		
			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == false && improvementsBasic.listOfImprovements[i].improvementLevel <= improvementsBasic.techTier && selectedPlanet != -1)
			{
				if(improvementsBasic.listOfImprovements[i].improvementCategory == playerTurnScript.playerRace ||  improvementsBasic.listOfImprovements[i].improvementCategory == "Generic")
				{
					if(improvementsBasic.listOfImprovements[i].improvementLevel == techTierToShow)
					{
						NGUITools.SetActive(improvementsList[i], true); //If tech has not been built, set it to active so it can be shown in the scrollview
						improvementsList[i].transform.Find ("Label").GetComponent<UILabel>().text = improvementsList[i].name + "\n" + (systemListConstructor.basicImprovementsList[i].cost - improvementsBasic.improvementCostModifier);
					}
					if(improvementsBasic.listOfImprovements[i].improvementLevel != techTierToShow)
					{
						NGUITools.SetActive(improvementsList[i], false);
					}
				}

				else
				{
					NGUITools.SetActive(improvementsList[i], false);
				}
			}

			else
			{
				NGUITools.SetActive(improvementsList[i], false);
			}

			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == true || improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.techTier)
			{
				NGUITools.SetActive(improvementsList[i], false);
			}
		}

		improvementsToBuildScrollView.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview position
		improvementsToBuildScrollView.GetComponent<UIGrid> ().Reposition (); //Reposition grid contents
	}
}
