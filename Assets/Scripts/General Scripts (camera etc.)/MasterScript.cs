using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour
{
	public MasterScript masterScript;

	public GUISystemDataScript guiPlanScript;
	public CameraFunctions cameraFunctionsScript;
	public LineRenderScript lineRenderScript;

	public TurnInfo turnInfoScript;
	public PlayerTurn playerTurnScript;
	public AIBasicParent baseAIScript;
	public EnemyOne enemyOneTurnScript;
	public EnemyTwo enemyTwoTurnScript;

	public TechTreeScript techTreeScript;
	public HeroScript heroScript;

	public MainGUIScript mainGUIScript;
}