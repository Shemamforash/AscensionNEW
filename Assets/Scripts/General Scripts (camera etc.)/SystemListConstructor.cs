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
				int tempInt = 0;

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
					system.planetOwnership.Add (0);
					system.planetScience.Add (FindPlanetSIM(system.planetType[j], "Science"));
					system.planetIndustry.Add (FindPlanetSIM(system.planetType[j], "Industry"));
					system.planetMoney.Add (FindPlanetSIM(system.planetType[j], "Money"));
					system.improvementSlots.Add ((int)FindPlanetSIM(system.planetType[j], "Improvement Slots"));
					tempInt += (int)FindPlanetSIM(system.planetType[j], "Improvement Slots");
				}

				system.improvementsBuilt = new string[tempInt];
				
				systemList.Add (system);
			}
		}
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
	public string planetType;
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
	public string[] improvementsBuilt;

	public List<string> planetName = new List<string>();
	public List<string> planetType = new List<string>();

	public List<float> planetScience = new List<float>();
	public List<float> planetIndustry = new List<float>();
	public List<float> planetMoney = new List<float>();
	public List<bool> planetColonised = new List<bool>();

	public List<int> planetOwnership = new List<int> ();
	public List<int> planetImprovementLevel = new List<int>();
	public List<int> improvementSlots= new List<int>();
}

