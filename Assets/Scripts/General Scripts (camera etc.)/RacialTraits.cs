using UnityEngine;
using System.Collections;

public class RacialTraits : MasterScript 
{
	public int ambitionCounter, ambitionOwnershipModifier;
	private int nereidesProgressionCounter;
	private float nereidesIndustryModifier, nereidesScienceModifier;
	private string nereidesEmpireModifier;
	public UILabel racialLabel;

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
			if(resource == "Industry")
			{
				return nereidesIndustryModifier;
			}
		}

		return 0f;
	}

	public void NereidesTrait(TurnInfo player)
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == "Nereides")
			{
				systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();

				if(nereidesProgressionCounter < 2000 || (nereidesProgressionCounter >= 4000 && nereidesProgressionCounter < 6000))
				{
					nereidesProgressionCounter += (int)(0.2f * systemSIMData.totalSystemScience);
					player.science -= 0.2f * systemSIMData.totalSystemScience;
					nereidesScienceModifier = -0.2f;
					nereidesIndustryModifier = 0f;
				}

				if(nereidesProgressionCounter >= 2000 && nereidesProgressionCounter < 4000 || (nereidesProgressionCounter >= 4000 && nereidesProgressionCounter < 6000))
				{
					nereidesProgressionCounter += (int)(0.2f * systemSIMData.totalSystemIndustry);
					player.industry -= 0.2f * systemSIMData.totalSystemIndustry;
					nereidesScienceModifier = 0f;
					nereidesIndustryModifier = -0.2f;
				}

				if(nereidesProgressionCounter >= 6000)
				{
					player.industry += systemSIMData.totalSystemIndustry;
					player.science += systemSIMData.totalSystemScience;
					nereidesScienceModifier = 1f;
					nereidesIndustryModifier = 1f;
				}
			}
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
			return "-20% SIM from Stage 4";;
		}
		if(nereidesProgressionCounter >= 6000)
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
				ambitionCounter += player.systemsColonisedThisTurn * 4;
			}
			if(player.planetsColonisedThisTurn > 0)
			{
				ambitionCounter += (player.planetsColonisedThisTurn - player.systemsColonisedThisTurn) * 2;
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

	void Update()
	{
		if(playerTurnScript.playerRace == "Humans")
		{
			racialLabel.text = ("Ambition: " + ambitionCounter.ToString());
		}

		if(playerTurnScript.playerRace == "Nereides")
		{
			racialLabel.text = CheckNereidesRacialMessage();
		}

		if(playerTurnScript.playerRace == "Selkies")
		{
			racialLabel.text = "Not implemented";
		}
	}
}
