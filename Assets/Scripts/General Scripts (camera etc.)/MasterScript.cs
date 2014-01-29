﻿using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour
{
	[HideInInspector]
	public MasterScript masterScript;
	[HideInInspector]
	public SystemListConstructor systemListConstructor;

	[HideInInspector]
	public SystemSIMData systemSIMData;
	[HideInInspector]
	public CameraFunctions cameraFunctionsScript;
	[HideInInspector]
	public LineRenderScript lineRenderScript;

	[HideInInspector]
	public TurnInfo turnInfoScript;
	[HideInInspector]
	public PlayerTurn playerTurnScript;
	[HideInInspector]
	public AIBasicParent baseAIScript;
	[HideInInspector]
	public EnemyOne enemyOneTurnScript;
	[HideInInspector]
	public EnemyTwo enemyTwoTurnScript;

	[HideInInspector]
	public HeroTechTree heroTechTree;
	[HideInInspector]
	public TechTreeScript techTreeScript;
	[HideInInspector]
	public HeroScriptParent heroScript;
	[HideInInspector]
	public Tier2HeroScript tier2HeroScript;
	[HideInInspector]
	public Tier3HeroScript tier3HeroScript;
	[HideInInspector]
	public ShipFunctions shipFunctions;
	[HideInInspector]
	public DiplomacyControlScript diplomacyScript;
	[HideInInspector]
	public RacialTraits racialTraitScript;
	[HideInInspector]
	public OverlayGUI overlayGUI;

	[HideInInspector]
	public SystemGUI systemGUI;
	[HideInInspector]
	public HeroGUI heroGUI;
	[HideInInspector]
	public GalaxyGUI galaxyGUI;

	private void Awake()
	{
		systemListConstructor = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<SystemListConstructor>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<EnemyTwo>();
		diplomacyScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<DiplomacyControlScript>();
		systemGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemGUI>();
		heroGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<HeroGUI>();
		tier2HeroScript = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<Tier2HeroScript> ();
		tier3HeroScript = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<Tier3HeroScript> ();
		racialTraitScript = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<RacialTraits> ();
		galaxyGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GalaxyGUI>();
		overlayGUI = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<OverlayGUI> ();
		heroTechTree = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<HeroTechTree> ();
	}

	public int RefreshCurrentSystem(GameObject thisSystem)
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}
		
		return 0;
	}

	public void WipePlanetInfo(int system, int planet)
	{
		systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised = false;
		systemListConstructor.systemList [system].planetsInSystem [planet].planetImprovementLevel = 0;

		for(int i = 0; i < systemListConstructor.systemList [system].planetsInSystem [planet].improvementSlots; ++i)
		{
			techTreeScript = systemListConstructor.systemList[system].systemObject.GetComponent<TechTreeScript>();

			for(int j = 0; j < techTreeScript.listOfImprovements.Count; ++j)
			{
				if(techTreeScript.listOfImprovements[j].improvementName == systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i])
				{
					techTreeScript.listOfImprovements[j].hasBeenBuilt = false;
					systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i] = null;
				}
			}
		}
	}
}


