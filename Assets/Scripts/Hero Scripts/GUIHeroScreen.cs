using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIHeroScreen : MasterScript 
{
	public bool openHeroLevellingScreen, heroIsMoving;
	public GameObject heroObject, merchantQuad;
	public GameObject tempHero, targetSystem;
	public string[] heroLevelTwoSpecs = new string[3] {"Diplomat", "Soldier", "Infiltrator"};
	private string[] heroLevelThreeSpecs = new string[9] {"President", "Peacemaker", "Merchant", "Vanguard", "Strike Team", "Warlord", "Spy", "Recon Drone", "Assassin"};
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
		
		if(Input.GetMouseButtonDown(0) && heroIsMoving == false) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					tempHero = hit.collider.gameObject;
				}
			}
		}

		if(Input.GetMouseButtonDown (1) && tempHero != null && heroIsMoving == false)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{
					targetSystem = hit.collider.gameObject;

					int i = RefreshCurrentSystem (targetSystem);
			
					StartHeroMovement(tempHero, i);
				}
			}
		}

		if (heroIsMoving == true) 
		{
			MoveHero();
		}
	}

	private void MoveHero()
	{		
		Vector3 targetPosition = heroScript.HeroPositionAroundStar();
		Vector3 currentPosition = tempHero.transform.position;
		
		if(tempHero.transform.position == targetPosition || Time.time > timer + 1.0f) //If lerp exceeds timer, camera position will lock to point at object
		{
			tempHero.transform.position = targetPosition;
			
			timer = 0.0f; //Reset timer
			
			heroIsMoving = false;
		}
		
		tempHero.transform.position = Vector3.Lerp (currentPosition, targetPosition, 0.1f);

		heroScript = tempHero.GetComponent<HeroScriptParent> ();

		if(heroScript.merchantLine != null)
		{
			Destroy(heroScript.merchantLine);
			heroScript.CreateConnectionLine ();
		}
	}

	private void StartHeroMovement(GameObject hero, int targetSystem)
	{
		if(heroIsMoving == false)
		{
			if(heroScript.merchantLine != null)
			{
				Destroy (heroScript.merchantLine);
			}

			heroScript = hero.GetComponent<HeroScriptParent> ();

			Destroy (heroScript.invasionObject);

			heroScript.isInvading = false;

			guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();

			guiPlanScript.underInvasion = false;

			int k = RefreshCurrentSystem(heroScript.heroLocation);

			for(int i = 0; i < 3; ++i)
			{
				if(systemListConstructor.systemList[k].heroesInSystem[i] == null)
				{
					continue;
				}

				if(systemListConstructor.systemList[k].heroesInSystem[i] == hero)
				{
					systemListConstructor.systemList[k].heroesInSystem[i] = null;
				}
			}

			heroScript.heroLocation = systemListConstructor.systemList[targetSystem].systemObject;

			k = RefreshCurrentSystem(heroScript.heroLocation);

			for(j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[targetSystem].heroesInSystem[j] == null)
				{
					timer = Time.time;

					heroScript.thisHeroNumber = j;

					systemListConstructor.systemList[k].heroesInSystem[j] = hero;

					heroIsMoving = true;

					break;
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
		Rect tier2Box7 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 + 95.0f, 180.0f, 50.0f);
		Rect tier2Box8 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 + 155.0f, 180.0f, 50.0f);
		Rect tier2Box9 = new Rect(Screen.width / 2 + 110.0f, Screen.height / 2 + 215.0f, 180.0f, 50.0f);

		tier2 = new Rect[9]{tier2Box1, tier2Box2, tier2Box3, tier2Box4, tier2Box5, tier2Box6, tier2Box7, tier2Box8, tier2Box9};
	}

	public void CheckIfCanHire(int system)
	{
		if(playerTurnScript.GP > 0)
		{
			for(int j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[system].heroesInSystem[j] == null)
				{
					GameObject instantiatedHero = (GameObject)Instantiate (heroObject, systemListConstructor.systemList[system].systemObject.transform.position, 
					                                                       systemListConstructor.systemList[system].systemObject.transform.rotation);

					string tempHero = "Basic Hero";

					instantiatedHero.name = tempHero;
					
					systemListConstructor.systemList[system].heroesInSystem[j] = instantiatedHero;

					heroScript = instantiatedHero.GetComponent<HeroScriptParent>();

					heroScript.heroLocation = systemListConstructor.systemList[system].systemObject;

					heroScript.thisHeroNumber = j;

					heroScript.heroOwnedBy = playerTurnScript.playerRace;

					instantiatedHero.transform.position = heroScript.HeroPositionAroundStar();

					++heroCounter;

					--playerTurnScript.GP;

					break;
				}
			}
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
			mainGUIScript.spendMenu = false;
			mainGUIScript.openImprovementList = false;

			if(GUI.Button (new Rect(Screen.width / 2 + 300.0f, Screen.height / 2 - 275.0f, 20.0f, 20.0f), "X"))
			{
				openHeroLevellingScreen = false;
			}

			GUI.Box (new Rect(Screen.width / 2 - 300.0f, Screen.height / 2 - 275.0f, 600.0f, 550.0f), "");

			GUI.Label(new Rect(Screen.width / 2 -290.0f, Screen.height / 2 - 25.0f, 180.0f, 50.0f), "Hero");

			heroScript = systemListConstructor.systemList[mainGUIScript.selectedSystem].heroesInSystem[selectedHero].GetComponent<HeroScriptParent>();

			if(heroScript.currentLevel == 1)
			{
				for(int i = 0; i < 3; ++i)
				{
					if(GUI.Button (tier1[i], heroLevelTwoSpecs[i]) && playerTurnScript.GP > 1)
					{
						playerTurnScript.GP -= 2;

						heroScript.heroTier2 = heroLevelTwoSpecs[i];

						systemListConstructor.systemList[mainGUIScript.selectedSystem].heroesInSystem[selectedHero].name = heroLevelTwoSpecs[i];

						++heroScript.currentLevel;

						if(heroLevelTwoSpecs[i] == "Infiltrator")
						{
							heroScript.isInvisible = true;
						}
					}
				}

				for(int i = 0; i < 9; ++i)
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

				for(int i = 0; i < 9; ++i)
				{
					if(GUI.Button (tier2[i], heroLevelThreeSpecs[i]) && playerTurnScript.GP > 2)
					{
						playerTurnScript.GP -= 3;

						heroScript.heroTier3 = heroLevelThreeSpecs[i];

						systemListConstructor.systemList[mainGUIScript.selectedSystem].heroesInSystem[selectedHero].name = heroLevelThreeSpecs[i];

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

				for(int i = 0; i < 9; ++i)
				{
					GUI.Label (tier2[i], heroLevelThreeSpecs[i]);
				}
			}
		}
	}
}
