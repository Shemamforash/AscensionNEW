using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, invasionButton, embargoButton, promoteButton, buttonContainer, turnInfoBar, heroDetailsContainer, currentHero, guardButton;
	public UILabel heroHealth, heroName;
	private RaycastHit hit;

	void Update()
	{
		hit = new RaycastHit();
		
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					currentHero = hit.collider.gameObject;
					heroShip = currentHero.GetComponent<HeroShip>();
					heroScript = currentHero.GetComponent<HeroScriptParent>();

					if(heroScript.heroOwnedBy != playerTurnScript.playerRace)
					{
						currentHero = null;
					}
				}
			}
		}

		if(Input.GetMouseButtonDown (1) && currentHero != null)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{
					heroMovement = currentHero.GetComponent<HeroMovement> ();
					heroMovement.FindPath(heroScript.heroLocation, hit.collider.gameObject);
					if(heroScript.invasionObject != null)
					{
						Destroy (heroScript.invasionObject);
					}
				}
			}
		}

		ShowHeroDetails();

		if(currentHero != null)
		{
			heroShip.UpdateButtons();
		}
	}

	public void ShowHeroDetails()
	{
		if(currentHero != null)
		{
			heroScript = currentHero.GetComponent<HeroScriptParent> ();
			NGUITools.SetActive(heroDetailsContainer, true);
			heroHealth.text = Math.Round(heroScript.currentArmour, 1) + "/" + Math.Round(heroScript.maxArmour, 1);
			heroName.text = "Hero Dude/Ette";
		}
		if(currentHero == null)
		{
			NGUITools.SetActive(heroDetailsContainer, false);
		}
	}

	public void InvasionButtonClick()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemInvasion.hero = heroScript;
		systemInvasion.StartSystemInvasion(heroScript.system);
	}

	public void Embargo()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.promotedBy = null;
		systemSIMData.embargoedBy = heroScript.heroOwnedBy;
		systemSIMData.embargoTimer = Time.time;
	}

	public void Promote()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.embargoedBy = null;
		systemSIMData.promotedBy = heroScript.heroOwnedBy;
		systemSIMData.promotionTimer = Time.time;
	}

	public void GuardProtectSwitch()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		if(guardButton.GetComponent<UILabel>().text == "Guard")
		{
			systemSIMData.guardedBy = heroScript.heroOwnedBy;
		}
		if(guardButton.GetComponent<UILabel>().text == "Protect")
		{
			systemSIMData.protectedBy = heroScript.gameObject;
		}
	}
}
