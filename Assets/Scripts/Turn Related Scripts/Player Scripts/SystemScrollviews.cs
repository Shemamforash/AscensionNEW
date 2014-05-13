using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SystemScrollviews : MasterScript 
{
	public GameObject improvementMessageLabel, availableImprovements, buttonLabel, improvementParent, improvementsWindow, improvementDetails;
	private List<UIButton> tabList = new List<UIButton> ();
	public int techTierToShow, selectedPlanet, selectedSystem;
	public GameObject[] tabs = new GameObject[4];
	private string improvementText;
	public UILabel systemInfoText, improvementLabel;
	private bool techSelected = false;

	public GameObject[] unbuiltImprovementList = new GameObject[10];
	public GameObject[] improvementsList = new GameObject[8];

	void Start()
	{		
		SetUpImprovementLabels ();
		selectedPlanet = -1;
	}

	private void SetUpImprovementLabels()
	{		
		for(int i = 0; i < improvementsList.Length; ++i)
		{
			EventDelegate.Add(improvementsList[i].GetComponent<UIButton>().onClick, OpenImprovementsWindow);
			
			NGUITools.SetActive(improvementsList[i], false); //Default set improvement to false so it won't be shown in scrollview unless needed
		}

		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);

			EventDelegate.Add(unbuiltImprovementList[i].GetComponent<UIButton>().onClick, ShowDetails);
		}
	}

	private void OpenImprovementsWindow()
	{
		NGUITools.SetActive (improvementsWindow, true);
		NGUITools.SetActive (improvementDetails, false);

		UpdateImprovementsWindow (0);
	}

	private void ShowDetails()
	{
		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			if(UIButton.current.gameObject == unbuiltImprovementList[i])
			{
				unbuiltImprovementList[i].GetComponent<UIButton>().enabled = false;
				unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Click";
				continue;
			}

			else
			{
				unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Normal";
				unbuiltImprovementList[i].GetComponent<UIButton>().enabled = true;
			}
		}

		Vector3 tempPos = UIButton.current.transform.localPosition;

		improvementDetails.transform.localPosition = new Vector3 (tempPos.x + 265f, tempPos.y, tempPos.z); 

		for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
		{
			if(systemListConstructor.basicImprovementsList[i].name.ToUpper() == UIButton.current.transform.Find ("Label").GetComponent<UILabel>().text)
			{
				improvementLabel.text = systemListConstructor.basicImprovementsList[i].details.ToUpper();
			}
		}

		NGUITools.SetActive (improvementDetails, true);
	}

	private void UpdateImprovementsWindow(int level)
	{
		int j = 0;

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementLevel == level)
			{
				if(improvementsBasic.listOfImprovements[i].improvementCategory == "Generic" || improvementsBasic.listOfImprovements[i].improvementCategory == playerTurnScript.playerRace)
				{
					if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == false)
					{
						NGUITools.SetActive(unbuiltImprovementList[j], true);
						
						unbuiltImprovementList[j].transform.Find("Label").GetComponent<UILabel>().text = improvementsBasic.listOfImprovements[i].improvementName.ToUpper();

						++j;
					}
				}
			}
		}

		for(int i = j; j < unbuiltImprovementList.Length; ++j)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);
		}
	}

	public void UpdateBuiltImprovements()
	{
		for(int i = 0; i < improvementsList.Length; ++i) //For all improvement slots
		{
			if(i < systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementSlots) //If is equal to or less than planets slots
			{
				NGUITools.SetActive(improvementsList[i], true); //Activate

				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i] != null) //If something built
				{
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i]; //Set text
					improvementsList[i].GetComponent<UIButton>().enabled = false;
					improvementsList[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}
				else //Else say is empty
				{
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = "Empty";
					improvementsList[i].GetComponent<UIButton>().enabled = true;
				}
			}

			else //Else deactivate
			{
				NGUITools.SetActive(improvementsList[i], false);
			}
		}
	}

	private void UpdateTabs()
	{
		for(int i = 0; i < tabs.Length; ++i)
		{
			if(i <= improvementsBasic.techTier)
			{
				if(tabs[i].GetComponent<UISprite>().spriteName == "Button Click")
				{
					tabs[i].GetComponent<UIButton>().enabled = false;
				}
				else
				{
					tabs[i].GetComponent<UIButton>().enabled = true;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}
			}
			else
			{
				tabs[i].GetComponent<UIButton>().enabled = false;
				tabs[i].GetComponent<UISprite>().spriteName = "Button Deactivated";
			}
		}
	}
	
	public void TabClick()
	{
		NGUITools.SetActive (improvementDetails, false);

		for(int i = 0; i < tabs.Length; ++i)
		{
			if(tabs[i] == UIButton.current.gameObject)
			{
				tabs[i].GetComponent<UISprite>().spriteName = "Button Click";
				tabs[i].GetComponent<UIButton>().enabled = false;
				UpdateImprovementsWindow(i);
				break;
			}
		}
	}

	void Update()
	{
		if(systemGUI.selectedSystem != selectedSystem)
		{
			NGUITools.SetActive(improvementsWindow, false);
			selectedSystem = systemGUI.selectedSystem;
			improvementsBasic = systemListConstructor.systemList [selectedSystem].systemObject.GetComponent<ImprovementsBasic> ();
		}

		if(cameraFunctionsScript.openMenu == true)
		{
			if(selectedPlanet != -1)
			{
				if(improvementsWindow.activeInHierarchy == true)
				{
					UpdateTabs();
				}

				UpdateBuiltImprovements();
			}
		}

		if(Input.GetKeyDown("c"))
		{
			NGUITools.SetActive(availableImprovements, false);
		}
	}



	public void BuildImprovement()
	{
		NGUITools.SetActive (improvementDetails, false);

		GameObject improvement = UIButton.current.gameObject;

		improvementsBasic = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<ImprovementsBasic>();
		
		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementName == improvement.name)
			{
				for(int j = 0; j < systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
				{
					if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(improvementsBasic.ImproveSystem(i) == true)
						{
							NGUITools.SetActive (improvement, false);
							systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = improvementsBasic.listOfImprovements[i].improvementName;
							UpdateBuiltImprovements();
							improvementsBasic.ActiveTechnologies(selectedSystem, playerTurnScript);
							break;
						}
					}
				}
			}
		}
	}
	
	private void CheckForTierUnlock()
	{
		for(int i = 0; i < 4; ++i)
		{
			UIButton temp = tabs[i].gameObject.GetComponent<UIButton>();

			if(improvementsBasic.techTier >= i && temp.enabled == false)
			{
				temp.enabled = true;
			}
			if(improvementsBasic.techTier < i && temp.enabled == true)
			{
				temp.enabled = false;
			}
		}
	}
}
