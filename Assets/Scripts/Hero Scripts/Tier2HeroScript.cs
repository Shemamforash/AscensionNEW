using UnityEngine;
using System.Collections;

public class Tier2HeroScript : HeroScriptParent 
{
	public void CheckTier2Heroes(GameObject selectedHero)
	{
		heroScript = selectedHero.GetComponent<HeroScriptParent> ();
		
		guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();

		if(heroScript.heroTier2 == "Diplomat")
		{
			Diplomat ();
		}
		if(heroScript.heroTier2 == "Infiltrator")
		{
			Infiltrator ();
		}
		if(heroScript.heroTier2 == "Soldier")
		{
			Soldier ();
		}
	}

	public void Diplomat()
	{				
		heroScript.heroSciBonus += 0.05f * guiPlanScript.totalSystemScience;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.totalSystemIndustry;
		heroScript.heroMonBonus += 0.10f * guiPlanScript.totalSystemMoney;
	}

	public void Infiltrator()
	{
		heroScript.heroSciBonus += 0.10f * guiPlanScript.totalSystemScience;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.totalSystemIndustry;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.totalSystemMoney;
	}

	public void Soldier()
	{
		heroScript.heroSciBonus += 0.05f * guiPlanScript.totalSystemScience;
		heroScript.heroIndBonus += 0.10f * guiPlanScript.totalSystemIndustry;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.totalSystemMoney;
	}
}
