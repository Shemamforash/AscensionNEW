using UnityEngine;
using System.Collections;

public class Tier2HeroScript : HeroScriptParent 
{
	public void CheckTier2Heroes(GameObject selectedHero)
	{
		heroScript = selectedHero.GetComponent<HeroScriptParent> ();

		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData>();

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
		heroScript.heroSciBonus += 0.05f * systemSIMData.tempTotalSci;
		heroScript.heroIndBonus += 0.05f * systemSIMData.tempTotalInd;

		heroScript.offensivePower -= 7.0f;
		heroScript.defensivePower -= 7.0f;

		if(tempObject != null)
		{
			++tempObject.peaceCounter;
		}
	}

	public void Infiltrator()
	{
		heroScript.heroSciBonus += 0.10f * systemSIMData.tempTotalSci;
		heroScript.heroIndBonus += 0.05f * systemSIMData.tempTotalInd;

		if(tempObject != null)
		{
			if (tempObject.peaceCounter > 50) 
			{
				--tempObject.peaceCounter;
			}
			if (tempObject.peaceCounter < -50) 
			{
				++tempObject.peaceCounter;
			}
		}
	}

	public void Soldier()
	{
		heroScript.heroSciBonus += 0.05f * systemSIMData.tempTotalSci;
		heroScript.heroIndBonus += 0.10f * systemSIMData.tempTotalInd;

		heroScript.offensivePower += 7.0f;
		heroScript.defensivePower += 7.0f;

		if(tempObject != null)
		{
			--tempObject.peaceCounter;
		}
	}
}
