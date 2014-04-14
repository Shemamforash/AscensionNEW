using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientStarRandomiser : MasterScript 
{
	public int totalStars;
	public float maxGenerateDistance, blueRand, redRand, greenRand;
	public GameObject ambientStar;
	public Texture starA, starB, starC, starD, starE, starF;
	private Texture tempTexture;
	private List<AmbientStar> ambientStarList = new List<AmbientStar> ();
	private Color randomColor;

	public void GenerateStars () 
	{
		int rnd = Random.Range (0, 2);

		greenRand = Random.Range (0.50f, 1.00f);

		if(rnd == 1)
		{
			redRand = 0f;
			blueRand = 1f;
		}

		if(rnd == 2)
		{
			redRand = 1f;
			blueRand = 0f;
		}

		float mult = 1f;

		if((1f + greenRand) / 3f < 0.75)
		{
			mult = 4f;
		}

		randomColor = new Color (redRand * mult, greenRand * mult, blueRand * mult);

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

				int rnd2 = Random.Range(0,3);
				float heightMult = 1f;

				if(rnd2 == 3)
				{
					heightMult = Random.Range(1.5f, 4.0f);
				}

				float zDis = Random.Range(-15f * heightMult, 15f * heightMult);

				float scale = Random.Range(0.15f, 0.3f);
		
				Vector3 location = new Vector3(xDis, yDis, zDis);

				GameObject star = Instantiate(ambientStar, location, Quaternion.identity) as GameObject;

				star.renderer.material.color = randomColor;

				star.transform.localScale = new Vector3(scale, scale, scale);

				tempObj.nearbyStars.Add(star);

				star.transform.parent = GameObject.Find ("Ambient Star Container").transform;
			}

			ambientStarList.Add(tempObj);
		}

		AmbientStarBatcher batcher = GameObject.Find ("Ambient Star Container").GetComponent<AmbientStarBatcher> ();
		batcher.BatchChildren ();
	}
}

public class AmbientStar
{
	public List<GameObject> nearbyStars = new List<GameObject>();
	public GameObject mainStar;
}
