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
	private float fissionBombTimer = 0, fusionBombTimer = 0, antimatterBombTimer = 0;
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
		if(heroGUI.currentHero != null)
		{
			heroScript = heroGUI.currentHero.GetComponent<HeroScriptParent> ();
			
			heroShip = heroGUI.currentHero.GetComponent<HeroShip>();
			
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

				UpdatePlanetInvasionValues(system);
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

	private void UpdatePlanetInvasionValues(int thisSystem)
	{
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].systemSize; i++)
		{	
			if(i < systemListConstructor.systemList[thisSystem].systemSize)
			{
				NGUITools.SetActive(planetList[i], true);
				
				if(systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == false || systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetCurrentDefence <= 0
				   || systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOwnership <= 0)
				{
					planetList[i].GetComponent<UIButton>().isEnabled = false;
					
					if(systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == false)
					{
						invasionInfo = "Uncolonised";
					}
				}
				
				else if(systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == true)
				{
					if(heroScript.heroType == "Infiltrator" && bombSelected == null)
					{
						planetList[i].GetComponent<UIButton>().isEnabled = false;
					}
		
					planetList[i].GetComponent<UIButton>().isEnabled = true;

					if(systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == true)
					{
						invasionInfo = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetName + "\n"
							+ systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetType + "\nOwnership: "
								+ systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOwnership + "\nDefence: "
								+ systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetCurrentDefence;
					}
				}
			}
			
			if(i >= systemListConstructor.systemList[thisSystem].systemSize)
			{
				NGUITools.SetActive(planetList[i], false);
				
				planetList[i].GetComponent<UIButton>().enabled = false;
			}
			
			planetList [i].GetComponent<UILabel> ().text = invasionInfo;
		}
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

						heroScript = heroGUI.currentHero.GetComponent<HeroScriptParent>();

						if((bombTimers[i] + (power / 4f) * heroScript.cooldownMod) <= Time.time || bombTimers[i] == 0.0f)
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
						}
					}
				}
			}
		}
	}

	public void OpenPlanetInvasionScreen()
	{
		NGUITools.SetActive (invasionScreen, true);

		heroScript = heroGUI.currentHero.GetComponent<HeroScriptParent> ();

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

	private void BombPlanet(int i, int system)
	{
		heroScript = heroGUI.currentHero.GetComponent<HeroScriptParent> ();
		DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (heroScript.heroOwnedBy, systemListConstructor.systemList [system].systemOwnedBy);
		temp.stateCounter -= 2;

		switch(bombSelected)
		{
		case "Fission":
			bombTimers[0] = Time.time;
			systemListConstructor.systemList[system].planetsInSystem[i].poisonActive = true;
			break;
		case "Fusion":
			bombTimers[1] = Time.time;
			systemListConstructor.systemList[system].planetsInSystem[i].maxOwnership -= 20;
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
		HeroScriptParent temp = heroGUI.currentHero.GetComponent<HeroScriptParent> ();

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{			
			if(planetList[i] == UIButton.current.gameObject)
			{
				if(UICamera.currentTouchID == -1)
				{
					systemListConstructor.systemList [system].planetsInSystem [i].planetCurrentDefence -= temp.primaryPower / 5f;
					systemListConstructor.systemList [system].planetsInSystem [i].planetOwnership -= temp.primaryPower / 5f;
					systemInvasion.PlanetInvasion(temp, system, i, true);
					break;
				}

				if(UICamera.currentTouchID == -2)
				{
					heroScript.planetInvade = i;
					systemInvasion.hero = temp;
					systemInvasion.PlanetInvasion(temp, temp.system, temp.planetInvade, false);

					if(bombSelected != null)
					{
						BombPlanet(i, temp.system);
					}

					break;
				}
			}
		}
	}
}
