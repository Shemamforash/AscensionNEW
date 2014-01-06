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
	public List<SystemInfo> systemList = new List<SystemInfo>();
	[HideInInspector]
	public List<PlanetInfo> planetList = new List<PlanetInfo>();

	private void Awake()
	{
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		enemyOneTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		enemyTwoTurnScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		diplomacyScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<DiplomacyControlScript>();
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();
		heroGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GUIHeroScreen>();
		masterScript = gameObject.GetComponent<MasterScript>();

		LoadSystemData();
	}

	public int RefreshCurrentSystem(GameObject thisSystem)
	{
		for(int i = 0; i < 60; ++i)
		{
			if(masterScript.systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}

		return 0;
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

				masterScript.planetList.Add (planet);
			}
		}

		using(StreamReader typeReader =  new StreamReader("SystemTypeData.txt"))
		{
			for(int i = 0; i < 60; ++i)
			{
				SystemInfo system = new SystemInfo();

				system.systemName = typeReader.ReadLine();

				system.systemObject = GameObject.Find (system.systemName);

				system.systemSize = int.Parse (typeReader.ReadLine());
				
				for(int j = 0; j < system.systemSize; ++j)
				{
					planetName = system.systemName + " " + j.ToString();
					system.planetName.Add (planetName);
					system.planetType.Add (typeReader.ReadLine ());
					system.planetImprovementLevel.Add (0);
					system.planetColonised.Add (false);
					system.planetScience.Add (FindPlanetSIM(system.planetType[j], "Science"));
					system.planetIndustry.Add (FindPlanetSIM(system.planetType[j], "Industry"));
					system.planetMoney.Add (FindPlanetSIM(system.planetType[j], "Money"));
					system.improvementSlots.Add ((int)FindPlanetSIM(system.planetType[j], "Improvement Slots"));
				}

				masterScript.systemList.Add (system);
			}
		}
	}

	private float FindPlanetSIM(string planetType, string resourceType)
	{
		for(int i = 0; i < 12; ++i)
		{
			if(masterScript.planetList[i].planetType == planetType)
			{
				if(resourceType == "ImprovementSlots")
				{
					return masterScript.planetList[i].improvementSlots;
				}
				else if(resourceType == "Science")
				{
					return masterScript.planetList[i].science;
				}
				else if(resourceType == "Industry")
				{
					return masterScript.planetList[i].industry;
				}
				else if(resourceType == "Money")
				{
					return masterScript.planetList[i].money;
				}
			}
		}

		return 0.0f;
	}
}

public class PlanetInfo
{
	public string planetType;
	public bool colonised;
	public int science, industry, money, improvementSlots;
}

public class SystemInfo
{
	public string systemName, systemOwnedBy;
	public GameObject systemObject;
	public int systemSize;
	public List<GameObject> heroesInSystem = new List<GameObject>();
	public List<string> planetName = new List<string>();
	public List<string> planetType = new List<string>();
	public List<float> planetScience = new List<float>();
	public List<float> planetIndustry = new List<float>();
	public List<float> planetMoney = new List<float>();
	public List<bool> planetColonised = new List<bool>();
	public List<int> planetImprovementLevel = new List<int>();
	public List<int> improvementSlots= new List<int>();
}


