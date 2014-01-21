using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SystemListConstructor : MasterScript 
{
	[HideInInspector]
	public List<SystemInfo> systemList = new List<SystemInfo>();
	[HideInInspector]
	public List<PlanetInfo> planetList = new List<PlanetInfo>();
	[HideInInspector]
	private float xPos, yPos, distanceXY;
	public float distanceMax;
	private int connections;
	public GameObject systemClone, originalSystem;

	private void Awake()
	{
		LoadSystemData();
		AssignMaximumConnections ();
		SortNearestConnections ();
		DrawMinimumSpanningTree ();
		ConnectSystems();
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

	public void LoadSystemData()
	{
		string planetName;
		
		using(StreamReader rimReader =  new StreamReader("PlanetRIMData.txt"))
		{
			for(int i = 0; i < 12; ++i)
			{
				PlanetInfo planet = new PlanetInfo();
				
				planet.planetType = rimReader.ReadLine ();
				planet.planetCategory = rimReader.ReadLine ();
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

				system.systemOwnedBy = null;

				system.numberOfConnections = 0;
				
				for(int j = 0; j < system.systemSize; ++j)
				{
					planetName = system.systemName + " " + j.ToString();

					Planet newPlanet = new Planet();

					newPlanet.planetName = planetName;
					newPlanet.planetType = typeReader.ReadLine();
					newPlanet.planetCategory = FindPlanetCategory(newPlanet.planetType);
					newPlanet.planetImprovementLevel = 0;
					newPlanet.planetColonised = false;
					newPlanet.planetOwnership = 0;
					newPlanet.planetScience = FindPlanetSIM(newPlanet.planetType, "Science");
					newPlanet.planetIndustry = FindPlanetSIM(newPlanet.planetType, "Industry");
					newPlanet.planetMoney = FindPlanetSIM(newPlanet.planetType, "Money");
					newPlanet.maxOwnership = 0;
					newPlanet.improvementSlots = (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots");

					for(int k = 0; k < (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots"); ++k)
					{
						newPlanet.improvementsBuilt.Add (null);
					}

					system.planetsInSystem.Add (newPlanet);
				}

				xPos = float.Parse (typeReader.ReadLine());
				yPos = float.Parse (typeReader.ReadLine());

				system.systemPosition = new Vector3(xPos, yPos, 0.0f);

				systemClone = (GameObject)Instantiate(originalSystem, system.systemPosition, Quaternion.identity);

				systemClone.name = system.systemName;

				system.systemObject = systemClone;

				systemList.Add (system);
			}
		}
	}

	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemList.Count; ++i) //Assign number of connections to system
		{
			for(int j = 0; j < systemList.Count; ++j)
			{
				float distance = Vector3.Distance (systemList[i].systemObject.transform.position, systemList[j].systemObject.transform.position);
				
				if(distance < distanceMax)
				{
					Node nearbySystem = new Node();
					
					nearbySystem.targetSystem = systemList[j].systemObject;
					nearbySystem.targetDistance = distance;
					
					systemList[i].tempConnections.Add (nearbySystem);
				}
			}

			systemList[i].numberOfConnections = systemList[i].tempConnections.Count;
		}
	}

	private void SortNearestConnections()
	{
		GameObject tempObject;
		float tempFloat;

		for(int i = 0; i < systemList.Count; ++i)
		{
			for(int j = systemList[i].tempConnections.Count - 1; j >= 0; --j)
			{
				bool swaps = false;

				for(int k = 1; k <= j; ++k)
				{
					tempObject = systemList[i].tempConnections[k-1].targetSystem;
					tempFloat = systemList[i].tempConnections[k-1].targetDistance;

					if(systemList[i].tempConnections[k-1].targetDistance > systemList[i].tempConnections[k].targetDistance)
					{
						systemList[i].tempConnections[k-1].targetSystem = systemList[i].tempConnections[k].targetSystem;
						systemList[i].tempConnections[k-1].targetDistance = systemList[i].tempConnections[k].targetDistance;

						systemList[i].tempConnections[k].targetSystem = tempObject;
						systemList[i].tempConnections[k].targetDistance = tempFloat;

						swaps = true;
					}
				}

				if(swaps == false)
				{
					break;
				}
			}

			if(systemList[i].tempConnections.Count > 6)
			{
				systemList[i].tempConnections.RemoveRange(6, systemList[i].tempConnections.Count - 6);
				systemList[i].numberOfConnections = systemList[i].tempConnections.Count;
			}
		}
	}

	public void DrawMinimumSpanningTree()
	{
		List<GameObject> linkedSystems = new List<GameObject> (); //Create empty list of linkedsystems

		linkedSystems.Add (systemList [0].systemObject); //Add initial system to list

		for(int i = 0; i < systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			int nearestSystem = 0;
			int thisSystem = 0;

			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				int system = RefreshCurrentSystemA(linkedSystems[j]);

				for(int k = 0; k < systemList[system].tempConnections.Count; ++k) //For all systems connected to this system
				{
					bool ignoreSystem = false;

					for(int l = 0; l < linkedSystems.Count; ++l) //If the target system has already been linked
					{
						if(systemList[system].tempConnections[k].targetSystem == linkedSystems[l])
						{
							ignoreSystem = true; //Ignore it
						}
					}

					if(ignoreSystem == true)
					{
						continue;
					}

					if(systemList[system].tempConnections[k].targetDistance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = systemList[system].tempConnections[k].targetDistance;
						nearestSystem = k;
						thisSystem = system;
					}
				}
			}

			linkedSystems.Add (systemList[thisSystem].tempConnections[nearestSystem].targetSystem); //Add nearest unlinked system to linkedsystems list

			AddPermanentSystem(thisSystem, nearestSystem);
		}
	}

	private void AddPermanentSystem(int thisSystem, int nearestSystem)
	{
		systemList[thisSystem].permanentConnections.Add (systemList[thisSystem].tempConnections[nearestSystem].targetSystem); //Add target system to current systems permanent connections
		
		int targetSystem = RefreshCurrentSystemA(systemList[thisSystem].tempConnections[nearestSystem].targetSystem); //Do the same for the target systems
		
		for(int j = 0; j < systemList[targetSystem].tempConnections.Count; ++j)
		{
			if(systemList[targetSystem].tempConnections[j].targetSystem == systemList[thisSystem].systemObject)
			{
				systemList[targetSystem].permanentConnections.Add (systemList[targetSystem].tempConnections[j].targetSystem);
				break;
			}
		}
	}

	private void ConnectSystems()
	{
		for(int j = 0; j < systemList.Count; ++j) //For all systems
		{
			Debug.Log (systemList[j].permanentConnections.Count);

			if(systemList[j].numberOfConnections == systemList[j].permanentConnections.Count) //If the number of assigned systems equals the maximum number of systems, continue
			{
				continue;
			}

			for(int k = systemList[j].permanentConnections.Count - 1; k < systemList[j].numberOfConnections; ++k) //If the number of permanent connections equals the maximum number of connections, continue
			{
				float tempDistance = distanceMax; //Reset variables
				int nearestSystem = -1;
				int thisSystem = -1;
				bool ignoreSystem = false;

				for(int l = 0; l < systemList[j].tempConnections.Count; ++l) //For all tempconnections
				{
					int tempSystem = RefreshCurrentSystemA(systemList[j].tempConnections[l].targetSystem); //Get target system

					if(systemList[tempSystem].permanentConnections.Count == systemList[tempSystem].numberOfConnections 
					   || systemList[j].permanentConnections.Count == systemList[j].numberOfConnections) //If target/this system's connections are already full, continue
					{
						continue;
					}

					for(int m = 0; m > systemList[j].permanentConnections.Count; ++m) //If connections has already been made, continue
					{
						if(systemList[j].tempConnections[l].targetSystem == systemList[j].permanentConnections[m])
						{
							ignoreSystem = true;
						}
					}

					if(ignoreSystem == true) //As above
					{
						continue;
					}

					if(systemList[j].tempConnections[l].targetDistance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = systemList[j].tempConnections[l].targetDistance;
						nearestSystem = l;
						thisSystem = j;
					}
				}

				if(nearestSystem != -1 && thisSystem != -1)
				{
					AddPermanentSystem(thisSystem, nearestSystem);
				}
			}
		}

		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].permanentConnections.Count != systemList[i].numberOfConnections)
			{
				systemList[i].numberOfConnections = systemList[i].permanentConnections.Count;
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
				else if(resourceType == "Money")
				{
					return planetList[i].money;
				}
			}
		}
		
		return 0.0f;
	}
}

public class PlanetInfo
{
	public string planetType, planetCategory;
	public bool colonised;
	public int science, industry, money, improvementSlots;
}

public class SystemInfo
{
	public string systemName, systemOwnedBy;
	public Vector3 systemPosition;
	public GameObject systemObject;
	public GameObject tradeRoute;
	public int systemSize, numberOfConnections;
	public GameObject[] heroesInSystem = new GameObject[3];
	public List<Planet> planetsInSystem = new List<Planet> ();
	public List<Node> tempConnections = new List<Node>();
	public List<GameObject> permanentConnections = new List<GameObject>();
}

public class Planet
{
	public string planetName, planetType, planetCategory;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetScience, planetIndustry, planetMoney;
	public bool planetColonised;
	public int planetOwnership, planetImprovementLevel, improvementSlots, maxOwnership;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}

