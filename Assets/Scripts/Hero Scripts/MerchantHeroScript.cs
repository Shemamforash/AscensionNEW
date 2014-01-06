using UnityEngine;
using System.Collections;

public class MerchantHeroScript : HeroScriptParent 
{
	public GameObject linked;
	public GameObject[] linkableSystems = new GameObject[60];
	private Vector2 scrollPosition = Vector2.zero;

	private void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent>();
	}

	public void Merchant()
	{
		if(linked != null)
		{
			guiPlanScript = linked.GetComponent<GUISystemDataScript>();

			heroScript.heroSciBonus += guiPlanScript.tempTotalSci / 2;
			heroScript.heroIndBonus += guiPlanScript.tempTotalInd / 2;
			heroScript.heroMonBonus += guiPlanScript.tempTotalMon / 2;
		}
	}

	private void FillLinkableSystems()
	{
		linkableSystems = null;

		for(int i = 0; i < 60; ++i)
		{
			if(masterScript.systemList[i].systemObject != heroScript.heroLocation && masterScript.systemList[i].systemOwnedBy != playerTurnScript.playerRace)
			{
				for(int j = 0; i < 3; ++j)
				{
					if(masterScript.systemList[i].heroesInSystem[j].name == "Merchant")
					{
						linkableSystems[i] = masterScript.systemList[i].systemObject;
					}
				}
			}
		}
	}

	void OnGUI()
	{
		if(linked == null)
		{
			FillLinkableSystems();

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 100.0f, Screen.height / 2 - 250.0f, 200.0f, 500.0f));

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			for(int i = 0; i < 60; ++i)
			{
				if(linkableSystems[i] == null)
				{
					continue;
				}

				if(GUILayout.Button (linkableSystems[i].name, GUILayout.Height (50.0f)))
				{
					linked = linkableSystems[i];

					merchantScript = linkableSystems[i].GetComponent<MerchantHeroScript>();

					merchantScript.linked = heroScript.heroLocation;
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();


		}
	}
}
