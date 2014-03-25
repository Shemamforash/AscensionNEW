using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientStarRandomiser : MasterScript 
{
	public int totalStars;
	public float maxGenerateDistance;
	public GameObject ambientStar;
	public Texture starA, starB, starC, starD, starE, starF;
	private Texture tempTexture;
	private List<AmbientStar> ambientStarList = new List<AmbientStar> ();

	public void GenerateStars () 
	{
		int ambientStarsPerSystem = totalStars / systemListConstructor.systemList.Count;

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			AmbientStar tempObj = new AmbientStar();

			for(int j = 0; j < ambientStarsPerSystem; ++j)
			{
				float systemX = systemListConstructor.systemList[i].systemObject.transform.position.x;
				float systemY = systemListConstructor.systemList[i].systemObject.transform.position.y;

				float xDis = Random.Range(systemX - maxGenerateDistance, systemX + maxGenerateDistance);
				float yDis = Random.Range(systemY - maxGenerateDistance, systemY + maxGenerateDistance);
				float zDis = Random.Range(-7.5f, 7.5f);

				float scale = Random.Range(0.65f, 0.85f);
		
				Vector3 location = new Vector3(xDis, yDis, zDis);

				GameObject star = Instantiate(ambientStar, location, Quaternion.identity) as GameObject;

				star.renderer.material.mainTexture = PickTexture(Random.Range (0,99));

				star.transform.localScale = new Vector3(scale, scale, scale);

				tempObj.nearbyStars.Add(star);

				star.transform.parent = GameObject.Find ("Ambient Star Container").transform;
			}

			ambientStarList.Add(tempObj);
		}

		AmbientStarBatcher batcher = GameObject.Find ("Ambient Star Container").GetComponent<AmbientStarBatcher> ();
		batcher.BatchChildren ();
	}

	private Texture PickTexture(int chooseTexture)
	{
		if(chooseTexture < 40)
		{
			return starA;
		}
		else if(chooseTexture >= 40 && chooseTexture < 80)
		{
			return starB;
		}
		else if(chooseTexture >= 80)
		{
			return starF;
		}
		return null;
	}
	
	public void AmbientColourChange(int system)
	{
		for(int i = 0; i < ambientStarList.Count; ++i)
		{
			if(ambientStarList[i].mainStar == systemListConstructor.systemList[system].systemObject)
			{
				for(int j = 0; j < ambientStarList[i].nearbyStars.Count; ++j)
				{
					switch(systemListConstructor.systemList[system].systemOwnedBy)
					{
					case "Humans":
						ambientStarList[i].nearbyStars[j].renderer.material.mainTexture = starC;
						break;
					case "Selkies":
						ambientStarList[i].nearbyStars[j].renderer.material.mainTexture = starD;
						break;
					case "Nereides":
						ambientStarList[i].nearbyStars[j].renderer.material.mainTexture = starE;
						break;
					default:
						ambientStarList[i].nearbyStars[j].renderer.material.mainTexture = PickTexture(Random.Range(0,99));
						break;
					}
				}
			}
		}
	}
}

public class AmbientStar
{
	public List<GameObject> nearbyStars = new List<GameObject>();
	public GameObject mainStar;
}
