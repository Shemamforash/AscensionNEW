using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InvasionGUI : MasterScript
{
	public GameObject invasionScreen, background, summary, token, tokenContainer, includeHero2, includeHero3;
	public GameObject[] planetList = new GameObject[6];
	public GameObject[] heroInterfaces = new GameObject[3];
	public bool openInvasionMenu = false;

	private List<HeroInvasionLabel> heroInvasionLabels = new List<HeroInvasionLabel>();
	private List<PlanetInvasionLabels> planetInvasionLabels = new List<PlanetInvasionLabels>();
	private List<float> bombTimers = new List<float>();
	private int system;
	private float fissionBombTimer = 0, fusionBombTimer = 0, antimatterBombTimer = 0;
	private TokenBehaviour behaviour;
	private bool createdTokens;


	private void SetUpHeroesAndPlanets()
	{
		for(int i = 0; i < 3; ++i)
		{
			NGUITools.SetActive(heroInterfaces[i], true);

			HeroInvasionLabel temp = new HeroInvasionLabel();
			
			temp.assaultTokenContainer = heroInterfaces[i].transform.Find("Assault Tokens").gameObject;
			temp.auxiliaryTokenContainer = heroInterfaces[i].transform.Find("Auxiliary Tokens").gameObject;
			temp.defenceTokenContainer = heroInterfaces[i].transform.Find("Defence Tokens").gameObject;

			Transform summary = heroInterfaces[i].transform.Find("Summary");

			temp.reset = summary.transform.Find ("Buttons").transform.Find("Reset").gameObject;
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

	public void ResetTokens() //Method activated when reset buttons are pressed
	{
		for(int i = 0; i < heroInvasionLabels.Count; ++i) //For all active heroes
		{
			if(heroInvasionLabels[i].reset == UIButton.current.gameObject || UIButton.current.gameObject.name == "Reset All") //If button pressed corresponds to this hero, or the button was reset all
			{
				ResetTokenPositions(heroInvasionLabels[i].assaultTokensList, false); //Call the reset function with the gameobject lists containing the tokens
				ResetTokenPositions(heroInvasionLabels[i].auxiliaryTokensList, false);
				ResetTokenPositions(heroInvasionLabels[i].defenceTokensList, false);
			}
		}
	}

	private void ResetTokenPositions(List<GameObject> tokenList, bool remove) //Method used to reset the token parent and position
	{
		for(int i = 0; i < tokenList.Count; ++i) //For all the tokens in the token list
		{
			if(remove == false)
			{
				TokenUI token = tokenList[i].GetComponent<TokenUI>(); //Get a reference to the attached script
				
				tokenList[i].transform.position = token.originalPosition; //Set the position to the original position
				tokenList[i].transform.parent = token.originalParent.transform; //Set the parent to the original parent
			}
			if(remove == true)
			{
				NGUITools.Destroy(tokenList[i]);
			}
		}

		if(remove == true)
		{
			tokenList.Clear ();
		}
	}

	void Start () 
	{
		behaviour = tokenContainer.GetComponent<TokenBehaviour>();

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

	private void CreateTokens(string tokenType, HeroScriptParent hero)
	{
		int tokenCount = 0;
		string container = null;

		switch (tokenType)
		{
		case "Assault":
			tokenCount = hero.assaultTokens;
			container = "Assault Tokens";
			break;
		case "Auxiliary":
			tokenCount = hero.auxiliaryTokens;
			container = "Auxiliary Tokens";
			break;
		case "Defence":
			tokenCount = hero.defenceTokens;
			container = "Defence Tokens";
			break;
		default:
			break;
		}

		for(int i = 0; i < 6; ++i)
		{
			GameObject parent = heroInterfaces[0].transform.Find (container).gameObject;
			GameObject tokenPositionObject = parent.transform.GetChild(i).gameObject;

			if(i < tokenCount)
			{
				NGUITools.SetActive(tokenPositionObject, true);
				GameObject tempToken = NGUITools.AddChild(parent, token);
				EventDelegate.Add (tempToken.GetComponent<UIButton>().onClick, behaviour.ButtonClicked); //Add button clicked
				tempToken.transform.position = parent.transform.GetChild(i).transform.position;

				TokenUI tokenUI = tempToken.GetComponent<TokenUI>();
				tokenUI.originalPosition = tempToken.transform.position;
				tokenUI.originalParent = parent;
				tokenUI.hero = heroGUI.currentHero;

				UIButton tokenButton = tempToken.GetComponent<UIButton>();

				switch (tokenType)
				{
				case "Assault":
					tokenButton.normalSprite = "Primary Weapon Normal";
					tokenButton.hoverSprite = "Primary Weapon Hover";
					tokenButton.pressedSprite = "Primary Weapon Pressed";
					tokenButton.disabledSprite = "Primary Weapon Pressed";
					heroInvasionLabels[0].assaultTokensList.Add (tempToken);
					break;
				case "Auxiliary":
					tokenButton.normalSprite = "Secondary Weapon Normal";
					tokenButton.hoverSprite = "Secondary Weapon Hover";
					tokenButton.pressedSprite = "Secondary Weapon Pressed";
					tokenButton.disabledSprite = "Secondary Weapon Pressed";
					heroInvasionLabels[0].auxiliaryTokensList.Add (tempToken);
					break;
				case "Defence":
					tokenButton.normalSprite = "Defence Normal";
					tokenButton.hoverSprite = "Defence Hover";
					tokenButton.pressedSprite = "Defence Pressed";
					tokenButton.disabledSprite = "Defence Pressed";
					heroInvasionLabels[0].defenceTokensList.Add (tempToken);
					break;
				default:
					break;
				}
			}

			else
			{
				NGUITools.SetActive(tokenPositionObject, false);
			}
		}
	}

	public void CacheInvasionInfo() //Used to cache the invasion info
	{
		SystemInvasionInfo cachedInvasion = new SystemInvasionInfo (); //Create a new invasion object

		int invasionLoc = -1; //Set a counter

		for(int i = 0; i < systemInvasion.currentInvasions.Count; ++i) //For all current cached invasions
		{
			if(systemInvasion.currentInvasions[i].system == systemListConstructor.systemList[system].systemObject) //If this system already has an invasion underway
			{
				invasionLoc = i; //Set the counter
			}
		}

		cachedInvasion.system = systemListConstructor.systemList[system].systemObject; //Set the system to equal this system
		cachedInvasion.player = heroGUI.currentHero.GetComponent<HeroScriptParent> ().heroOwnedBy;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i) //For all planets in the system
		{
			PlanetInvasionInfo cachedPlanet = new PlanetInvasionInfo(); //Create object for planet

			foreach(Transform child in behaviour.tokenContainers[i]) //For all the containers within each planet
			{
				foreach(Transform subChild in child) //For all children within the container
				{
					if(subChild.name == "Label") //If the name of the child is not label
					{
						continue;
					}
					else //It must be a token
					{
						TokenUI tokenScript = subChild.GetComponent<TokenUI>(); //So get a reference to it's script

						switch(child.name) //Switch based on name of container
						{
						case "Defence Token":
							cachedPlanet.defenceTokenAllocation.Add (tokenScript.hero); //If it's a defence token cache the token's hero into the planet list
							break;
						case "Assault Token":
							cachedPlanet.assaultTokenAllocation.Add (tokenScript.hero); //Same for assault token
							break;
						case "Auxiliary Token":
							cachedPlanet.auxiliaryTokenAllocation.Add (tokenScript.hero); //And for auxiliary token
							break;
						default:
							break;
						}
					}
				}
			}

			cachedInvasion.tokenAllocation.Add (cachedPlanet);
		}

		if(invasionLoc == -1) //If this invasion is not already cached
		{
			systemInvasion.currentInvasions.Add (cachedInvasion); //Cache it
		}
		else //If it is
		{
			systemInvasion.currentInvasions[invasionLoc] = cachedInvasion; //Replace it with the updated one
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

				for(int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; ++i)
				{
					if(playerTurnScript.playerOwnedHeroes[i] == heroGUI.currentHero)
					{
						continue;
					}

					heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

					if(heroScript.heroLocation == systemListConstructor.systemList[system].systemObject)
					{
						if(includeHero2.activeInHierarchy == false)
						{
							NGUITools.SetActive(includeHero2, true);
							includeHero2.transform.Find ("Label").GetComponent<UILabel>().text = heroScript.heroType;
						}
						if(includeHero2.activeInHierarchy == true)
						{
							NGUITools.SetActive(includeHero3, true);
							includeHero3.transform.Find ("Label").GetComponent<UILabel>().text = heroScript.heroType;
						}
					}
				}

				if(background.activeInHierarchy == false) //Set the background to active
				{
					NGUITools.SetActive(background, true);
				}
				
				UpdatePlanetInvasionValues(system);
				UpdateHeroInterfaces();
				
				if(createdTokens == false)
				{
					CreateTokens("Assault", heroGUI.currentHero.GetComponent<HeroScriptParent>());
					CreateTokens("Auxiliary", heroGUI.currentHero.GetComponent<HeroScriptParent>());
					CreateTokens("Defence", heroGUI.currentHero.GetComponent<HeroScriptParent>());
					
					createdTokens = true;
				}
			}
		}
		
		if(openInvasionMenu == false)
		{
			NGUITools.SetActive(heroGUI.heroDetailsContainer, true);
			NGUITools.SetActive(includeHero2, false);
			NGUITools.SetActive(includeHero3, false);

			for(int i = 0; i < 3; ++i)
			{
				ResetTokenPositions(heroInvasionLabels[i].assaultTokensList, true); //Call the reset function with the gameobject lists containing the tokens
				ResetTokenPositions(heroInvasionLabels[i].auxiliaryTokensList, true);
				ResetTokenPositions(heroInvasionLabels[i].defenceTokensList, true);
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

	public void IncludeHero()
	{
		for(int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; ++i)
		{
			if(playerTurnScript.playerOwnedHeroes[i] == heroGUI.currentHero)
			{
				continue;
			}

			heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
			
			if(heroScript.heroLocation == systemListConstructor.systemList[system].systemObject)
			{
				if(UIButton.current.gameObject == includeHero2)
				{
					NGUITools.SetActive(includeHero2, true);
					includeHero2.transform.Find ("Label").GetComponent<UILabel>().text = heroScript.heroType;
				}
				if(UIButton.current.gameObject == includeHero3)
				{
					NGUITools.SetActive(includeHero3, true);
					includeHero3.transform.Find ("Label").GetComponent<UILabel>().text = heroScript.heroType;
				}

				CreateTokens("Assault", heroScript);
				CreateTokens("Auxiliary", heroScript);
				CreateTokens("Defence", heroScript);
				break;
			}
		}

		NGUITools.SetActive (UIButton.current.gameObject, false);
	}

	private void UpdateHeroInterfaces()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(includeHero2.activeInHierarchy == false && i == 1 || includeHero3.activeInHierarchy == false && i == 2)
			{
				NGUITools.SetActive (heroInterfaces [i], true);
				
				heroInvasionLabels [i].health.text = heroScript.currentHealth + "/" + heroScript.maxHealth + " HEALTH";
				heroInvasionLabels [i].name.text = "A HERO";
				heroInvasionLabels [i].type.text = heroScript.heroType;
				
				heroInvasionLabels [i].assaultDamage.text = Math.Round (heroScript.assaultDamage, 0) + " ASSAULT DAMAGE";
				heroInvasionLabels [i].assaultDamagePerToken.text = Math.Round (heroScript.assaultDamage / (float)heroScript.assaultTokens, 0) + " PER";
				heroInvasionLabels [i].auxiliaryDamage.text = Math.Round (heroScript.auxiliaryDamage, 0) + " AUXILIARY DAMAGE";
				heroInvasionLabels [i].auxiliaryDamagePerToken.text = Math.Round (heroScript.auxiliaryDamage / (float)heroScript.auxiliaryTokens, 0) + " PER";
				heroInvasionLabels [i].defence.text = Math.Round (heroScript.defence, 0) + " DEFENCE";
				heroInvasionLabels [i].defencePerToken.text = Math.Round (heroScript.defence / (float)heroScript.defenceTokens, 0) + " PER";
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
		public GameObject defenceTokenContainer, assaultTokenContainer, auxiliaryTokenContainer, reset;
		public List<GameObject> defenceTokensList = new List<GameObject>();
		public List<GameObject> assaultTokensList = new List<GameObject>();
		public List<GameObject> auxiliaryTokensList = new List<GameObject>();
		public UILabel defence, defencePerToken, assaultDamage, assaultDamagePerToken, auxiliaryDamage, auxiliaryDamagePerToken, health, type, name;
	}
}
