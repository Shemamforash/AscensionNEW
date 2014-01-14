using UnityEngine;
using System.Collections;

public class Tier2HeroScript : HeroScriptParent 
{
	public void CheckTier2Heroes(GameObject selectedHero)
	{
		heroScript = selectedHero.GetComponent<HeroScriptParent> ();
		
		guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();

		tempObject = heroScript.FindDiplomaticConnection ();

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

		heroScript.offensivePower -= 7.0f;
		heroScript.defensivePower -= 7.0f;

		++tempObject.peaceCounter;
	}

	public void Infiltrator()
	{
		heroScript.heroSciBonus += 0.10f * guiPlanScript.tempTotalSci;
		heroScript.heroIndBonus += 0.05f * guiPlanScript.tempTotalInd;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.tempTotalMon;

		if (tempObject.peaceCounter > 50) 
		{
			--tempObject.peaceCounter;
		}
		if (tempObject.peaceCounter < -50) 
		{
			++tempObject.peaceCounter;
		}
	}

	public void Soldier()
	{
		heroScript.heroSciBonus += 0.05f * guiPlanScript.tempTotalSci;
		heroScript.heroIndBonus += 0.10f * guiPlanScript.tempTotalInd;
		heroScript.heroMonBonus += 0.05f * guiPlanScript.tempTotalMon;

		heroScript.offensivePower += 7.0f;
		heroScript.defensivePower += 7.0f;
		
		--tempObject.peaceCounter;
	}
}
