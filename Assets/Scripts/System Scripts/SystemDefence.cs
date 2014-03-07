using UnityEngine;
using System.Collections;

public class SystemDefence : MasterScript 
{
	public int system, regenerateTimer;
	public float maxSystemDefence, defenceRegenerator;
	public bool underInvasion, canEnter, regenerated = true;

	void Start () 
	{
		systemSIMData = gameObject.GetComponent<SystemSIMData> ();
		CalculateSystemDefence ();
		system = RefreshCurrentSystem (gameObject);
	}

	public void CalculateSystemDefence() 
	{
		maxSystemDefence = 0f;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			maxSystemDefence += systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
		}

		maxSystemDefence = (maxSystemDefence / systemListConstructor.systemList [system].systemSize) * 10f;

		defenceRegenerator = maxSystemDefence / 5f;

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

			if(regenerateTimer <= 0 && systemListConstructor.systemList [system].systemDefence != maxSystemDefence)
			{
				regenerateTimer = 0;

				systemListConstructor.systemList [system].systemDefence += (int)defenceRegenerator;

				if(systemListConstructor.systemList [system].systemDefence >= maxSystemDefence)
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
				systemListConstructor.systemList [system].systemDefence = (int)maxSystemDefence;
			}
		}
	}

	public void CalculatePlanetDefence(int planet)
	{
		float maxPlanetDefence = systemListConstructor.systemList[system].planetsInSystem[planet].planetOwnership * systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel;
		
		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence != maxPlanetDefence)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence += defenceRegenerator;

			if(systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence > maxPlanetDefence)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence = maxPlanetDefence;
			}
		}
	}
}
