using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HeroTechTree : MasterScript 
{
	public List<HeroTech> heroTechList = new List<HeroTech>();

	private void Start () 
	{
		ReadTechFile ();
	}

	private void ReadTechFile()
	{
		using(StreamReader reader = new StreamReader("HeroTechTreeData.txt"))
		{
			int counter = 0;

			while (counter != 24)
			{
				bool nextTech = false;

				HeroTech tech = new HeroTech();

				tech.techName = reader.ReadLine();
				tech.techType = reader.ReadLine();
				tech.heroType = reader.ReadLine();
				tech.scienceCost = int.Parse (reader.ReadLine());

				while(nextTech == false)
				{
					string text = reader.ReadLine();

					switch (text)
					{
						case "Armour":
							tech.armourRating = int.Parse (reader.ReadLine());
							break;
						case "Engine":
							tech.engineRating = int.Parse (reader.ReadLine());
							break;
						case "Primary":
							tech.primaryOffenceRating = int.Parse (reader.ReadLine());
							break;
						case "Secondary":
							tech.secondaryOffenceRating = int.Parse (reader.ReadLine());
							break;
						case "Collateral":
							tech.collateralRating  = int.Parse (reader.ReadLine());
							break;
						case "Stealth":
							tech.stealthRating = int.Parse (reader.ReadLine());
							break;
						case "Trade":
							tech.logisticsRating = int.Parse (reader.ReadLine());
							break;
						case "Prerequisite":
							tech.prerequisite = reader.ReadLine();
							break;
						default:
							nextTech = true;
							break;
					}
				}

				if(tech.prerequisite == null)
				{
					tech.canPurchase = true;
				}

				heroTechList.Add (tech);
				++counter;
			}
		}
	}
}

public class HeroTech
{
	public int scienceCost = 0, armourRating = 0, primaryOffenceRating = 0, secondaryOffenceRating = 0, engineRating = 0, collateralRating = 0, stealthRating = 0, logisticsRating = 0;
	public string heroType, techType, techName, prerequisite = null;
	public bool canPurchase = false ,isActive = false;
}
