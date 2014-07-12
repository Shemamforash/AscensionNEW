using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private GameObject[] boundRings;
	public GameObject blankCircle;
	public Transform boundaryContainer;
	public float[] radius;
	public Material humansMat, selkiesMat, nereidesMat;
	private List<VoronoiCellVertices> voronoiVertices = new List<VoronoiCellVertices> ();
	private List<Triangle> triangles = new List<Triangle> ();
	private List<Triangle> tempTri = new List<Triangle>();
	private List<GameObject> unvisitedStars = new List<GameObject> ();
	private List<GameObject> externalPoints = new List<GameObject> ();
	private Triangle activeTriangle;

	private class VoronoiCellVertices
	{
		public List<Vector3> vertices = new List<Vector3>();
	}

	public void SetArrSize()
	{
		CreateVoronoiCells ();
		boundRings = new GameObject[systemListConstructor.systemList.Count];
		radius = new float[systemListConstructor.mapSize];
	}

	void Update()
	{
		for(int i = 0; i < boundRings.Length; ++i)
		{
			if(boundRings[i] != null)
			{
				boundRings[i].transform.position = systemListConstructor.systemList[i].systemObject.transform.position;
			}
		}
	}

	private class Triangle
	{
		public List<GameObject> points = new List<GameObject>(); //A, B, C points
		public Vector3[] lines = new Vector3[3]; //AB, BC, CA line equations
		public bool isInternal = false; //This allows me to ignore any triangles with no external points
	}

	private float AreaOfTriangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
	{
		float determinant = vertexA.x * (vertexB.y - vertexC.y) + vertexB.x * (vertexC.y - vertexA.y) + vertexC.x * (vertexA.y - vertexB.y); //det = x1(y2-y3) + x2(y3-y1) + x3(y1-y2)
		return determinant / 2f; //area is half determinant
	}

	private bool IsInTriangle(Vector3 point)
	{
		for(int i = 0; i < triangles.Count; ++i)
		{
			float originalArea = AreaOfTriangle(triangles[i].points[0].transform.position, triangles[i].points[1].transform.position, triangles[i].points[2].transform.position); //Get area of triangle
			float areaA = AreaOfTriangle(triangles[i].points[0].transform.position, triangles[i].points[1].transform.position, point); //Get area of new triangle formed by a and b of original triangle and new point to be tested
			float areaB = AreaOfTriangle(triangles[i].points[1].transform.position, triangles[i].points[2].transform.position, point); //Get area of new triangle formed by b and c of original triangle and new point to be tested
			float areaC = AreaOfTriangle(triangles[i].points[2].transform.position, triangles[i].points[0].transform.position, point); //Get area of new triangle formed by c and a of original triangle and new point to be tested

			float u = areaC / originalArea; //Calculate u of barycentric coordinates
			float v = areaA / originalArea; //Calculate v of barycentric coordinates
			float w = areaB / originalArea; //Calculate w of barycentric coordinate

			if(0f <= u && u <= 1) //If u is within 0 and 1
			{
				if(0f <= v && v <= 1) //If v is within 0 and 1
				{
					if(0f <= w && w <= 1) //If w is within 0 and 1
					{
						activeTriangle = triangles[i];
						return true; //Point lies within triangle so return true
					}
				}
			}
		}

		return false;
	}

	private void CacheNearestStars()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //Add all systems to a list of unvisited nodes
		{
			unvisitedStars.Add (systemListConstructor.systemList[i].systemObject);
		}

		Vector3 centre = new Vector3 (50f, 50f, 0f); //Create centre point at middle of map

		for(int j = unvisitedStars.Count; j > 0; --j) //For all unvisited stars
		{
			bool swapsMade = false;
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float distanceA = Vector3.Distance (unvisitedStars[k].transform.position, centre); //Check distance to centre
				float distanceB = Vector3.Distance (unvisitedStars[k - 1].transform.position, centre); //Check distance to centre

				if(distanceA < distanceB) //Sort smallest to largest
				{
					GameObject temp = unvisitedStars[k];
					unvisitedStars[k] = unvisitedStars[k - 1];
					unvisitedStars[k - 1] = temp;
					swapsMade = true;
				}
			}

			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
	}

	private Triangle FindNextTri(int curExternal)
	{
		for(int i = curExternal; i < externalPoints.Count; ++i)
		{
			for(int j = 0; j < triangles.Count; ++j)
			{
				for(int k = 0; k < 3; ++k)
				{
					if(triangles[j].points[k] == externalPoints[i])
					{
						return triangles[j];
					}
				}
			}
		}

		return null;
	}

	private bool CheckPointIsCloseToPoint(Vector3 pointA, Vector3 pointB)
	{
		if(pointA.x - 0.5f <= pointB.x && pointA.x + 0.5f >= pointB.x)
		{
			if(pointA.y - 0.5f <= pointB.y && pointA.y + 0.5f >= pointB.y)
			{
				return true;
			}
		}

		return false;
	}

	private bool IsIllegalIntersection(int external, int point, Vector3 curToExt)
	{
		for(int j = 0; j < triangles.Count; ++j) //For every triangle
		{
			if(triangles[j].isInternal == true)
			{
				continue;
			}

			for(int k = 0; k < 3; ++k) //For each line in that triangle
			{
				Vector3 pointA = Vector3.zero; //Get the points at each end of the line
				Vector3 pointB = Vector3.zero;
				
				if(k == 0)
				{
					pointA = triangles[j].points[0].transform.position;
					pointB = triangles[j].points[1].transform.position;
				}
				if(k == 1)
				{
					pointA = triangles[j].points[1].transform.position;
					pointB = triangles[j].points[2].transform.position;
				}
				if(k == 2)
				{
					pointA = triangles[j].points[2].transform.position;
					pointB = triangles[j].points[0].transform.position;
				}
				
				Vector2 intersection = mapConstructor.IntersectionOfTwoLines(curToExt, triangles[j].lines[k]); //Get the intersection of the line with the current star to external point line

				if(CheckPointIsCloseToPoint(intersection, new Vector2(externalPoints[external].transform.position.x, externalPoints[external].transform.position.y))) //If the intersection is on the external point
				{
					continue; //This is not illegal so keep going
				}

				if(mapConstructor.PointLiesOnLine(externalPoints[external].transform.position, unvisitedStars[point].transform.position, intersection) == true) //If the point lies elsewhere on the line
				{
					if(mapConstructor.PointLiesOnLine(pointA, pointB, intersection) == true)
					{
						return true; //This IS an illegal intersection so return that, otherwise keep checking
					}
				}
			}
		}

		return false;
	}

	private void LinkPointToTris(int curPoint) //Send the current unvisited star as the seed point
	{
		for(int i = 0; i < externalPoints.Count; ++i) //For all the external points
		{
			Vector3 lineCurToExternal = mapConstructor.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[i].transform.position); //Create a line between the external point and the unvisited star
			bool illegalLine = false;

			if(IsIllegalIntersection(i, curPoint, lineCurToExternal) == true)
			{
				continue;
			}

			for(int j = 0; j < triangles.Count; ++j) //For all triangles
			{
				if(triangles[j].points.Contains(externalPoints[i])) //If triangle contains the external point currently being tested
				{
					bool validTri = false;

					for(int k = 0; k < externalPoints.Count; ++k) //Go through all the other external points
					{
						if(k == i)
						{
							continue;
						}

						if(triangles[j].points.Contains(externalPoints[k])) //To make sure triangle contains another external point - we are not interested in triangle with only 1 external point (a tri cannot be formed)
						{
							if(IsIllegalIntersection(k, curPoint, mapConstructor.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[k].transform.position)) == false)
							{
								GameObject pointA = externalPoints[i], pointB = externalPoints[k], pointC = unvisitedStars[curPoint];

								bool triangleExists = false;

								for(int l = 0; l < tempTri.Count; ++l)
								{
									if(tempTri[l].points.Contains (pointA) == true && tempTri[l].points.Contains (pointB) == true && tempTri[l].points.Contains (pointC) == true)
									{
										triangleExists = true;
										break;
									}
								}

								if(triangleExists == false)
								{
									Triangle newTri = new Triangle();
									newTri.points.Add (pointA);
									newTri.points.Add (pointB);
									newTri.points.Add (pointC);
									newTri.lines[0] = mapConstructor.ABCLineEquation (pointA.transform.position, pointB.transform.position);
									newTri.lines[1] = mapConstructor.ABCLineEquation (pointB.transform.position, pointC.transform.position);
									newTri.lines[2] = mapConstructor.ABCLineEquation (pointC.transform.position, pointA.transform.position);
									tempTri.Add (newTri);
								}
							}
						}
					}
				}
			}
		}
	}

	private void CacheTempTris(int curPoint)
	{
		for(int i = 0; i < tempTri.Count; ++i)
		{
			triangles.Add (tempTri[i]);
		}

		externalPoints.Add (unvisitedStars [curPoint]);
		//unvisitedStars.RemoveAt(curPoint);
		CheckInteriorPoints ();
		tempTri.Clear ();
	}

	private void CheckInteriorPoints() //Checks through external point list to see if any points have become internal ones (are within the polygon formed by linking triangles
	{
		List<int> pointsToRemove = new List<int> ();

		for(int i = 0; i < externalPoints.Count; ++i)
		{
			float tempAngle = 0;

			for(int j = 0; j < triangles.Count; ++j)
			{
				for(int k = 0; k < 3; ++k)
				{
					if(triangles[j].points[k] == externalPoints[i])
					{
						int a = 0, b = 0;

						if(k == 0)
						{
							a = 1; b = 2;
						}
						if(k == 1)
						{
							a = 0; b = 2;
						}
						if(k == 2)
						{
							a = 0; b = 1;
						}

						Vector3 line1 = triangles[j].points[k].transform.position - triangles[j].points[a].transform.position;
						Vector3 line2 = triangles[j].points[k].transform.position - triangles[j].points[b].transform.position;
						
						tempAngle += Vector3.Angle (line1, line2);
					}
				}
			}

			if(tempAngle > 359)
			{
				pointsToRemove.Add (i);
			}
		}

		for(int j = 0; j < triangles.Count; ++j)
		{
			bool isInternalTri = true;
			
			for(int k = 0; k < 3; ++k)
			{
				if(externalPoints.Contains (triangles[j].points[k]))
				{
					isInternalTri = false;
				}
			}
			
			if(isInternalTri == true)
			{
				triangles[j].isInternal = true;
			}
		}
		
		int counter = 0;

		for(int i = 0; i < pointsToRemove.Count; ++i)
		{
			externalPoints.RemoveAt(pointsToRemove[i] - counter);
			++counter;
		}
	}

	private void SimpleTriangulation()
	{
		CacheNearestStars ();

		Triangle newTri = new Triangle();
		newTri.points.Add (unvisitedStars [0]);
		newTri.points.Add (unvisitedStars [1]);
		newTri.points.Add (unvisitedStars [2]);
		newTri.lines[0] = mapConstructor.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position);
		newTri.lines[1] = mapConstructor.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position);
		newTri.lines[2] = mapConstructor.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position);
		externalPoints.Add (unvisitedStars [0]);
		externalPoints.Add (unvisitedStars [1]);
		externalPoints.Add (unvisitedStars [2]);
		triangles.Add (newTri);

		unvisitedStars.RemoveRange (0, 3);


		for(int i = 0; i < unvisitedStars.Count; ++i)//unvisitedStars.Count; ++i) //For all unchecked points
		{
			LinkPointToTris(i);
			CacheTempTris(i);
		}

		Debug.Log (unvisitedStars.Count);

		Debug.Log (triangles.Count);
	}

	/*
	private void CheckForTriangleCreation(int curExternal, int curPoint)
	{
		List<GameObject> newTriPoints = new List<GameObject>();
		Triangle activeTriangle = FindNextTri(curExternal);

		for(int k = 0; k < 3; ++k) //For all vertexes of the active triangle
		{
			Debug.Log (curPoint + " | " + unvisitedStars.Count);

			Vector3 lineToVertex = mapConstructor.ABCLineEquation(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position); //Get the abc line between the vertex and the point

			bool illegalIntersection = false; //Say there is no intersection

			for(int l = 0; l < triangles.Count; ++l) //For all triangles
			{
				for(int m = 0; m < 3; ++m) //For all lines in those triangles
				{
					Vector3 pointA = Vector3.zero;
					Vector3 pointB = Vector3.zero;

					if(m == 0)
					{
						pointA = triangles[l].points[0].transform.position;
						pointB = triangles[l].points[2].transform.position;
					}
					if(m == 1)
					{
						pointA = triangles[l].points[1].transform.position;
						pointB = triangles[l].points[0].transform.position;
					}
					if(m == 2)
					{
						pointA = triangles[l].points[2].transform.position;
						pointB = triangles[l].points[1].transform.position;
					}

					if(triangles[l] == activeTriangle) //If the triangle to test is the same as the triangle containing the current vertex, check to skip the lines connecting to the current vertex
					{
						if(k == m)
						{
							continue;
						}
						if(k - 1 < 0)
						{
							if(2 == m)
							{
								continue;
							}
						}
						if(k - 1 >= 0)
						{
							if(k - 1 == m)
							{
								continue;
							}
						}
					}

					Vector3 intersection = mapConstructor.IntersectionOfTwoLines(lineToVertex, triangles[l].lines[m]); //Intersection between line to testing point and other lines in triangle

					if(intersection == Vector3.zero)
					{
						Debug.Log ("Wut");
					}

					if(CheckPointIsCloseToPoint(activeTriangle.points[k].transform.position, intersection)) //If the intersection lies where the two ends of the line meet this is not illegal
					{
						break;
					}

					if(mapConstructor.PointLiesOnLine(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position, intersection))
					{
						Instantiate(systemInvasion.invasionQuad, intersection, Quaternion.identity);

						illegalIntersection = true;
						break;
					}
				}
				
				if(illegalIntersection == true) //If there is an illegal intersection at all
				{
					//DrawDebugLine(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position, turnInfoScript.selkiesMaterial);
					break; //Stop the algorithm
				}

				//DrawDebugLine(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position, turnInfoScript.humansMaterial);

			}
			
			if(illegalIntersection == false && newTriPoints.Contains(activeTriangle.points[k]) == false) //If there were no illegal intersections
			{
				newTriPoints.Add (activeTriangle.points[k]); //Add the vertex of this triangle to the points to be added to a new triangle
			}
		}
		
		if(newTriPoints.Count == 2) //If there are exactly 2 points to be added this is a valid triangle so make one
		{
			Triangle newTri = new Triangle();
			newTri.points.Add (unvisitedStars[curPoint]);
			newTri.points.Add (newTriPoints[0]);
			newTri.points.Add (newTriPoints[1]);
			newTri.lines[0] = mapConstructor.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position);
			newTri.lines[1] = mapConstructor.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position);
			newTri.lines[2] = mapConstructor.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position);
			tempTri.Add (newTri);
		}
		
		newTriPoints.Clear (); //Clear the new tri points list
	}*/

	public void CreateVoronoiCells()
	{
		SimpleTriangulation ();

		/*
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			bool looped = false;

			if(systemListConstructor.systemList[i].permanentConnections.Count >= 2)
			{

			VoronoiCellVertices newCell = new VoronoiCellVertices(); //Create new voronoi cell


				for(int j = 0; j < systemListConstructor.systemList[i].permanentConnections.Count; ++j) //For all permanent connections
				{
					Vector3 systemA = systemListConstructor.systemList[i].permanentConnections[j].transform.position; //System A equals this system

					int nextSys = j + 1;

					if(j + 1 == systemListConstructor.systemList[i].permanentConnections.Count)
					{
						nextSys = 0;
						looped = true;
					}

					Vector3 systemB = systemListConstructor.systemList[i].systemObject.transform.position;

					Vector3 systemC = systemListConstructor.systemList[i].permanentConnections[nextSys].transform.position;

					Vector3 lineAB = mapConstructor.PerpendicularLineEquation(systemA, systemB);
					Vector3 lineBC = mapConstructor.PerpendicularLineEquation(systemB, systemC);

					Vector3 centre = mapConstructor.IntersectionOfTwoLines(lineAB, lineBC);

					newCell.vertices.Add (centre);
					//Instantiate(systemInvasion.invasionQuad, centre, Quaternion.identity);

					if(looped)
					{
						break;
					}
				}

			voronoiVertices.Add (newCell);
			}
		}
		*/


		for(int i = 0; i < triangles.Count; ++i)
		{
			bool looped = false;

			for(int j = 0; j < 3; ++j)
			{
				Vector3 start = triangles[i].points[j].transform.position;

				Vector3 end = Vector3.zero;

				if(start == end)
				{
					continue;
				}

				if(j + 1 >= 3)
				{
					end = triangles[i].points[0].transform.position;
					looped = true;
				}
				else
				{
					end = triangles[i].points[j + 1].transform.position;
				}

				DrawDebugLine(start, end, turnInfoScript.nereidesMaterial);

				if(looped == true)
				{
					break;
				}
			}
		}
	}

	private void DrawDebugLine(Vector3 start, Vector3 end, Material mat)
	{
		float distance = Vector3.Distance (start, end);
		
		float rotationZRad = Mathf.Acos ((end.y - start.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(start.x < end.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		Quaternion rotation = new Quaternion();
		
		rotation.eulerAngles = vectRotation;
		
		Vector3 midPoint = (start + end) / 2; //Get midpoint between target and current system
		
		midPoint = new Vector3 (midPoint.x, midPoint.y, -2.0f); //Create vector from midpoint
		
		lineRenderScript = systemListConstructor.systemList[0].systemObject.GetComponent<LineRenderScript>();
		
		GameObject line = (GameObject)Instantiate(lineRenderScript.line, midPoint, rotation);

		line.renderer.material = mat;

		float width = 0.10f;

		if(mat == turnInfoScript.humansMaterial || mat == turnInfoScript.selkiesMaterial)
		{
			width = 0.15f;
		}
		
		line.transform.localScale = new Vector3(width, distance, 0f);
	}

	public void CalculateRadius(int system)
	{
		float tempRadius = 100f;
		
		for(int j = 0; j < systemListConstructor.systemList[system].permanentConnections.Count; ++j)
		{
			float tempDistance = Vector3.Distance(systemListConstructor.systemList[system].systemObject.transform.position, systemListConstructor.systemList[system].permanentConnections[j].transform.position);
			
			if(tempDistance < tempRadius)
			{
				tempRadius = tempDistance / 2;
			}
		}
		
		radius[system] = tempRadius;
		
		if(radius[system] > 3f)
		{
			radius[system] = 3f;
		}
	}
	
	private void CreateCircle (int system, Material material)
	{
		if(boundRings[system] != null)
		{
			Destroy(boundRings[system]);
		}
		
		CalculateRadius (system);
		
		GameObject temp = (GameObject)Instantiate (blankCircle, systemListConstructor.systemList [system].systemObject.transform.position, Quaternion.identity);

		temp.AddComponent ("BoundaryFadeScript");
		
		temp.transform.parent = boundaryContainer;
		
		temp.transform.localScale = new Vector3 (radius[system], radius[system], radius[system]);
		
		temp.renderer.material = material;
		
		boundRings [system] = temp;
	}
	
	public void ModifyBoundaryCircles()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			switch(systemListConstructor.systemList[i].systemOwnedBy)
			{
			case "Humans":
				CreateCircle(i, humansMat);
				break;
			case "Selkies":
				CreateCircle(i, selkiesMat);
				break;
			case "Nereides":
				CreateCircle(i, nereidesMat);
				break;
			default:
				if(boundRings[i] != null)
				{
					Destroy (boundRings[i]);
				}
				break;
			}
		}
	}
}