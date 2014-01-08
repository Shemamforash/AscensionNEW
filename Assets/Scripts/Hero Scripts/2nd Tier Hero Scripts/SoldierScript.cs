using UnityEngine;
using System.Collections;

public class SoldierScript : HeroScriptParent 
{
	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}
	
	void Update()
	{
		if(heroScript.endTurnBonus == true)
		{
			guiPlanScript = heroScript.heroLocation.GetComponent<GUISystemDataScript>();
			
			heroScript.heroSciBonus += 0.05f * guiPlanScript.totalSystemScience;
			heroScript.heroIndBonus += 0.10f * guiPlanScript.totalSystemIndustry;
			heroScript.heroMonBonus += 0.05f * guiPlanScript.totalSystemMoney;

			heroScript.endTurnBonus = false;
		}
	}
}
