using UnityEngine;
using System.Collections;

public class MerchantHeroScript : HeroScriptParent 
{
	public string linked = null;

	public void Merchant()
	{
		if(linked != null)
		{
			guiPlanScript = GameObject.Find (linked).GetComponent<GUISystemDataScript>();

			heroScript.heroSciBonus = guiPlanScript.tempTotalSci / 2;
			heroScript.heroIndBonus = guiPlanScript.tempTotalInd / 2;
			heroScript.heroMonBonus = guiPlanScript.tempTotalMon / 2;
		}
	}

	public void AddMerchant()
	{
		for(int i = 0; i < 60; ++i)
		{
			if(masterScript.systemList[i].systemObject == null)
			{
				continue;
			}
			
			GameObject tempSystem = masterScript.systemList[i].systemObject;
			
			lineRenderScript = tempSystem.GetComponent<LineRenderScript>();
			
			if(lineRenderScript.ownedBy != null && linked == "")
			{
				heroScript = tempSystem.GetComponent<HeroScriptParent>();
				
				for(int j = 0; j < 3; ++j)
				{
					if(masterScript.systemList[i].heroesInSystem[j] == "Merchant")
					{
						heroScript.allLinkableSystems[i] = tempSystem;
					}
				}
			}
		}
	}
}
