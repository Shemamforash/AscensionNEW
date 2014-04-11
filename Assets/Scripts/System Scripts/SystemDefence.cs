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

		maxSystemDefence = (maxSystemDefence / systemListConstructor.systemList [system].systemSize) * 20f;
		systemListConstructor.systemList [system].systemOffence = (int)(maxSystemDefence / 2f);

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
		systemListConstructor.systemList [system].planetsInSystem[planet].planetOffence = maxPlanetDefence;

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence != maxPlanetDefence)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence += defenceRegenerator;

			if(systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence > maxPlanetDefence)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].planetDefence = maxPlanetDefence;
			}
		}
	}

	public void CheckStatusEffects(int planet)
	{
		systemSIMData.knowledgeBuffModifier = 1.0f;
		systemSIMData.powerBuffModifier = 1.0f;

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillActive == true)
		{
			Chill (0, planet);
		}
		if(systemListConstructor.systemList[system].planetsInSystem[planet].virusActive == true)
		{
			Virus (planet);
		}
		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonActive == true)
		{
			Poison(planet);
		}
	}

	private void Virus(int planet)
	{
		if(systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer == 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer = Time.time;
		}

		float timeDifference = Time.time - systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer;

		if(timeDifference > 600f)
		{
			timeDifference = 600f;
		}

		timeDifference = (timeDifference - 300f) / 191f;

		float sinDifference = Mathf.Sin(timeDifference) + 1;

		systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * sinDifference; // y = sin((x-300)/191) + 1
		systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * sinDifference;

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

			bool skipSystem = false;

			for(int k = 0; k < systemListConstructor.systemList[j].systemSize; ++k)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[k].virusActive == true)
				{
					skipSystem = true;
					break;
				}
			}

			if(skipSystem == false)
			{
				for(int k = 0; k < systemListConstructor.systemList[j].systemSize; ++k)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[k].virusActive == false)
					{
						float ratio = Mathf.Max(systemListConstructor.systemList[system].planetsInSystem[k].planetDefence, maxSystemDefence) / 
										Mathf.Min (systemListConstructor.systemList[system].planetsInSystem[k].planetDefence, maxSystemDefence);

						if(ratio * 2 < sinDifference)
						{
							systemListConstructor.systemList[system].planetsInSystem[k].virusActive = true;
							continue;
						}
					}
				}
			}
		}
	}

	private void Chill(int newLength, int planet)
	{
		if(newLength != 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = systemListConstructor.systemList[system].planetsInSystem[planet].chillLength + newLength;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer == 0.0f && systemListConstructor.systemList[system].planetsInSystem[planet].chillLength != 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = Time.time;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + systemListConstructor.systemList[system].planetsInSystem[planet].chillLength >= Time.time)
		{
			systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * 0.5f;
			systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * 0.5f;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + systemListConstructor.systemList[system].planetsInSystem[planet].chillLength < Time.time)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = 0.0f;
			systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = 0.0f;
			systemListConstructor.systemList[system].planetsInSystem[planet].chillActive = false;
		}
	}

	private void Poison(int planet)
	{
		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer == 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = Time.time;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f < Time.time)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f;

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				float ownership = systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;

				systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership -= (ownership / 40f) + 0.5f;
			}
		}
	}
}
