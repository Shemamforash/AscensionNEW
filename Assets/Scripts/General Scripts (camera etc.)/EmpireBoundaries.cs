using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private LineRenderer lineRenderer;
	private List<Vector3> vertexPoints = new List<Vector3>();
	private float width = 1.5f, radius;
	private int numberOfPoints = 40, system;
	private Vector3 intersectionLeft, intersectionRight;
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

		float A1 = endPoint.y - startPoint.y;
		float B1 = startPoint.x - endPoint.x;
		float C1 = (A1 * startPoint.x) + (B1 * endPoint.y);

		Vector3 line = endPoint - startPoint;

		Vector3 cross = Vector3.Cross (line, new Vector3 (xIntersectOne, yIntersectOne, 0f));

		Debug.Log ("Cross " + cross);

		if(cross.z <= 0)
		{
			intersectionLeft = new Vector3 (xIntersectOne, yIntersectOne, startPoint.z);
			intersectionRight = new Vector3 (xIntersectTwo, yIntersectTwo, startPoint.z);
		}
		if(cross.z > 0)
		{
			intersectionRight = new Vector3 (xIntersectOne, yIntersectOne, startPoint.z);
			intersectionLeft = new Vector3 (xIntersectTwo, yIntersectTwo, startPoint.z);
		}
	}

	public void GetBound(int thisSystem, int parentSystem) //Gets bounds between systems
	{
		float boundOne = Mathf.Rad2Deg * Mathf.Acos (intersectionRight.x / radius); //Bound one is connection point between system and parent
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
				boundTwo = Mathf.Rad2Deg * Mathf.Acos (intersectionLeft.x / radius); //Set bound two to point of intersection
				break;
			}
		}

		RoundLine (systemListConstructor.systemList [thisSystem].systemObject.transform.position, boundOne, boundTwo); //Round the line off between bounds one and two
	}

	private bool AddVertices(int i, int j)
	{
		int k = 0; //Reset counter

		CircleLineIntersection(systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position); //Find intersections of perpline and circle

		if(vertexPoints.Contains(intersectionLeft)) //If vertexpoints contains the left intersection, increase the counter
		{
			++k;
		}

		vertexPoints.Add (intersectionLeft); //Add the intersection to vertexpoints

		CircleLineIntersection(systemListConstructor.systemList[j].systemObject.transform.position, systemListConstructor.systemList[i].systemObject.transform.position); //Find intersection of perpline in reverse

		if(vertexPoints.Contains(intersectionRight)) //If vertexpoints contains corresponding intersection on nearest circle, increase counter
		{
			++k;
		}

		vertexPoints.Add (intersectionRight); //Add the intersection to vertexpoints

		Debug.Log ("VertexPoints " + intersectionLeft + " | " + intersectionRight);

		//GetBound (i, j)
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

	public void SetVertexPoints(int selectedSystem, bool isInitial)
	{
		bool neighboursFound = false, moveSystem = false;
		int target = -1;

		for(int i = 0; i < systemListConstructor.systemList[selectedSystem].permanentConnections.Count; ++i) //For each connection in permanent connections
		{
			target = RefreshCurrentSystem(systemListConstructor.systemList[selectedSystem].permanentConnections[i]); //Target is first owner permanent connections (clockwise)

			if(systemListConstructor.systemList[target].systemOwnedBy == systemListConstructor.systemList[selectedSystem].systemOwnedBy)
			{
				if(IsBoundarySystem(target) == true)
				{
					neighboursFound = true;
					
					bool tempBool = AddVertices(system, target);
					
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
		}

		if(selectedSystem == system && isInitial == false)
		{
			moveSystem = false;
		}

		if(moveSystem == true)
		{
			SetVertexPoints(target, false);
		}

		if(neighboursFound == false)
		{
			RoundLine (systemListConstructor.systemList [system].systemObject.transform.position, -100.0f, 1000.0f);
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

		SetVertexPoints (system, true); //Set all the other vertex points

		for(int i = 0; i < vertexPoints.Count; ++i)
		{
			Debug.Log ("Vertex " + vertexPoints[i]);

			if(vertexPoints.Count < 8)
			{
				AmbientStarRandomiser ambientStars = GameObject.Find ("ScriptsContainer").GetComponent<AmbientStarRandomiser> ();;
				GameObject thingy = (GameObject)Instantiate(ambientStars.ambientStar, vertexPoints[i], Quaternion.identity);
			}
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
