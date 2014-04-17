using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SystemListConstructor : MasterScript 
{
	private AmbientStarRandomiser ambientStars;

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
	public float systemScale = 0.0f, sysDistMin;
	public Transform systemContainer;

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

		empireBoundaries.SetArrSize ();

		ambientStars = GameObject.Find ("ScriptsContainer").GetComponent<AmbientStarRandomiser> ();
		ambientStars.GenerateStars ();

		LoadBasicTechTree ();

		for(int i = 0; i < systemList.Count; ++i)
		{
			lineRenderScript = systemList[i].systemObject.GetComponent<LineRenderScript>();

			lineRenderScript.StartUp();
		}

		systemPopup.LoadOverlays ();

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

		int difference = systemList.Count - firmSystems.Count;

		for(int j = 0; j < difference; ++j)
		{
			if(firmSystems.Count == mapSize)
			{
				break;
			}
			
			randomInt = Random.Range (0, uncheckedSystems.Count - 1);
			
			if(CheckWithinMinMaxDistance(uncheckedSystems[randomInt]) == true)
			{
				firmSystems.Add(uncheckedSystems[randomInt]);
			}

			uncheckedSystems.RemoveAt(randomInt);
		}
	}

	private bool CheckWithinMinMaxDistance(string system)
	{
		Vector3 sysOne = systemList [CheckSystemName (system)].systemPosition;

		for(int i = 0; i < firmSystems.Count; ++i)
		{
			Vector3 sysTwo = systemList[CheckSystemName(firmSystems[i])].systemPosition;

			float distance = Vector3.Distance(sysOne, sysTwo);

			if(distance < sysDistMin)
			{
				return false;
			}
		}

		return true;
	}

	private int CheckSystemName(string name)
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].systemName == name)
			{
				return i;
			}
		}

		return -1;
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

			systemClone.transform.parent = systemContainer;
		
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
						newPlanet.planetKnowledge = FindPlanetSIM(newPlanet.planetType, "Knowledge");
						newPlanet.planetPower = FindPlanetSIM(newPlanet.planetType, "Power");
						newPlanet.wealthValue = (int)FindPlanetSIM(newPlanet.planetType, "Wealth");
						newPlanet.maxOwnership = 0;
						newPlanet.improvementSlots = (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots");
						newPlanet.underEnemyControl = false;

						int hasResources = Random.Range (0, 3);

						if(hasResources == 0)
						{
							switch(newPlanet.planetCategory)
							{
							case "Hot":
								newPlanet.rareResourceType = "Radioisotopes";
								break;
							case "Cold":
								newPlanet.rareResourceType = "Liquid Hydrogen";
								break;
							case "Terran":
								newPlanet.rareResourceType = "Blue Carbon";
								break;
							case "Gas Giant":
								newPlanet.rareResourceType = "Antimatter";
								break;
							default:
								newPlanet.rareResourceType = null;
								break;
							}
						}
						
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
						if(PlayerPrefs.GetString("Planet One") == system.systemName || PlayerPrefs.GetString("Planet Two") == system.systemName || PlayerPrefs.GetString("Planet Three") == system.systemName)
						{
							firmSystems.Add (system.systemName);
							uncheckedSystems.Remove(system.systemName);
						}
					}
				}
			}
		}
	}

	public void PlanetRead()
	{		
		using(XmlReader reader =  XmlReader.Create("PlanetSICData.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					PlanetInfo planet = new PlanetInfo();
					
					planet.planetType = reader.GetAttribute ("A");
					planet.planetCategory = reader.GetAttribute ("B");
					planet.knowledge = float.Parse (reader.GetAttribute("C"));
					planet.power = float.Parse (reader.GetAttribute("D"));
					planet.improvementSlots = int.Parse (reader.GetAttribute("E"));
					planet.wealthCost = int.Parse (reader.GetAttribute("F"));
					
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
				case "Knowledge":
					return planetList[i].knowledge;
				case "Power":
					return planetList[i].power;
				case "Wealth":
					return (float)planetList[i].wealthCost;
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
	public float knowledge, power;
	public int improvementSlots, wealthCost;
}

public class StarSystem
{
	public string systemName, systemOwnedBy;
	public Vector3 systemPosition;
	public GameObject systemObject, allyHero, enemyHero;
	public GameObject tradeRoute;
	public int systemSize, numberOfConnections;
	public float systemDefence, systemOffence;
	public List<Planet> planetsInSystem = new List<Planet> ();
	public List<Node> tempConnections = new List<Node>();
	public List<GameObject> permanentConnections = new List<GameObject>();
}

public class Planet
{
	public string planetName, planetType, planetCategory, rareResourceType;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetKnowledge, planetPower, planetOwnership, planetDefence, planetOffence, virusTimer, chillTimer, poisonTimer, chillLength, maxOwnership;
	public bool planetColonised, underEnemyControl, virusActive, chillActive, poisonActive;
	public int planetImprovementLevel, improvementSlots, wealthValue;
}

public class BasicImprovement
{
	public string name, category, details;
	public float cost;
	public int level;
}

