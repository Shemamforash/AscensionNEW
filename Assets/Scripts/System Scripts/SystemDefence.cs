using UnityEngine;
using System.Collections;

public class SystemDefence : MasterScript 
{
	public int defenceRegenerator, system, regenerateTimer;
	public bool underInvasion, canEnter;

	void Start () 
	{
		systemSIMData = gameObject.GetComponent<SystemSIMData> ();
		CalculateSystemDefence ();
		system = RefreshCurrentSystem (gameObject);
	}

	public void CalculateSystemDefence() 
	{
		int tempDefence = 0;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			tempDefence += systemListConstructor.systemList[system].planetsInSystem[i].planetOwnership;
		}

		tempDefence = (tempDefence / systemListConstructor.systemList [system].systemSize) * 10;

		systemListConstructor.systemList [system].systemDefence = tempDefence;

		defenceRegenerator = tempDefence / 5;

		if(underInvasion == false)
		{
			--regenerateTimer;

			if(regenerateTimer <= 0)
			{
				regenerateTimer = 0;
				systemListConstructor.systemList [system].systemDefence += defenceRegenerator;
			}
		}
	}
}
