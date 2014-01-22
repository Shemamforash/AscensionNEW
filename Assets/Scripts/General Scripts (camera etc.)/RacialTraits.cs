using UnityEngine;
using System.Collections;

public class RacialTraits : MasterScript 
{
	public int ambitionCounter, ambitionOwnershipModifier;
	private int nereidesProgressionCounter;
	private float nereidesIndustryModifier, nereidesScienceModifier, nereidesMoneyModifier;
	private string nereidesEmpireModifier;

	void Start()
	{
		ambitionCounter = 0;
	}

	public float IncomeModifier(TurnInfo player, string resource)
	{
		if(player.playerRace == "Nereides")
		{
			if(resource == "Science")
			{
				return nereidesScienceModifier;
			}
			if(resource == "Science")
			{
				return nereidesIndustryModifier;
			}
			if(resource == "Science")
			{
				return nereidesMoneyModifier;
			}
		}

		return 1f;
	}

	public void NereidesTrait(TurnInfo player)
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == "Nereides")
			{
				guiPlanScript = systemListConstructor.systemList[i].systemObject.GetComponent<GUISystemDataScript>();

				if(nereidesProgressionCounter < 2000 || (nereidesProgressionCounter >= 6000 && nereidesProgressionCounter < 8000))
				{
					nereidesProgressionCounter += (int)(0.2f * guiPlanScript.totalSystemScience);
					player.science -= 0.2f * guiPlanScript.totalSystemScience;
					nereidesScienceModifier = 0.8f;
					nereidesIndustryModifier = 1f;
					nereidesMoneyModifier = 1f;
				}

				if(nereidesProgressionCounter >= 2000 && nereidesProgressionCounter < 4000 || (nereidesProgressionCounter >= 6000 && nereidesProgressionCounter < 8000))
				{
					nereidesProgressionCounter += (int)(0.2f * guiPlanScript.totalSystemIndustry);
					player.industry -= 0.2f * guiPlanScript.totalSystemIndustry;
					nereidesScienceModifier = 1f;
					nereidesIndustryModifier = 0.8f;
					nereidesMoneyModifier = 1f;
				}

				if(nereidesProgressionCounter >= 4000 && nereidesProgressionCounter < 6000 || (nereidesProgressionCounter >= 6000 && nereidesProgressionCounter < 8000))
				{
					nereidesProgressionCounter += (int)(0.2f * guiPlanScript.totalSystemMoney);
					player.money -= 0.2f * guiPlanScript.totalSystemMoney;
					nereidesScienceModifier = 1f;
					nereidesIndustryModifier = 1f;
					nereidesMoneyModifier = 0.8f;
				}

				if(nereidesProgressionCounter >= 8000)
				{
					player.money += guiPlanScript.totalSystemMoney;
					player.industry += guiPlanScript.totalSystemIndustry;
					player.science += guiPlanScript.totalSystemScience;
					nereidesScienceModifier = 2f;
					nereidesIndustryModifier = 2f;
					nereidesMoneyModifier = 2f;
				}
			}
		}

		if(nereidesProgressionCounter >= 8000)
		{
			player.GP += 2;
		}
	}

	public string CheckNereidesRacialMessage()
	{
		if(nereidesProgressionCounter < 2000)
		{
			return "-20% Science from Stage 1";
		}
		if(nereidesProgressionCounter >= 2000 && nereidesProgressionCounter < 4000)
		{
			return "-20% Industry from Stage 2";
		}
		if(nereidesProgressionCounter >= 4000 && nereidesProgressionCounter < 6000)
		{
			return "-20% Money from Stage 3";
		}
		if(nereidesProgressionCounter >= 6000 && nereidesProgressionCounter < 8000)
		{
			return "-20% SIM from Stage 4";
		}
		if(nereidesProgressionCounter >= 8000)
		{
			return "+100% SIM from Stage 5";
		}

		return "";
	}

	public int HumanTrait()
	{
		if(ambitionCounter < -100)
		{
			ambitionCounter = -100;
		}
		if(ambitionCounter > 100)
		{
			ambitionCounter = 100;
		}

		ambitionOwnershipModifier = ambitionCounter / 25;

		return ambitionOwnershipModifier;
	}

	public void RacialBonus(TurnInfo player)
	{
		if(player.playerRace == "Humans")
		{
			if(player.systemsColonisedThisTurn > 0)
			{
				ambitionCounter += player.systemsColonisedThisTurn * 2;
			}
			if(player.planetsColonisedThisTurn > 0)
			{
				ambitionCounter += (player.planetsColonisedThisTurn - player.systemsColonisedThisTurn);
			}
			if(player.systemsColonisedThisTurn == 0 && player.planetsColonisedThisTurn == 0)
			{
				ambitionCounter -= 2;
			}
		}

		if(player.playerRace == "Nereides")
		{
			NereidesTrait(player);
		}
	}

	void OnGUI()
	{
		GUI.skin = mainGUIScript.mySkin;

		string labelText = CheckNereidesRacialMessage();

		Rect racialMessage = new Rect (10.0f, Screen.height - 190.0f, 160.0f, 60.0f);

		if(playerTurnScript.playerRace == "Humans")
		{
			GUI.Label (racialMessage, ambitionCounter.ToString());
		}

		if(playerTurnScript.playerRace == "Nereides")
		{
			GUI.Label(racialMessage, labelText + "\n" + nereidesProgressionCounter);
		}
	}
}
