﻿using UnityEngine;
using System.Collections;

public class CameraRotation : SystemRotate 
{	
	public AudioClip clickNoise;

	new void Start()
	{
		galacticCentre = new Vector3(45f, 45f, 0f);
		speed = 0.001f;
	}
	
	void Update ()
	{
		Vector3 position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, 0f);
		radius = Vector3.Distance (position, galacticCentre);
		UpdateRotation();
	}

	public void PlaySound()
	{
		AudioSource.PlayClipAtPoint(clickNoise, Camera.main.transform.position);
	}
}