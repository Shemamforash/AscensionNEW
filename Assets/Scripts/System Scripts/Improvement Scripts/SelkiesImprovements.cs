using UnityEngine;
using System.Collections;

public class SelkiesImprovements : MasterScript 
{
	private float amberPenalty, amberPenaltyModifier;

	public void CheckSelkiesImprovements(int system, ImprovementsBasic improvements, TurnInfo thisPlayer)
	{
		amberPenaltyModifier = 0.05f;

		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		if(improvements.listOfImprovements[29].hasBeenBuilt == true)
		{
			improvements.improvementCostModifier += (int)systemSIMData.totalSystemAmber;

			improvements.listOfImprovements[29].improvementMessage = ("System Improvements cost " + systemSIMData.totalSystemAmber + " fewer Industry from Amber production");
		}

		if(improvements.listOfImprovements[30].hasBeenBuilt == true)
		{
			int adjacentSystems = 0;

			for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
			{
				int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

				if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
				{
					++adjacentSystems;
				}
			}

			amberPenaltyModifier -= adjacentSystems * 0.05f;

			improvements.listOfImprovements[30].improvementMessage = ("-" + (adjacentSystems * 0.05f) + " Amber Penalty from adjacent Selkies Systems");
		}

		if(improvements.listOfImprovements[32].hasBeenBuilt == true)
		{
			improvements.amberPercentBonus += systemSIMData.totalSystemIndustry * 0.01f;

			improvements.listOfImprovements[32].improvementMessage = ("+" + (systemSIMData.totalSystemIndustry * 0.01f) + "% Amber production from Industry production");
		}

		if(improvements.listOfImprovements[34].hasBeenBuilt == true)
		{
			if(systemSIMData.totalSystemAmber > 10.0f)
			{
				improvements.amberPercentBonus += 0.5f;
			}
		}

		if(improvements.listOfImprovements[35].hasBeenBuilt == true)
		{
			improvements.maxOwnershipBonus += systemSIMData.totalSystemAmber;

			improvements.listOfImprovements[35].improvementMessage = ("+" + systemSIMData.totalSystemAmber + "% Ownership Cap from Amber production");
		}

		if(improvements.listOfImprovements[36].hasBeenBuilt == true)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Molten")
				{
					systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots = 4;
					systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Add (null);
					systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
				
				if(tempString == "Plains")
				{
					systemListConstructor.systemList[system].planetsInSystem[i].improvementSlots = 3;
					systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
			
			improvements.listOfImprovements[23].improvementMessage = ("+1/+2 Improvement Slot(s) on Plains/Molten Planets");
		}

		if(improvements.listOfImprovements[31].hasBeenBuilt == true)
		{
			improvements.amberPointBonus -= 4f;

			amberPenaltyModifier = amberPenaltyModifier / 2f;

			improvements.listOfImprovements[31].improvementMessage = "-4 Amber production, Amber penalty is halved";
		}

		if(improvements.listOfImprovements[33].hasBeenBuilt == true)
		{
			improvements.amberPointBonus -= 2f;
			
			amberPenaltyModifier = amberPenaltyModifier / 2f;
			
			improvements.listOfImprovements[33].improvementMessage = "-2 Amber production, Amber penalty is halved";
		}

		if(improvements.listOfImprovements[28].hasBeenBuilt == true)
		{
			if(amberPenaltyModifier < 0f)
			{
				amberPenaltyModifier = 0f;
			}

			systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

			amberPenalty = systemSIMData.totalSystemAmber * amberPenaltyModifier;

			improvements.ownershipModifier -= amberPenalty;
			improvements.sciencePercentBonus -= amberPenalty;
			improvements.industryPercentBonus -= amberPenalty;
			
			improvements.listOfImprovements[28].improvementMessage = ("System is suffering -" + amberPenalty * 100 + "% Resource production from Amber Penalty");
		}
	}
}
