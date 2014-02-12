using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, grid;
	private List<GameObject> planetList = new List<GameObject>();
	private int system;
	private string invasionInfo;
	public bool openInvasionMenu = false;

	void Start () 
	{
		NGUITools.SetActive (invasionScreen, true);

		string[] tempString = new string[6] {"Planet Invade 1", "Planet Invade 2", "Planet Invade 3", "Planet Invade 4", "Planet Invade 5", "Planet Invade 6"};

		for(int i = 0; i < 6; ++i)
		{
			planetList.Add (GameObject.Find (tempString[i]));
		}

		grid = invasionScreen.transform.Find ("Planet Scroll Window").transform.Find ("Grid").gameObject;

		NGUITools.SetActive (invasionScreen, false);
	}

	void Update()
	{
		if(cameraFunctionsScript.selectedSystem != null)
		{
			systemSIMData = cameraFunctionsScript.selectedSystem.GetComponent<SystemSIMData> ();
			systemDefence = cameraFunctionsScript.selectedSystem.GetComponent<SystemDefence> ();

			if(systemDefence.underInvasion == true && openInvasionMenu == true)
			{
				system = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);
				heroScript = systemListConstructor.systemList[system].enemyHero.GetComponent<HeroScriptParent>();

				for(int i = 0; i < systemListConstructor.systemList[system].systemSize; i++)
				{	
					if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
					{
						invasionInfo = systemListConstructor.systemList[system].planetsInSystem[i].planetName + "\n"
							+ systemListConstructor.systemList[system].planetsInSystem[i].planetType + "\n"
							+ systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership + "\n"
							+ systemListConstructor.systemList[system].planetsInSystem[i].planetDefence;
					}
					else if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false)
					{
						invasionInfo = "Uncolonised";
					}
					
					planetList[i].GetComponent<UILabel>().text = invasionInfo;
					
					if(i < systemListConstructor.systemList[system].systemSize)
					{
						NGUITools.SetActive(planetList[i], true);

						if(i == heroScript.planetInvade || invasionInfo == "Uncolonised")
						{
							planetList[i].GetComponent<UIButton>().enabled = false;
						}
						else
						{
							planetList[i].GetComponent<UIButton>().enabled = true;
						}
					}
					
					else if(i >= systemListConstructor.systemList[system].systemSize)
					{
						NGUITools.SetActive(planetList[i], false);
						
						planetList[i].GetComponent<UIButton>().enabled = false;
					}
				}
			}

			if(systemDefence.underInvasion == false)
			{
				NGUITools.SetActive(invasionScreen, false);
			}
		}

		else
		{
			NGUITools.SetActive(invasionScreen, false);
		}
	}

	public void OpenPlanetInvasionScreen()
	{
		NGUITools.SetActive (invasionScreen, true);

		system = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

		systemGUI.PositionGrid (grid, systemListConstructor.systemList[system].systemSize);
	}
	
	public void PlanetInvasionClick()
	{
		for(int i = 0; i < 6; ++i)
		{			
			if(planetList[i] == UIButton.current.gameObject)
			{
				heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();
				heroScript.planetInvade = i;
				break;
			}
		}
	}
}
