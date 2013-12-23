using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MasterScript : MonoBehaviour
{
	[HideInInspector]
	public MasterScript masterScript;

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
	public DiplomacyControlScript diplomacyScript;
	[HideInInspector]
	public MerchantHeroScript merchantScript;

	[HideInInspector]
	public MainGUIScript mainGUIScript;
	[HideInInspector]
	public GUIHeroScreen heroGUIScript;

	[HideInInspector]
	List<SystemInfo> systemList = new List<SystemInfo>();
	[HideInInspector]
	List<PlanetInfo> planetList = new List<PlanetInfo>();

	public void Awake()
	{
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		diplomacyScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<DiplomacyControlScript>();
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();
		heroGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GUIHeroScreen>();

		LoadSystemData();
	}

	private void LoadSystemData()
	{
		string planetName;

		using(StreamReader rimReader =  new StreamReader("PlanetRIMData.txt"))
		{
			for(int i = 0; i < 12; ++i)
			{
				PlanetInfo planet = new PlanetInfo();

				planet.planetType = rimReader.ReadLine ();
				planet.science = int.Parse (rimReader.ReadLine ());
				planet.industry = int.Parse (rimReader.ReadLine ());
				planet.money = int.Parse (rimReader.ReadLine ());
				planet.improvementSlots = int.Parse (rimReader.ReadLine ());

				planetList.Add (planet);
			}
		}

		using(StreamReader typeReader =  new StreamReader("SystemTypeData.txt"))
		{
			for(int i = 0; i < 60; ++i)
			{
				SystemInfo system = new SystemInfo();

				system.systemName = typeReader.ReadLine();

				system.systemSize = int.Parse (typeReader.ReadLine());
				
				for(int j = 0; j < system.systemSize; ++j)
				{
					planetName = system.systemName + " " + j.ToString();
					system.planetName.Add (planetName);
					system.planetType.Add (typeReader.ReadLine ());
					system.planetImprovementLevel.Add (0);

					system.improvementSlots.Add (FindPlanetImprovementSlots(system.planetType[j]));

					systemList.Add (system);
				}
			}
		}
	}

	private int FindPlanetImprovementSlots(string planetType)
	{
		for(int j = 0; j < 12; ++j)
		{
			if(planetList[j].planetType == planetType)
			{
				return planetList[j].improvementSlots;
			}
		}

		return 0;
	}
}

public class PlanetInfo
{
	public string planetType;
	public int science, industry, money, improvementSlots;
}

public class SystemInfo
{
	public string systemName, systemOwnedBy;
	public int systemSize;
	public List<GameObject> heroesInSystem = new List<GameObject>();
	public List<string> planetName = new List<string>();
	public List<string> planetType = new List<string>();
	public List<int> planetImprovementLevel = new List<int>();
	public List<int> improvementSlots= new List<int>();
}


