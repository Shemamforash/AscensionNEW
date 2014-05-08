﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientStarRandomiser : MasterScript 
{
	public int totalStars, pointIterator = 0;
	public float maxGenerateDistance, blueRand, redRand, greenRand;
	public GameObject ambientStar;
	private Texture tempTexture;
	private List<AmbientStar> ambientStarList = new List<AmbientStar> ();
	public Material sharedMat;
	private float scale = 0.1f;
	private List<GameObject> rotatePoints = new List<GameObject>();
	private bool startup = false;
	public GameObject[] starPrefabs = new GameObject[2];

	private void LoadPoints()
	{
		rotatePoints.Add (GameObject.Find ("Galaxy Anchor One"));
		rotatePoints.Add (GameObject.Find ("Galaxy Anchor Two"));
		rotatePoints.Add (GameObject.Find ("Galaxy Anchor Three"));
		rotatePoints.Add (GameObject.Find ("Galaxy Anchor Four"));
		rotatePoints.Add (GameObject.Find ("Galaxy Anchor Five"));
	}

	private void Update()
	{
		if(startup == true)
		{
			for(int j = 0; j < rotatePoints.Count; ++j)
			{
				float speed = (j * 0.04f) + 0.12f;
				
				rotatePoints[j].transform.Rotate(0f, 0f, -Time.deltaTime * speed);
			}
		}
	}

	public void GenerateStars () 
	{
		if(startup == false)
		{
			LoadPoints();
			startup = true;
		}

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
		
				Vector3 location = new Vector3(xDis, yDis, zDis);

				int rnd = Random.Range (0,2);

				GameObject star = Instantiate(starPrefabs[rnd], location, Quaternion.identity) as GameObject;

				star.renderer.sharedMaterial = sharedMat;

				star.transform.parent = rotatePoints[pointIterator].transform;

				++pointIterator;

				if(pointIterator == rotatePoints.Count)
				{
					pointIterator = 0;
				}
			}

			ambientStarList.Add(tempObj);
		}
	}
}

public class AmbientStar
{
	public List<GameObject> nearbyStars = new List<GameObject>();
	public GameObject mainStar;
}
