using UnityEngine;
using System.Collections;
using System;

public class SystemRotate : MonoBehaviour
{
	private Vector3 galacticCentre = new Vector3(45f, 45f, 0f);
	private float radius, xPos, yPos, bound = 400f;
	private bool skip;

	void Start()
	{
		radius = Vector3.Distance (gameObject.transform.position, galacticCentre);
	}

	void Update () //Need to work out gradient from angle
	{
		Vector3 direction = new Vector3(gameObject.transform.position.y - galacticCentre.y, gameObject.transform.position.x - galacticCentre.x);

		float angle = Vector3.Angle (Vector3.up, direction);

		if(gameObject.transform.position.y < galacticCentre.y)
		{
			angle = -angle; //Angle is okay
		}

		angle = (float)Math.Round ((double)(angle + 0.1f),  1);

		skip = false;

		if(angle == 90f)
		{
			xPos = 45f;
			yPos = 45f + radius;
			skip = true;
		}
		if(angle == -90f)
		{
			xPos = 45f;
			yPos = 45f - radius;
			skip = true;
		}

		float gradient = Mathf.Tan (angle * Mathf.Deg2Rad);

		if(skip == false)
		{
			float yIntersect = galacticCentre.y - (gradient * galacticCentre.x);

			float A = (gradient * gradient) + 1f;
			float B = 2 * ((gradient * yIntersect) - (gradient * 45f) - 45f);
			float C = (yIntersect * yIntersect) + (45f * 45f) + (45f * 45f) 
				- (2 * 45f * yIntersect) - (radius * radius);

			if(angle > 90f || angle < -90f)
			{
				xPos = (-B - Mathf.Sqrt ((B * B) - (4 * A * C))) / (2 * A);
			}
			else
			{
				xPos = (-B + Mathf.Sqrt ((B * B) - (4 * A * C))) / (2 * A);
			}

			yPos = (gradient * xPos) + yIntersect;
		}

		Vector3 newPos = new Vector3 (xPos, yPos, gameObject.transform.position.z);

		gameObject.transform.position = newPos;
	}
}
