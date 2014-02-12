using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SystemListConstructor : MasterScript 
{
	[HideInInspector]
	public List<StarSystem> systemList = new List<StarSystem>();
	[HideInInspector]
	public List<PlanetInfo> planetList = new List<PlanetInfo>();
	[HideInInspector]
	private List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	private float xPos, yPos, distanceXY;
	public float distanceMax;
	private int connections;
	public GameObject systemClone, originalSystem;

	private void Awake()
	{
		LoadSystemData();
		DrawMinimumSpanningTree ();
		AssignMaximumConnections ();
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
				planet.improvementSlots = int.Parse (rimReader.ReadLine ());
				
				planetList.Add (planet);
			}
		}
		
		using(StreamReader typeReader =  new StreamReader("SystemTypeData.txt"))
		{
			for(int i = 0; i < 60; ++i)
			{
				StarSystem system = new StarSystem();
				
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
					newPlanet.maxOwnership = 0;
					newPlanet.improvementSlots = (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots");
					newPlanet.underEnemyControl = false;

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

	public bool TestForIntersection(GameObject thisSystem, GameObject targetSystem)
	{
		float A1 = thisSystem.transform.position.y - targetSystem.transform.position.y;
		float B1 = targetSystem.transform.position.x - thisSystem.transform.position.y;
		float C1 = (thisSystem.transform.position.x * targetSystem.transform.position.y) - (targetSystem.transform.position.x * thisSystem.transform.position.y);

		for (int i = 0; i < coordinateList.Count; ++i) 
		{
			float A2 = coordinateList [i].systemA.transform.position.y - coordinateList [i].systemB.transform.position.y;
			float B2 = coordinateList [i].systemB.transform.position.x - coordinateList [i].systemA.transform.position.x;
			float C2 = (coordinateList[i].systemA.transform.position.x * coordinateList [i].systemB.transform.position.y) - (coordinateList[i].systemB.transform.position.x * coordinateList [i].systemA.transform.position.y);

			float parallel = (A1 * B2) - (A2 * B1);

			if (parallel == 0) 
			{
				continue;
			}

			float xIntersect = ((B2 * C1) - (B1 * C2)) / parallel;
			float yIntersect = ((A1 * C2) - (A2 * C1)) / parallel;

			if (xIntersect >= Mathf.Min (thisSystem.transform.position.x + 0.001f, targetSystem.transform.position.x + 0.001f) && xIntersect <= Mathf.Max (thisSystem.transform.position.x - 0.001f, targetSystem.transform.position.x - 0.001f)) 
			{
				if (yIntersect >= Mathf.Min (thisSystem.transform.position.y + 0.001f, targetSystem.transform.position.y + 0.001f) && yIntersect <= Mathf.Max (thisSystem.transform.position.y - 0.001f, targetSystem.transform.position.y - 0.001f)) 
				{
					return false;
				}
			}
		}
		return true;
	}

	public void DrawMinimumSpanningTree() //Working
	{
		List<GameObject> linkedSystems = new List<GameObject> (); //Create empty list of linkedsystems
		List<GameObject> unlinkedSystems = new List<GameObject> ();
		
		linkedSystems.Add (systemList [0].systemObject); //Add initial system to list

		for (int i = 1; i < systemList.Count; ++i) 
		{
			unlinkedSystems.Add (systemList[i].systemObject);
		}
		
		for(int i = 0; i < systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			int nearestSystem = -1;
			int thisSystem = -1;
			
			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				int system = RefreshCurrentSystemA(linkedSystems[j]);
				
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(TestForIntersection(linkedSystems[j], unlinkedSystems[k]) == false)
					{
						continue;
					}

					float distance = Vector3.Distance(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position);
					
					if(distance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = distance;
						nearestSystem = RefreshCurrentSystemA(unlinkedSystems[k]);
						thisSystem = system;
					}
				}
			}

			if(nearestSystem != -1 && thisSystem != -1)
			{
				linkedSystems.Add (systemList[nearestSystem].systemObject); //Add nearest unlinked system to linkedsystems list
				unlinkedSystems.Remove (systemList[nearestSystem].systemObject);

				AddPermanentSystem(thisSystem, nearestSystem);
			}
		}

		for(int i = 0; i < systemList.Count; ++i)
		{
			systemList[i].numberOfConnections = systemList[i].permanentConnections.Count;
		}
	}

	private void AddPermanentSystem(int thisSystem, int nearestSystem)
	{
		systemList[thisSystem].permanentConnections.Add (systemList[nearestSystem].systemObject); //Add target system to current systems permanent connections
		systemList [nearestSystem].permanentConnections.Add (systemList [thisSystem].systemObject);

		ConnectionCoordinates connection = new ConnectionCoordinates ();

		connection.systemA = systemList [thisSystem].systemObject;
		connection.systemB = systemList [nearestSystem].systemObject;

		coordinateList.Add (connection);
	}

	private int WeightedConnectionFinder(int randomInt)
	{
		if(randomInt < 9)
		{
			return 1;
		}
		if(randomInt >= 9 && randomInt < 24)
		{
			return 2;
		}
		if(randomInt >= 24 && randomInt < 49)
		{
			return 3;
		}
		if(randomInt >= 49 && randomInt < 74)
		{
			return 4;
		}
		if(randomInt >= 74 && randomInt < 89)
		{
			return 5;
		}
		if(randomInt >= 89)
		{
			return 6;
		}

		return 0;
	}

	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemList.Count; ++i) //For all systems
		{
			int randomInt = WeightedConnectionFinder(Random.Range (0,99)); //Generate number

			if(systemList[i].systemName == "Heracles" || systemList[i].systemName == "Sol" || systemList[i].systemName == "Nepthys")
			{
				randomInt = WeightedConnectionFinder(Random.Range (49, 99));
			}

			if(systemList[i].numberOfConnections < randomInt) //If number of connections is lower than number
			{
				systemList[i].numberOfConnections = randomInt; //Increase number of connections
			}

			for(int j = 0; j < systemList.Count; ++j) //For all systems
			{
				if(i == j)
				{
					continue;
				}

				bool skipSystem = false;

				float distance = Vector3.Distance (systemList[i].systemObject.transform.position, systemList[j].systemObject.transform.position); //Assign distance

				for(int k = 0; k < systemList[i].permanentConnections.Count; ++k) //For all of this systems permanent connections
				{
					if(systemList[i].permanentConnections[k] == systemList[j].systemObject) //If target systems is already in permanent connections, continue;
					{
						skipSystem = true;
						break;
					}
				}

				if(skipSystem == true)
				{
					continue;
				}
		
				if(distance < distanceMax) //If distance is less than maximum distance, add it to temporary connections
				{
					Node nearbySystem = new Node();
					
					nearbySystem.targetSystem = systemList[j].systemObject;
					nearbySystem.targetDistance = distance;
					
					systemList[i].tempConnections.Add (nearbySystem);
				}
			}

			SortNearestConnections();

			if(systemList[i].tempConnections.Count < (systemList[i].numberOfConnections - systemList[i].permanentConnections.Count))
			{
				systemList[i].numberOfConnections = systemList[i].tempConnections.Count +  systemList[i].permanentConnections.Count;
			}
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

			if(systemList[i].tempConnections.Count > (systemList[i].numberOfConnections - systemList[i].permanentConnections.Count))
			{
				int tempInt = systemList[i].tempConnections.Count - (systemList[i].numberOfConnections - systemList[i].permanentConnections.Count);

				for(int j = 0; j < tempInt; ++j)
				{
					systemList[i].tempConnections.RemoveAt(systemList[i].numberOfConnections - systemList[i].permanentConnections.Count);
				}
			}
		}
	}

	private void ConnectSystems()
	{
		for(int j = 0; j < systemList.Count; ++j) //For all systems
		{
			if(systemList[j].numberOfConnections == systemList[j].permanentConnections.Count) //If the number of assigned systems equals the maximum number of systems, continue
			{
				continue;
			}

			bool ignoreSystem = false;

			for(int l = 0; l < systemList[j].tempConnections.Count; ++l) //For all tempconnections
			{
				if(systemList[j].numberOfConnections == systemList[j].permanentConnections.Count)
				{
					break;
				}

				int targetSystem = RefreshCurrentSystemA(systemList[j].tempConnections[l].targetSystem); //Get target system

				if(systemList[targetSystem].permanentConnections.Count < systemList[targetSystem].numberOfConnections) //If target/this system's connections are already full, continue
				{
					for(int m = 0; m < systemList[j].permanentConnections.Count; ++m) //If connections has already been made, continue
					{
						if(systemList[j].tempConnections[l].targetSystem == systemList[j].permanentConnections[m])
						{
							ignoreSystem = true;
						}
					}
					for(int m = 0; m < systemList[targetSystem].permanentConnections.Count; ++m)
					{
						if(systemList[targetSystem].permanentConnections[m] == systemList[j].systemObject)
						{
							ignoreSystem = true;
						}
					}

					if(TestForIntersection(systemList[j].systemObject, systemList[targetSystem].systemObject) == false)
					{
						continue;
					}

					if(ignoreSystem == false) //As above
					{
						AddPermanentSystem(j, targetSystem);
					}
				}
			}
		}

		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].permanentConnections.Count != systemList[i].numberOfConnections)
			{
				systemList[i].numberOfConnections = systemList[i].permanentConnections.Count;
			}

			//Debug.Log (systemList [i].numberOfConnections + " | " + systemList [i].permanentConnections.Count);
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
}

public class ConnectionCoordinates
{
	public GameObject systemA, systemB;
}

public class PlanetInfo
{
	public string planetType, planetCategory;
	public bool colonised;
	public int science, industry, improvementSlots;
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

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}

