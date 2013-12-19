using UnityEngine;
using System.Collections;

public class GUIHeroScreen : MasterScript 
{
	public bool openHeroScreen, canHireHero, openMerchantConnectionMenu;
	public GameObject merchantObject;
	public GUISkin mySkin;
	private GameObject[] systemsWithHeroSlot = new GameObject[60];
	private Rect[] allHeroGUIButtons;
	private string[] listOfHeroes;
	private Vector2 scrollPosition = Vector2.zero;
	private string tempHero;


	void Start()
	{
		heroBoxConstructor();
	}

	private void heroBoxConstructor()
	{
		Rect topLeft = new Rect(Screen.width / 2 - 200.0f, Screen.height / 2 - 200.0f, 100.0f, 100.0f);
		Rect topMiddle = new Rect(Screen.width / 2 - 50.0f, Screen.height / 2 - 200.0f, 100.0f, 100.0f);
		Rect topRight = new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 - 200.0f, 100.0f, 100.0f);
		
		Rect centreLeft = new Rect(Screen.width / 2 - 200.0f, Screen.height / 2 - 50.0f, 100.0f, 100.0f);
		Rect centreMiddle = new Rect(Screen.width / 2 - 50.0f, Screen.height / 2 - 50.0f, 100.0f, 100.0f);
		Rect centreRight = new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 - 50.0f, 100.0f, 100.0f);

		Rect bottomLeft = new Rect(Screen.width / 2 - 200.0f, Screen.height / 2 + 100.0f, 100.0f, 100.0f);
		Rect bottomMiddle = new Rect(Screen.width / 2 - 50.0f, Screen.height / 2 + 100.0f, 100.0f, 100.0f);
		Rect bottomRight = new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 + 100.0f, 100.0f, 100.0f);

		allHeroGUIButtons = new Rect[9] {topLeft, topMiddle, topRight, centreLeft, centreMiddle, centreRight, bottomLeft, bottomMiddle, bottomRight};

		listOfHeroes = new string[9] {"President", "Peacemaker", "Merchant",
									"Strike Team", "Warlord", "Vanguard",
									"Spy", "Assassin", "Recon Drone"};
	}

	private void CheckIfCanHire()
	{
		if(playerTurnScript.GP > 0)
		{
			for(int i = 0; i < 60; ++i)
			{
				systemsWithHeroSlot[i] = null;

				lineRenderScript = turnInfoScript.systemList[i].GetComponent<LineRenderScript>();

				if(lineRenderScript.ownedBy == playerTurnScript.playerRace)
				{
					heroScript = turnInfoScript.systemList[i].GetComponent<HeroScriptParent>();

					for(int j = 0; j < 3; ++j)
					{
						if(heroScript.heroesInSystem[j] == null && canHireHero == false)
						{
							canHireHero = true;

							--playerTurnScript.GP;

							systemsWithHeroSlot[i] = turnInfoScript.systemList[i];

							break;
						}
					}
				}
			}
		}
	}

	void OnGUI()
	{
		GUI.skin = mySkin;

		if(GUI.Button (new Rect(125.0f, 15.0f, 100.0f, 40.0f), "Open Hero Screen"))
		{
			openHeroScreen = true;
			cameraFunctionsScript.coloniseMenu = false;
			cameraFunctionsScript.openMenu = false;
			cameraFunctionsScript.doubleClick = false;
			mainGUIScript.spendMenu = false;
			mainGUIScript.openImprovementList = false;
		}


		if(openHeroScreen == true)
		{
			GUI.Box (new Rect(0.0f, 0.0f, Screen.width, Screen.height), "\n Heroes for purchase");
		
			for(int i = 0; i < 9; ++i)
			{
				if(GUI.Button (allHeroGUIButtons[i], listOfHeroes[i]))
				{
					CheckIfCanHire();

					if(canHireHero == true)
					{
						tempHero = listOfHeroes[i];
					}
				}
			}

			if(canHireHero == true)
			{
				GUILayout.BeginArea(new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 200.0f, 200.0f, 400.0f));

				scrollPosition = GUILayout.BeginScrollView(scrollPosition);

				for(int i = 0; i < 60; ++i)
				{
					if(systemsWithHeroSlot[i] == null)
					{
						continue;
					}

					if(GUILayout.Button (systemsWithHeroSlot[i].name, GUILayout.Height(40.0f)))
					{
						heroScript = systemsWithHeroSlot[i].GetComponent<HeroScriptParent>();

						for(int j = 0; j < 3; ++j)
						{
							if(heroScript.heroesInSystem[j] == null)
							{
								GameObject instantiatedHero = (GameObject)Instantiate (merchantObject, systemsWithHeroSlot[i].transform.position, systemsWithHeroSlot[i].transform.rotation);

								instantiatedHero.name = tempHero;

								heroScript.heroesInSystem[j] = instantiatedHero;

								break;
							}
						}
					}
				}

				GUILayout.EndScrollView();

				GUILayout.EndArea();
			}
		}


		if(openMerchantConnectionMenu == true)
		{
			GameObject tempSystem = playerTurnScript.tempObject;

			heroScript = tempSystem.GetComponent<HeroScriptParent>();

			FindMerchantScript();

			merchantScript.AddMerchant();
			
			if(GUI.Button (new Rect(Screen.width / 2 + 100.0f, Screen.height / 2 - 200.0f, 20.0f, 20.0f), "X"))
			{
				openMerchantConnectionMenu = false;
			}
			
			GUILayout.BeginArea (new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 200.0f, 200.0f, 400.0f));
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			
			for(int i = 0; i < 60; ++i)
			{
				if(heroScript.allLinkableSystems[i] != "" && heroScript.allLinkableSystems[i] != playerTurnScript.tempObject.name)
				{
					if(GUILayout.Button (heroScript.allLinkableSystems[i], GUILayout.Height (40.0f)))
					{
						merchantScript.linked = heroScript.allLinkableSystems[i];
						
						heroScript.allLinkableSystems[i] = "";
						
						heroScript = GameObject.Find (merchantScript.linked).GetComponent<HeroScriptParent>();
						
						heroScript.allLinkableSystems[i] = "";

						FindMerchantScript();
						
						merchantScript.linked = playerTurnScript.tempObject.name;
						
						openMerchantConnectionMenu = false;
					}
				}
			}
			
			GUILayout.EndScrollView();
			
			GUILayout.EndArea();
		}
	}

	private void FindMerchantScript()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(heroScript.heroesInSystem[i].name == "Merchant")
			{
				merchantScript = heroScript.heroesInSystem[i].GetComponent<MerchantHeroScript>();
			}
		}
	}
}
