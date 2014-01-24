using UnityEngine;
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
	public TechTreeScript techTreeScript;
	[HideInInspector]
	public HeroScriptParent heroScript;
	[HideInInspector]
	public Tier2HeroScript tier2HeroScript;
	[HideInInspector]
	public Tier3HeroScript tier3HeroScript;
	[HideInInspector]
	public DiplomacyControlScript diplomacyScript;
	[HideInInspector]
	public RacialTraits racialTraitScript;

	[HideInInspector]
	public SystemGUI systemGUI;
	[HideInInspector]
	public HeroGUI heroGUI;
	[HideInInspector]
	public GalaxyGUI galaxyGUI;

	private void Awake()
	{
		systemListConstructor = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemListConstructor>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		diplomacyScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<DiplomacyControlScript>();
		systemGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemGUI>();
		heroGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<HeroGUI>();
		tier2HeroScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<Tier2HeroScript> ();
		tier3HeroScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<Tier3HeroScript> ();
		racialTraitScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<RacialTraits> ();
		galaxyGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GalaxyGUI>();
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


