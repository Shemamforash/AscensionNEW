using UnityEngine;
using System.Collections;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string[] playerStates = new string[3]; //player-enemy1, player-enemy2, enemy1-enemy2
	private float e = 2.718281859f, baseChangePercentage, warChangePercentage, peaceChangePercentage, diplomacyCoefficient = 0.00095650705f, tempChangePercentage, randomNumber;
	public string tempState;

	public void CheckForDiplomaticStateChange(TurnInfo thisPlayer)
	{
		RefreshNumbers (thisPlayer);

		baseChangePercentage = diplomacyCoefficient * (float)Math.Pow (thisPlayer.stalemateCounter, e);

		if(baseChangePercentage > 1.0f)
		{
			baseChangePercentage = 1.0f;
		}

		GenerateNumber ();

		if(randomNumber < thisPlayer.peaceCounter / 100.0f)
		{
			tempChangePercentage = baseChangePercentage * (thisPlayer.peaceCounter / 100.0f);
			tempState = "Peace";
		}

		if (randomNumber > (thisPlayer.peaceCounter / 100.0f) && randomNumber < ((thisPlayer.peaceCounter + thisPlayer.warCounter) / 100.0f)) 
		{
			tempChangePercentage = baseChangePercentage * (thisPlayer.warCounter / 100.0f);
			tempState = "War";
		}

		if(tempChangePercentage > 1.0f)
		{
			tempChangePercentage = 1.0f;
		}

		GenerateNumber();
			
		if(randomNumber < tempChangePercentage)
		{
			thisPlayer.diplomaticState = tempState;
			thisPlayer.stalemateCounter -= 2;
		}

		if (randomNumber > tempChangePercentage) 
		{
			thisPlayer.diplomaticState = "Stalemate";
			++thisPlayer.stalemateCounter;
		}
	}

	private void GenerateNumber()
	{
		randomNumber = UnityEngine.Random.Range (0.00f, 1.00f);
	}

	private void RefreshNumbers(TurnInfo thisPlayer)
	{
		tempChangePercentage = 0.0f;
		tempState = "";

		if(thisPlayer.diplomaticState == "War")
		{
			thisPlayer.warCounter += 10;
		}
		if(thisPlayer.diplomaticState == "Peace")
		{
			thisPlayer.peaceCounter += 10;
		}
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
		if(thisPlayer.stalemateCounter < 0)
		{
			thisPlayer.stalemateCounter = 0;
		}
	}
}
