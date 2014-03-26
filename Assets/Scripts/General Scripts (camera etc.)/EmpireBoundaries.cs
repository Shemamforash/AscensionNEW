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

	private void RoundLine(Vector3 systemPosition, float boundOne, float boundTwo)
	{
		for(int i = 0; i < 19; ++i)
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

	private void CalculateRadius()
	{
		radius = 100f;

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			float distance = Vector3.Distance(systemListConstructor.systemList[system].systemObject.transform.position, systemListConstructor.systemList[system].permanentConnections[i].transform.position);
			
			if(radius > distance)
			{
				radius = distance;
			}
		}

		radius = radius / 2;
	}

	private void CircleLineIntersection(Vector3 startPoint, Vector3 endPoint)
	{
		float gradient = (endPoint.y - startPoint.y) / (endPoint.x - startPoint.y); //Straight Line
		gradient = -(1/gradient);
		float yIntersect = startPoint.y - gradient * startPoint.x;

		float A = (gradient * gradient) + 1;
		float B = (gradient * yIntersect) - (gradient * startPoint.y) - startPoint.x;
		float C = (startPoint.y * startPoint.y) - (radius * radius) + (startPoint.x * startPoint.x) - (2 * yIntersect * startPoint.y) + (yIntersect * yIntersect);

		float xIntersectOne = (-B + Mathf.Sqrt ((B * B) - 4 * A * C)) / (2 * A);
		float xIntersectTwo = (-B - Mathf.Sqrt ((B * B) - 4 * A * C)) / (2 * A);
		float yIntersectOne = (gradient * xIntersectOne) + yIntersect;
		float yIntersectTwo = (gradient * xIntersectTwo) + yIntersect;

		intersectionOne = new Vector3 (xIntersectOne, yIntersectOne, startPoint.z);
		intersectionTwo = new Vector3 (xIntersectTwo, yIntersectTwo, startPoint.z);
	}

	public void SetVertexPoints(int selectedSystem)
	{
		bool neighboursFound = false, moveSystem = false;
		int target = -1;

		for(int i = 0; i < systemListConstructor.systemList[selectedSystem].permanentConnections.Count; ++i)
		{
			target = RefreshCurrentSystem(systemListConstructor.systemList[selectedSystem].permanentConnections[i]);
			
			if(systemListConstructor.systemList[target].systemOwnedBy == systemListConstructor.systemList[system].systemOwnedBy)
			{
				neighboursFound = true;
				
				CircleLineIntersection(systemListConstructor.systemList[selectedSystem].systemObject.transform.position, systemListConstructor.systemList[target].systemObject.transform.position);
				vertexPoints.Add (intersectionOne);
				CircleLineIntersection(systemListConstructor.systemList[target].systemObject.transform.position, systemListConstructor.systemList[selectedSystem].systemObject.transform.position);
				vertexPoints.Add (intersectionTwo);
				CalculateRadius ();
				
				float boundOne = Mathf.Rad2Deg * Mathf.Acos (intersectionTwo.x / radius);
				float boundTwo = Mathf.Rad2Deg * Mathf.Acos (intersectionOne.x / radius);
				
				RoundLine (systemListConstructor.systemList [target].systemObject.transform.position, boundOne, boundTwo);

				moveSystem = true;
			}
			
			if(target == system || moveSystem == true)
			{
				break;	
			}
		}

		if(moveSystem == true && target != system)
		{
			SetVertexPoints(target);
		}

		if(neighboursFound == false)
		{
			CalculateRadius ();
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
