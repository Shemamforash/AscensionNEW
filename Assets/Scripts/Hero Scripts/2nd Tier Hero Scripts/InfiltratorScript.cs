using UnityEngine;
using System.Collections;

public class InfiltratorScript : HeroScriptParent 
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
			
			heroScript.heroSciBonus += 0.10f * guiPlanScript.totalSystemScience;
			heroScript.heroIndBonus += 0.05f * guiPlanScript.totalSystemIndustry;
			heroScript.heroMonBonus += 0.05f * guiPlanScript.totalSystemMoney;

			heroScript.endTurnBonus = false;
		}
	}
}
