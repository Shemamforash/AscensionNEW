using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadraticBezierCurve : MasterScript
{
	private List<Vector2> firstControlPoints = new List<Vector2>();
	private List<Vector2> secondControlPoints = new List<Vector2>();
	private List<Vector2> knots = new List<Vector2>();
	public List<Vector3> pathToFollow = new List<Vector3>();
	private float numberOfPoints = 50f;
	public bool moving = false;
	private int currentVertex = 0;
	public int target;

	public void Update()
	{
		if(moving == true)
		{
			for(int j = currentVertex; j < pathToFollow.Count; ++j)
			{
				bool reachedPoint = false;
			
				if(gameObject.transform.position.x < pathToFollow[j].x + 0.001f)
				{
					if(gameObject.transform.position.x > pathToFollow[j].x - 0.001f)
					{
						if(gameObject.transform.position.y < pathToFollow[j].y + 0.001f)
						{
							if(gameObject.transform.position.y > pathToFollow[j].y - 0.001f)
							{
								reachedPoint = true; //Check to see if it has reached the next point on its path
							}
						}
					}
				}
				
				if(j + 1 == pathToFollow.Count && reachedPoint == true) //If it has reached final point
				{
					systemDefence = systemListConstructor.systemList[target].systemObject.GetComponent<SystemDefence>(); //Get reference to target system
					systemDefence.TakeDamage(500f, 0f, -1); //Force system to take damage
					GameObject.Destroy (gameObject); //Destroy gameobject
				}
				else if(j + 1 != pathToFollow.Count && reachedPoint == true) //If it has not reached final point but has reached a point
				{
					++currentVertex; //Increase the current point
					break;
				}
				else if (reachedPoint == false) //If it has not reached a point
				{
					gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pathToFollow[currentVertex], 0.5f * Time.deltaTime); //Move towards next point
				}
			}
		}
	}
	
	public void FollowBezierCurve(List<Vector2> temp)
	{
		knots = temp;

		GetCurveControlPoints ();

		for(int i = 1; i < knots.Count - 1; ++i)
		{
			//Vector3 finalPos = new Vector3(knots[i].x, knots[i].y, 0f);

			float dist = Vector2.Distance(knots[i], knots[i - 1]);
			numberOfPoints = dist / 0.1f;

			if(firstControlPoints.Count > 1)
			{
				for(int j = 0; j < numberOfPoints; ++j)
				{
					/*
					float t = j / (firstControlPoints.Count - 1);
					Vector2 argOne = (1 - t) * (1 - t) * (1 - t) * knots[i];
					Vector2 argTwo = 3 * ((1 - t) * (1 - t)) * t * firstControlPoints[i];
					Vector2 argThree = 3 * (1 - t) * t * t * secondControlPoints[i];
					Vector2 argFour = t * t * t * knots[i + 1];

					Vector2 tempPos = argOne + argTwo + argThree + argFour;
					finalPos = new Vector3(tempPos.x, tempPos.y, 0f);
					pathToFollow.Add (finalPos);
					*/

					float t = i / (numberOfPoints - 1f);
					Vector2 position = (1 - t) * (1 - t) * knots[i - 1] + 2 * (1 - t) * t * knots[i] + t * t * knots[i + 1];
					pathToFollow.Add (new Vector3(position.x, position.y, 0f));
				}
			}

			//pathToFollow.Add (finalPos);
		}

		pathToFollow.Add (new Vector3(knots[knots.Count - 1].x, knots[knots.Count - 1].y, 0f));
	}

	public void GetCurveControlPoints()
	{
		int n = knots.Count - 1;
	
		if (n == 1)
		{
			Vector2 tempVect = new Vector2();
			tempVect.y = (2 * knots[0].x + knots[1].x) / 3;
			tempVect.x = (2 * knots[0].y + knots[1].y) / 3;
			firstControlPoints.Add (tempVect);

			tempVect.x = 2 * firstControlPoints[0].x - knots[0].x;
			tempVect.y = 2 * firstControlPoints[0].y - knots[0].y;
			secondControlPoints.Add (tempVect);
			return;
		}
		
		// Calculate first Bezier control points
		// Right hand side vector
		float[] rhs = new float[n];
		
		// Set right hand side X values
		for (int i = 1; i < n - 1; ++i)
		{
			rhs[i] = 4 * knots[i].x + 2 * knots[i + 1].x;
			rhs[0] = knots[0].x + 2 * knots[1].x;
			rhs[n - 1] = (8 * knots[n - 1].x + knots[n].x) / 2.0f;
		}
		// Get first control points X-values
		float[] x = GetFirstControlPoints(rhs);

		for (int i = 1; i < n - 1; ++i)
		{
			rhs[i] = 4 * knots[i].y + 2 * knots[i + 1].y;
			rhs[0] = knots[0].y + 2 * knots[1].y;
			rhs[n - 1] = (8f * knots[n - 1].y + knots[n].y) / 2.0f;
		}
		
		float[] y = GetFirstControlPoints(rhs);

		for (int i = 0; i < n; ++i)
		{
			firstControlPoints.Add(new Vector2(x[i], y[i]));
			
			if (i < n - 1)
			{
				secondControlPoints.Add (new Vector2(2 * knots[i + 1].x - x[i + 1], 2 * knots[i + 1].y - y[i + 1]));
			}
			else
			{
				secondControlPoints.Add (new Vector2((knots[n].x + x[n - 1]) / 2, (knots[n].y + y[n - 1]) / 2));
			}
		}
	}

	private static float[] GetFirstControlPoints(float[] rhs)
	{
		int n = rhs.Length;
		float[] x = new float[n]; 
		float[] tmp = new float[n]; 
		
		float b = 2.0f;

		x[0] = rhs[0] / b;

		for (int i = 1; i < n; i++) 
		{
			tmp[i] = 1 / b;

			if(i < n - 1)
			{
				b = 4.0f - tmp[i];
			}
			else
			{
				b = 3.5f - tmp[i];
			}
		}

		for (int i = 1; i < n; i++)
		{
			x[n - i - 1] -= tmp[n - i] * x[n - i]; 
		}

		return x;
	}
}

