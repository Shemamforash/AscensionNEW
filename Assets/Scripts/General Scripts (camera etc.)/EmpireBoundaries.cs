using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private LineRenderer lineRenderer;
	private List<Vector3> vertexPoints = new List<Vector3>();
	private float width = 1.5f, radius;
	private int numberOfPoints = 40, system;
	private Vector3 intersectionOne, intersectionTwo;
	private bool finalIteration = false;

	private void RoundLine(Vector3 systemPosition, float boundOne, float boundTwo)
	{
		for(int i = 0; i < 17; ++i)
		{
			if(i * 22.5f < boundOne || i * 22.5f > boundTwo)
			{
				continue;
			}

			float xPos = radius * Mathf.Cos (Mathf.Deg2Rad * (i * 22.5f));
			float yPos = radius * Mathf.Sin (Mathf.Deg2Rad * (i * 22.5f));
			Vector3 position = new Vector3(systemPosition.x + xPos, systemPosition.y + yPos, systemPosition.z);
			vertexPoints.Add(position);
		}
	}

	private void CalculateRadius(int j)
	{
		radius = 100f;

		for(int i = 0; i < systemListConstructor.systemList[j].permanentConnections.Count; ++i)
		{
			float distance = Vector3.Distance(systemListConstructor.systemList[j].systemObject.transform.position, systemListConstructor.systemList[j].permanentConnections[i].transform.position);
			
			if(radius > distance)
			{
				radius = distance;
			}
		}

		radius = radius / 2;
	}

	private void CircleLineIntersection(Vector3 startPoint, Vector3 endPoint) //Finally working DO NOT TOUCH
	{
		float gradient = (startPoint.y - endPoint.y) / (startPoint.x - endPoint.x); //Straight Line
		gradient = -(1/gradient);
		float yIntersect = startPoint.y - (gradient * startPoint.x);

		float A = (gradient * gradient) + 1; //M^2 + 1 This is fine
		float B = 2 * ((gradient * yIntersect) - (gradient * startPoint.y) - startPoint.x); //2(MC - MB - A) This appears good
		float C = (startPoint.x * startPoint.x) + (startPoint.y * startPoint.y) + (yIntersect * yIntersect) - (radius * radius) - (2 * startPoint.y * yIntersect); //A^2 + B^2 + C^2 - R^2 - 2BC Fine too

		float xIntersectOne = (-B + Mathf.Sqrt ((B * B) - (4 * A * C))) / (2 * A);
		float xIntersectTwo = (-B - Mathf.Sqrt ((B * B) - (4 * A * C))) / (2 * A);

		float yIntersectOne = (gradient * xIntersectOne) + yIntersect;
		float yIntersectTwo = (gradient * xIntersectTwo) + yIntersect;

		intersectionOne = new Vector3 (xIntersectOne, yIntersectOne, startPoint.z);
		intersectionTwo = new Vector3 (xIntersectTwo, yIntersectTwo, startPoint.z);
	}

	public void GetBound(int thisSystem, int parentSystem)
	{
		float boundOne = Mathf.Rad2Deg * Mathf.Acos (intersectionTwo.x / radius);
		float boundTwo = 0;

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
		{
			if(systemListConstructor.systemList[thisSystem].permanentConnections[i] == systemListConstructor.systemList[parentSystem].systemObject)
			{
				bool foundNextSystem = false;

				int sys = i;

				while(foundNextSystem == false)
				{
					++sys;

					if(sys >= systemListConstructor.systemList[thisSystem].permanentConnections.Count)
					{
						sys = 0;
					}

					int j = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[sys]);

					if(systemListConstructor.systemList[j].systemOwnedBy == systemListConstructor.systemList[thisSystem].systemOwnedBy)
					{
						foundNextSystem = true;
					}
				}

				CalculateRadius (thisSystem);
				CircleLineIntersection(systemListConstructor.systemList[thisSystem].systemObject.transform.position, systemListConstructor.systemList[thisSystem].permanentConnections[sys].transform.position);
				boundTwo = Mathf.Rad2Deg * Mathf.Acos (intersectionOne.x / radius);
				break;
			}
		}

		RoundLine (systemListConstructor.systemList [thisSystem].systemObject.transform.position, boundOne, boundTwo);
	}

	private void AddVertices(int i, int j)
	{
		CalculateRadius (i);
		
		CircleLineIntersection(systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position);

		if(vertexPoints.Contains(intersectionOne) == true)
		{
			vertexPoints.Add (intersectionTwo);
		}
		if(vertexPoints.Contains(intersectionOne) == false)
		{
			vertexPoints.Add (intersectionOne);
		}
	}

	public void SetVertexPoints(int selectedSystem)
	{
		bool neighboursFound = false, moveSystem = false;
		int target = -1;

		for(int i = 0; i < systemListConstructor.systemList[selectedSystem].permanentConnections.Count; ++i) //For each connection in permanent connections
		{
			target = RefreshCurrentSystem(systemListConstructor.systemList[selectedSystem].permanentConnections[i]); //Target is first owner permanent connections (clockwise)
			
			if(systemListConstructor.systemList[target].systemOwnedBy == systemListConstructor.systemList[system].systemOwnedBy)
			{
				neighboursFound = true;

				AddVertices(system, target);
				AddVertices(target, system);

				//GetBound(target, selectedSystem);

				moveSystem = true;
			}
			
			if(target == system || moveSystem == true)
			{
				break;	
			}
		}

		if(moveSystem == true && finalIteration == false)
		{
			if(target == system)
			{
				finalIteration = true;
			}

			SetVertexPoints(target);
		}

		if(neighboursFound == false)
		{
			CalculateRadius (system);
			RoundLine (systemListConstructor.systemList [system].systemObject.transform.position, -100.0f, 1000.0f);
		}
	}

	public void CreateBoundary(int selectedSystem)
	{
		lineRenderer = gameObject.GetComponent<LineRenderer> ();

		vertexPoints.Clear();

		system = selectedSystem;

		lineRenderer.SetWidth (width, width);

		SetVertexPoints (system);

		vertexPoints.Add (vertexPoints [0]);
		vertexPoints.Add (vertexPoints [1]);

		for(int i = 0; i < vertexPoints.Count; ++i)
		{
			Debug.Log (vertexPoints[i]);
		}

		lineRenderer.SetVertexCount (numberOfPoints * (vertexPoints.Count - 2));

		Vector3 point0, point1, point2;

		for(int i = 0; i < vertexPoints.Count; ++i)
		{
			if(i + 2 >= vertexPoints.Count)
			{
				break;
			}

			point0 = 0.5f * (vertexPoints[i] + vertexPoints[i + 1]);
			point1 = vertexPoints[i + 1];
			point2 = 0.5f * (vertexPoints[i + 1] + vertexPoints[i + 2]);

			float pointStep = 1f / numberOfPoints, delta;
			Vector3 tempPosition;

			if(i == vertexPoints.Count - 3)
			{
				pointStep = 1f / (numberOfPoints - 1.0f);
			}

			for(int j = 0; j < numberOfPoints; ++j)
			{
				delta = j * pointStep;
				tempPosition = (1f - delta) * (1f - delta) * point0 + 2f * (1f - delta) * delta * point1 + delta * delta * point2;
				lineRenderer.SetPosition(j + i * numberOfPoints, tempPosition);
			}
		}
	}
}
