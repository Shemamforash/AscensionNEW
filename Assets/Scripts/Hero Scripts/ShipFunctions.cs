using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipFunctions : MasterScript
{
	public int stealthValue, primaryWeaponPower, secondaryWeaponPower, collateralWeaponPower, engineValue, armourRating, logisticsRating;

	public void UpdateShips()
	{
		stealthValue = 0;
		primaryWeaponPower = 0;
		secondaryWeaponPower = 0;
		collateralWeaponPower = 0;
		engineValue = 0;
		armourRating = 0;
		logisticsRating = 0;

		for(int i = 0; i < heroTechTree.heroTechList.Count; ++i)
		{
			if(heroTechTree.heroTechList[i].isActive == true)
			{
				stealthValue += heroTechTree.heroTechList[i].stealthRating;
				primaryWeaponPower += heroTechTree.heroTechList[i].primaryOffenceRating;
				secondaryWeaponPower += heroTechTree.heroTechList[i].secondaryOffenceRating;
				collateralWeaponPower += heroTechTree.heroTechList[i].collateralRating;
				engineValue += heroTechTree.heroTechList[i].engineRating;
				armourRating += heroTechTree.heroTechList[i].armourRating;
				logisticsRating += heroTechTree.heroTechList[i].logisticsRating;
			}
		}

		Debug.Log (primaryWeaponPower);
	}
}
