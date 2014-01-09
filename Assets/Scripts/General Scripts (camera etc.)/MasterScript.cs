using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour
{
	[HideInInspector]
	public MasterScript masterScript;
	[HideInInspector]
	public SystemListConstructor systemListConstructor;

	[HideInInspector]
	public GUISystemDataScript guiPlanScript;
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
	public MainGUIScript mainGUIScript;
	[HideInInspector]
	public GUIHeroScreen heroGUIScript;
	

	private void Awake()
	{
		systemListConstructor = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemListConstructor>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		diplomacyScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<DiplomacyControlScript>();
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();
		heroGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GUIHeroScreen>();
		tier2HeroScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<Tier2HeroScript> ();
		tier3HeroScript = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<Tier3HeroScript> ();
	}

	public int RefreshCurrentSystem(GameObject thisSystem)
	{
		for(int i = 0; i < 60; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}
		
		return 0;
	}
}


