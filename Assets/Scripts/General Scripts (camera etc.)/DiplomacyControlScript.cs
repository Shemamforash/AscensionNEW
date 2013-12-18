using UnityEngine;
using System.Collections;

public class DiplomacyControlScript : MasterScript 
{
	public string[] playerStates = new string[3]; //player-enemy1, player-enemy2, enemy1-enemy2
	private int[] tempTurn = new int[3];

	private void Update()
	{
		for(int i = 0; i < 3; ++i)
		{
			CountdownToPeace(i);
		}
	}

	private void CountdownToPeace(int tempInt)
	{
		if(tempTurn[tempInt] == 0)
		{
			tempTurn[tempInt] = turnInfoScript.turn;
		}

		if(turnInfoScript.turn - 10 == tempTurn[tempInt])
		{
			playerStates[tempInt] = "Peace";
		}
	}
}
