using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ShipFunctions
{
	public static int stealthValue, primaryWeaponWealth, artilleryWealth, artilleryCollateral, bombWealth, bombCollateral, dropshipWealth, 
			   dropshipCollateral, engineValue, armourRating, logisticsRating;
	public static bool infiltratorEngine, soldierPrimary;

	public static void UpdateShips()
	{
		stealthValue = 0;
		primaryWeaponWealth = 20;
		soldierPrimary = false;
		artilleryWealth = 10;
		artilleryCollateral = 10;
		bombWealth = 0;
		bombCollateral = 0;
		dropshipWealth = 0;
		dropshipCollateral = 0;
		engineValue = 1;
		infiltratorEngine = false;
		armourRating = 100;
		logisticsRating = 0;

		for(int i = 0; i < HeroTechTree.heroTechList.Count; ++i)
		{
			if(HeroTechTree.heroTechList[i].isActive == true)
			{
				if(HeroTechTree.heroTechList[i].techName == "Miniature Warp Sphere")
				{
					infiltratorEngine = true;
					continue;
				}

				if(HeroTechTree.heroTechList[i].techName == "Full Broadside")
				{
					soldierPrimary = true;
					continue;
				}

				stealthValue += HeroTechTree.heroTechList[i].stealthRating;
				primaryWeaponWealth += HeroTechTree.heroTechList[i].primaryOffenceRating;

				if(HeroTechTree.heroTechList[i].heroType == "Soldier")
				{
					artilleryWealth += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					artilleryCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}
				if(HeroTechTree.heroTechList[i].heroType == "Infiltrator")
				{
					bombWealth += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					bombCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}
				if(HeroTechTree.heroTechList[i].heroType == "Soldier")
				{
					dropshipWealth += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					dropshipCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}

				engineValue += HeroTechTree.heroTechList[i].engineRating;
				armourRating += HeroTechTree.heroTechList[i].armourRating;
				logisticsRating += HeroTechTree.heroTechList[i].logisticsRating;
			}
		}
	}
}
