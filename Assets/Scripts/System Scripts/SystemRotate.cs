using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SystemRotate : MasterScript
{
	public Vector3 galacticCentre = new Vector3(50f, 50f, 0f);
	public float radius, xPos, yPos, speed, rndSpd1, rndSpd2, rndSpd3;
	public GameObject corona1, corona2, corona3;

	public void Start()
	{
		radius = Vector3.Distance (gameObject.transform.position, galacticCentre);
		speed = UnityEngine.Random.Range (0.0009f, 0.0011f);

		if(gameObject.tag == "StarSystem")
		{
			corona1 = gameObject.transform.Find ("Point01").transform.Find ("corona1").gameObject;
			rndSpd1 = UnityEngine.Random.Range (8f, 12f);
			corona2 = gameObject.transform.Find ("Point01").transform.Find ("corona2").gameObject;
			rndSpd2 = UnityEngine.Random.Range (8f, 12f);
			corona3 = gameObject.transform.Find ("Point01").transform.Find ("corona03").gameObject;
			rndSpd3 = UnityEngine.Random.Range (8f, 12f);
		}
	}

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		if(corona1 != null)
		{
			corona1.transform.Rotate (Vector3.forward, Time.deltaTime * rndSpd1);
			corona2.transform.Rotate (Vector3.forward, Time.deltaTime * -rndSpd2);
			corona2.transform.Rotate (Vector3.forward, Time.deltaTime * rndSpd3);
			gameObject.transform.Rotate (Vector3.forward, Time.deltaTime * 5f);
		}

		UpdateRotation ();
	}

	public void UpdateRotation()
	{
		double angle = -speed * Mathf.Deg2Rad;

		xPos = (float)(Math.Cos(angle) * (gameObject.transform.position.x - galacticCentre.x) - Math.Sin(angle) * (gameObject.transform.position.y - galacticCentre.y) + galacticCentre.x);
		yPos = (float)(Math.Sin(angle) * (gameObject.transform.position.x - galacticCentre.x) + Math.Cos(angle) * (gameObject.transform.position.y - galacticCentre.y) + galacticCentre.y);

		Vector3 newPos = new Vector3 (xPos, yPos, gameObject.transform.position.z);

		gameObject.transform.position = newPos;
	}
}
