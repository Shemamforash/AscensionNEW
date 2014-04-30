using UnityEngine;
using System.Collections;

public class CameraRotation : SystemRotate 
{	
	private CameraFunctions cameraFunctions;

	void Start()
	{
		cameraFunctions = gameObject.GetComponent<CameraFunctions>();
		galacticCentre = new Vector3(45f, 45f, 0f);
		speed = 0.01f;
	}
	
	void Update ()
	{
		Vector3 position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, 0f);
		radius = Vector3.Distance (position, galacticCentre);
		UpdateRotation();
	}
}
