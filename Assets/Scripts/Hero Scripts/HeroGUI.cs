using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen, listFilled;
	public GameObject heroObject, merchantQuad, selectedHero, invasionButton, embargoButton, promoteButton, buttonContainer, turnInfoBar, levelUpPrefab, heroDetailsContainer;
	public UILabel heroHealth, heroName;
	public UIPopupList dropDownOne, dropDownTwo;
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
		ShowHeroDetails();
	}

	public void ShowHeroDetails()
	{
		if(selectedHero != null)
		{
			NGUITools.SetActive(heroDetailsContainer, true);
			heroHealth.text = Math.Round(heroScript.currentArmour, 1) + "/" + Math.Round(heroScript.maxArmour, 1);
			heroName.text = "Hero Dude/Ette";

			if(heroScript.canLevelUp == false)
			{
				if(heroScript.heroTier2 != "")
				{
					dropDownOne.value = heroScript.heroTier2;
				}
				else if(heroScript.heroTier2 == "")
				{
					dropDownOne.value = "Unspecialised";
				}
				if(heroScript.heroTier3 != "")
				{
					dropDownTwo.value = heroScript.heroTier3;
				}
				else if(heroScript.heroTier3 == "")
				{
					dropDownTwo.value = "Unspecialised";
				}

				dropDownOne.enabled = false;
				dropDownTwo.enabled = false;
				dropDownOne.gameObject.GetComponent<UIButton>().enabled = false;
				dropDownTwo.gameObject.GetComponent<UIButton>().enabled = false;
			}
			
			if(heroScript.canLevelUp == true)
			{
				if(heroScript.currentLevel == 2 && heroScript.heroTier2 == "" && listFilled == false)
				{
					FillList(dropDownOne);
					listFilled = true;
					dropDownOne.gameObject.GetComponent<UIButton>().enabled = true;
					dropDownOne.enabled = true;
				}
				
				if(heroScript.currentLevel == 3 && heroScript.heroTier3 == "" && heroScript.heroTier2 != "" && listFilled == false)
				{
					FillList(dropDownTwo);
					listFilled = true;
					dropDownTwo.gameObject.GetComponent<UIButton>().enabled = true;
					dropDownTwo.enabled = true;
				}
			}
	
		}
		if(selectedHero == null)
		{
			NGUITools.SetActive(heroDetailsContainer, false);
		}
	}

	public void InvasionButtonClick()
	{
		systemInvasion.hero = heroScript;
		systemInvasion.StartSystemInvasion(heroScript.system);
	}

	public void Embargo()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.promotedBy = null;
		systemSIMData.embargoedBy = heroScript.heroOwnedBy;
		systemSIMData.embargoTimer = Time.time;
	}

	public void Promote()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.embargoedBy = null;
		systemSIMData.promotedBy = heroScript.heroOwnedBy;
		systemSIMData.promotionTimer = Time.time;
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

	public void FillList(UIPopupList popup)
	{
		popup.items.Clear ();

		if(heroScript.currentLevel == 2)
		{
			popup.items.Add("");
			popup.items.Add("Infiltrator");
			popup.items.Add("Soldier");
			popup.items.Add("Diplomat");
		}

		if(heroScript.currentLevel == 3)
		{
			popup.items.Add("");

			if(heroScript.heroTier2 == "Infiltrator")
			{
				popup.items.Add("Drone");
				popup.items.Add("Hacker");
			}
			if(heroScript.heroTier2 == "Soldier")
			{
				popup.items.Add("Warlord");
				popup.items.Add("Vanguard");
			}
			if(heroScript.heroTier2 == "Diplomat")
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
			if(playerTurnScript.playerOwnedHeroes[i] == heroGUI.selectedHero)
			{
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

				if(heroScript.canLevelUp == true)
				{
					string tempString = UIPopupList.current.gameObject.GetComponent<UIPopupList>().value;

					if(tempString != "")
					{
						if(UIPopupList.current.gameObject.name == "First Specialisation")
						{
							heroScript.heroTier2 = tempString;

							switch(heroScript.heroTier2)
							{
							case "Soldier":
								heroScript.classModifier = 1.75f;
								break;
							case "Infiltrator":
								heroScript.classModifier = 1f;
								break;
							case "Diplomat":
								heroScript.classModifier = 1.5f;
								break;
							}
						}

						if(UIPopupList.current.gameObject.name == "Second Specialisation")
						{					
							heroScript.heroTier3 = tempString;
						}

						heroScript.canLevelUp = false;

						listFilled = false;
					}
				}
			}
		}
	}
}
