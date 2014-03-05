using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, grid, fissionBombButton, fusionBombButton, antimatterBombButton;
	private List<GameObject> planetList = new List<GameObject>();
	private int system, activeButtons;
	private string invasionInfo, bombSelected;
	public bool openInvasionMenu = false;
	private float fissionBombTimer, fusionBombTimer, antimatterBombTimer;
	private List<GameObject> bombButtons = new List<GameObject> ();
	private List<float> bombTimers = new List<float>();

	void Start () 
	{
		NGUITools.SetActive (invasionScreen, true);

		string[] tempString = new string[6] {"Planet Invade 1", "Planet Invade 2", "Planet Invade 3", "Planet Invade 4", "Planet Invade 5", "Planet Invade 6"};

		for(int i = 0; i < 6; ++i)
		{
			GameObject temp = invasionScreen.transform.Find ("Planet Scroll Window").transform.Find ("Grid").transform.Find (tempString[i]).gameObject;

			planetList.Add (temp);
		}

		bombButtons.Add (fissionBombButton);
		bombButtons.Add (fusionBombButton);
		bombButtons.Add (antimatterBombButton);

		bombTimers.Add (fissionBombTimer);
		bombTimers.Add (fusionBombTimer);
		bombTimers.Add (antimatterBombTimer);

		for(int i = 0; i < planetList.Count; ++i)
		{
			NGUITools.SetActive (planetList[i], false);
		}
	}

	void Update()
	{
		if(heroGUI.selectedHero != null)
		{
			heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();
			
			heroShip = heroGUI.selectedHero.GetComponent<HeroShip>();
			
			systemDefence = heroScript.heroLocation.GetComponent<SystemDefence> ();
			
			if(systemDefence.underInvasion == true && openInvasionMenu == true)
			{
				if(bombSelected != null)
				{
					for(int i = 0; i < bombButtons.Count; ++i)
					{
						bombButtons[i].GetComponent<UIButton> ().isEnabled = false;
					}
				}

				switch(heroShip.canViewSystem)
				{
				case true:
					UpdateBombButton();
					break;
				case false:
					for(int i = 0; i < bombButtons.Count; ++i)
					{
						NGUITools.SetActive(bombButtons[i], false);
					}
					break;
				default:
					break;
				}
				
				for(int i = 0; i < systemListConstructor.systemList[system].systemSize; i++)
				{	
					UpdatePlanetInvasionValues(i);
				}
			}
			
			if(systemDefence.underInvasion == false)
			{
				for(int i = 0; i < planetList.Count; ++i)
				{
					NGUITools.SetActive (planetList[i], false);
				}
			}
		}
		
		if(openInvasionMenu == false)
		{
			for(int i = 0; i < bombButtons.Count; ++i)
			{
				NGUITools.SetActive(bombButtons[i], false);
			}

			for(int i = 0; i < planetList.Count; ++i)
			{
				NGUITools.SetActive (planetList[i], false);
			}
		}
	}

	private void UpdatePlanetInvasionValues(int i)
	{
		if(i < systemListConstructor.systemList[system].systemSize)
		{
			NGUITools.SetActive(planetList[i], true);
			
			if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false || 
			   systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership == 0 ||
			   systemListConstructor.systemList[system].planetsInSystem[i].planetDefence == 0)
			{
				planetList[i].GetComponent<UIButton>().isEnabled = false;
				
				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false)
				{
					invasionInfo = "Uncolonised";
				}
			}
			
			else
			{
				if(heroScript.heroTier2 == "Infiltrator" && bombSelected == null)
				{
					planetList[i].GetComponent<UIButton>().isEnabled = false;
				}

				else if(i == heroScript.planetInvade)
				{
					planetList[i].GetComponent<UIButton>().isEnabled = false;
				}

				else if(i != heroScript.planetInvade)
				{
					planetList[i].GetComponent<UIButton>().isEnabled = true;
				}

				if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == true)
				{
					invasionInfo = systemListConstructor.systemList[system].planetsInSystem[i].planetName + "\n"
						+ systemListConstructor.systemList[system].planetsInSystem[i].planetType + "\nOwnership: "
							+ systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership + "\nDefence: "
							+ systemListConstructor.systemList[system].planetsInSystem[i].planetDefence;
				}
			}
		}
		
		if(i >= systemListConstructor.systemList[system].systemSize)
		{
			NGUITools.SetActive(planetList[i], false);
			
			planetList[i].GetComponent<UIButton>().enabled = false;
		}
		
		planetList[i].GetComponent<UILabel>().text = invasionInfo;
	}

	private void UpdateBombButton()
	{
		for(int i = 0; i < bombButtons.Count; ++i)
		{
			for(int j = 0; j < HeroTechTree.heroTechList.Count; ++j)
			{
				if(bombButtons[i].name == HeroTechTree.heroTechList[j].techName)
				{
					if(bombButtons[i].activeInHierarchy == false)
					{
						NGUITools.SetActive(bombButtons[i], true);
						bombButtons[i].GetComponent<UIButton>().isEnabled = false;
					}

					if(HeroTechTree.heroTechList[i].isActive == true)
					{
						float power = HeroTechTree.heroTechList[i].secondaryOffenceRating;

						if((bombTimers[i] + (power / 4f)) <= Time.time || bombTimers[i] == 0.0f)
						{
							bombButtons[i].GetComponent<UIButton>().isEnabled = true;
							bombButtons[i].GetComponent<UISprite>().fillAmount = 1;
							bombTimers[i] = 0.0f;
						}
						
						if(power == 0 || (bombTimers[i] + (power / 4f)) > Time.time)
						{
							bombButtons[i].GetComponent<UIButton>().isEnabled = false;
						}

						if(bombTimers[i] != 0.0f)
						{
							bombButtons[i].GetComponent<UISprite>().fillAmount = ((Time.time - bombTimers[i]) * 4) / power;
							Debug.Log (bombTimers[i] + " bacon");
						}
					}
				}
			}
		}
	}

	public void OpenPlanetInvasionScreen()
	{
		NGUITools.SetActive (invasionScreen, true);

		heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();

		system = RefreshCurrentSystem(heroScript.heroLocation);

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		systemDefence.underInvasion = true;

		systemGUI.PositionGrid (grid, systemListConstructor.systemList[system].systemSize);
	}

	public void SelectBomb()
	{
		string tempButton = UIButton.current.gameObject.name;

		switch(tempButton)
		{
		case "Fission Bomb":
			bombSelected = "Fission";
			break;
		case "Fusion Bomb":
			bombSelected = "Fusion";
			break;
		case "Antimatter Bomb":
			bombSelected = "Antimatter";
			break;
		default:
			break;
		}
	}

	private void BombPlanet(int i)
	{
		switch(bombSelected)
		{
		case "Fission":
			bombTimers[0] = Time.time;
			break;
		case "Fusion":
			bombTimers[1] = Time.time;
			break;
		case "Antimatter":
			bombTimers[2] = Time.time;
			break;
		default:
			break;
		}

		heroScript.planetInvade = -1;
		bombSelected = null;
	}

	public void PlanetInvasionClick()
	{
		heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();

		for(int i = 0; i < 6; ++i)
		{			
			if(planetList[i] == UIButton.current.gameObject)
			{
				heroScript.planetInvade = i;
				heroScript.PlanetInvasion();

				if(bombSelected != null)
				{
					BombPlanet(i);
				}

				break;
			}
		}
	}
}
