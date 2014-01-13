using UnityEngine;
using System.Collections;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string[] playerStates = new string[3]; //player-enemy1, player-enemy2, enemy1-enemy2
	public string tempState;

	public void CheckForWarDeclarationAndPeaceExpiration(TurnInfo thisPlayer)
	{
		if(thisPlayer.turnsAtColdWar > 10)
		{
			thisPlayer.canDeclareWar = true;
		}

		if(thisPlayer.turnsAtPeace > 10)
		{
			thisPlayer.ceaseFirePeriodExpired = true;
		}
	}

	public void CheckForDiplomaticStateChange(TurnInfo thisPlayer)
	{
		RefreshNumbers (thisPlayer);

		InvokeDiplomaticStateBonuses (thisPlayer);

		CheckForWarDeclarationAndPeaceExpiration (thisPlayer);

		if(thisPlayer.peaceCounter > 66 || thisPlayer.ceaseFirePeriodExpired == false)
		{
			thisPlayer.diplomaticState = "Peace";

			if(thisPlayer.peaceCounter < 66)
			{
				thisPlayer.peaceCounter = 66;
			}
		}

		if(thisPlayer.warCounter > 66)
		{
			thisPlayer.diplomaticState = "War";
		}

		if(thisPlayer.peaceCounter < 66 &&  thisPlayer.warCounter < 66 && thisPlayer.ceaseFirePeriodExpired == true)
		{
			thisPlayer.diplomaticState = "Cold War";
		}
	}

	public void InvokeDiplomaticStateBonuses(TurnInfo thisPlayer)
	{
		thisPlayer.peaceBonus = thisPlayer.peaceCounter / 1000.0f;
		
		thisPlayer.warBonus = thisPlayer.warCounter / 100.0f;

		if (thisPlayer.diplomaticState == "War")
		{
			//prevent adjacency bonuses, merchants etc.
		}

		if (thisPlayer.diplomaticState == "Peace")
		{
			//prevent invasions, start timer for peace
		}

		if(thisPlayer.diplomaticState == "Cold War")
		{
			//increase possibility of stealth detection
		}
	}

	private void RefreshNumbers(TurnInfo thisPlayer)
	{
		if (thisPlayer.peaceCounter > 100) 
		{
			thisPlayer.peaceCounter = 100;
		}
		if (thisPlayer.warCounter > 100) 
		{
			thisPlayer.warCounter = 100;
		}
		if (thisPlayer.peaceCounter < 0) 
		{
			thisPlayer.peaceCounter = 0;
		}
		if (thisPlayer.warCounter < 0) 
		{
			thisPlayer.warCounter = 0;
		}
		if(thisPlayer.turnsAtColdWar < 0)
		{
			thisPlayer.turnsAtColdWar = 0;
		}
	}
}
