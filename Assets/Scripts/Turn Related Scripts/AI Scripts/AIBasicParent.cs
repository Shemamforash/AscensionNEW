using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, highestSIM, tempFloat;
	private int tempPlanet, tempSystem, tempPlanetB, tempSystemB, currentPlanet, currentSystem, checkHeroTimer = 0, tempTech;
	private bool saveForHero;
	private TurnInfo thisPlayer;
	private AIHeroBehaviour heroBehaviour;
	private GenericImprovements improvements;

	public void Start()
	{
		improvements = GameObject.Find ("ScriptsContainer").GetComponent<GenericImprovements> ();
	}

	public void Expand(TurnInfo player)
	{
		thisPlayer = player;

		for(float i = thisPlayer.capital; i > 0; --i)
		{
			CheckToSaveForHero();

			if(saveForHero == false)
			{
				AIExpansion();
			}

			if(saveForHero == true)
			{
				string temp = heroBehaviour.SetSpecialisation();
				heroGUI.CheckIfCanHire(thisPlayer, temp);
			}
		}

		turnInfoScript.TurnEnd(thisPlayer);

		if(heroBehaviour == null)
		{
			heroBehaviour = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<AIHeroBehaviour> ();
		}

		heroBehaviour.HeroDecisionStart (player);
	}

	private void OptimumTechToBuild(int system)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();

		float sciRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemScience;
		float indRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemIndustry;

		tempFloat = 0f;
		tempTech = -1;
		tempPlanet = -1;

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementCost > thisPlayer.industry)
			{
				continue;
			}

			if(improvementsBasic.listOfImprovements[i].improvementLevel <= improvementsBasic.techTier && improvementsBasic.listOfImprovements[i].hasBeenBuilt == false)
			{
				improvements.TechSwitch(i, improvementsBasic, thisPlayer, true);
				bool okToBuild = false;

				if(improvementsBasic.planetToBuildOn.Count > 0)
				{
					for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem.Count; ++j)
					{
						if(improvementsBasic.planetToBuildOn.Contains(systemListConstructor.systemList[system].planetsInSystem[j].planetType))
						{
							okToBuild = true;
							tempPlanet = j;
						}
					}
				}

				if(tempPlanet != -1)
				{
					int tempPlanSize = 0;

					for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem.Count; ++j)
					{
						int noSlots = 0;

						for(int k = 0; k < systemListConstructor.systemList[system].planetsInSystem[j].improvementSlots; ++k)
						{
							if(systemListConstructor.systemList[system].planetsInSystem[j].improvementsBuilt[k] == null)
							{
								++noSlots;
							}
						}

						if(noSlots > tempPlanSize)
						{
							tempPlanSize = noSlots;
							tempPlanet = j;
						}
					}
				}

				if(improvementsBasic.planetToBuildOn.Count == 0)
				{
					okToBuild = true;
				}

				if(okToBuild == true)
				{
					RacialTechCheck(i);
					GenericTechCheck(i);
				}
			}
		}

		if(tempTech != -1f)
		{
			if(improvementsBasic.ImproveSystem(system) == true)
			{
				for(int i = 0; i < systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementSlots; ++i)
				{
					if(systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementsBuilt[i] == null)
					{
						systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementsBuilt[i] = improvementsBasic.listOfImprovements[i].improvementName;
					}
				}
			}
		}
	}

	private void RacialTechCheck(int i)
	{
		if(thisPlayer.playerRace == "Humans")
		{
			if(improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
			{
				if(improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
				{
					tempFloat = improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(thisPlayer.playerRace == "Selkies")
		{
			if((improvementsBasic.tempAmberPenalty + improvementsBasic.amberPenalty) * 10 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.tempAmberPenalty > improvementsBasic.amberPenalty)
				{
					tempFloat = (improvementsBasic.tempAmberPenalty + improvementsBasic.amberPenalty) * 10 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
			
			if(improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
				{
					if(tempTech == i)
					{
						tempFloat += improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					}
					else
					{
						tempFloat = improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
						tempTech = i;
					}
				}
			}
			
			if(improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
				{
					if(tempTech == i)
					{
						tempFloat += improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
					}
					else
					{
						tempFloat = improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
						tempTech = i;
					}
				}
			}
		}
	}

	private void GenericTechCheck(int i)
	{
		if(improvementsBasic.tempIndUnitBonus * 4 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempIndUnitBonus > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempIndUnitBonus * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempIndUnitBonus * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempSciUnitBonus * 4 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempSciUnitBonus > tempFloat || tempTech == i)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempSciUnitBonus * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempSciUnitBonus * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempCapital * 25 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempCapital * 25 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempCapital * 25 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempCapital * 25 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
	}

	private void CheckToSaveForHero()
	{
		checkHeroTimer++;
		
		if(checkHeroTimer == 6)
		{
			float temp = 0;
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
			{
				if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
				{
					systemSIMData = systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();
					temp += systemSIMData.totalSystemScience + systemSIMData.totalSystemIndustry;
				}
				
				if(temp >= ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = true;
				}

				else if(temp < ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = false;
				}
			}

			checkHeroTimer = 0;
		}
	}

	public void AIExpansion()
	{
		if(thisPlayer.capital > 1)
		{
			turnInfoScript.RefreshPlanetPower();

			currentPlanet = -1;
			currentSystem = -1;

			float planetSIM = CheckThroughPlanets (thisPlayer);
			
			float systemSIM = CheckThroughSystems (thisPlayer);
			
			if(planetSIM > systemSIM && thisPlayer.capital >= systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].capitalValue)
			{
				currentPlanet = tempPlanet;
				
				currentSystem = tempSystem;

				thisPlayer.capital -= systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].capitalValue;

				thisPlayer.capitalModifier += 0.05f;
			}
			
			if(systemSIM > planetSIM && thisPlayer.capital >= 20.0f)
			{
				currentPlanet = tempPlanetB;
				
				currentSystem = tempSystemB;
				
				systemListConstructor.systemList[currentSystem].systemOwnedBy = thisPlayer.playerRace;
				
				lineRenderScript = systemListConstructor.systemList[currentSystem].systemObject.GetComponent<LineRenderScript>();
				
				lineRenderScript.SetRaceLineColour(thisPlayer.playerRace);
				
				systemListConstructor.systemList[currentSystem].systemObject.renderer.material = thisPlayer.materialInUse;

				ambientStarRandomiser.AmbientColourChange(currentSystem);

				++systemsInPlay;

				++thisPlayer.systemsColonisedThisTurn;

				thisPlayer.capital -= 20.0f;

				thisPlayer.capitalModifier += 0.1f;

				empireBoundaries.ModifyBoundaryCircles ();
			}

			if(currentPlanet != -1 && currentSystem != -1)
			{
				systemListConstructor.systemList[currentSystem].planetsInSystem[currentPlanet].planetColonised = true;
				
				++thisPlayer.planetsColonisedThisTurn;
			}

			CheckToImprovePlanet (thisPlayer);
		}
	}

	public float CheckThroughPlanets(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				systemDefence = systemListConstructor.systemList [i].systemObject.GetComponent<SystemDefence> ();
				
				if(systemDefence.underInvasion == true)
				{
					continue;
				}

				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true || systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel == 3)
					{
						continue;
					}

					tempSIM = (systemListConstructor.systemList[i].planetsInSystem[j].planetScience + systemListConstructor.systemList[i].planetsInSystem[j].planetIndustry)
						* (systemListConstructor.systemList[i].planetsInSystem[j].improvementSlots * 1.5f);

					if(tempSIM > highestSIM)
					{
						highestSIM = tempSIM;

						tempPlanet = j;
						
						tempSystem = i;
					}
				}
			}
		}

		return highestSIM;
	}

	public float CheckThroughSystems(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				lineRenderScript = systemListConstructor.systemList[i].systemObject.GetComponent<LineRenderScript>();

				for(int j = 0; j < systemListConstructor.systemList[i].numberOfConnections; ++j)
				{
					tempSIM = 0.0f;

					int k = RefreshCurrentSystem(systemListConstructor.systemList[i].permanentConnections[j]);

					if(systemListConstructor.systemList[k].systemOwnedBy == null)
					{
						float tempPlanetSIM = 0.0f;
						int tempPlanet = -1;
						float tempHighestPlanetSIM = 0.0f;

						for(int l = 0; l < systemListConstructor.systemList[k].systemSize; ++l)
						{
							tempPlanetSIM = (systemListConstructor.systemList[k].planetsInSystem[l].planetScience + systemListConstructor.systemList[k].planetsInSystem[l].planetIndustry)  
								* (systemListConstructor.systemList[k].planetsInSystem[l].improvementSlots);

							if(tempPlanetSIM > tempHighestPlanetSIM)
							{
								tempHighestPlanetSIM = tempPlanetSIM;

								tempPlanet = l;
							}

							tempSIM += tempPlanetSIM;
						}

						tempSIM = (tempSIM / systemListConstructor.systemList[k].systemSize);

						if(tempSIM > highestSIM)
						{
							highestSIM = tempSIM;

							tempSystemB = k;

							tempPlanetB = tempPlanet;
						}
					}
				}
			}
		}

		return highestSIM;
	}

	public void CheckToImprovePlanet(TurnInfo thisPlayer)
	{
		for(int i = 0; i < turnInfoScript.mostPowerfulPlanets.Count - 1; i++)
		{
			if(thisPlayer.industry < 0.8f && thisPlayer.capital < 1.0f)
			{
				break;
			}

			int j = RefreshCurrentSystem(turnInfoScript.mostPowerfulPlanets[i].system);

			if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
			{
				ImprovePlanet(turnInfoScript.mostPowerfulPlanets[i].planetPosition, j, thisPlayer);
			}
		}
	}
	
	public void ImprovePlanet(int planetPosition, int system, TurnInfo thisPlayer)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemSIMData.improvementNumber = systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
		
		systemFunctions.CheckImprovement(system, planetPosition);
		
		if(systemSIMData.canImprove == true && systemDefence.underInvasion == false)
		{
			float industryImprovementCost = systemFunctions.IndustryCost(systemSIMData.improvementNumber, system, planetPosition);

			if(thisPlayer.industry >= industryImprovementCost && thisPlayer.capital >= systemSIMData.improvementCost)
			{
				++systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				thisPlayer.industry -= industryImprovementCost;

				thisPlayer.capital -= systemSIMData.improvementCost;
			}
		}
	}
}

