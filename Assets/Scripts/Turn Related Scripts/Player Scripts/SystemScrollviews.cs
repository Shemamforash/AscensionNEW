using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SystemScrollviews : MasterScript 
{
	public GameObject builtImprovementLabel, improvementsToBuildScrollView, improvementMessageLabel, improvementMessageScrollview;
	private List<GameObject> builtImprovementList = new List<GameObject>();
	private Vector3 improvementListPosition = new Vector3();
	private List<GameObject> improvementsList = new List<GameObject>();
	public int selectedPlanet;

	void Start()
	{		
		SetUpImprovementLabels ();
	}

	void Update()
	{
		if(cameraFunctionsScript.openMenu == false)
		{
			NGUITools.SetActive(improvementsToBuildScrollView, false);
			
			foreach(Transform child in improvementMessageScrollview.transform)
			{
				NGUITools.SetActive(child.gameObject, false);
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

	private void SetUpImprovementLabels()
	{		
		using(XmlReader reader = XmlReader.Create ("ImprovementList.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					GameObject message = NGUITools.AddChild(improvementMessageScrollview, improvementMessageLabel);
					
					NGUITools.SetActive(message, false);
					
					message.GetComponent<UIDragScrollView>().scrollView = improvementMessageScrollview.GetComponent<UIScrollView>();
					
					builtImprovementList.Add (message);
					
					GameObject improvement = NGUITools.AddChild(improvementsToBuildScrollView, builtImprovementLabel); //Scrollviewwindow is gameobject containing scrollview, scrollviewbutton is the button prefab
					
					improvement.transform.Find ("Sprite").GetComponent<UISprite>().depth = 1; //Depth set to 20 to ensure I can see it, will be changed when scrollview actually works
					
					improvement.transform.Find ("Label").GetComponent<UILabel>().depth = 2;
					
					improvement.GetComponent<UIDragScrollView>().scrollView = improvementsToBuildScrollView.GetComponent<UIScrollView>(); //Assigning scrollview variable of prefab
					
					improvement.name = reader.GetAttribute("A"); //Just naming the object in the hierarchy
					
					EventDelegate.Add(improvement.GetComponent<UIButton>().onClick, BuildImprovement);
					
					improvement.transform.Find ("Label").GetComponent<UILabel>().text = improvement.name + "\n" + reader.GetAttribute("D"); //Add label text
					
					improvementsList.Add (improvement); //Add improvement into a list so I can enable/disable improvements as needed
					
					NGUITools.SetActive(improvement, false); //Default set improvement to false so it won't be shown in scrollview unless needed
				}
			}
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
							UpdateScrollviewContents();
							UpdateBuiltImprovements();
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
			if(builtImprovementList[i].activeInHierarchy == false && techTreeScript.listOfImprovements[i].hasBeenBuilt == true && techTreeScript.listOfImprovements[i].improvementMessage != "")
			{
				builtImprovementList[i].transform.Find("Sprite").GetComponent<UISprite>().depth = 1;
				builtImprovementList[i].GetComponent<UILabel>().depth = 2;
				builtImprovementList[i].GetComponent<UILabel>().text = techTreeScript.listOfImprovements[i].improvementMessage;
				NGUITools.SetActive(builtImprovementList[i], true);
				improvementMessageScrollview.GetComponent<UIScrollView>().ResetPosition();
				improvementMessageScrollview.GetComponent<UIGrid>().repositionNow = true;
			}
		}
	}

	public void UpdateScrollviewContents()
	{
		techTreeScript = systemListConstructor.systemList[systemGUI.selectedSystem].systemObject.GetComponent<TechTreeScript>();
				
		improvementsToBuildScrollView.transform.position = systemGUI.planetElementList[selectedPlanet].spriteObject.transform.position;
		
		improvementListPosition = new Vector3(improvementsToBuildScrollView.transform.localPosition.x, 
		                                      improvementsToBuildScrollView.transform.localPosition.y + 240.0f, 
		                                      improvementsToBuildScrollView.transform.localPosition.z);
		
		improvementsToBuildScrollView.transform.localPosition = improvementListPosition;
		
		for(int i = 0; i < techTreeScript.listOfImprovements.Count; ++i)
		{			
			improvementsToBuildScrollView.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview position
			improvementsToBuildScrollView.GetComponent<UIGrid> ().Reposition (); //Reposition grid contents

			if(techTreeScript.listOfImprovements[i].hasBeenBuilt == true || techTreeScript.listOfImprovements[i].improvementLevel > techTreeScript.techTier 
			   || techTreeScript.listOfImprovements[i].improvementCategory == enemyOneTurnScript.playerRace 
			   || techTreeScript.listOfImprovements[i].improvementCategory == enemyTwoTurnScript.playerRace) 
			{
				NGUITools.SetActive(improvementsList[i], false);
				continue;
			}

			NGUITools.SetActive(improvementsList[i], true); //If tech has not been built, set it to active so it can be shown in the scrollview
		}

		improvementsToBuildScrollView.GetComponent<UIScrollView>().ResetPosition(); //Reset scrollview position
		improvementsToBuildScrollView.GetComponent<UIGrid> ().Reposition (); //Reposition grid contents
	}

}
