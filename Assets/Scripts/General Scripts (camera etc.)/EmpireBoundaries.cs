using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private LineRenderer lineRenderer;
	private List<Vector3> vertexPoints = new List<Vector3>();
	private float width = 1.5f, radius;
	private int numberOfPoints = 40, system;
	private Vector3 intersectionLeftStart, intersectionLeftEnd;
	private List<Vector3> vectorList = new List<Vector3>();

	public void CalculateRadius()
	{
		float tempRadius = 100f;

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList[i].permanentConnections.Count; ++j)
			{
				float tempDistance = Vector3.Distance(systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[i].permanentConnections[j].transform.position);

				if(tempDistance < tempRadius)
				{
					tempRadius = tempDistance;
				}
			}
		}

		radius = tempRadius / 2;
	}

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

	private void CircleLineIntersection(Vector3 startPoint, Vector3 endPoint) //Finally working DO NOT TOUCH
	{
		float posGradient = (startPoint.y - endPoint.y) / (startPoint.x - endPoint.x); //Straight Line
		float yIntersect0 = startPoint.y - (posGradient * startPoint.x);
		float negGradient = -(1/posGradient);
		float yIntersect1 = startPoint.y - (negGradient * startPoint.x);
		float yIntersect2 = endPoint.y - (negGradient * endPoint.x);

		float A1 = (negGradient * negGradient) + 1; //M^2 + 1 This is fine
		float B1 = 2 * ((negGradient * yIntersect1) - (negGradient * startPoint.y) - startPoint.x); //2(MC - MB - A) This appears good
		float C1 = (startPoint.x * startPoint.x) + (startPoint.y * startPoint.y) + (yIntersect1 * yIntersect1) - (radius * radius) - (2 * startPoint.y * yIntersect1); //A^2 + B^2 + C^2 - R^2 - 2BC Fine too

		float A2 = (negGradient * negGradient) + 1; //M^2 + 1 This is fine
		float B2 = 2 * ((negGradient * yIntersect2) - (negGradient * endPoint.y) - endPoint.x); //2(MC - MB - A) This appears good
		float C2 = (endPoint.x * endPoint.x) + (endPoint.y * endPoint.y) + (yIntersect2 * yIntersect2) - (radius * radius) - (2 * endPoint.y * yIntersect2); //A^2 + B^2 + C^2 - R^2 - 2BC Fine too

		float xIntersectOneA = (-B1 + Mathf.Sqrt ((B1 * B1) - (4 * A1 * C1))) / (2 * A1);
		float xIntersectTwoA = (-B1 - Mathf.Sqrt ((B1 * B1) - (4 * A1 * C1))) / (2 * A1);

		float yIntersectOneA = (negGradient * xIntersectOneA) + yIntersect1;
		float yIntersectTwoA = (negGradient * xIntersectTwoA) + yIntersect1;

		float xIntersectOneB = (-B2 + Mathf.Sqrt ((B2 * B2) - (4 * A2 * C2))) / (2 * A2);
		float xIntersectTwoB = (-B2 - Mathf.Sqrt ((B2 * B2) - (4 * A2 * C2))) / (2 * A2);
		
		float yIntersectOneB = (negGradient * xIntersectOneB) + yIntersect2;
		float yIntersectTwoB = (negGradient * xIntersectTwoB) + yIntersect2;

		bool inverse = false;

		if(startPoint.y > endPoint.y)
		{
			inverse = true;
		}

		if(startPoint.y == endPoint.y)
		{
			if(yIntersectOneA > (posGradient * xIntersectOneA - yIntersect0))
			{
				intersectionLeftStart = new Vector3(xIntersectOneA, yIntersectOneA, startPoint.z);
			}
			else
			{
				intersectionLeftStart = new Vector3(xIntersectTwoA, yIntersectTwoA, startPoint.z);
			}
			if(yIntersectOneB > (posGradient * xIntersectOneB - yIntersect0))
			{
				intersectionLeftEnd = new Vector3 (xIntersectOneB, yIntersectOneB, startPoint.z);
			}
			else
			{
				intersectionLeftEnd = new Vector3 (xIntersectTwoB, yIntersectTwoB, startPoint.z);
			}
		}

		if(startPoint.y != endPoint.y)
		{
			if(xIntersectOneA < (yIntersectOneA - yIntersect0 / posGradient))
			{
				if(inverse == true)
				{
					intersectionLeftStart = new Vector3(xIntersectTwoA, yIntersectTwoA, startPoint.z);
				}
				else
				{
					intersectionLeftStart = new Vector3 (xIntersectOneA, yIntersectOneA, startPoint.z);
				}
			}

			if(xIntersectOneA > (yIntersectOneA - yIntersect0 / posGradient))
			{
				if(inverse == true)
				{
					intersectionLeftStart = new Vector3 (xIntersectOneA, yIntersectOneA, startPoint.z);
				}
				else
				{
					intersectionLeftStart = new Vector3(xIntersectTwoA, yIntersectTwoA, startPoint.z);
				}
			}

			if(xIntersectOneB < (yIntersectOneB - yIntersect0 / posGradient))
			{
				if(inverse == true)
				{
					intersectionLeftEnd = new Vector3 (xIntersectTwoB, yIntersectTwoB, startPoint.z);
				}
				else
				{
					intersectionLeftEnd = new Vector3 (xIntersectOneB, yIntersectOneB, startPoint.z);
				}
			}

			if(xIntersectOneB > (yIntersectOneB - yIntersect0 / posGradient))
			{
				if(inverse == true)
				{
					intersectionLeftEnd = new Vector3 (xIntersectOneB, yIntersectOneB, startPoint.z);
				}
				else
				{
					intersectionLeftEnd = new Vector3 (xIntersectTwoB, yIntersectTwoB, startPoint.z);
				}
			}
		}
	}

	public void GetBound(int thisSystem, int parentSystem) //Gets bounds between systems
	{
		float boundOne = Mathf.Rad2Deg * Mathf.Acos (intersectionLeftEnd.x / radius); //Bound one is connection point between system and parent
		float boundTwo = 0; //Reset boundtwo

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i) //For all permanent connections
		{
			if(systemListConstructor.systemList[thisSystem].permanentConnections[i] == systemListConstructor.systemList[parentSystem].systemObject) //If connection == parent system
			{
				bool foundNextSystem = false; //Default variables

				int sys = i;

				while(foundNextSystem == false) //While next system == false
				{
					++sys; //Increase the iterator of permanentconnections

					if(sys >= systemListConstructor.systemList[thisSystem].permanentConnections.Count) //If iterator is greater than count, reset to 0
					{
						sys = 0;
					}

					int j = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[sys]);

					if(systemListConstructor.systemList[j].systemOwnedBy == systemListConstructor.systemList[thisSystem].systemOwnedBy) //If connected system is owned by player
					{
						foundNextSystem = true; //Found next system to move to
					}
				}

				CircleLineIntersection(systemListConstructor.systemList[thisSystem].systemObject.transform.position, systemListConstructor.systemList[thisSystem].permanentConnections[sys].transform.position); //Get new intersection
				boundTwo = Mathf.Rad2Deg * Mathf.Acos (intersectionLeftStart.x / radius); //Set bound two to point of intersection
				break;
			}
		}

		RoundLine (systemListConstructor.systemList [thisSystem].systemObject.transform.position, boundOne, boundTwo); //Round the line off between bounds one and two
	}

	private bool AddVertices(int i, int j)
	{
		int k = 0; //Reset counter

		CircleLineIntersection(systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position); //Find intersections of perpline and circle

		if(vertexPoints.Contains(intersectionLeftStart)) //If vertexpoints contains the left intersection, increase the counter
		{
			++k;
		}

		//else
		//{
			vertexPoints.Add (intersectionLeftStart); //Add the intersection to vertexpoints
		//}

		if(vertexPoints.Contains(intersectionLeftEnd)) //If vertexpoints contains corresponding intersection on nearest circle, increase counter
		{
			++k;
		}

		vertexPoints.Add (intersectionLeftEnd); //Add the intersection to vertexpoints

		if(k == 2) //If both intersections are already in vertexpoints, we must be at the beginning
		{
			return false; //Return
		}

		return true; //Continue
	}

	public bool IsBoundarySystem(int currentSystem) //Checks if system is a boundary system
	{
		vectorList.Clear (); //Clear list

		//Create vectors to represent +/- x and y lines originating from system
		Vector3 left = new Vector3(systemListConstructor.systemList[currentSystem].systemObject.transform.position.x - 0.5f, -20f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.z);
		Vector3 right = new Vector3(systemListConstructor.systemList[currentSystem].systemObject.transform.position.x + 0.5f, 110f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.z);
		Vector3 above = new Vector3(110f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.y + 0.5f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.z);
		Vector3 below = new Vector3(-20f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.y - 0.5f, systemListConstructor.systemList[currentSystem].systemObject.transform.position.z);

		//Add to list
		vectorList.Add (left);
		vectorList.Add (right);
		vectorList.Add (above);
		vectorList.Add (below);

		int counter = 0;

		for(int i = 0; i < vectorList.Count; ++i)
		{
			if(mapConstructor.TestForIntersection(systemListConstructor.systemList[currentSystem].systemObject.transform.position, vectorList[i], true) == true) //Check to see if these vectors intersect with any connections
			{
				for(int j = 0; j < mapConstructor.allIntersections.Count; ++j) //Returns an array of all intersections
				{
					int k = RefreshCurrentSystem(mapConstructor.coordinateList[mapConstructor.allIntersections[j]].systemOne); 
					int l = RefreshCurrentSystem(mapConstructor.coordinateList[mapConstructor.allIntersections[j]].systemTwo);

					if(systemListConstructor.systemList[k].systemOwnedBy == systemListConstructor.systemList[currentSystem].systemOwnedBy && 
					   systemListConstructor.systemList[l].systemOwnedBy == systemListConstructor.systemList[currentSystem].systemOwnedBy) //If both systems in connection are owned by player
					{
						++counter; //This is not a boundary system
					}
				}
			}
		}

		if(counter < 4)
		{
			return true;
		}

		return false;
	}

	public void SetVertexPoints(int selectedSystem, int prevSystem, bool isInitial)
	{
		bool neighboursFound = false, moveSystem = false;
		int target = -1, startingIterator = 0;

		if(isInitial == false)
		{
			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].permanentConnections.Count; ++i)
			{
				if(systemListConstructor.systemList[selectedSystem].permanentConnections[i] == systemListConstructor.systemList[prevSystem].systemObject)
				{
					startingIterator = i + 1;
				}                                                                                                         
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[selectedSystem].permanentConnections.Count; ++i) //For each connection in permanent connections
		{
			if(startingIterator >= systemListConstructor.systemList[selectedSystem].permanentConnections.Count)
			{
				startingIterator = 0;
			}

			target = RefreshCurrentSystem(systemListConstructor.systemList[selectedSystem].permanentConnections[startingIterator]); //Target is first owner permanent connections (clockwise)

			if(systemListConstructor.systemList[target].systemOwnedBy == systemListConstructor.systemList[selectedSystem].systemOwnedBy)
			{
				if(IsBoundarySystem(target) == true)
				{
					neighboursFound = true;
					
					bool tempBool = AddVertices(selectedSystem, target);
					
					if(tempBool == true)
					{
						moveSystem = true;
					}
				}
			}
			
			if(moveSystem == true)
			{
				break;	
			}

			startingIterator++;
		}

		if(selectedSystem == system && isInitial == false)
		{
			moveSystem = false;
		}

		if(moveSystem == true)
		{
			SetVertexPoints(target, selectedSystem, false);
		}

		if(neighboursFound == false)
		{
			RoundLine (systemListConstructor.systemList [system].systemObject.transform.position, -100.0f, 1000.0f);
		}
	}

	private void CheckForIntersection(int i)
	{
		float A1 = vertexPoints[i - 3].y - vertexPoints[i - 2].y;
		float B1 = vertexPoints[i - 2].x - vertexPoints[i - 3].x;
		float C1 = (A1 * vertexPoints[i - 2].x) + (B1 * vertexPoints[i - 2].y);

		float A2 = vertexPoints[i - 1].y - vertexPoints[i].y;
		float B2 = vertexPoints[i].x - vertexPoints[i - 1].x;
		float C2 = (A2 * vertexPoints[i].x) + (B2 * vertexPoints[i].y);
		
		float determinant = (A1 * B2) - (A2 * B1);
		
		if (determinant != 0.0f) 
		{
			float x = (B2 * C1 - B1 * C2) / determinant;
			float y = (A1 * C2 - A2 * C1) / determinant;
			
			Vector2 intersection = new Vector2(x, y);
			
			if(mapConstructor.PointLiesOnLine(vertexPoints[i - 3], vertexPoints[i - 2], intersection))
			{
				if(mapConstructor.PointLiesOnLine(vertexPoints[i - 1], vertexPoints[i], intersection))
				{
					vertexPoints[i - 2] = new Vector3(intersection.x, intersection.y, vertexPoints[i].z);
					vertexPoints.RemoveAt (i - 1);
				}
			}
		}
	}

	public void CreateBoundary(string race)
	{
		lineRenderer = gameObject.GetComponent<LineRenderer> (); //Get reference to linerenderer component

		vertexPoints.Clear(); //Clear current linerenderer vertices

		lineRenderer.SetWidth (width, width); //Set width

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == race && IsBoundarySystem(i) == true) //Find a boundary system
			{
				system = i; //Set starting system to this system
			}
		}

		SetVertexPoints (system, -1, true); //Set all the other vertex points

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
