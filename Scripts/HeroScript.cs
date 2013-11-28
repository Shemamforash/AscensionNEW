using UnityEngine;
using System.Collections;

public class HeroScript : MonoBehaviour 
{
	public string[] heroesInSystem = new string[3];

	private TechTreeScript techTreeScript;
	private LineRenderScript lineRenderScript;
	private HeroScript heroScript;

	void Start()
	{
		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
	}

	public void CheckHeroesInSystem()
	{
		foreach(string hero in heroesInSystem)
		{
			switch(hero)
			{
			case "President":
				President ();
				break;

			default:
				break;
			}
		}
	}

	private void President()
	{
		techTreeScript.sciencePercentBonus += 0.05f;
		techTreeScript.industryPercentBonus += 0.05f;
		techTreeScript.moneyPercentBonus += 0.05f;

		foreach(GameObject system in lineRenderScript.connections)
		{
			if(system == null)
			{
				break;
			}

			heroScript = system.GetComponent<HeroScript>();

			int presidentNum = 0;

			foreach(string hero in heroScript.heroesInSystem)
			{
				if(hero == "President")
				{
					if(presidentNum > 0 && techTreeScript.leadership == false)
					{
						break;
					}

					else
					{
						techTreeScript.sciencePercentBonus += 0.025f;
						techTreeScript.industryPercentBonus += 0.025f;
						techTreeScript.moneyPercentBonus += 0.025f;
					}

					presidentNum++;
				}
			}
		}
	}
}
