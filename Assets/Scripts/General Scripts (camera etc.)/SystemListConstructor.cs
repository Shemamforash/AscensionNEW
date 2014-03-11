using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SystemListConstructor : MasterScript 
{
	[HideInInspector]
	public List<StarSystem> systemList = new List<StarSystem>();
	[HideInInspector]
	public List<PlanetInfo> planetList = new List<PlanetInfo>();
	[HideInInspector]
	private List<string> uncheckedSystems = new List<string> ();
	[HideInInspector]
	private List<string> firmSystems = new List<string> ();
	public List<BasicImprovement> basicImprovementsList = new List<BasicImprovement> ();

	private int connections;
	public int mapSize;
	public GameObject systemClone, originalSystem;
	private float xPos, yPos, distanceXY;
	public float systemScale = 0.0f;

	private void Start()
	{
		mapSize = PlayerPrefs.GetInt ("Map Size");

		PlanetRead ();
		SystemRead ();
		HeroTechTree.ReadTechFile ();
		SelectSystemsForMap ();
		CheckSystem ();
		CreateObjects ();
		mapConstructor.DrawMinimumSpanningTree ();
		LoadBasicTechTree ();

		for(int i = 0; i < systemList.Count; ++i)
		{
			lineRenderScript = systemList[i].systemObject.GetComponent<LineRenderScript>();

			lineRenderScript.StartUp();
		}

		galaxyGUI.SelectRace(PlayerPrefs.GetString ("Player Race"));
	}

	public int RefreshCurrentSystemA(GameObject thisSystem)
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}
		
		return 0;
	}

	private void SelectSystemsForMap()
	{
		int randomInt = -1;
		mapConstructor.distanceMax = (mapSize - 260) / -8f;

		systemScale = (mapSize - 300.0f) / -160.0f;

		bool filledSystem = false;

		for(int i = 0; i < 30; ++i)
		{
			if(filledSystem == true)
			{
				break;
			}

			for(int j = 0; j < 6; ++j)
			{
				if(firmSystems.Count == mapSize)
				{
					filledSystem = true;
					break;
				}

				randomInt = Random.Range (0, uncheckedSystems.Count);

				if(j < mapSize / 30)
				{
					firmSystems.Add(uncheckedSystems[randomInt]);
				}

				uncheckedSystems.RemoveAt(randomInt);
			}
		}
	}
	
	private void CheckSystem()
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(firmSystems.Contains (systemList[i].systemName) == false)
			{
				systemList.RemoveAt(i);
				--i;
			}
		}
	}

	private void CreateObjects()
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			systemClone = (GameObject)Instantiate(originalSystem, systemList[i].systemPosition, Quaternion.identity);
		
			systemClone.transform.localScale = new Vector3(systemScale, systemScale, systemScale);

			systemClone.name = systemList[i].systemName;
		
			systemList[i].systemObject = systemClone;
		}
	}

	public void SystemRead()
	{
		string[] planetLocations = new string[6]{"C","D","E","F","G","H"};
		string planetName;

		using(XmlReader reader = XmlReader.Create ("SystemData.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					StarSystem system = new StarSystem();
					
					system.systemName = reader.GetAttribute("A");

					uncheckedSystems.Add(system.systemName);
					
					system.systemSize = int.Parse (reader.GetAttribute("B"));
					
					system.systemOwnedBy = null;
					
					system.numberOfConnections = 0;
					
					for(int j = 0; j < system.systemSize; ++j)
					{
						planetName = system.systemName + " " + j.ToString();
						
						Planet newPlanet = new Planet();
						
						newPlanet.planetName = planetName;
						newPlanet.planetType = reader.GetAttribute(planetLocations[j]);
						newPlanet.planetCategory = FindPlanetCategory(newPlanet.planetType);
						newPlanet.planetImprovementLevel = 0;
						newPlanet.planetColonised = false;
						newPlanet.planetOwnership = 0;
						newPlanet.planetScience = FindPlanetSIM(newPlanet.planetType, "Science");
						newPlanet.planetIndustry = FindPlanetSIM(newPlanet.planetType, "Industry");
						newPlanet.capitalValue = (int)FindPlanetSIM(newPlanet.planetType, "Capital");
						newPlanet.maxOwnership = 0;
						newPlanet.improvementSlots = (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots");
						newPlanet.underEnemyControl = false;
						
						for(int k = 0; k < (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots"); ++k)
						{
							newPlanet.improvementsBuilt.Add (null);
						}
						
						system.planetsInSystem.Add (newPlanet);
					}
					
					xPos = float.Parse (reader.GetAttribute("I"));
					yPos = float.Parse (reader.GetAttribute("J"));
					
					system.systemPosition = new Vector3(xPos, yPos, 0.0f);
					
					systemList.Add (system);

					if(system.systemName == "Nepthys" || system.systemName == "Midgard" || system.systemName == "Samael")
					{
						firmSystems.Add (system.systemName);
						uncheckedSystems.Remove(system.systemName);
					}
				}
			}
		}
	}

	public void PlanetRead()
	{		
		using(XmlReader reader =  XmlReader.Create("PlanetSicData.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					PlanetInfo planet = new PlanetInfo();
					
					planet.planetType = reader.GetAttribute ("A");
					planet.planetCategory = reader.GetAttribute ("B");
					planet.science = float.Parse (reader.GetAttribute("C"));
					planet.industry = float.Parse (reader.GetAttribute("D"));
					planet.improvementSlots = int.Parse (reader.GetAttribute("E"));
					planet.capitalCost = int.Parse (reader.GetAttribute("F"));
					
					planetList.Add (planet);
				}
			}
		}
	}

	private string FindPlanetCategory(string planetType)
	{
		for(int i = 0; i < 12; ++i)
		{
			if(planetList[i].planetType == planetType)
			{
				return planetList[i].planetCategory;
			}
		}

		return null;
	}

	private float FindPlanetSIM(string planetType, string resourceType)
	{
		for(int i = 0; i < 12; ++i)
		{
			if(planetList[i].planetType == planetType)
			{
				switch(resourceType)
				{
				case "Improvement Slots":
					return planetList[i].improvementSlots;
				case "Science":
					return planetList[i].science;
				case "Industry":
					return planetList[i].industry;
				case "Capital":
					return (float)planetList[i].capitalCost;
				default:
					break;
				}
			}
		}
		
		return 0.0f;
	}

	public void LoadBasicTechTree()
	{
		using(XmlReader reader = XmlReader.Create ("ImprovementList.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					BasicImprovement improvement = new BasicImprovement();
					
					improvement.name = reader.GetAttribute("A");
					improvement.category = reader.GetAttribute("B");
					improvement.level = int.Parse (reader.GetAttribute("C"));
					improvement.cost = float.Parse(reader.GetAttribute("D"));
					improvement.details = reader.GetAttribute("E");
					
					basicImprovementsList.Add (improvement);
				}
			}
		}
	}
}

public class PlanetInfo
{
	public string planetType, planetCategory;
	public bool colonised;
	public float science, industry;
	public int improvementSlots, capitalCost;
}

public class StarSystem
{
	public string systemName, systemOwnedBy;
	public Vector3 systemPosition;
	public GameObject systemObject, allyHero, enemyHero;
	public GameObject tradeRoute;
	public int systemSize, numberOfConnections, systemDefence;
	public List<Planet> planetsInSystem = new List<Planet> ();
	public List<Node> tempConnections = new List<Node>();
	public List<GameObject> permanentConnections = new List<GameObject>();
}

public class Planet
{
	public string planetName, planetType, planetCategory;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetScience, planetIndustry, planetOwnership, planetDefence, virusTimer, chillTimer, poisonTimer, chillLength;
	public bool planetColonised, underEnemyControl, virusActive, chillActive, poisonActive;
	public int planetImprovementLevel, improvementSlots, maxOwnership, capitalValue;
}

public class BasicImprovement
{
	public string name, category, details;
	public float cost;
	public int level;
}

