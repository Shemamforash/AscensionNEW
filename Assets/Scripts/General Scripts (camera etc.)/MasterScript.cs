using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour
{
	[HideInInspector]
	public MasterScript masterScript;
	[HideInInspector]
	public SystemListConstructor systemListConstructor;
	[HideInInspector]
	public MapConstructor mapConstructor;

	[HideInInspector]
	public SystemSIMData systemSIMData;
	[HideInInspector]
	public CameraFunctions cameraFunctionsScript;
	[HideInInspector]
	public LineRenderScript lineRenderScript;
	[HideInInspector]
	public SystemDefence systemDefence;

	[HideInInspector]
	public TurnInfo turnInfoScript;
	[HideInInspector]
	public PlayerTurn playerTurnScript;
	[HideInInspector]
	public AIBasicParent baseAIScript;
	[HideInInspector]
	public WinConditions winConditions;
	
	[HideInInspector]
	public ImprovementsBasic improvementsBasic;
	[HideInInspector]
	public HeroScriptParent heroScript;
	[HideInInspector]
	public HeroMovement heroMovement;
	[HideInInspector]
	public HeroShip heroShip;
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
	[HideInInspector]
	public InvasionGUI invasionGUI;

	private void Awake()
	{
		systemListConstructor = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<SystemListConstructor>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<PlayerTurn>();
		diplomacyScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<DiplomacyControlScript>();
		systemGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemGUI>();
		heroGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<HeroGUI>();
		racialTraitScript = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<RacialTraits> ();
		galaxyGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GalaxyGUI>();
		overlayGUI = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<OverlayGUI> ();
		invasionGUI = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<InvasionGUI> ();
		mapConstructor = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<MapConstructor> ();
		winConditions = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<WinConditions> ();
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
			improvementsBasic = systemListConstructor.systemList[system].systemObject.GetComponent<ImprovementsBasic>();

			for(int j = 0; j < improvementsBasic.listOfImprovements.Count; ++j)
			{
				if(improvementsBasic.listOfImprovements[j].improvementName == systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i])
				{
					improvementsBasic.listOfImprovements[j].hasBeenBuilt = false;
					systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i] = null;
				}
			}
		}
	}
}


