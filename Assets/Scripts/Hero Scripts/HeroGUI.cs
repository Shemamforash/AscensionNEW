using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, selectedHero, invasionButton, embargoButton, promoteButton, buttonContainer, turnInfoBar, heroDetailsContainer;
	public UILabel heroHealth, heroName;
	public int heroCounter = 1;

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

	public void CheckIfCanHire(TurnInfo player, string heroType)
	{
		if(player.capital >= 50 && player.playerOwnedHeroes.Count < 7)
		{
			int i = RefreshCurrentSystem(GameObject.Find(player.homeSystem));

			GameObject instantiatedHero = (GameObject)Instantiate (heroObject, systemListConstructor.systemList[i].systemObject.transform.position, 
			                                                       systemListConstructor.systemList[i].systemObject.transform.rotation);

			instantiatedHero.name = "Basic Hero";

			heroScript = instantiatedHero.GetComponent<HeroScriptParent>();

			heroScript.heroType = heroType;

			heroMovement = instantiatedHero.GetComponent<HeroMovement>();

			heroScript.heroLocation = systemListConstructor.systemList[i].systemObject;

			heroScript.heroOwnedBy = player.playerRace;

			heroShip = instantiatedHero.GetComponent<HeroShip>();

			instantiatedHero.transform.position = heroMovement.HeroPositionAroundStar(heroScript.heroLocation);

			++heroCounter;

			player.capital -= 50;

			player.playerOwnedHeroes.Add (instantiatedHero);

			switch(heroScript.heroType)
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
	}
}
