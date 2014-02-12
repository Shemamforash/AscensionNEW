using UnityEngine;
using System.Collections;

public class SystemDefence : MasterScript 
{
	public int defenceRegenerator, system, regenerateTimer;
	public bool underInvasion, canEnter, regenerated = true;

	void Start () 
	{
		systemSIMData = gameObject.GetComponent<SystemSIMData> ();
		CalculateSystemDefence ();
		system = RefreshCurrentSystem (gameObject);
	}

	public void CalculateSystemDefence() 
	{
		int tempDefence = 0;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			tempDefence += systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
		}

		tempDefence = (tempDefence / systemListConstructor.systemList [system].systemSize) * 10;

		defenceRegenerator = tempDefence / 5;

		if(underInvasion == false)
		{
			if(regenerateTimer > 0)
			{
				--regenerateTimer;
			}

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				CalculatePlanetDefence(i);
			}

			if(regenerateTimer <= 0 && systemListConstructor.systemList [system].systemDefence != tempDefence)
			{
				regenerateTimer = 0;

				systemListConstructor.systemList [system].systemDefence += defenceRegenerator;

				if(systemListConstructor.systemList [system].systemDefence >= tempDefence)
				{
					regenerated = true;
				}
				else
				{
					regenerated = false;
				}
			}
			
			if(regenerated == true)
			{
				systemListConstructor.systemList [system].systemDefence = tempDefence;
			}
		}
	}

	public void CalculatePlanetDefence(int planet)
	{
		int maxDefence = systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership * systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel;
		
		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence != maxDefence)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence += defenceRegenerator;

			if(systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence > maxDefence)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence = maxDefence;
			}
		}
	}
}
