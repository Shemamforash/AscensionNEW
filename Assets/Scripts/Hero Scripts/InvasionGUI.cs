using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, grid, bombButton;
	private List<GameObject> planetList = new List<GameObject>();
	private int system;
	private string invasionInfo;
	public bool openInvasionMenu = false;
	private bool bombSelected = false;
	private float bombTimer;

	void Start () 
	{
		NGUITools.SetActive (invasionScreen, true);

		string[] tempString = new string[6] {"Planet Invade 1", "Planet Invade 2", "Planet Invade 3", "Planet Invade 4", "Planet Invade 5", "Planet Invade 6"};

		for(int i = 0; i < 6; ++i)
		{
			GameObject temp = invasionScreen.transform.Find ("Planet Scroll Window").transform.Find ("Grid").transform.Find (tempString[i]).gameObject;

			planetList.Add (temp);
		}

		NGUITools.SetActive (invasionScreen, false);
	}

	void Update()
	{
		if(heroGUI.selectedHero != null)
		{
			heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();

			systemDefence = heroScript.heroLocation.GetComponent<SystemDefence> ();

			if(systemDefence.underInvasion == true && openInvasionMenu == true)
			{
				if(heroScript.heroTier2 == "Infiltrator")
				{
					NGUITools.SetActive(bombButton, true);

					if((bombTimer + (heroScript.secondaryPower / 8)) < Time.time || bombTimer == 0.0f)
					{
						bombButton.GetComponent<UIButton>().isEnabled = true;
						bombTimer = 0.0f;
					}
					
					if(heroScript.secondaryPower == 0)
					{
						bombButton.GetComponent<UIButton>().isEnabled = false;
					}

					if(bombTimer != 0.0f)
					{
						bombButton.GetComponent<UISprite>().fillAmount = heroScript.secondaryPower / ((Time.time - bombTimer) * 8);
						Debug.Log (bombButton.GetComponent<UISprite>().fillAmount);
					}
				}

				else
				{
					NGUITools.SetActive(bombButton, false);
				}

				for(int i = 0; i < systemListConstructor.systemList[system].systemSize; i++)
				{	
					if(i < systemListConstructor.systemList[system].systemSize)
					{
						NGUITools.SetActive(planetList[i], true);

						if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false || 
						   systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership == 0 ||
						   systemListConstructor.systemList[system].planetsInSystem[i].planetDefence == 0)
						{
							planetList[i].GetComponent<UIButton>().enabled = false;

							if(systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false)
							{
								invasionInfo = "Uncolonised";
							}
						}

						else
						{

							if(i == heroScript.planetInvade || bombTimer != 0.0f)
							{
								planetList[i].GetComponent<UIButton>().enabled = false;
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
					
					else if(i >= systemListConstructor.systemList[system].systemSize)
					{
						NGUITools.SetActive(planetList[i], false);
						
						planetList[i].GetComponent<UIButton>().enabled = false;
					}

					planetList[i].GetComponent<UILabel>().text = invasionInfo;
				}
			}

			if(systemDefence.underInvasion == false)
			{
				NGUITools.SetActive(invasionScreen, false);
			}
		}

		if(openInvasionMenu == false)
		{
			NGUITools.SetActive(bombButton, false);
			NGUITools.SetActive(invasionScreen, false);
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
		bombSelected = true;
	}

	public void BombSystem()
	{
		for(int i = 0; i < 6; ++i)
		{			
			if(planetList[i] == UIButton.current.gameObject)
			{
				heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();
				heroScript.planetInvade = i;
				heroScript.PlanetInvasion();
				heroScript.planetInvade = -1;
				bombSelected = false;
				bombTimer = Time.time;
			}
		}
	}
	
	public void PlanetInvasionClick()
	{
		if(bombSelected == true)
		{
			BombSystem();
		}

		else
		{
			for(int i = 0; i < 6; ++i)
			{			
				if(planetList[i] == UIButton.current.gameObject)
				{
					heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent> ();
					heroScript.planetInvade = i;
					heroScript.PlanetInvasion();
					break;
				}
			}
		}
	}
}
