using UnityEngine;
using System.Collections;

public class Tier3HeroScript : HeroScriptParent 
{
	public GameObject[] linkableSystems = new GameObject[60];
	private Vector2 scrollPosition = Vector2.zero;
	public bool openSystemLinkScreen, linkableSystemsExist;
	public GameObject selectedHero;
	private int availableSystems;

	public void CheckTier3Heroes()
	{
		heroScript = selectedHero.GetComponent<HeroScriptParent> ();
		
		guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();

		if(heroScript.heroTier2 == "Diplomat")
		{
			if(heroScript.heroTier3 == "Peacemaker")
			{
				Peacemaker ();
			}
			if(heroScript.heroTier3 == "President")
			{
				President ();
			}
			if(heroScript.heroTier3 == "Merchant")
			{
				Merchant ();
			}
		}
		if(heroScript.heroTier2 == "Infiltrator")
		{
			if(heroScript.heroTier3 == "Spy")
			{
				Spy ();
			}
			if(heroScript.heroTier3 == "Assassin")
			{
				Assassin ();
			}
			if(heroScript.heroTier3 == "Recon Drone")
			{
				ReconDrone ();
			}
		}
		if(heroScript.heroTier2 == "Soldier")
		{
			if(heroScript.heroTier3 == "Warlord")
			{
				Warlord ();
			}
			if(heroScript.heroTier3 == "Vanguard")
			{
				Vanguard ();
			}
			if(heroScript.heroTier3 == "Strike Team")
			{
				StrikeTeam ();
			}
		}
	}

	public void Merchant()
	{
		if(heroScript.linked != null)
		{
			guiPlanScript = heroScript.linked.GetComponent<GUISystemDataScript>();
			
			heroScript.heroSciBonus += guiPlanScript.tempTotalSci / 2;
			heroScript.heroIndBonus += guiPlanScript.tempTotalInd / 2;
			heroScript.heroMonBonus += guiPlanScript.tempTotalMon / 2;
		}
	}
	public void Peacemaker()
	{
	}
	public void President()
	{
	}
	public void Spy()
	{
	}
	public void Assassin()
	{
	}
	public void ReconDrone()
	{
	}
	public void Warlord()
	{
	}
	public void Vanguard()
	{
	}
	public void StrikeTeam()
	{
	}

	public void FillLinkableSystems()
	{
		for(int i = 0; i < 60; ++i)
		{
			linkableSystems[i] = null;

			/*&& systemListConstructor.systemList[i].systemOwnedBy != playerTurnScript.playerRace*/
			for(int j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
				{
					continue;
				}

				heroScript = systemListConstructor.systemList[i].heroesInSystem[j].GetComponent<HeroScriptParent>();

				Debug.Log (systemListConstructor.systemList[i].heroesInSystem[j].name);

				if(systemListConstructor.systemList[i].heroesInSystem[j].name == "Merchant" && systemListConstructor.systemList[i].systemObject != heroScript.heroLocation && heroScript.linked == null)
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
			heroScript = selectedHero.GetComponent<HeroScriptParent>();

			if(GUI.Button (new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 - 270.0f, 20.0f, 20.0f), "X"))
			{
				openSystemLinkScreen = false;
			}

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 250.0f, 200.0f, 500.0f));

			GUILayout.Box ("Systems with Merchant");

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			for(int i = 0; i < 60; ++i)
			{
				if(linkableSystems[i] == null || linkableSystems[i] == heroScript.heroLocation)
				{
					continue;
				}

				if(GUILayout.Button (linkableSystems[i].name, GUILayout.Height (50.0f)))
				{
					GameObject tempLocation = null;

					tempLocation = heroScript.heroLocation;

					heroScript.linked = linkableSystems[i];

					heroScript = linkableSystems[i].GetComponent<HeroScriptParent>();

					heroScript.linked = tempLocation;

					openSystemLinkScreen = false;
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}
	}
}
