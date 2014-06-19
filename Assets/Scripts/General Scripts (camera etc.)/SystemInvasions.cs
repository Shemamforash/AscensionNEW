using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemInvasions : MasterScript
{
	public GameObject invasionQuad;
	public HeroScriptParent hero;
	private List<SystemInvasionInfo> currentInvasions = new List<SystemInvasionInfo>();

	public void PlanetInvasion(HeroScriptParent curHero, int system, int planet, bool click)
	{
		if(systemListConstructor.systemList [system].planetsInSystem [planet].underEnemyControl == false)
		{
			if(click == false)
			{
				systemListConstructor.systemList [system].planetsInSystem [planet].planetPopulation -= curHero.auxiliaryDamage / 4f;
				curHero.currentHealth -= systemListConstructor.systemList [system].planetsInSystem[planet].planetOffence / (curHero.currentHealth * curHero.classModifier);
			}
			
			if(systemListConstructor.systemList [system].planetsInSystem [planet].planetPopulation <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised = false;
				systemListConstructor.systemList [system].planetsInSystem [planet].expansionPenaltyTimer = 0f;
				systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt.Clear ();
				systemListConstructor.systemList [system].planetsInSystem [planet].planetImprovementLevel = 0;
				systemListConstructor.systemList [system].planetsInSystem [planet].planetPopulation = 0;
				curHero.planetInvade = -1;
			}

			else if(systemListConstructor.systemList [system].planetsInSystem [planet].planetCurrentDefence <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planet].underEnemyControl = true;
				curHero.planetInvade = -1;
			}
		}
		
		CheckSystemStatus (system, planet);
	}
	
	private void CheckSystemStatus(int system, int planet)
	{
		int planetsDestroyed = 0;
		int planetsEnemyControlled = 0;
		
		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			if(systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == false)
			{
				++planetsDestroyed;
				continue;
			}
			
			if(systemListConstructor.systemList [system].planetsInSystem [i].underEnemyControl == true)
			{
				++planetsEnemyControlled;
			}
		}
		
		if(planetsDestroyed == systemListConstructor.systemList [system].systemSize)
		{
			DestroySystem(system);

			invasionGUI.openInvasionMenu = false;

			for(int i = 0; i < systemListConstructor.systemList[system].planetsInSystem.Count; ++i)
			{
				systemListConstructor.systemList[system].planetsInSystem[i].underEnemyControl = false;
			}
		}
		
		else if(planetsDestroyed + planetsEnemyControlled == systemListConstructor.systemList [system].systemSize && planetsDestroyed != systemListConstructor.systemList [system].systemSize)
		{
			OwnSystem(system);

			invasionGUI.openInvasionMenu = false;

			for(int i = 0; i < systemListConstructor.systemList[system].planetsInSystem.Count; ++i)
			{
				systemListConstructor.systemList[system].planetsInSystem[i].underEnemyControl = false;
			}
		}

		if(planetsDestroyed + planetsEnemyControlled == systemListConstructor.systemList[system].systemSize)
		{
			for(int i = 0; i < turnInfoScript.allPlayers.Count; ++i)
			{
				if(turnInfoScript.allPlayers[i].playerRace == hero.heroOwnedBy)
				{
					hero.aiInvadeTarget = -1;
				}
			}
		}
	}
	
	private void DestroySystem(int system)
	{
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemListConstructor.systemList [system].systemDefence = 0;
		systemListConstructor.systemList [system].systemOwnedBy = null;
		
		turnInfoScript = GameObject.Find ("ScriptsContainer").GetComponent<TurnInfo> ();
		
		//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.emptyMaterial;

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		systemDefence.underInvasion = false;
		empireBoundaries.ModifyBoundaryCircles ();
	}
	
	private void OwnSystem(int system)
	{
		systemListConstructor.systemList [system].systemOwnedBy = hero.heroOwnedBy;

		switch(hero.heroOwnedBy)
		{
		case "Humans":
			//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.humansMaterial;
			break;
		case "Selkies":
			//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.selkiesMaterial;
			break;
		case "Nereides":
			//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.nereidesMaterial;
			break;
		default:
			//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.emptyMaterial;
			break;
		}

		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = false;
		
		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				for(int k = 0; k < improvementsBasic.listOfImprovements.Count; ++k)
				{
					if(systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt[j] == improvementsBasic.listOfImprovements[k].improvementName)
					{
						if(improvementsBasic.listOfImprovements[k].improvementCategory != "Generic" || improvementsBasic.listOfImprovements[k].improvementCategory != playerTurnScript.playerRace)
						{
							systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.RemoveAt(j);
							improvementsBasic.listOfImprovements[k].hasBeenBuilt = false;
						}
					}
				}
			}
		}

		empireBoundaries.ModifyBoundaryCircles ();
	}
	
	public void StartSystemInvasion(int system)
	{
		hero.isInvading = true;
		
		hero.invasionObject = (GameObject)Instantiate (invasionQuad, systemListConstructor.systemList[system].systemObject.transform.position, 
		                                          systemListConstructor.systemList[system].systemObject.transform.rotation);
		
		systemListConstructor.systemList [system].enemyHero = gameObject;
		
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = true;
	}
	
	public void ContinueInvasion(int system)
	{		
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		hero.currentHealth -= systemListConstructor.systemList [system].systemOffence / (hero.currentHealth * hero.classModifier);
		systemListConstructor.systemList [system].systemDefence -= hero.assaultDamage;

		DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (hero.heroOwnedBy, systemListConstructor.systemList[system].systemOwnedBy);
		temp.stateCounter -= 1;
		
		if(systemListConstructor.systemList [system].systemDefence <= 0)
		{
			systemListConstructor.systemList [system].systemDefence = 0;
			systemDefence.canEnter = true;
			Destroy(hero.invasionObject);
		}
	}
}

public class SystemInvasionInfo
{
	public int system;
	public List<int> assaultTokenAllocation = new List<int>();
	public List<int> auxiliaryTokenAllocation = new List<int>();
	public List<int> defenceTokenAllocation = new List<int>();
	public List<Vector3> tokenPositions = new List<Vector3> ();
}
