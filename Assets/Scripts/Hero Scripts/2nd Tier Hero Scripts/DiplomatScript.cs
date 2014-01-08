using UnityEngine;
using System.Collections;

public class DiplomatScript : HeroScriptParent 
{
	public override void HeroBonusFunction()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();
				
		heroScript.heroSciBonus += 0.05f * guiPlanScript.totalSystemScience;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.totalSystemIndustry;
		heroScript.heroMonBonus += 0.10f * guiPlanScript.totalSystemMoney;

		CheckTier3Heroes ();
	}

	public void CheckTier3Heroes()
	{
		if(heroScript.heroTier3 == "President")
		{
			heroScript = gameObject.GetComponent<PresidentScript>();
			
			heroScript.HeroBonusFunctions();
		}
		
		if(heroScript.heroTier3 == "Peacemaker")
		{
			heroScript = gameObject.GetComponent<PeacemakerScript>();
			
			heroScript.HeroBonusFunctions();
		}
		
		if(heroScript.heroTier3 == "Merchant")
		{
			heroScript = gameObject.GetComponent<MerchantScript>();
			
			heroScript.HeroBonusFunctions();
		}
	}
}
