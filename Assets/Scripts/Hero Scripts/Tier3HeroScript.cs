using UnityEngine;
using System.Collections;

public class Tier3HeroScript : MasterScript
{
	public DiplomaticPosition tempObject;

	public GameObject[] linkableSystems = new GameObject[60];
	private Vector2 scrollPosition = Vector2.zero;
	public bool openSystemLinkScreen, linkableSystemsExist;
	private int availableSystems;

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
		GUI.skin = systemGUI.mySkin;

		if(openSystemLinkScreen == true && linkableSystemsExist == true)
		{
			heroScript = heroGUI.selectedHero.GetComponent<HeroScriptParent>();

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
					SetUpTradeRoute (systemGUI.selectedSystem, heroGUI.selectedHero, i);

					openSystemLinkScreen = false;
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}
	}

	void SetUpTradeRoute(int thisSystem, GameObject thisHero, int targetSystem)
	{
		systemListConstructor.systemList [targetSystem].tradeRoute = thisHero;

		heroScript = thisHero.GetComponent<HeroScriptParent> ();

		heroScript.linkedHeroObject = systemListConstructor.systemList [targetSystem].systemObject;

		heroScript.CreateConnectionLine ();
	}
}
