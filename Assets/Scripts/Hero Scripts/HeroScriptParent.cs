using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, invasionObject;
	public int currentLevel = 1, movementSpeed, planetInvade = -1, system;
	public int primaryPower, secondaryPower, secondaryCollateral, invasionStrength; 
	public string heroTier2 = null, heroTier3 = null, heroOwnedBy, heroShipType;
	public bool isInvading = false, canLevelUp, reachedLevel2, reachedLevel3;
	public float heroAge, classModifier, maxArmour, currentArmour;
	private GameObject levelUpLabel;

	void Start()
	{
		heroAge = Time.time;
	
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroShip = gameObject.GetComponent<HeroShip> ();

		system = RefreshCurrentSystem (heroLocation);

		movementSpeed = 1;
		classModifier = 1;
		currentArmour = 100;

		levelUpLabel = NGUITools.AddChild(GameObject.Find ("UI Root"), heroGUI.levelUpPrefab);
		NGUITools.SetActive (levelUpLabel, false);
	}

	void Update()
	{
		system = RefreshCurrentSystem (heroLocation);

		if(levelUpLabel != null)
		{
			Vector3 position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2.0f, gameObject.transform.position.z);

			position = cameraFunctionsScript.cameraMain.WorldToViewportPoint (position);

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

	public void HeroEndTurnFunctions(TurnInfo thisPlayer)
	{
		heroShip = gameObject.GetComponent<HeroShip> ();
		heroShip.ShipAbilities (thisPlayer);

		if(isInvading == true)
		{
			systemInvasion.hero = this;
			systemDefence = systemListConstructor.systemList[system].systemObject.GetComponent<SystemDefence>();

			if(systemDefence.canEnter == false)
			{
				systemInvasion.ContinueInvasion(system);
			}
			if(systemDefence.canEnter == true && planetInvade != -1)
			{
				systemInvasion.PlanetInvasion(system, planetInvade);
			}
		}

		if(isInvading == false && currentArmour != maxArmour)
		{
			currentArmour += maxArmour * 0.02f;

			if(currentArmour >= maxArmour)
			{
				currentArmour = maxArmour;
			}
		}

		if(heroAge + 6 <= Time.time && reachedLevel2 == false) 
		{
			reachedLevel2 = true;
			if(thisPlayer == playerTurnScript)
			{
				AddLevelUpIcon();
			}
		}

		if(heroAge + 7 <= Time.time && reachedLevel3 == false && heroTier2 != "")
		{
			reachedLevel3 = true;
			if(thisPlayer == playerTurnScript)
			{
				AddLevelUpIcon();
			}
		}

		if(thisPlayer == playerTurnScript)
		{
			if(levelUpLabel.activeInHierarchy != false)
			{
				if(canLevelUp == false && levelUpLabel.activeInHierarchy == true)
				{
					NGUITools.SetActive(levelUpLabel, false);
				}
			}
		}
	}

	private void AddLevelUpIcon()
	{
		NGUITools.SetActive(levelUpLabel, true);
		
		levelUpLabel.transform.Find ("Label").GetComponent<UILabel>().depth = 1;

		canLevelUp = true;

		++heroScript.currentLevel;
	}
}


