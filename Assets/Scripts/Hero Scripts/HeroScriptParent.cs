using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MasterScript 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, linkedHeroObject = null, merchantLine, invasionObject;
	public DiplomaticPosition tempObject;
	public int currentLevel = 1, thisHeroNumber;
	public float heroSciBonus = 0, heroIndBonus = 0, heroMonBonus = 0, offensivePower = 14.0f, defensivePower = 14.0f, heroDiplomacyChange;
	public string heroTier2, heroTier3, heroOwnedBy;
	private Vector3 position;
	public bool isInvading = false;

	void Start()
	{
		offensivePower = 14.0f;

		techTreeScript = gameObject.GetComponent<TechTreeScript>();
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		int i = RefreshCurrentSystem (heroLocation);

		if (systemListConstructor.systemList [i].systemOwnedBy == playerTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<PlayerTurn>();
		}
		if (systemListConstructor.systemList [i].systemOwnedBy == enemyOneTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyOne>();
		}
		if (systemListConstructor.systemList [i].systemOwnedBy == enemyTwoTurnScript.playerRace) 
		{
			turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<EnemyTwo>();
		}
	}

	public void Update()
	{
		if(linkedHeroObject != null && heroGUIScript.heroIsMoving == true)
		{
			Destroy(merchantLine);

			CreateConnectionLine();
		}
	}

	public DiplomaticPosition FindDiplomaticConnection()
	{
		int i = RefreshCurrentSystem (heroLocation);

		if(heroOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyTwoRelations;
			}
		}

		if(heroOwnedBy == enemyOneTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyOneRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyTwoTurnScript.playerRace)
			{
				return diplomacyScript.enemyOneEnemyTwoRelations;
			}
		}

		if(heroOwnedBy == enemyTwoTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
			{
				return diplomacyScript.playerEnemyTwoRelations;
			}
			if(systemListConstructor.systemList[i].systemOwnedBy == enemyOneTurnScript.playerRace)
			{
				return diplomacyScript.enemyOneEnemyTwoRelations;
			}
		}

		return null;
	}

	public void CreateConnectionLine()
	{
		float distance = Vector3.Distance(gameObject.transform.position, linkedHeroObject.transform.position);
		
		float rotationZRad = Mathf.Acos ((linkedHeroObject.transform.position.y - gameObject.transform.position.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(gameObject.transform.position.x < linkedHeroObject.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		Vector3 midPoint = (gameObject.transform.position + linkedHeroObject.transform.position)/2;
		
		Vector3 scale = new Vector3(0.2f, distance, 0.0f);
		
		Quaternion directQuat = new Quaternion();
		
		directQuat.eulerAngles = rotation;
		
		merchantLine = (GameObject)Instantiate (heroGUIScript.merchantQuad, midPoint, directQuat);
		
		merchantLine.transform.localScale = scale;
	}

	public Vector3 HeroPositionAroundStar()
	{
		if(thisHeroNumber == 0)
		{
			position.x = heroLocation.transform.position.x;
			position.y = heroLocation.transform.position.y + 1.5f;
		}

		if(thisHeroNumber == 1)
		{
			position.x = heroLocation.transform.position.x + 0.75f;
			position.y = heroLocation.transform.position.y - 0.5f;
		}

		if(thisHeroNumber == 2)
		{
			position.x = heroLocation.transform.position.x - 0.75f;
			position.y = heroLocation.transform.position.y - 0.5f;
		}

		position.z = heroLocation.transform.position.z;

		return position;
	}

	public void HeroEndTurnFunctions()
	{
		heroSciBonus = 10.0f; 
		heroIndBonus = 10.0f;
		heroMonBonus = 10.0f;

		heroDiplomacyChange = 0.0f;

		offensivePower = 14.0f;
		defensivePower = 7.0f;

		if(heroTier2 != "")
		{
			tier2HeroScript.CheckTier2Heroes (gameObject);

			if(heroTier3 != "")
			{
				heroGUIScript.selectedHero = thisHeroNumber;

				tier3HeroScript.CheckTier3Heroes (heroScript);
			}
		}

		if(isInvading == true)
		{
			diplomacyScript.ContinueInvasion(heroScript);
		}
	}

	public void LevelUp()
	{
		heroGUIScript.selectedHero = thisHeroNumber;
		heroGUIScript.openHeroLevellingScreen = true;
	}
}


