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

	private void Awake()
	{
		LoadSystemData();
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
				
				system.systemObject = GameObject.Find (system.systemName);
				
				system.systemSize = int.Parse (typeReader.ReadLine());
				
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
				
				systemList.Add (system);
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
	public GameObject systemObject;
	public GameObject tradeRoute;
	public int systemSize;
	public GameObject[] heroesInSystem = new GameObject[3];
	public List<Planet> planetsInSystem = new List<Planet> ();
}

public class Planet
{
	public string planetName, planetType, planetCategory;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetScience, planetIndustry, planetMoney;
	public bool planetColonised;
	public int planetOwnership, planetImprovementLevel, improvementSlots, maxOwnership;
}

