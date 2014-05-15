using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SystemScrollviews : MasterScript 
{
	public GameObject improvementMessageLabel, availableImprovements, buttonLabel, improvementParent, improvementsWindow, improvementDetails;
	public int techTierToShow, selectedPlanet, selectedSystem, selectedSlot;
	public GameObject[] tabs = new GameObject[4];
	private string improvementText, currentImprovement;
	public UILabel improvementLabel, improvementWealthCost, improvementPowerCost, systemEffects;

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
		currentImprovement = null;
	
		bool reset = false;

		for(int i = 0; i < tabs.Length; ++i)
		{
			if(tabs[i].GetComponent<UISprite>().spriteName == "Button Hover (Orange)")
			{
				UpdateImprovementsWindow (i);
				reset = true;
				break;
			}
		}

		if(reset == false)
		{
			tabs[0].GetComponent<UIButton>().enabled = false;
			tabs[0].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
			UpdateImprovementsWindow (0);
		}

		selectedSlot = -1;
		
		for(int i = 0; i < improvementsList.Length; ++i)
		{
			if(UIButton.current.gameObject == improvementsList[i])
			{
				selectedSlot = i;
				break;
			}
		}
		
		Debug.Log (selectedSlot);
		
		NGUITools.SetActive(improvementsWindow, true);
	}

	private void ShowDetails()
	{
		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			if(UIButton.current.gameObject == unbuiltImprovementList[i])
			{
				unbuiltImprovementList[i].GetComponent<UIButton>().enabled = false;
				unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Click";
				currentImprovement = UIButton.current.transform.Find ("Label").gameObject.GetComponent<UILabel>().text;
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

				improvementPowerCost.text = systemListConstructor.basicImprovementsList[i].cost.ToString();
				improvementWealthCost.text = (systemListConstructor.basicImprovementsList[i].cost / 25).ToString();

			}
		}

		NGUITools.SetActive (improvementDetails, true);
	}

	private void UpdateImprovementsWindow(int level)
	{
		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);
		}

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
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i].ToUpper(); //Set text
					improvementsList[i].GetComponent<UIButton>().enabled = false;
					improvementsList[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}

				else //Else say is empty
				{
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = "Empty";

					if(selectedSlot == i)
					{
						improvementsList[i].GetComponent<UIButton>().enabled = false;
						improvementsList[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
					}

					else
					{
						improvementsList[i].GetComponent<UIButton>().enabled = true;
					}
				}

				continue;
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
				if(tabs[i].GetComponent<UISprite>().spriteName == "Button Hover (Orange)")
				{
					continue;
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
		currentImprovement = null;

		for(int i = 0; i < tabs.Length; ++i)
		{
			if(tabs[i] == UIButton.current.gameObject)
			{
				tabs[i].GetComponent<UIButton>().enabled = false;
				tabs[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
				UpdateImprovementsWindow(i);
			}

			else
			{
				if(i <= improvementsBasic.techTier)
				{
					tabs[i].GetComponent<UIButton>().enabled = true;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}
				else
				{
					tabs[i].GetComponent<UIButton>().enabled = false;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Deactivated";
				}
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
				UpdateSystemEffects ();
			}
		}

		if(Input.GetKeyDown("c"))
		{
			NGUITools.SetActive(availableImprovements, false);
		}
	}

	public void UpdateSystemEffects()
	{
		improvementsBasic = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<ImprovementsBasic>();

		string temp = "";

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == true)
			{
				if(temp == "")
				{
					temp = improvementsBasic.listOfImprovements[i].improvementMessage.ToUpper();
				}

				else
				{
					temp = temp + "\n" + improvementsBasic.listOfImprovements[i].improvementMessage.ToUpper();
				}
			}
		}

		if(temp == "")
		{
			temp = "NO EFFECTS ON SYSTEM";
		}

		systemEffects.text = temp;
		systemEffects.transform.parent.GetComponent<UISprite> ().height = systemEffects.height + 20;
	}

	public void BuildImprovement()
	{
		NGUITools.SetActive (improvementDetails, false);

		improvementsBasic = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<ImprovementsBasic>();
		
		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementName.ToUpper () == currentImprovement)
			{
				for(int j = 0; j < systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementSlots; ++j)
				{
					if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(improvementsBasic.ImproveSystem(i) == true)
						{
							improvementsBasic.ActiveTechnologies(selectedSystem, playerTurnScript);
							systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = improvementsBasic.listOfImprovements[i].improvementName;
							UpdateImprovementsWindow(improvementsBasic.listOfImprovements[i].improvementLevel);
							UpdateBuiltImprovements();
							currentImprovement = null;
							UpdateSystemEffects();
							selectedSlot = -1;
							break;
						}
					}
				}
			}
		}

		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Normal";
			unbuiltImprovementList[i].GetComponent<UIButton>().enabled = true;
		}

		NGUITools.SetActive(improvementsWindow, false);
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
