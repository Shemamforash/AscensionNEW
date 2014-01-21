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

	public bool TestForIntersection(GameObject thisSystem, GameObject targetSystem)
	{
		float tempCoefficientA = targetSystem.transform.position.y - thisSystem.transform.position.y;
		float tempCoefficientB = thisSystem.transform.position.x - targetSystem.transform.position.x;
		float tempCoefficientC = (tempCoefficientA * thisSystem.transform.position.x) + (tempCoefficientB * thisSystem.transform.position.y);

		for(int i = 0; i < coordinateList.Count; ++i)
		{
			float parallel = (tempCoefficientA * coordinateList[i].coefficientB) - (coordinateList[i].coefficientA * tempCoefficientB);

			if(parallel == 0)
			{
				continue;
			}

			float xIntersect = ((coordinateList[i].coefficientB * tempCoefficientC) - (tempCoefficientB * coordinateList[i].coefficientC)) / parallel;
			float yIntersect = ((tempCoefficientA * coordinateList[i].coefficientC) - (coordinateList[i].coefficientA * tempCoefficientC)) / parallel;

			if(Mathf.Min (targetSystem.transform.position.x, thisSystem.transform.position.x) > xIntersect || xIntersect > Mathf.Max (targetSystem.transform.position.x, thisSystem.transform.position.x))
			{
				if(Mathf.Min (targetSystem.transform.position.y, thisSystem.transform.position.y) > yIntersect || yIntersect > Mathf.Max (targetSystem.transform.position.y, thisSystem.transform.position.y))
				{
					continue;
				}
			}

			return false;
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

		connection.coefficientA = systemList[nearestSystem].systemObject.transform.position.y - systemList[thisSystem].systemObject.transform.position.y;
		connection.coefficientB = systemList[thisSystem].systemObject.transform.position.x - systemList[nearestSystem].systemObject.transform.position.x;
		connection.coefficientC = (connection.coefficientA * systemList[thisSystem].systemObject.transform.position.x) 
			+ (connection.coefficientB * systemList[thisSystem].systemObject.transform.position.y);

		connection.xMax = Mathf.Max (systemList [thisSystem].systemObject.transform.position.x, systemList [nearestSystem].systemObject.transform.position.x);
		connection.xMin = Mathf.Min (systemList [thisSystem].systemObject.transform.position.x, systemList [nearestSystem].systemObject.transform.position.x);
		connection.yMax = Mathf.Max (systemList [thisSystem].systemObject.transform.position.y, systemList [nearestSystem].systemObject.transform.position.y);
		connection.yMin = Mathf.Min (systemList [thisSystem].systemObject.transform.position.y, systemList [nearestSystem].systemObject.transform.position.y);

		coordinateList.Add (connection);
	}

	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemList.Count; ++i) //For all systems
		{
			int randomInt = Random.Range (1,6); //Generate number

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

						if(ignoreSystem == false) //As above
					{
						AddPermanentSystem(j, targetSystem);
					}
				}
			}
		}

		/*for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].permanentConnections.Count < systemList[i].numberOfConnections)
			{
				for(int j = 0; j < systemList[i].numberOfConnections - systemList[i].permanentConnections.Count; ++j)
				{
					int targetPlanet = 0;

					for(int k = 0; k < systemList.Count; ++k)
					{
						float tempDistance = 5000.0f;

						if(k == i)
						{
							continue;
						}

						if(systemList[k].permanentConnections.Count < systemList[k].numberOfConnections)
						{
							float distance = Vector3.Distance(systemList[i].systemObject.transform.position, systemList[k].systemObject.transform.position);

							if(distance < tempDistance)
							{
								tempDistance = distance;
								targetPlanet = k;
							}

							if(TestForIntersection(systemList[i].systemObject, systemList[targetPlanet].systemObject) == true)
							{
								AddPermanentSystem(i, k);
							}
						}
					}
				}
			}
		}*/

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
				else if(resourceType == "Money")
				{
					return planetList[i].money;
				}
			}
		}
		
		return 0.0f;
	}
}

public class ConnectionCoordinates
{
	public float coefficientA;
	public float coefficientB;
	public float coefficientC;
	public float xMax, xMin, yMax, yMin;
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

