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
	public int selectedPlanet, techTierToShow;

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
				selectedPlanet = -1;

				foreach(Transform child in improvementMessageScrollview.transform)
				{
					NGUITools.SetActive(child.gameObject, false);
				}
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

		techTreeScript = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<TechTreeScript>();
		
		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{
			if(techTreeScript.listOfImprovements[i].improvementName == improvement.name)
			{
				for(int j = 0; j < systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
				{
					if(systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(techTreeScript.ImproveSystem(i) == true)
						{
							NGUITools.SetActive (improvement, false);
							systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = techTreeScript.listOfImprovements[i].improvementName;
							UpdateBuiltImprovements();
							UpdateScrollviewContents();
							techTreeScript.ActiveTechnologies(systemGUI.selectedSystem, playerTurnScript);
							break;
						}
					}
				}
			}
		}
	}

	public void UpdateBuiltImprovements()
	{
		techTreeScript = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<TechTreeScript>();

		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{
			if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true && techTreeScript.listOfImprovements[i].improvementMessage != "")
			{
				if(builtImprovementList[i].activeInHierarchy == false)
				{
					builtImprovementList[i].transform.Find("Sprite").GetComponent<UISprite>().depth = 1;
					builtImprovementList[i].GetComponent<UILabel>().depth = 2;
					NGUITools.SetActive(builtImprovementList[i], true);
					improvementMessageScrollview.GetComponent<UIScrollView>().ResetPosition();
					improvementMessageScrollview.GetComponent<UIGrid>().repositionNow = true;
				}
				builtImprovementList[i].GetComponent<UILabel>().text = techTreeScript.listOfImprovements[i].improvementMessage;
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
			if(techTreeScript.techTier >= i && tabList[i].enabled == false)
			{
				tabList[i].enabled = true;
			}
			if(techTreeScript.techTier < i && tabList[i].enabled == true)
			{
				tabList[i].enabled = false;
			}
		}
	}

	public void UpdateScrollviewContents()
	{
		techTreeScript = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<TechTreeScript>();

		CheckForTierUnlock ();
						
		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{		
			if(techTreeScript.listOfImprovements[i].hasBeenBuilt == false && techTreeScript.listOfImprovements[i].improvementLevel <= techTreeScript.techTier)
			{
				if(techTreeScript.listOfImprovements[i].improvementCategory == playerTurnScript.playerRace ||  techTreeScript.listOfImprovements[i].improvementCategory == "Generic")
				{
					if(techTreeScript.listOfImprovements[i].improvementLevel == techTierToShow)
					{
						NGUITools.SetActive(improvementsList[i], true); //If tech has not been built, set it to active so it can be shown in the scrollview
						improvementsList[i].transform.Find ("Label").GetComponent<UILabel>().text = improvementsList[i].name + "\n" + (systemListConstructor.basicImprovementsList[i].cost - techTreeScript.improvementCostModifier);
					}
					if(techTreeScript.listOfImprovements[i].improvementLevel != techTierToShow)
					{
						NGUITools.SetActive(improvementsList[i], false);
					}
				}

				else
				{
					NGUITools.SetActive(improvementsList[i], false);
				}
			}

			if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true || techTreeScript.listOfImprovements[i].improvementLevel > techTreeScript.techTier)
			{
				NGUITools.SetActive(improvementsList[i], false);
			}
		}

		improvementsToBuildScrollView.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview position
		improvementsToBuildScrollView.GetComponent<UIGrid> ().Reposition (); //Reposition grid contents
	}
}
