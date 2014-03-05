using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, selectedHero, invasionButton, embargoButton, promoteButton, buttonContainer, turnInfoBar, levelUpPrefab, heroDetailsPrefab, heroDetailsContainer;
	public string[] heroLevelTwoSpecs = new string[3] {"Diplomat", "Soldier", "Infiltrator"};
	private string[] heroLevelThreeSpecs = new string[6] {"Ambassador", "Smuggler", "Vanguard", "Warlord", "Hacker", "Drone"};
	public GUISkin mySkin;
	public int heroCounter = 1, j;
	private float timer;
	private Rect[] tier1, tier2;

	void Update()
	{
		RaycastHit hit = new RaycastHit();
		
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					selectedHero = hit.collider.gameObject;
					heroShip = selectedHero.GetComponent<HeroShip>();
					heroScript = selectedHero.GetComponent<HeroScriptParent>();
				}
			}
		}
		
		if(Input.GetMouseButtonDown (1) && selectedHero != null)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{					
					heroMovement = selectedHero.GetComponent<HeroMovement> ();
					heroMovement.pathfindTarget = hit.collider.gameObject;
					heroMovement.FindPath();
				}
			}
		}
	}

	public void InvasionButtonClick()
	{
		heroScript.StartSystemInvasion();
	}

	public void Embargo()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.isPromoted = false;
		systemSIMData.isEmbargoed = true;
		systemSIMData.SystemSIMCounter(RefreshCurrentSystem(heroScript.heroLocation), playerTurnScript);
	}

	public void Promote()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.isEmbargoed = false;
		systemSIMData.isPromoted = true;
		systemSIMData.SystemSIMCounter(RefreshCurrentSystem(heroScript.heroLocation), playerTurnScript);
	}

	public void CheckIfCanHire()
	{
		if(playerTurnScript.capital >= 50 && playerTurnScript.playerOwnedHeroes.Count < 7)
		{
			int i = RefreshCurrentSystem(GameObject.Find(playerTurnScript.homeSystem));

			GameObject instantiatedHero = (GameObject)Instantiate (heroObject, systemListConstructor.systemList[i].systemObject.transform.position, 
			                                                       systemListConstructor.systemList[i].systemObject.transform.rotation);

			instantiatedHero.name = "Basic Hero";

			playerTurnScript.playerOwnedHeroes.Add (instantiatedHero);

			heroScript = instantiatedHero.GetComponent<HeroScriptParent>();

			heroMovement = instantiatedHero.GetComponent<HeroMovement>();

			heroScript.heroLocation = systemListConstructor.systemList[i].systemObject;

			heroScript.heroOwnedBy = playerTurnScript.playerRace;

			heroShip = instantiatedHero.GetComponent<HeroShip>();

			instantiatedHero.transform.position = heroMovement.HeroPositionAroundStar(heroScript.heroLocation);

			++heroCounter;

			playerTurnScript.capital -= 50;;
		}
	}

	public void OpenHeroDetails()
	{
		if(heroDetailsContainer.activeInHierarchy == false)
		{
			NGUITools.SetActive (heroDetailsContainer, true);

			for (int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; ++i)
			{
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

				NGUITools.SetActive(heroScript.heroDetails.window, true);

				heroScript.heroDetails.dropDownOne.enabled = false;
				heroScript.heroDetails.dropDownTwo.enabled = false;
				heroScript.heroDetails.dropDownOne.gameObject.GetComponent<UIButton>().enabled = false;
				heroScript.heroDetails.dropDownTwo.gameObject.GetComponent<UIButton>().enabled = false;

				if(heroScript.canLevelUp == true)
				{
					if(heroScript.currentLevel == 2 && heroScript.heroTier2 == "")
					{
						FillList(heroScript.heroDetails.dropDownOne, heroScript);
						heroScript.heroDetails.dropDownOne.gameObject.GetComponent<UIButton>().enabled = true;
						heroScript.heroDetails.dropDownOne.enabled = true;
					}

					if(heroScript.currentLevel == 3 && heroScript.heroTier3 == "")
					{
						FillList(heroScript.heroDetails.dropDownTwo, heroScript);
						heroScript.heroDetails.dropDownTwo.gameObject.GetComponent<UIButton>().enabled = true;
						heroScript.heroDetails.dropDownTwo.enabled = true;
					}
				}
			}
		}

		else if(heroDetailsContainer.activeInHierarchy == true)
		{
			NGUITools.SetActive (heroDetailsContainer, false);
		}
	}

	public void FillList(UIPopupList popup, HeroScriptParent hero)
	{
		if(hero.currentLevel == 2)
		{
			popup.items.Add("");
			popup.items.Add("Infiltrator");
			popup.items.Add("Soldier");
			popup.items.Add("Diplomat");
		}

		if(hero.currentLevel == 3)
		{
			popup.items.Add("");

			if(hero.heroTier2 == "Infiltrator")
			{
				popup.items.Add("Drone");
				popup.items.Add("Hacker");
			}
			if(hero.heroTier2 == "Soldier")
			{
				popup.items.Add("Warlord");
				popup.items.Add("Vanguard");
			}
			if(hero.heroTier2 == "Diplomat")
			{
				popup.items.Add("Ambassador");
				popup.items.Add("Merchant");
			}
		}
	}

	public void SetSpecialisation()
	{
		for(int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; ++i)
		{
			heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

			if(heroScript.canLevelUp == true)
			{
				if(heroScript.heroDetails.window = UIPopupList.current.transform.parent.gameObject)
				{
					string tempString = UIPopupList.current.gameObject.GetComponent<UIPopupList>().value;

					if(tempString != "")
					{
						if(UIPopupList.current.gameObject.name == "First Specialisation")
						{
							heroScript.heroTier2 = tempString;
						}

						if(UIPopupList.current.gameObject.name == "Second Specialisation")
						{					
							heroScript.heroTier3 = tempString;
						}

						heroScript.canLevelUp = false;

						UIPopupList.current.gameObject.GetComponent<UILabel>().text = tempString;

						UIPopupList.current.enabled = false;

						UIPopupList.current.gameObject.GetComponent<UIButton>().enabled = false;
					}
				}
			}
		}
	}
}

public class HeroDetailsWindow
{
	public GameObject window;
	public UIPopupList dropDownOne, dropDownTwo;
	public UILabel movementPoints;
}
