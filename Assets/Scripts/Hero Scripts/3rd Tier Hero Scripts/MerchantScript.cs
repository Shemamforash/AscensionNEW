using UnityEngine;
using System.Collections;

public class MerchantScript : HeroScriptParent 
{
	public GameObject linked = null;
	public GameObject[] linkableSystems = new GameObject[60];
	private Vector2 scrollPosition = Vector2.zero;

	private void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent>();
	}

	public void Update()
	{
		if(linked != null && heroScript.endTurnBonus == true)
		{
			guiPlanScript = linked.GetComponent<GUISystemDataScript>();

			heroScript.heroSciBonus += guiPlanScript.tempTotalSci / 2;
			heroScript.heroIndBonus += guiPlanScript.tempTotalInd / 2;
			heroScript.heroMonBonus += guiPlanScript.tempTotalMon / 2;

			heroScript.endTurnBonus = false;
		}
	}

	private void FillLinkableSystems()
	{
		for(int i = 0; i < 60; ++i)
		{
			linkableSystems[i] = null;

			if(systemListConstructor.systemList[i].systemObject != heroScript.heroLocation && systemListConstructor.systemList[i].systemOwnedBy != playerTurnScript.playerRace)
			{
				for(int j = 0; j < 3; ++j)
				{
					if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
					{
						continue;
					}

					if(systemListConstructor.systemList[i].heroesInSystem[j].name == "Merchant")
					{
						linkableSystems[i] = systemListConstructor.systemList[i].systemObject;
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

					merchantScript = linkableSystems[i].GetComponent<MerchantScript>();

					merchantScript.linked = heroScript.heroLocation;
				}
			}

			GUILayout.EndScrollView();

			GUILayout.EndArea();
		}
	}
}
