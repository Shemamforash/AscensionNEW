using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, background, summary, token;
	public GameObject[] planetList = new GameObject[6];
	public GameObject[] heroInterfaces = new GameObject[3];
	public bool openInvasionMenu = false;

	private List<HeroInvasionLabel> heroInvasionLabels = new List<HeroInvasionLabel>();
	private List<PlanetInvasionLabels> planetInvasionLabels = new List<PlanetInvasionLabels>();
	private List<float> bombTimers = new List<float>();
	private List<GameObject> activeTokens = new List<GameObject> ();
	private int system;
	private float fissionBombTimer = 0, fusionBombTimer = 0, antimatterBombTimer = 0;

	private void SetUpHeroesAndPlanets()
	{
		for(int i = 0; i < 3; ++i)
		{
			NGUITools.SetActive(heroInterfaces[i], true);

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

			NGUITools.SetActive(heroInterfaces[i], false);
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

	void Start () 
	{
		NGUITools.SetActive (invasionScreen, true);

		bombTimers.Add (fissionBombTimer);
		bombTimers.Add (fusionBombTimer);
		bombTimers.Add (antimatterBombTimer);

		SetUpHeroesAndPlanets ();
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

	private void CreateTokens(string tokenType, SystemInvasionInfo temp)
	{
		int tokenCount = 0;
		string container = null;

		switch (tokenType)
		{
		case "Assault":
			tokenCount = heroScript.assaultTokens;
			container = "Assault Tokens";
			break;
		case "Auxiliary":
			tokenCount = heroScript.auxiliaryTokens;
			container = "Auxiliary Tokens";
			break;
		case "Defence":
			tokenCount = heroScript.defenceTokens;
			container = "Defence Tokens";
			break;
		default:
			break;
		}

		for(int i = 0; i < 6; ++i)
		{
			GameObject parent = heroInterfaces[0].transform.Find (container).GetChild(i).gameObject;

			if(i < tokenCount)
			{
				NGUITools.SetActive(parent, true);
				GameObject tempToken = NGUITools.AddChild(parent, token);
				tempToken.transform.position = parent.transform.position;
				activeTokens.Add (tempToken);

				UIButton tokenButton = tempToken.GetComponent<UIButton>();

				switch (tokenType)
				{
				case "Assault":
					tokenButton.normalSprite = "Primary Weapon Normal";
					tokenButton.hoverSprite = "Primary Weapon Hover";
					tokenButton.pressedSprite = "Primary Weapon Pressed";
					tokenButton.disabledSprite = "Primary Weapon Pressed";
					break;
				case "Auxiliary":
					tokenButton.normalSprite = "Secondary Weapon Normal";
					tokenButton.hoverSprite = "Secondary Weapon Hover";
					tokenButton.pressedSprite = "Secondary Weapon Pressed";
					tokenButton.disabledSprite = "Secondary Weapon Pressed";
					break;
				case "Defence":
					tokenButton.normalSprite = "Defence Normal";
					tokenButton.hoverSprite = "Defence Hover";
					tokenButton.pressedSprite = "Defence Pressed";
					tokenButton.disabledSprite = "Defence Pressed";
					break;
				default:
					break;
				}
			}
			else
			{
				NGUITools.SetActive(parent, false);
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
				NGUITools.SetActive(summary, true);

				if(background.activeInHierarchy == false) //Set the background to active
				{
					NGUITools.SetActive(background, true);
				}

				UpdatePlanetInvasionValues(system);
				UpdateHeroInterfaces();

				if(activeTokens.Count == 0)
				{
					SystemInvasionInfo temp = new SystemInvasionInfo();

					CreateTokens("Assault", temp);
					CreateTokens("Auxiliary", temp);
					CreateTokens("Defence", temp);
				}
			}
		}
		
		if(openInvasionMenu == false)
		{
			activeTokens.Clear ();

			NGUITools.SetActive(heroGUI.heroDetailsContainer, true);

			for(int i = 0; i < 3; ++i)
			{
				NGUITools.SetActive(heroInterfaces[i], false);
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

	private void UpdateHeroInterfaces()
	{
		NGUITools.SetActive (heroInterfaces [0], true);

		heroInvasionLabels [0].health.text = heroScript.currentHealth + "/" + heroScript.maxHealth + " HEALTH";
		heroInvasionLabels [0].name.text = "A HERO";
		heroInvasionLabels [0].type.text = heroScript.heroType;

		heroInvasionLabels [0].assaultDamage.text = Math.Round (heroScript.assaultDamage, 0) + " ASSAULT DAMAGE";
		heroInvasionLabels [0].assaultDamagePerToken.text = Math.Round (heroScript.assaultDamage / (float)heroScript.assaultTokens, 0) + " PER";
		heroInvasionLabels [0].auxiliaryDamage.text = Math.Round (heroScript.auxiliaryDamage, 0) + " AUXILIARY DAMAGE";
		heroInvasionLabels [0].auxiliaryDamagePerToken.text = Math.Round (heroScript.auxiliaryDamage / (float)heroScript.auxiliaryTokens, 0) + " PER";
		heroInvasionLabels [0].defence.text = Math.Round (heroScript.defence, 0) + " DEFENCE";
		heroInvasionLabels [0].defencePerToken.text = Math.Round (heroScript.defence / (float)heroScript.defenceTokens, 0) + " PER";
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

	/*
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
	*/

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
