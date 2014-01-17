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
	private List<GameObject> nearestSystems = new List<GameObject> ();
	private bool systemUsed = false;

	private void Awake()
	{
		LoadSystemData();
		AssignMaximumConnections ();
		DrawMapSkeleton ();
		for(int i = 0; i < systemList.Count; ++i)
		{
			GenerateConnections (i);
		}
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

	public void DrawMapSkeleton()
	{
		List<GameObject> unlinkedSystems = new List<GameObject> ();
		List<GameObject> linkedSystems = new List<GameObject> ();

		for(int i = 0; i < systemList.Count; ++i)
		{
			unlinkedSystems.Add (systemList[i].systemObject);
		}

		linkedSystems.Add (unlinkedSystems [0]);

		unlinkedSystems.Remove (linkedSystems[0]);

		using(StreamWriter writer = new StreamWriter("SkeletonConnections.txt"))
		{
			for(int k = 0; k < systemList.Count - 1; ++k)
			{
				int nearestSystem = 0;
				int selectedSystem = 0;
				float tempDistance = distanceMax;
				bool connect = false;

				for(int i = 0; i < linkedSystems.Count; ++i)
				{
					int system = RefreshCurrentSystemA(linkedSystems[i]);

					if(systemList[system].connectedSystems.Count < systemList[system].numberOfConnections)
					{
						for(int j = 0; j < unlinkedSystems.Count; ++j)
						{
							float distance = Vector3.Distance (linkedSystems[i].transform.position, unlinkedSystems[j].transform.position);
							
							if(distance < tempDistance)
							{
								tempDistance = distance;
								nearestSystem = j;
								selectedSystem = i;
								connect = true;
							}
						}
					}
				}

				if(connect == true)
				{
					int tempInt = RefreshCurrentSystemA (linkedSystems [selectedSystem]);

					systemList [tempInt].connectedSystems.Add (unlinkedSystems [nearestSystem]);

					int tempInt2 = RefreshCurrentSystemA(unlinkedSystems[nearestSystem]);

					systemList[tempInt2].connectedSystems.Add (linkedSystems[selectedSystem]);

					writer.WriteLine(systemList[tempInt].systemName + " | " + systemList[tempInt].connectedSystems[0]);

					linkedSystems.Add (unlinkedSystems [nearestSystem]);

					unlinkedSystems.Remove (unlinkedSystems [nearestSystem]);
				}
			}
		}
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
			int tempInt = Random.Range (0, 99);
			
			if(tempInt < 15)
			{
				connections = 2;
			}
			if(tempInt >= 15 && tempInt < 35)
			{
				connections = 3;
			}
			if(tempInt >= 35 && tempInt < 65)
			{
				connections = 4;
			}
			if(tempInt >= 65 && tempInt < 85)
			{
				connections = 5;
			}
			if(tempInt >= 85 && tempInt < 99)
			{
				connections = 6;
			}

			if(systemList[i].systemName == "Heracles")
			{
				connections = 5;
			}
			if(systemList[i].systemName == "Sol")
			{
				connections = 4;
			}
			if(systemList[i].systemName == "Nepthys")
			{
				connections = 3;
			}
			
			systemList[i].numberOfConnections = connections;
		}
	}

	private void SelectSystems(int system, int availableConnections)
	{
		for(int i = 1; i <= availableConnections; ++i)
		{
			float tempDistance = distanceMax;
			int tempSystem = -1;
			systemUsed = false;

			for(int j = 0; j < systemList.Count; ++j)
			{
				if(j == system)
				{
					continue;
				}

				for(int k = 0; k < nearestSystems.Count; ++k)
				{
					if(nearestSystems[k] == systemList[j].systemObject)
					{
						systemUsed = true;
						break;
					}
				}

				for(int k = 0; k < systemList[system].connectedSystems.Count; ++k) //For all systems connected to current system
				{
					if(systemList[system].connectedSystems[k] == systemList[j].systemObject) //If target system is already connected, ignore it
					{
						systemUsed = true;

						break;
					}
				}

				if(systemUsed == false)
				{
					if(systemList[j].connectedSystems.Count < systemList[j].numberOfConnections)
					{
						distanceXY = Vector3.Distance(systemList[system].systemObject.transform.position, systemList[j].systemObject.transform.position); //Distance between systems
					
						if(distanceXY < tempDistance) //If distance is less than the maximum range, 
						{
							tempDistance = distanceXY;
							tempSystem = j;
						}
					}
				}
			}

			if(tempSystem != -1)
			{
				nearestSystems.Add (systemList[tempSystem].systemObject);
			}
		}
	}

	private void GenerateConnections(int system)
	{
		nearestSystems.Clear();

		int availableConnections = systemList [system].numberOfConnections - systemList [system].connectedSystems.Count;
		
		if(availableConnections != 0)
		{
			SelectSystems(system, availableConnections);
		}

		if(nearestSystems.Count > 1)
		{
			for(int j = nearestSystems.Count - 1; j >= 0; --j) //Sort nearest systems in order of size
			{
				bool swaps = false;
				
				for(int k = 1; k <= j; ++k)
				{
					float tempDistanceA = Vector3.Distance (systemList[system].systemObject.transform.position, nearestSystems[k-1].transform.position);
					float tempDistanceB = Vector3.Distance (systemList[system].systemObject.transform.position, nearestSystems[k].transform.position);
					
					if(tempDistanceA > tempDistanceB)
					{
						GameObject tempSystem = nearestSystems[k-1];
						
						nearestSystems[k-1] = nearestSystems[k];
						nearestSystems[k] = tempSystem;
						
						swaps = true;
					}
				}
				
				if(swaps == false)
				{
					break;
				}
			}
		}

		for(int j = 0; j < nearestSystems.Count; ++j)
		{
			systemList[system].connectedSystems.Add (nearestSystems[j]);

			int k = RefreshCurrentSystemA(nearestSystems[j]);

			systemList[k].connectedSystems.Add (systemList[system].systemObject);
		}

		if(systemList[system].connectedSystems.Count != systemList[system].numberOfConnections)
		{
			systemList[system].numberOfConnections = systemList[system].connectedSystems.Count;
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
	public List<GameObject> connectedSystems = new List<GameObject>();
}

public class Planet
{
	public string planetName, planetType, planetCategory;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetScience, planetIndustry, planetMoney;
	public bool planetColonised;
	public int planetOwnership, planetImprovementLevel, improvementSlots, maxOwnership;
}

