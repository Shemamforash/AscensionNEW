using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadInvasionScreen : MasterScript 
{
	public bool CheckForExistingInvasion(int system)
	{
		for(int i = 0; i < systemInvasion.currentInvasions.Count; ++i)
		{
			if(systemInvasion.currentInvasions[i].system == systemListConstructor.systemList[system].systemObject)
			{
				return true;
			}
		}

		return false;
	}

	private void PositionTokens(List<GameObject> tokenList, int loc)
	{
		for(int i = 0; i < tokenList.Count; ++i)
		{

		}
	}

	public void ReloadInvasionScreen(int loc)
	{
		for(int i = 0; i < systemInvasion.currentInvasions[loc].tokenAllocation.Count; ++i)
		{
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].assaultTokenAllocation, i);
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].auxiliaryTokenAllocation, i);
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].defenceTokenAllocation, i);
		}
	}
}
