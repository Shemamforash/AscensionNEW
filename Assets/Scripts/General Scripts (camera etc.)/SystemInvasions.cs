using UnityEngine;
using System.Collections;

public class SystemInvasions : MasterScript
{
	public GameObject invasionQuad;
	public HeroScriptParent hero;

	public void PlanetInvasion(int system, int planet)
	{
		if(systemListConstructor.systemList [system].planetsInSystem [planet].underEnemyControl == false)
		{
			systemListConstructor.systemList [system].planetsInSystem [planet].planetDefence -= hero.secondaryWealth;
			systemListConstructor.systemList [system].planetsInSystem [planet].planetOwnership -= hero.secondaryCollateral;
			hero.currentArmour -= systemListConstructor.systemList [system].planetsInSystem[planet].planetOffence / (hero.currentArmour * hero.classModifier);
			
			if(systemListConstructor.systemList [system].planetsInSystem [planet].planetOwnership <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised = false;
				systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt.Clear ();
				systemListConstructor.systemList [system].planetsInSystem [planet].planetImprovementLevel = 0;
				systemListConstructor.systemList [system].planetsInSystem [planet].planetOwnership = 0;
				hero.planetInvade = -1;
			}
			else if(systemListConstructor.systemList [system].planetsInSystem [planet].planetDefence <= 0)
			{
				systemListConstructor.systemList [system].planetsInSystem [planet].underEnemyControl = true;
				hero.planetInvade = -1;
			}
		}
		
		CheckSystemStatus (system, planet);
	}
	
	private void CheckSystemStatus(int system, int planet)
	{
		bool systemDestroyed = true;
		bool systemEnemyControlled = true;
		
		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			if(systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised == false)
			{
				continue;
			}
			
			if(systemListConstructor.systemList [system].planetsInSystem [planet].underEnemyControl == false)
			{
				systemEnemyControlled = false;
			}
			
			if(systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised == true)
			{
				systemDestroyed = false;
			}
		}
		
		if(systemDestroyed == true)
		{
			DestroySystem(system);
			invasionGUI.openInvasionMenu = false;
		}
		
		else if(systemEnemyControlled == true)
		{
			OwnSystem(system);
			invasionGUI.openInvasionMenu = false;
		}

		if(systemDestroyed == true || systemEnemyControlled == true)
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
		
		systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.emptyMaterial;

		ambientStarRandomiser.AmbientColourChange(system);
		
		lineRenderScript = systemListConstructor.systemList [system].systemObject.GetComponent<LineRenderScript> ();
		
		lineRenderScript.SetRaceLineColour ("None");
		
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		systemDefence.underInvasion = false;
	}
	
	private void OwnSystem(int system)
	{
		systemListConstructor.systemList [system].systemOwnedBy = playerTurnScript.playerRace;
		systemListConstructor.systemList [system].systemObject.renderer.material = playerTurnScript.materialInUse;

		ambientStarRandomiser.AmbientColourChange(system);
		
		lineRenderScript = systemListConstructor.systemList [system].systemObject.GetComponent<LineRenderScript> ();
		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = false;
		
		lineRenderScript.SetRaceLineColour (playerTurnScript.playerRace);
		
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
		hero.currentArmour -= systemListConstructor.systemList [system].systemOffence / (hero.currentArmour * hero.classModifier);
		systemListConstructor.systemList [system].systemDefence -= hero.primaryWealth;

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
