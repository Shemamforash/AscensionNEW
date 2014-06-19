using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, fissionBombButton, fusionBombButton, antimatterBombButton, background;
	public GameObject[] planetList = new GameObject[6];
	public GameObject[] heroInterfaces = new GameObject[3];
	public bool openInvasionMenu = false;

	private List<HeroInvasionLabel> heroInvasionLabels = new List<HeroInvasionLabel>();
	private List<PlanetInvasionLabels> planetInvasionLabels = new List<PlanetInvasionLabels>();
	private List<GameObject> bombButtons = new List<GameObject> ();
	private List<float> bombTimers = new List<float>();
	private int system;
	private string bombSelected;
	private float fissionBombTimer = 0, fusionBombTimer = 0, antimatterBombTimer = 0;

	void Start () 
	{
		NGUITools.SetActive (invasionScreen, true);

		bombButtons.Add (fissionBombButton);
		bombButtons.Add (fusionBombButton);
		bombButtons.Add (antimatterBombButton);

		bombTimers.Add (fissionBombTimer);
		bombTimers.Add (fusionBombTimer);
		bombTimers.Add (antimatterBombTimer);

		for(int i = 0; i < 3; ++i)
		{
			HeroInvasionLabel temp = new HeroInvasionLabel();

			temp.assaultTokens = heroInterfaces[i].transform.Find("Assault Tokens").gameObject;
			temp.auxiliaryTokens = heroInterfaces[i].transform.Find("Auxiliary Tokens").gameObject;
			temp.defenceTokens = heroInterfaces[i].transform.Find("Defence Tokens").gameObject;

			Transform summary = heroInterfaces[i].transform.Find("Summary");

			temp.assaultDamage = summary.Find ("Assault Damage").GetComponent<UILabel>();
			temp.assaultDamagePerToken = temp.assaultDamage.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.auxiliaryDamage = summary.Find ("Auxiliary Damage").GetComponent<UILabel>();
			temp.auxiliaryDamagePerToken = temp.auxiliaryDamage.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.defence = summary.Find ("Defence").GetComponent<UILabel>();
			temp.defencePerToken = temp.defence.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.health = summary.Find ("Health").GetComponent<UILabel>();
			temp.name = summary.Find ("Name").GetComponent<UILabel>();
			temp.type = summary.Find ("Type").GetComponent<UILabel>();

			heroInvasionLabels.Add (temp);
		}

		for(int i = 0; i < 6; ++i)
		{
			PlanetInvasionLabels temp = new PlanetInvasionLabels();

			Transform info = planetList[i].transform.Find ("Info");

			temp.name = info.Find("Name").GetComponent<UILabel>();
			temp.type = info.Find("Type").GetComponent<UILabel>();
			temp.offence = info.Find("Offence").GetComponent<UILabel>();
			temp.defence = info.Find("Defence").GetComponent<UILabel>();
			temp.population = info.Find("Population").GetComponent<UILabel>();
			planetInvasionLabels.Add (temp);
		}
	}

	private void LayoutPlanets(int size) //Used to position the list of planets to be invaded
	{
		float maxHeight = ((size - 1) * 10f) + (65f * size); //Height of all planet sprites
		maxHeight = maxHeight / 2f; //Over 2 to get difference from y = 0

		for(int i = 0; i < 6; ++i) //For all planet sprites
		{
			if(i < size) //If it is less than the system size
			{
				NGUITools.SetActive(planetList[i], true); //Activate it
				float y = (i * 75f) + 32.5f; //Its y height is it's iterator value multiplied by 75, +32.5
				y = maxHeight - y; //Get the actual y value by subtracting it from maxheight
				Vector3 temp = new Vector3(-85f, y, 0f); //Create a new vector with a -85 x offset to centre the sprite
				planetList[i].transform.localPosition = temp; //Set the local position
			}
			else //If it's greater than the system size
			{
				NGUITools.SetActive(planetList[i], false); //Deactivate it
			}
		}
	}

	void Update()
	{
		if(heroGUI.currentHero != null) //If a hero is selected
		{
			heroScript = heroGUI.currentHero.GetComponent<HeroScriptParent> (); //Get references to hero scripts
			
			heroShip = heroGUI.currentHero.GetComponent<HeroShip>();
			
			systemDefence = heroScript.heroLocation.GetComponent<SystemDefence> (); //And the defence script of that system
			
			if(systemDefence.underInvasion == true && openInvasionMenu == true) //If system is under invasion and the invasion menu is open
			{
				NGUITools.SetActive(heroGUI.heroDetailsContainer, false); //Close the hero details window

				if(background.activeInHierarchy == false) //Set the background to active
				{
					NGUITools.SetActive(background, true);
				}

				if(bombSelected != null) //If a bomb is selected
				{
					for(int i = 0; i < bombButtons.Count; ++i) //Disable all the bomb buttons
					{
						bombButtons[i].GetComponent<UIButton> ().isEnabled = false;
					}
				}

				if(heroShip.canViewSystem == true) //Not sure what this does
				{
					UpdateBombButton();
				}

				else if (heroShip.canViewSystem == false)
				{
					for(int i = 0; i < bombButtons.Count; ++i)
					{
						NGUITools.SetActive(bombButtons[i], false);
					}
				}

				UpdatePlanetInvasionValues(system);
			}
			
			if(systemDefence.underInvasion == false)
			{
				for(int i = 0; i < planetList.Length; ++i)
				{
					NGUITools.SetActive (planetList[i], false);
				}
			}
		}
		
		if(openInvasionMenu == false)
		{
			NGUITools.SetActive(heroGUI.heroDetailsContainer, true);

			for(int i = 0; i < bombButtons.Count; ++i)
			{
				NGUITools.SetActive(bombButtons[i], false);
			}

			for(int i = 0; i < planetList.Length; ++i)
			{
				NGUITools.SetActive (planetList[i], false);
			}

			if(background.activeInHierarchy == true)
			{
				NGUITools.SetActive(background, false);
			}
		}
	}

	private void UpdatePlanetInvasionValues(int thisSystem) //Used to update the labels of the planets
	{
		for(int i = 0; i < 6; i++) //For all labels
		{	
			if(i < systemListConstructor.systemList[thisSystem].systemSize) //If this planet is active in the system
			{
				NGUITools.SetActive(planetList[i], true); //Set the label container to be active

				planetInvasionLabels[i].name.text = systemListConstructor.systemList[thisSystem].systemName.ToUpper() + " " + i; //Set it's name
				planetInvasionLabels[i].type.text = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetType.ToUpper(); //And it's type

				if(systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == false) //If it's not colonised
				{
					planetInvasionLabels[i].defence.text = "NA"; //Set appropriate values for defence, offence, and population
					planetInvasionLabels[i].offence.text = "NA";
					planetInvasionLabels[i].population.text = "UNCOLONISED";
					continue;
				}

				planetInvasionLabels[i].defence.text = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetCurrentDefence.ToString() + "/"
					+ systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetMaxDefence.ToString () + " DEFENCE"; //Else display the current values of defence, offence, and population
				planetInvasionLabels[i].offence.text = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOffence.ToString ();
				planetInvasionLabels[i].population.text = systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetPopulation + "%/" 
					+ systemListConstructor.systemList[thisSystem].planetsInSystem[i].maxPopulation + "% POPULATION";
			}
			
			if(i >= systemListConstructor.systemList[thisSystem].systemSize) //If the planet is not active i.e the label number is greater than the system size
			{
				NGUITools.SetActive(planetList[i], false); //Set the label container to inactive
			}
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

		LayoutPlanets(systemListConstructor.systemList[system].systemSize);
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
			systemListConstructor.systemList[system].planetsInSystem[i].maxPopulation -= 20;
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
				if(UICamera.currentTouchID == -1) //Left Click
				{
					systemListConstructor.systemList [system].planetsInSystem [i].planetCurrentDefence -= temp.primaryPower / 5f;
					systemListConstructor.systemList [system].planetsInSystem [i].planetPopulation -= temp.primaryPower / 5f;
					systemInvasion.PlanetInvasion(temp, system, i, true);
					break;
				}

				if(UICamera.currentTouchID == -2) //Right Click
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

	private class PlanetInvasionLabels
	{
		public UILabel offence, defence, name, population, type; 
	}
	private class HeroInvasionLabel
	{
		public GameObject defenceTokens, assaultTokens, auxiliaryTokens;
		public UILabel defence, defencePerToken, assaultDamage, assaultDamagePerToken, auxiliaryDamage, auxiliaryDamagePerToken, health, type, name;
	}
}
