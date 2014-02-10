using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipFunctions : MasterScript
{
	public int stealthValue, primaryWeaponPower, artilleryPower, artilleryCollateral, bombPower, bombCollateral, dropshipPower, 
			   dropshipCollateral, engineValue, armourRating, logisticsRating;
	public bool infiltratorEngine, soldierPrimary;

	public void UpdateShips()
	{
		stealthValue = 0;
		primaryWeaponPower = 20;
		soldierPrimary = false;
		artilleryPower = 0;
		artilleryCollateral = 0;
		bombPower = 0;
		bombCollateral = 0;
		dropshipPower = 0;
		dropshipCollateral = 0;
		engineValue = 1;
		infiltratorEngine = false;
		armourRating = 0;
		logisticsRating = 0;

		for(int i = 0; i < heroTechTree.heroTechList.Count; ++i)
		{
			if(heroTechTree.heroTechList[i].isActive == true)
			{
				if(heroTechTree.heroTechList[i].techName == "Miniature Warp Sphere")
				{
					infiltratorEngine = true;
					continue;
				}

				if(heroTechTree.heroTechList[i].techName == "Full Broadside")
				{
					soldierPrimary = true;
					continue;
				}

				stealthValue += heroTechTree.heroTechList[i].stealthRating;
				primaryWeaponPower += heroTechTree.heroTechList[i].primaryOffenceRating;

				if(heroTechTree.heroTechList[i].heroType == "Soldier")
				{
					artilleryPower += heroTechTree.heroTechList[i].secondaryOffenceRating;
					artilleryCollateral += heroTechTree.heroTechList[i].collateralRating;
				}
				if(heroTechTree.heroTechList[i].heroType == "Infiltrator")
				{
					bombPower += heroTechTree.heroTechList[i].secondaryOffenceRating;
					bombCollateral += heroTechTree.heroTechList[i].collateralRating;
				}
				if(heroTechTree.heroTechList[i].heroType == "Soldier")
				{
					dropshipPower += heroTechTree.heroTechList[i].secondaryOffenceRating;
					dropshipCollateral += heroTechTree.heroTechList[i].collateralRating;
				}

				engineValue += heroTechTree.heroTechList[i].engineRating;
				armourRating += heroTechTree.heroTechList[i].armourRating;
				logisticsRating += heroTechTree.heroTechList[i].logisticsRating;
			}
		}
	}
}
