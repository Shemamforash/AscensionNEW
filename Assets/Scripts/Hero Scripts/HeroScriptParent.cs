using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, invasionObject;
	public HeroDetailsWindow heroDetails;
	public int currentLevel = 1, movementSpeed, planetInvade = -1, system;
	public int primaryPower, secondaryPower, secondaryCollateral, invasionStrength, armour;
	public string heroTier2, heroTier3, heroOwnedBy, heroShipType;
	public bool isInvading = false, canLevelUp, reachedLevel2, reachedLevel3;
	private float heroAge;
	private GameObject levelUpLabel;

	void Start()
	{
		heroAge = Time.time;
	
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroShip = gameObject.GetComponent<HeroShip> ();

		system = RefreshCurrentSystem (heroLocation);

		movementSpeed = 1;

		heroGUI.heroDetailsContainer.GetComponent<UIGrid> ().enabled = true;

		heroDetails = new HeroDetailsWindow ();

		heroDetails.window = NGUITools.AddChild (heroGUI.heroDetailsContainer, heroGUI.heroDetailsPrefab);

		heroDetails.dropDownOne = heroDetails.window.transform.Find ("First Specialisation").gameObject.GetComponent<UIPopupList>();
		EventDelegate.Add (heroDetails.dropDownOne.gameObject.GetComponent<UIPopupList> ().onChange, heroGUI.SetSpecialisation);
		heroDetails.dropDownTwo = heroDetails.window.transform.Find ("Second Specialisation").gameObject.GetComponent<UIPopupList>();
		EventDelegate.Add (heroDetails.dropDownTwo.gameObject.GetComponent<UIPopupList> ().onChange, heroGUI.SetSpecialisation);

		heroGUI.heroDetailsContainer.GetComponent<UIGrid>().repositionNow = true;

		NGUITools.SetActive (heroDetails.window, false);
	}

	void Update()
	{
		system = RefreshCurrentSystem (heroLocation);

		if(levelUpLabel != null)
		{
			Vector3 position = cameraFunctionsScript.cameraMain.WorldToViewportPoint (gameObject.transform.position);
			
			position = overlayGUI.uiCamera.ViewportToWorldPoint (position);
			
			Vector3 newPosition = new Vector3(position.x, position.y, -37.0f);
			
			levelUpLabel.transform.position = newPosition;
		}
	}

	public DiplomaticPosition FindDiplomaticConnection()
	{
		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(heroOwnedBy == diplomacyScript.relationsList[i].playerOne.playerRace)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == diplomacyScript.relationsList[i].playerTwo.playerRace)
				{
					return diplomacyScript.relationsList[i];
				}
			}

			if(heroOwnedBy == diplomacyScript.relationsList[i].playerTwo.playerRace)
			{
				if(systemListConstructor.systemList[system].systemOwnedBy == diplomacyScript.relationsList[i].playerOne.playerRace)
				{
					return diplomacyScript.relationsList[i];
				}
			}
		}

		return null;
	}

	public void HeroEndTurnFunctions()
	{
		if(isInvading == true)
		{
			systemInvasion.heroScript = this;
			systemInvasion.ContinueInvasion(system);

			if(systemDefence.canEnter == true && planetInvade != -1)
			{
				systemInvasion.heroScript = this;
				systemInvasion.PlanetInvasion(system, planetInvade);
			}

			DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (heroOwnedBy, systemListConstructor.systemList[system].systemOwnedBy);
			temp.stateCounter -= 1;
		}

		if(heroAge + 6 <= Time.time && reachedLevel2 == false) 
		{
			reachedLevel2 = true;
			AddLevelUpDelegate();
		}

		if(heroAge + 7 <= Time.time && reachedLevel3 == false)
		{
			reachedLevel3 = true;
			AddLevelUpDelegate();
		}

		heroShip.ShipAbilities ();
	}

	private void AddLevelUpDelegate()
	{
		levelUpLabel = NGUITools.AddChild(GameObject.Find ("UI Root"), heroGUI.levelUpPrefab);
		
		levelUpLabel.transform.Find ("Label").GetComponent<UILabel>().depth = 1;
		
		EventDelegate.Add(levelUpLabel.GetComponent<UIButton>().onClick, LevelUp);
	}

	public void LevelUp()
	{
		NGUITools.Destroy (UIButton.current.gameObject);
		heroGUI.selectedHero = gameObject;
		++heroScript.currentLevel;
		canLevelUp = true;
		NGUITools.SetActive (heroGUI.heroDetailsContainer, false);
		heroGUI.OpenHeroDetails();
	}
}


