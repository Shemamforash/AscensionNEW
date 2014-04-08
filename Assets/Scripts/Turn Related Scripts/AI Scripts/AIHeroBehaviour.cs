using UnityEngine;
using System.Collections;

public class AIHeroBehaviour : MasterScript 
{
	private TurnInfo player;

	public void HeroDecisionStart(TurnInfo thisPlayer)
	{
		player = thisPlayer;

		CheckForLevelUp ();

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == player.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == player.playerRace)
			{
				if(diplomacyScript.relationsList[i].diplomaticState == "War")
				{
					float protectSystemValue = 0f;
					int systemToProtect = -1;

					for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
					{
						if(systemListConstructor.systemList[j].systemOwnedBy == player.playerRace)
						{
							systemDefence = systemListConstructor.systemList[j].systemObject.GetComponent<SystemDefence>();

							if(systemDefence.underInvasion == true)
							{
								systemSIMData = systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();

								if(systemSIMData.totalSystemSIM > protectSystemValue)
								{
									protectSystemValue = systemSIMData.totalSystemSIM;
									systemToProtect = j;
								}
							}
						}
					}

					if(systemToProtect != -1)
					{
						ProtectSystem(systemToProtect);
					}
				}
			}
		}
	}

	private void CheckForLevelUp()
	{
		float dipMod = 0;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == player.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == player.playerRace)
			{
				switch(diplomacyScript.relationsList[i].diplomaticState)
				{
				case "War":
					dipMod += 3;
					break;
				case "Cold War":
					dipMod += 2;
					break;
				case "Peace":
					dipMod += 1;
					break;
				}
			}
		}

		dipMod = dipMod / diplomacyScript.relationsList.Count;

		for(int i = 0; i < player.playerOwnedHeroes.Count; ++i)
		{
			heroScript = player.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

			if(heroScript.reachedLevel2 == true && heroScript.heroTier2 == null)
			{
				int randomNo = Random.Range (0, 100);

				if(dipMod <= 1.0f)
				{
					if(randomNo < 25)
					{
						heroScript.heroTier2 = "Soldier";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						heroScript.heroTier2 = "Infiltrator";
					}
					if(randomNo > 50)
					{
						heroScript.heroTier2 = "Diplomat";
					}
				}

				if(dipMod > 1.0f && dipMod <= 2.0f)
				{
					if(randomNo < 25)
					{
						heroScript.heroTier2 = "Soldier";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						heroScript.heroTier2 = "Diplomat";
					}
					if(randomNo > 50)
					{
						heroScript.heroTier2 = "Infiltrator";
					}
				}

				if(dipMod > 2.0f)
				{
					if(randomNo < 25)
					{
						heroScript.heroTier2 = "Diplomat";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						heroScript.heroTier2 = "Infiltrator";
					}
					if(randomNo > 50)
					{
						heroScript.heroTier2 = "Soldier";
					}
				}
			}

			if(heroScript.reachedLevel3 == true && heroScript.heroTier3 == null)
			{
				int randomNo = Random.Range (0, 1);
				string choiceOne = null, choiceTwo = null;

				switch(heroScript.heroTier2)
				{
				case "Diplomat":
					choiceOne = "Merchant";
					choiceTwo = "Ambassador";
					break;
				case "Soldier":
					choiceOne = "Warlord";
					choiceTwo = "Vanguard";
					break;
				case "Infiltrator":
					choiceOne = "Drone";
					choiceTwo = "Hacker";
					break;
				default:
					break;
				}

				if(randomNo == 0)
				{
					heroScript.heroTier3 = choiceOne;
				}
				else
				{
					heroScript.heroTier3 = choiceTwo;
				}
			}
		}
	}

	private void ProtectSystem(int targetSystem)
	{
		float offence = 0f;
		int hero = -1;
		
		for(int k = 0; k < player.playerOwnedHeroes.Count; ++k)
		{
			heroScript = player.playerOwnedHeroes[k].GetComponent<HeroScriptParent>();
			
			if(offence < heroScript.primaryPower)
			{
				offence = heroScript.primaryPower;
				hero = k;
			}
		}
		
		heroMovement = player.playerOwnedHeroes[hero].GetComponent<HeroMovement>();
		
		heroMovement.pathfindTarget = systemListConstructor.systemList[targetSystem].systemObject;
		
		heroMovement.FindPath();
	}
}
