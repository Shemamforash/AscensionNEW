using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tier3HeroScript : MasterScript
{
	public DiplomaticPosition tempObject;

	public List<GameObject> linkableSystems = new List<GameObject>();
	private Vector2 scrollPosition = Vector2.zero;
	public bool openSystemLinkScreen, linkableSystemsExist;
	private int availableSystems;

	public void FillLinkableSystems()
	{
		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			linkableSystems[i] = null;

			if(systemListConstructor.systemList[i].systemOwnedBy == turnInfoScript.allPlayers[0].playerRace || 
			   systemListConstructor.systemList[i].systemOwnedBy == turnInfoScript.allPlayers[1].playerRace && systemListConstructor.systemList[i].tradeRoute == null)
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

			for(int i = 0; i < systemListConstructor.mapSize; ++i)
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
