using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SystemRotate : MasterScript
{
	public Vector3 galacticCentre = new Vector3(45f, 45f, 0f);
	public float radius, xPos, yPos, speed;
	private int system;

	public void Start()
	{
		system = RefreshCurrentSystem (gameObject);
		radius = Vector3.Distance (gameObject.transform.position, galacticCentre);
		speed = UnityEngine.Random.Range (0.0009f, 0.0011f);
	}

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		UpdateRotation ();
	}

	public void UpdateRotation()
	{
		Vector3 direction = new Vector3(gameObject.transform.position.y - galacticCentre.y, gameObject.transform.position.x - galacticCentre.x);

		double angle = -speed * Mathf.Deg2Rad;

		xPos = (float)(Math.Cos(angle) * (gameObject.transform.position.x - galacticCentre.x) - Math.Sin(angle) * (gameObject.transform.position.y - galacticCentre.y) + galacticCentre.x);
		yPos = (float)(Math.Sin(angle) * (gameObject.transform.position.x - galacticCentre.x) + Math.Cos(angle) * (gameObject.transform.position.y - galacticCentre.y) + galacticCentre.y);

		Vector3 newPos = new Vector3 (xPos, yPos, gameObject.transform.position.z);

		gameObject.transform.position = newPos;
	}
}
