using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipFunctions : MasterScript
{
	int stealthValue, primaryWeaponPower, secondaryWeaponPower, collateralWeaponPower, engineValue, armourRating, logisticsRating;
	bool canViewEnemySystem, canEmbargo, canPromote;
	List<GameObject> tradeRoutes = new List<GameObject>();
	HeroTech activePrimary, activeSecondary, activeEngine, activeArmour, activeStealth, activeLogistics;

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroScript.heroShipType = "Basic Ship";
	}

	void CheckShip()
	{
		if(heroScript.heroShipType == "Stealth Ship")
		{
			canViewEnemySystem = true;

			activeStealth = SearchForActiveTech("Stealth");
		}

		if(heroScript.heroShipType == "War Ship")
		{
			
		}

		if(heroScript.heroShipType == "Command Ship")
		{
			if(heroScript.heroTier3 == "Ambassador")
			{
				canEmbargo = true;
				canPromote = true;
			}

			logisticsRating = 1;
		}
	}

	private HeroTech SearchForActiveTech(string techType)
	{
		return null;
	}
}
