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
		heroScript.heroSciBonus += 0.05f * guiPlanScript.tempTotalSci;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.tempTotalInd;
		heroScript.heroMonBonus += 0.10f * guiPlanScript.tempTotalMon;
	}

	public void Infiltrator()
	{
		heroScript.heroSciBonus += 0.10f * guiPlanScript.tempTotalSci;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.tempTotalInd;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.tempTotalMon;
	}

	public void Soldier()
	{
		heroScript.heroSciBonus += 0.05f * guiPlanScript.tempTotalSci;
		heroScript.heroIndBonus += 0.10f * guiPlanScript.tempTotalInd;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.tempTotalMon;
	}
}
