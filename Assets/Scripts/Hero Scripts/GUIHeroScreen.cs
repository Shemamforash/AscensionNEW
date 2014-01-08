using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIHeroScreen : MasterScript 
{
	public bool openMerchantConnectionMenu, openHeroLevellingScreen;
	public GameObject heroObject, selectedHero;
	public string[] heroLevel1Specs = new string[3] {"Diplomat", "Soldier", "Infiltrator"};
	private string[] heroLevel2Specs = new string[9] {"President", "Peacemaker", "Merchant", "Vanguard", "Strike Team", "Warlord", "Spy", "Recon Drone", "Assassin"};
	public GUISkin mySkin;
	private int heroCounter = 1;
	private string tempHero, scriptToAdd;
	private Rect[] tier1, tier2;

	void Start()
	{
		LevellingTreeBuilder();
	}

	private void LevellingTreeBuilder()
	{
		Rect tier1Box1 = new Rect(Screen.height - 162.5f, Screen.width - 90.0f, 180.0f, 50.0f);
		Rect tier1Box2 = new Rect(Screen.height - 25.0f, Screen.width - 90.0f, 180.0f, 50.0f);
		Rect tier1Box3 = new Rect(Screen.height + 137.5f, Screen.width - 90.0f, 180.0f, 50.0f);

		tier1 = new Rect[3]{tier1Box1, tier1Box2, tier1Box3};

		Rect tier2Box1 = new Rect(Screen.height - 265.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box2 = new Rect(Screen.height - 205.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box3 = new Rect(Screen.height - 145.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box4 = new Rect(Screen.height - 85.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box5 = new Rect(Screen.height - 25.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box6 = new Rect(Screen.height + 35.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box7 = new Rect(Screen.height + 95.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box8 = new Rect(Screen.height + 155.0f, Screen.width + 110.0f, 180.0f, 50.0f);
		Rect tier2Box9 = new Rect(Screen.height - 215.0f, Screen.width + 110.0f, 180.0f, 50.0f);

		tier2 = new Rect[9]{tier2Box1, tier2Box2, tier2Box3, tier2Box4, tier2Box5, tier2Box6, tier2Box7, tier2Box8, tier2Box9};
	}

	public void CheckIfCanHire(GameObject system)
	{
		if(playerTurnScript.GP > 0)
		{
			for(int i = 0; i < 60; ++i)
			{
				if(systemListConstructor.systemList[i].systemObject == cameraFunctionsScript.selectedSystem)
				{
					for(int j = 0; j < 3; ++j)
					{
						if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
						{
							GameObject instantiatedHero = (GameObject)Instantiate (heroObject, systemListConstructor.systemList[i].systemObject.transform.position, 
							                                                       systemListConstructor.systemList[i].systemObject.transform.rotation);

							tempHero = "Hero" + heroCounter.ToString();

							instantiatedHero.name = tempHero;
							
							systemListConstructor.systemList[i].heroesInSystem[j] = instantiatedHero;

							heroScript = instantiatedHero.GetComponent<HeroScriptParent>();

							heroScript.heroLocation = systemListConstructor.systemList[i].systemObject;

							++heroCounter;

							--playerTurnScript.GP;

							break;
						}
					}

					break;
				}
			}
		}
	}

	void OnGUI()
	{
		GUI.skin = mySkin;

		if(openHeroLevellingScreen == true)
		{
			cameraFunctionsScript.coloniseMenu = false;
			cameraFunctionsScript.openMenu = false;
			cameraFunctionsScript.doubleClick = false;
			mainGUIScript.spendMenu = false;
			mainGUIScript.openImprovementList = false;

			if(GUI.Button (new Rect(Screen.width / 2 + 300.0f, Screen.height / 2 +225.0f, 20.0f, 20.0f), "X"))
			{
				openHeroLevellingScreen = false;
			}

			GUI.Box (new Rect(Screen.width / 2 - 300.0f, Screen.height / 2 - 275.0f, 600.0f, 550.0f), "");

			GUI.Label(new Rect(Screen.width / 2 -290.0f, Screen.height / 2, 180.0f, 50.0f), "Hero");

			heroScript = selectedHero.GetComponent<HeroScriptParent>();

			if(heroScript.currentLevel == 1)
			{
				for(int i = 0; i < 3; ++i)
				{
					if(GUI.Button (tier1[i], heroLevel1Specs[i]) && playerTurnScript.GP > 0)
					{
						--playerTurnScript.GP;

						scriptToAdd = heroLevel1Specs[i] + "Script";

						selectedHero.AddComponent(scriptToAdd);

						++heroScript.currentLevel;
					}
				}

				for(int i = 0; i < 9; ++i)
				{
					GUI.Label (tier2[i], heroLevel2Specs[i]);
				}
			}

			if(heroScript.currentLevel == 2)
			{
				for(int i = 0; i < 3; ++i)
				{
					GUI.Label (tier1[i], heroLevel1Specs[i]);
				}

				for(int i = 0; i < 9; ++i)
				{
					if(GUI.Button (tier2[i], heroLevel2Specs[i]) && playerTurnScript.GP > 1)
					{
						playerTurnScript.GP -= 2;
						
						scriptToAdd = heroLevel2Specs[i] + "Script";
						
						selectedHero.AddComponent(scriptToAdd);
						
						++heroScript.currentLevel;
					}
				}
			}

			if(heroScript.currentLevel == 3)
			{
				for(int i = 0; i < 3; ++i)
				{
					GUI.Label (tier1[i], heroLevel1Specs[i]);
				}

				for(int i = 0; i < 9; ++i)
				{
					GUI.Label (tier2[i], heroLevel2Specs[i]);
				}
			}
			
		}

	}
}
