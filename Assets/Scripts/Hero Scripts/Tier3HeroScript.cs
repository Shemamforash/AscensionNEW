using UnityEngine;
using System.Collections;

public class Tier3HeroScript : HeroScriptParent 
{
	public GameObject[] linkableSystems = new GameObject[60];
	private Vector2 scrollPosition = Vector2.zero;
	public bool openSystemLinkScreen, linkableSystemsExist;
	private int availableSystems;

	public void CheckTier3Heroes(GameObject selectedHero)
	{
		heroScript = selectedHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData>();
		tempObject = heroScript.FindDiplomaticConnection ();

		if(heroScript.heroTier2 == "Diplomat")
		{
			heroScript.heroSciBonus += 0.10f * systemSIMData.tempTotalSci;
			heroScript.heroIndBonus += 0.10f * systemSIMData.tempTotalInd;
			heroScript.heroMonBonus += 0.20f * systemSIMData.tempTotalMon;

			heroScript.offensivePower = 0.0f;
			heroScript.defensivePower = 0.0f;

			if(heroScript.heroTier3 == "Ambassador")
			{
				Ambassador ();
			}
			if(heroScript.heroTier3 == "Smuggler")
			{
				Smuggler ();
			}
		}
		if(heroScript.heroTier2 == "Infiltrator")
		{
			heroScript.heroSciBonus += 0.20f * systemSIMData.tempTotalSci;
			heroScript.heroIndBonus += 0.10f * systemSIMData.tempTotalInd;
			heroScript.heroMonBonus += 0.10f * systemSIMData.tempTotalMon;

			if(heroScript.heroTier3 == "Hacker")
			{
				Hacker ();
			}
			if(heroScript.heroTier3 == "Drone")
			{
				Drone ();
			}
		}
		if(heroScript.heroTier2 == "Soldier")
		{
			heroScript.heroSciBonus += 0.10f * systemSIMData.tempTotalSci;
			heroScript.heroIndBonus += 0.20f * systemSIMData.tempTotalInd;
			heroScript.heroMonBonus += 0.10f * systemSIMData.tempTotalMon;

			heroScript.offensivePower += 14.0f;
			heroScript.defensivePower += 14.0f;

			if(heroScript.heroTier3 == "Warlord")
			{
				Warlord ();
			}
			if(heroScript.heroTier3 == "Vanguard")
			{
				Vanguard ();
			}
		}
	}

	public void Smuggler()
	{
		if(heroScript.linkedHeroObject != null)
		{
			int i = RefreshCurrentSystem(heroScript.linkedHeroObject);

			systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			
			heroScript.heroSciBonus += systemSIMData.tempTotalSci / 2;
			heroScript.heroIndBonus += systemSIMData.tempTotalInd / 2;
			heroScript.heroMonBonus += systemSIMData.tempTotalMon / 2;
		}
	}

	public void Ambassador()
	{
		heroScript.heroSciBonus += systemSIMData.tempTotalSci * 0.25f;
		heroScript.heroIndBonus += systemSIMData.tempTotalInd * 0.25f;
		heroScript.heroMonBonus += systemSIMData.tempTotalMon * 0.25f;
	}

	public void Hacker()
	{
	}
	public void Drone()
	{
	}

	public void Warlord()
	{
		heroScript.offensivePower += 14.0f;
		heroScript.defensivePower -= 7.0f;
	}

	public void Vanguard()
	{
		heroScript.offensivePower -= 7.0f;
		heroScript.defensivePower += 14.0f;
	}

	public void FillLinkableSystems()
	{
		for(int i = 0; i < 60; ++i)
		{
			linkableSystems[i] = null;

			if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace || systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace && systemListConstructor.systemList[i].tradeRoute == null)
			{
				if(systemListConstructor.systemList[i].systemObject != heroScript.heroLocation && heroScript.linkedHeroObject == null)
				{
					linkableSystems[i] = systemListConstructor.systemList[i].systemObject;
					availableSystems++;
				}
			}
		}

		if(availableSystems > 0)
		{
			linkableSystemsExist = true;
		}
	}

	void OnGUI()
	{
		GUI.skin = mainGUIScript.mySkin;

		if(openSystemLinkScreen == true && linkableSystemsExist == true)
		{
			heroScript = systemListConstructor.systemList[mainGUIScript.selectedSystem].heroesInSystem[heroGUIScript.selectedHero].GetComponent<HeroScriptParent>();

			if(GUI.Button (new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 - 270.0f, 20.0f, 20.0f), "X"))
			{
				openSystemLinkScreen = false;
			}

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 250.0f, 200.0f, 500.0f));

			GUILayout.Box ("Start Trade With:");

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			for(int i = 0; i < 60; ++i)
			{
				if(linkableSystems[i] == null || linkableSystems[i] == heroScript.heroLocation)
				{
					continue;
				}

				if(GUILayout.Button (linkableSystems[i].name, GUILayout.Height (50.0f)))
				{
					SetUpTradeRoute (mainGUIScript.selectedSystem, heroGUIScript.selectedHero, i);

					openSystemLinkScreen = false;
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}
	}

	void SetUpTradeRoute(int thisSystem, int thisHero, int targetSystem)
	{
		systemListConstructor.systemList [targetSystem].tradeRoute = systemListConstructor.systemList [thisSystem].heroesInSystem [thisHero];

		heroScript = systemListConstructor.systemList [thisSystem].heroesInSystem [thisHero].GetComponent<HeroScriptParent> ();

		heroScript.linkedHeroObject = systemListConstructor.systemList [targetSystem].systemObject;

		heroScript.CreateConnectionLine ();
	}
}
