using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, selectedHero, invasionButton, embargoButton;
	public string[] heroLevelTwoSpecs = new string[3] {"Diplomat", "Soldier", "Infiltrator"};
	private string[] heroLevelThreeSpecs = new string[6] {"Ambassador", "Smuggler", "Vanguard", "Warlord", "Hacker", "Drone"};
	public GUISkin mySkin;
	public int heroCounter = 1, j;
	private float timer;
	private Rect[] tier1, tier2;

	void Start()
	{
		LevellingTreeBuilder();
		invasionButton = GameObject.Find ("Invasion Button");
		embargoButton = GameObject.Find ("Embargo Button");
		NGUITools.SetActive (invasionButton, false);
		NGUITools.SetActive (embargoButton, false);
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
					selectedHero = hit.collider.gameObject;
					heroShip = selectedHero.GetComponent<HeroShip>();
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
	}

	public void InvasionButtonClick()
	{
		if(heroShip.canViewSystem == true)
		{
			cameraFunctionsScript.openMenu = true;
		}
		else
		{
			heroScript.StartSystemInvasion();
		}
	}

	public void Embargo()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		
		systemSIMData.isEmbargoed = true;
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

	public void CheckIfCanHire()
	{
		if(playerTurnScript.capital >= 50)
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

			instantiatedHero.transform.position = heroMovement.HeroPositionAroundStar(heroScript.heroLocation);

			++heroCounter;

			playerTurnScript.capital -= 50;;
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

			if(GUI.Button (new Rect(Screen.width / 2 + 300.0f, Screen.height / 2 - 275.0f, 20.0f, 20.0f), "X"))
			{
				openHeroLevellingScreen = false;
			}

			GUI.Box (new Rect(Screen.width / 2 - 300.0f, Screen.height / 2 - 275.0f, 600.0f, 550.0f), "");

			GUI.Label(new Rect(Screen.width / 2 -290.0f, Screen.height / 2 - 25.0f, 180.0f, 50.0f), "Hero");

			heroScript = selectedHero.GetComponent<HeroScriptParent>();

			if(heroScript.currentLevel == 1)
			{
				for(int i = 0; i < 3; ++i)
				{
					if(GUI.Button (tier1[i], heroLevelTwoSpecs[i]))
					{
						heroScript.heroTier2 = heroLevelTwoSpecs[i];

						selectedHero.name = heroLevelTwoSpecs[i];

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
					if(GUI.Button (tier2[i], heroLevelThreeSpecs[i]))
					{
						heroScript.heroTier3 = heroLevelThreeSpecs[i];

						selectedHero.name = heroLevelThreeSpecs[i];

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
