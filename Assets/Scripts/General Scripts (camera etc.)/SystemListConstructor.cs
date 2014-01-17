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

	private void Awake()
	{
		LoadSystemData();
		AssignMaximumConnections ();
		for(int i = 0; i < systemList.Count; ++i)
		{
			GenerateConnections (systemList[i].systemObject, i);
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
				connections = 1;
			}
			if(tempInt >= 15 && tempInt < 35)
			{
				connections = 2;
			}
			if(tempInt >= 35 && tempInt < 65)
			{
				connections = 3;
			}
			if(tempInt >= 65 && tempInt < 85)
			{
				connections = 4;
			}
			if(tempInt >= 85)
			{
				connections = 5;
			}
			
			systemList[i].numberOfConnections = connections;
		}
	}

	private void GenerateConnections(GameObject system, int position)
	{
		nearestSystems = null;

		for(int i = 0; i < systemList.Count; ++i) //For all systems except this one
		{
			if(systemList[i].systemObject == system)
			{
				continue;
			}

			bool systemUsed = false;

			for(int j = 0; j < systemList[i].connectedSystems.Count; ++j) //For all connected systems of target system
			{
				if(systemList[i].connectedSystems[j] == null)
				{
					continue;
				}

				if(systemList[i].connectedSystems[j] == system) //If it is connected to this system
				{
					systemList[position].connectedSystems.Add(systemList[i].systemObject); //Add target system to this systems list of connections
					systemUsed = true;
					break;
				}
			}

			if(systemUsed == false)
			{
				distanceXY = Vector3.Distance(system.transform.position, systemList[i].systemObject.transform.position); //Distance between systems

				if(distanceXY < distanceMax) //If distance is less than the maximum range, 
				{
					if(nearestSystems.Count < connections && systemUsed == false) //If there are still spaces in nearestSystem list
					{
						nearestSystems.Add(systemList[i].systemObject); //Add this list

						systemUsed = true; //Tell rest of code that this system has been assigned already
					}

					if(systemUsed == false) //If system not assigned
					{
						for(int j = 0; j < nearestSystems.Count; ++j) //For all systems in nearestSystems list
						{
							float tempDistance = Vector3.Distance (system.transform.position, nearestSystems[j].transform.position); //Assign temporary distance
							int tempPos = 0;
							
							if(distanceXY < tempDistance) //If distance of current object is less than an object in the nearestSystems list
							{
								tempPos = j;
							}

							nearestSystems[tempPos] = systemList[i].systemObject; //Replace position in nearestSystems with current object
						}
					}
				}
			}
		}

		if(nearestSystems.Count > 1)
		{
			for(int j = nearestSystems.Count - 1; j >= 0; --j) //Sort nearest systems in order of size
			{
				bool swaps = false;
				
				for(int k = 1; k <= j; ++k)
				{
					float tempDistanceA = Vector3.Distance (system.transform.position, nearestSystems[k-1].transform.position);
					float tempDistanceB = Vector3.Distance (system.transform.position, nearestSystems[k].transform.position);
					
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
			systemList[position].connectedSystems.Add (nearestSystems[j]);

			if(systemList[position].connectedSystems.Count == systemList[position].numberOfConnections || nearestSystems[j] == null)
			{
				break;
			}
		}

		if(systemList[position].connectedSystems.Count < systemList[position].numberOfConnections)
		{
			systemList[position].numberOfConnections = systemList[position].connectedSystems.Count;
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

