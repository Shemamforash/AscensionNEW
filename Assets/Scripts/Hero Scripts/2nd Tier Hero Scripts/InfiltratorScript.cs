using UnityEngine;
using System.Collections;

public class InfiltratorScript : HeroScriptParent 
{
	public override void HeroBonusFunction()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();
			
		heroScript.heroSciBonus += 0.10f * guiPlanScript.totalSystemScience;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.totalSystemIndustry;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.totalSystemMoney;

		CheckTier3Heroes ();
	}

	public void CheckTier3Heroes()
	{
		if(heroScript.heroTier3 == "Spy")
		{
			heroScript = gameObject.GetComponent<SpyScript>();
			
			heroScript.HeroBonusFunctions();
		}
		
		if(heroScript.heroTier3 == "Recon Drone")
		{
			heroScript = gameObject.GetComponent<ReconDroneScript>();
			
			heroScript.HeroBonusFunctions();
		}
		
		if(heroScript.heroTier3 == "Assassin")
		{
			heroScript = gameObject.GetComponent<AssassinScript>();
			
			heroScript.HeroBonusFunctions();
		}
	}
}
