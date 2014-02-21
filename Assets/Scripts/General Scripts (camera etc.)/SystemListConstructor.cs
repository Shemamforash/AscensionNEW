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
	public GameObject systemClone, originalSystem;
	private float xPos, yPos, distanceXY;
	public int mapSize;
	public float systemScale = 0.0f;

	private void Start()
	{
		PlanetRead ();
		SystemRead ();
		SelectSystemsForMap ();
		CheckSystem ();
		CreateObjects ();
		mapConstructor.DrawMinimumSpanningTree ();
		LoadBasicTechTree ();
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

	public void SelectSystemsForMap()
	{
		int randomInt = -1;
		string system = null;
		mapConstructor.distanceMax = (mapSize - 260) / -8f;

		systemScale = (mapSize - 300.0f) / -160.0f;

		for(int i = 0; i < 30; ++i)
		{
			for(int j = 0; j < 6; ++j)
			{
				if(uncheckedSystems.Count == 0 || firmSystems.Count == mapSize)
				{
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
		bool found = false;

		for(int i = 0; i < systemList.Count; ++i)
		{
			if(firmSystems.Contains (systemList[i].systemName) == false)
			{
				systemList.RemoveAt(i);
				i = 0;
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
		string planetName;
		
		using(StreamReader rimReader =  new StreamReader("PlanetRIMData.txt"))
		{
			for(int i = 0; i < 12; ++i)
			{
				PlanetInfo planet = new PlanetInfo();
				
				planet.planetType = rimReader.ReadLine ();
				planet.planetCategory = rimReader.ReadLine ();
				planet.science = float.Parse (rimReader.ReadLine ());
				planet.industry = float.Parse (rimReader.ReadLine ());
				planet.improvementSlots = int.Parse (rimReader.ReadLine ());
				
				planetList.Add (planet);
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
				if(resourceType == "Improvement Slots")
				{
					return planetList[i].improvementSlots;
				}
				else if(resourceType == "Science")
				{
					return planetList[i].science;
				}
				else if(resourceType == "Industry")
				{
					return planetList[i].industry;
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
	public int improvementSlots;
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
	public float planetScience, planetIndustry;
	public bool planetColonised, underEnemyControl;
	public int planetOwnership, planetDefence, planetImprovementLevel, improvementSlots, maxOwnership;
}

public class BasicImprovement
{
	public string name, category, details;
	public float cost;
	public int level;
}

