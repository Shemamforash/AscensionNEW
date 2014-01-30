﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad;
	public GameObject tempHero;
	public string[] heroLevelTwoSpecs = new string[3] {"Diplomat", "Soldier", "Infiltrator"};
	private string[] heroLevelThreeSpecs = new string[6] {"Ambassador", "Smuggler", "Vanguard", "Warlord", "Hacker", "Drone"};
	public GUISkin mySkin;
	public int heroCounter = 1, selectedHero, j;
	private float timer;
	private Rect[] tier1, tier2;

	void Start()
	{
		LevellingTreeBuilder();
	}

	void Update()
	{
		RaycastHit hit = new RaycastHit();
		
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					tempHero = hit.collider.gameObject;
				}
			}
		}
		
		if(Input.GetMouseButtonDown (1) && tempHero != null)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{					
					heroMovement = tempHero.GetComponent<HeroMovement> ();

					if(heroMovement.heroIsMoving == false)
					{
						heroMovement.pathfindTarget = hit.collider.gameObject;
						heroMovement.FindPath();
					}
				}
			}
		}
	}

	private void LevellingTreeBuilder()
	{
		Rect tier1Box1 = new Rect(Screen.width / 2 - 90.0f, Screen.height / 2 - 205.0f, 180.0f, 50.0f);
		Rect tier1Box2 = new Rect(Screen.width / 2 - 90.0f, Screen.height /2 - 25.0f, 180.0f, 50.0f);
		Rect tier1Box3 = new Rect(Screen.width / 2 - 90.0f, Screen.height / 2 + 155.0f, 180.0f, 50.0f);

		tier1 = new Rect[3]{tier1Box1, tier1Box2, tier1Box3};

		Rect tier2Box1 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 - 265.0f, 180.0f, 50.0f);
		Rect tier2Box2 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 - 205.0f, 180.0f, 50.0f);
		Rect tier2Box3 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 - 145.0f, 180.0f, 50.0f);
		Rect tier2Box4 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 - 85.0f, 180.0f, 50.0f);
		Rect tier2Box5 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 - 25.0f, 180.0f, 50.0f);
		Rect tier2Box6 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 + 35.0f, 180.0f, 50.0f);

		tier2 = new Rect[6]{tier2Box1, tier2Box2, tier2Box3, tier2Box4, tier2Box5, tier2Box6};
	}

	public void CheckIfCanHire(int system)
	{
		if(playerTurnScript.GP > 0)
		{
			GameObject instantiatedHero = (GameObject)Instantiate (heroObject, systemListConstructor.systemList[system].systemObject.transform.position, 
			                                                       systemListConstructor.systemList[system].systemObject.transform.rotation);

			string tempHero = "Basic Hero";

			instantiatedHero.name = tempHero;
			
			systemListConstructor.systemList[system].heroesInSystem.Add(instantiatedHero);

			heroScript = instantiatedHero.GetComponent<HeroScriptParent>();

			heroMovement = instantiatedHero.GetComponent<HeroMovement>();

			heroScript.heroLocation = systemListConstructor.systemList[system].systemObject;

			heroScript.thisHeroNumber = j;

			heroScript.heroOwnedBy = playerTurnScript.playerRace;

			instantiatedHero.transform.position = heroMovement.HeroPositionAroundStar(heroScript.heroLocation);

			++heroCounter;

			--playerTurnScript.GP;
		}
	}

	private bool CheckIfCanInvade(HeroScriptParent heroScript)
	{
		int i = RefreshCurrentSystem (heroScript.heroLocation);

		if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace || systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace && heroScript.isInvading == false)
		{
			return true;
		}

		return false;
	}

	void OnGUI()
	{
		GUI.skin = mySkin;

		if(tempHero != null)
		{
			heroScript = tempHero.GetComponent<HeroScriptParent> ();

			bool canInvade = CheckIfCanInvade (heroScript);

			if(canInvade == true)
			{
				if(GUI.Button (new Rect(Screen.width / 2 - 75.0f, Screen.height / 2 + 400.0f, 150.0f, 60.0f), "Start Invasion"))
				{
					heroScript.StartSystemInvasion();
				}
			}
		}
	
		if(openHeroLevellingScreen == true)
		{
			cameraFunctionsScript.coloniseMenu = false;
			cameraFunctionsScript.openMenu = false;
			cameraFunctionsScript.doubleClick = false;
			systemGUI.spendMenu = false;
			systemGUI.openImprovementList = false;

			if(GUI.Button (new Rect(Screen.width / 2 + 300.0f, Screen.height / 2 - 275.0f, 20.0f, 20.0f), "X"))
			{
				openHeroLevellingScreen = false;
			}

			GUI.Box (new Rect(Screen.width / 2 - 300.0f, Screen.height / 2 - 275.0f, 600.0f, 550.0f), "");

			GUI.Label(new Rect(Screen.width / 2 -290.0f, Screen.height / 2 - 25.0f, 180.0f, 50.0f), "Hero");

			heroScript = systemListConstructor.systemList[systemGUI.selectedSystem].heroesInSystem[selectedHero].GetComponent<HeroScriptParent>();

			if(heroScript.currentLevel == 1)
			{
				for(int i = 0; i < 3; ++i)
				{
					if(GUI.Button (tier1[i], heroLevelTwoSpecs[i]) && playerTurnScript.GP > 1)
					{
						playerTurnScript.GP -= 2;

						heroScript.heroTier2 = heroLevelTwoSpecs[i];

						systemListConstructor.systemList[systemGUI.selectedSystem].heroesInSystem[selectedHero].name = heroLevelTwoSpecs[i];

						++heroScript.currentLevel;

						if(heroLevelTwoSpecs[i] == "Infiltrator")
						{
							heroScript.heroShipType = "Stealth Ship";
						}

						if(heroLevelTwoSpecs[i] == "Soldier")
						{
							heroScript.heroShipType = "War Ship";
						}

						if(heroLevelTwoSpecs[i] == "Diplomat")
						{
							heroScript.heroShipType = "Command Ship";
						}
					}
				}

				for(int i = 0; i < 6; ++i)
				{
					GUI.Label (tier2[i], heroLevelThreeSpecs[i]);
				}
			}

			if(heroScript.currentLevel == 2)
			{
				for(int i = 0; i < 3; ++i)
				{
					GUI.Label (tier1[i], heroLevelTwoSpecs[i]);
				}

				for(int i = 0; i < 6; ++i)
				{
					if(GUI.Button (tier2[i], heroLevelThreeSpecs[i]) && playerTurnScript.GP > 2)
					{
						playerTurnScript.GP -= 3;

						heroScript.heroTier3 = heroLevelThreeSpecs[i];

						systemListConstructor.systemList[systemGUI.selectedSystem].heroesInSystem[selectedHero].name = heroLevelThreeSpecs[i];

						++heroScript.currentLevel;
					}
				}
			}

			if(heroScript.currentLevel == 3)
			{
				for(int i = 0; i < 3; ++i)
				{
					GUI.Label (tier1[i], heroLevelTwoSpecs[i]);
				}

				for(int i = 0; i < 6; ++i)
				{
					GUI.Label (tier2[i], heroLevelThreeSpecs[i]);
				}
			}
		}
	}
}