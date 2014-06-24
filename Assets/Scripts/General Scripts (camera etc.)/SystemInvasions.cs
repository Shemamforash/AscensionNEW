using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemInvasions : MasterScript
{
	public GameObject invasionQuad;
	public HeroScriptParent hero;
	public List<SystemInvasionInfo> currentInvasions = new List<SystemInvasionInfo>();

	private float CalculateTotalTokenValue(List<GameObject> tokenList)
	{
		float total = 0;
		
		for(int k = 0; k < tokenList.Count; ++k)
		{
			heroScript = tokenList[k].GetComponent<HeroScriptParent>();
			
			total += heroScript.assaultDamage / heroScript.assaultTokens;
		}

		return total;
	}

	public void UpdateInvasions()
	{
		for(int i = 0; i < currentInvasions.Count; ++i)
		{
			systemDefence = currentInvasions[i].system.GetComponent<SystemDefence>();
			int system = RefreshCurrentSystem(currentInvasions[i].system);

			for(int j = 0; j < systemListConstructor.systemList[system].systemSize; ++j)
			{
				float assaultDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].assaultTokenAllocation);
				float auxiliaryDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].auxiliaryTokenAllocation) - systemListConstructor.systemList[system].planetsInSystem[j].planetCurrentDefence / 10f;

				systemDefence.TakeDamage(assaultDamage, auxiliaryDamage, j);

				if(systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation <= 0)
				{
					systemListConstructor.systemList [system].planetsInSystem [j].planetColonised = false;
					systemListConstructor.systemList [system].planetsInSystem [j].expansionPenaltyTimer = 0f;
					systemListConstructor.systemList [system].planetsInSystem [j].improvementsBuilt.Clear ();
					systemListConstructor.systemList [system].planetsInSystem [j].planetImprovementLevel = 0;
					systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation = 0;
				}
			}

			CheckSystemStatus (system, currentInvasions[i].player);
		}
	}

	private void CheckSystemStatus(int system, string player)
	{
		int planetsDestroyed = 0;

		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			if(systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == false)
			{
				++planetsDestroyed;
				continue;
			}
		}
		
		if(planetsDestroyed == systemListConstructor.systemList [system].systemSize)
		{
			bool captured = false;

			for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
			{
				int sys = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

				if(systemListConstructor.systemList[sys].systemOwnedBy == player)
				{
					OwnSystem(system);
					captured = true;
					break;
				}
			}

			if(captured == false)
			{
				DestroySystem(system);
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
				systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Clear ();
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
	public GameObject system;
	public string player;
	public List<PlanetInvasionInfo> tokenAllocation = new List<PlanetInvasionInfo> ();
}

public class PlanetInvasionInfo
{
	public List<GameObject> assaultTokenAllocation = new List<GameObject>();
	public List<GameObject> auxiliaryTokenAllocation = new List<GameObject>();
	public List<GameObject> defenceTokenAllocation = new List<GameObject>();
}
