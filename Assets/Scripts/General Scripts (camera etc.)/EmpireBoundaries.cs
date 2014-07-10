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
		public GameObject[] points = new GameObject[3]; //A, B, C points
		public Vector3[] lines = new Vector3[3]; //AB, BC, CA line equations
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

	private bool CheckForTriangleCreation(int curExternal, int curPoint)
	{
		List<GameObject> newTriPoints = new List<GameObject>();
		Triangle activeTriangle = FindNextTri(curExternal);

		for(int k = 0; k < 3; ++k) //For all vertexes of those triangles
		{
			Vector3 lineToVertex = mapConstructor.ABCLineEquation(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position); //Get the abc line between the vertex and the point
			bool illegalIntersection = false;
			
			for(int l = 0; l < triangles.Count; ++l) //For all triangles
			{
				bool isInternalTri = false;

				for(int m = 0; m < 3; ++m)
				{
					if(externalPoints.Contains(triangles[l].points[m]))
					{
						isInternalTri = false;
						break;
					}
				}

				if(isInternalTri == true)
				{
					continue;
				}

				for(int m = 0; m < 3; ++m) //For all lines in those triangles
				{
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

					if(mapConstructor.PointLiesOnLine(unvisitedStars[curPoint].transform.position, activeTriangle.points[k].transform.position, intersection))
					{
						illegalIntersection = true;
						break;
					}
				}
				
				if(illegalIntersection == true) //If there is an illegal intersection at all
				{
					break; //Stop the algorithm
				}
			}
			
			if(illegalIntersection == false && newTriPoints.Contains(activeTriangle.points[k]) == false) //If there were no illegal intersections
			{
				newTriPoints.Add (activeTriangle.points[k]); //Add the vertex of this triangle to the points to be added to a new triangle
			}
		}
		
		if(newTriPoints.Count == 2) //If there are exactly 2 points to be added this is a valid triangle so make one
		{
			Triangle newTri = new Triangle();
			newTri.points[0] = unvisitedStars[curPoint];
			newTri.points[1] = newTriPoints[0];
			newTri.points[2] = newTriPoints[1];
			newTri.lines[0] = mapConstructor.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position);
			newTri.lines[1] = mapConstructor.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position);
			newTri.lines[2] = mapConstructor.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position);
			triangles.Add (newTri);
			externalPoints.Add (unvisitedStars [curPoint]);
			CheckInteriorPoints();
			return true;
		}
		
		newTriPoints.Clear (); //Clear the new tri points list

		return false;
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
		newTri.points[0] = unvisitedStars [0];
		newTri.points[1] = unvisitedStars [1];
		newTri.points[2] = unvisitedStars [2];
		newTri.lines[0] = mapConstructor.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position);
		newTri.lines[1] = mapConstructor.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position);
		newTri.lines[2] = mapConstructor.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position);
		externalPoints.Add (unvisitedStars [0]);
		externalPoints.Add (unvisitedStars [1]);
		externalPoints.Add (unvisitedStars [2]);
		triangles.Add (newTri);

		unvisitedStars.RemoveRange (0, 3);

		for(int i = 0; i < 20; ++i)//unvisitedStars.Count; ++i) //For all unchecked points
		{
			int exteriorsChecked = 0;
			bool finishedGeneratingTriangles = false;

			while(finishedGeneratingTriangles == false)
			{
				if(CheckForTriangleCreation(exteriorsChecked, i) == false)
				{
					finishedGeneratingTriangles = true;
				}

				++exteriorsChecked;
			}
		}
	}

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

				float distance = Vector3.Distance (start, end);
				
				float rotationZRad = Mathf.Acos ((end.y - start.y) / distance);
				
				float rotationZ = rotationZRad * Mathf.Rad2Deg;
				
				if(start.x < end.x)
				{
					rotationZ = -rotationZ;
				}
				
				Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);
				
				Quaternion rotation = new Quaternion();

				//Debug.Log (start + " | " + end);

				rotation.eulerAngles = vectRotation;

				Vector3 midPoint = (start + end) / 2; //Get midpoint between target and current system
				
				midPoint = new Vector3 (midPoint.x, midPoint.y, -2.0f); //Create vector from midpoint

				lineRenderScript = systemListConstructor.systemList[0].systemObject.GetComponent<LineRenderScript>();

				GameObject line = (GameObject)Instantiate(lineRenderScript.line, midPoint, rotation);

				line.transform.localScale = new Vector3(0.10f, distance, 0f);

				if(looped == true)
				{
					break;
				}
			}
		}
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