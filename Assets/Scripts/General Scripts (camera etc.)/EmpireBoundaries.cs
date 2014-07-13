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

	private int flips = 0;

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
		public List<Vector3> lines = new List<Vector3>(); //AB, BC, CA line equations
		public bool isInternal = false; //This allows me to ignore any triangles with no external points
	}

	private float AngleOppositeLineInTri(int tri, int line)
	{
		Vector2 lineA = Vector2.zero;
		Vector2 lineB = Vector2.zero;

		if(line == 0)
		{
			lineA = new Vector2(triangles[tri].lines[1].x, triangles[tri].lines[1].y);
			lineB = new Vector2(triangles[tri].lines[2].x, triangles[tri].lines[2].y);
		}
		if(line == 1)
		{
			lineA = new Vector2(triangles[tri].lines[0].x, triangles[tri].lines[0].y);
			lineB = new Vector2(triangles[tri].lines[2].x, triangles[tri].lines[2].y);
		}
		if(line == 2)
		{
			lineA = new Vector2(triangles[tri].lines[0].x, triangles[tri].lines[0].y);
			lineB = new Vector2(triangles[tri].lines[1].x, triangles[tri].lines[1].y);
		}

		return Vector2.Angle (lineA, lineB);
	}

	private int GetPointNotInLine(int line)
	{
		int point = -1;

		if(line == 0)
		{
			point = 2;
		}
		if(line == 1)
		{
			point = 0;
		}
		if(line == 2)
		{
			point = 1;
		}

		return point;
	}

	private void CheckForDupes()
	{
		int dupes = 0;

		for(int i = 0; i < triangles.Count; ++i)
		{
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}

				int count = 0;

				for(int k = 0; k < 3; ++k)
				{
					for(int l = 0; l < 3; ++l)
					{
						if(triangles[i].points[k] == triangles[j].points[l])
						{
							++count;
						}
					}
				}

				if(count == 3)
				{
					++dupes;
				}
			}
		}

		Debug.Log (dupes);
	}

	private float AngleBetweenLinesOfTri(Triangle tri, int anglePoint) //Anglepoint is the point at which the angle needs to be found
	{
		float lengthAB = Mathf.Sqrt(Mathf.Pow(tri.points[0].transform.position.x - tri.points[1].transform.position.x, 2f) + Mathf.Pow(tri.points[0].transform.position.y - tri.points[1].transform.position.y, 2f));
		float lengthBC = Mathf.Sqrt(Mathf.Pow(tri.points[1].transform.position.x - tri.points[2].transform.position.x, 2f) + Mathf.Pow(tri.points[1].transform.position.y - tri.points[2].transform.position.y, 2f));
		float lengthCA = Mathf.Sqrt(Mathf.Pow(tri.points[0].transform.position.x - tri.points[2].transform.position.x, 2f) + Mathf.Pow(tri.points[0].transform.position.y - tri.points[2].transform.position.y, 2f));
		float angle = 0f;

		if(anglePoint == 0)
		{
			angle = CosLawAngle(lengthBC, lengthCA, lengthAB);
		}
		if(anglePoint == 1)
		{
			angle = CosLawAngle(lengthCA, lengthAB, lengthBC);
		}
		if(anglePoint == 2)
		{
			angle = CosLawAngle(lengthAB, lengthBC, lengthCA);
		}

		return angle;
	}

	private float CosLawAngle(float a, float b, float c)
	{
		float numerator = (b * b) + (c * c) - (a * a);
		float denominator = 2 * b * c;
		float angleRad = Mathf.Acos (numerator / denominator);

		return angleRad * Mathf.Rad2Deg;
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
									newTri.lines.Add (mapConstructor.ABCLineEquation (pointA.transform.position, pointB.transform.position));
									newTri.lines.Add (mapConstructor.ABCLineEquation (pointB.transform.position, pointC.transform.position));
									newTri.lines.Add (mapConstructor.ABCLineEquation (pointC.transform.position, pointA.transform.position));
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

	private bool TriangulationToDelaunay()
	{
		List<GameObject> sharedPoints = new List<GameObject>();
		
		for(int i = 0; i < triangles.Count; ++i) //For each triangle
		{
			for(int j = 0; j < triangles.Count; ++j) //For all other triangles
			{
				if(i == j)
				{
					continue;
				}
				
				for(int k = 0; k < 3; ++k)
				{
					for(int l = 0; l < 3; ++l)
					{
						if(triangles[i].points[k] == triangles[j].points[l]) //If the current triangle shares a side with another triangle
						{
							sharedPoints.Add (triangles[i].points[k]);
							break;
						}
					}
					
					if(sharedPoints.Count == 2)
					{
						//break;
					}
					if(sharedPoints.Count == 3)
					{
						Debug.Log("bananas");
					}
				}
				
				if(sharedPoints.Count == 2)
				{
					if(CheckIsDelaunay(sharedPoints[0], sharedPoints[1], triangles[i], triangles[j]) == false)
					{
						return false;
					}
				}
				
				sharedPoints.Clear ();
			}
		}
		
		return true;
	}

	private bool CheckIsDelaunay(GameObject sharedPointA, GameObject sharedPointB, Triangle triOne, Triangle triTwo)
	{
		GameObject unsharedPointOne = new GameObject ();
		GameObject unsharedPointTwo = new GameObject ();
		float angleAlpha = 0f, angleBeta = 0f;
		
		for(int i = 0; i < 3; ++i)
		{
			if(triOne.points[i] != sharedPointA && triOne.points[i] != sharedPointB)
			{
				unsharedPointOne = triOne.points[i];
				angleAlpha = AngleBetweenLinesOfTri(triOne, i);
			}
			if(triTwo.points[i] != sharedPointA && triTwo.points[i] != sharedPointB)
			{
				unsharedPointTwo = triTwo.points[i];
				angleBeta = AngleBetweenLinesOfTri(triTwo, i);
			}
		}
		
		Vector3 sharedPointLine = mapConstructor.ABCLineEquation (sharedPointA.transform.position, sharedPointB.transform.position);
		Vector3 unsharedPointLine = mapConstructor.ABCLineEquation (unsharedPointOne.transform.position, unsharedPointTwo.transform.position);
		Vector2 intersection = mapConstructor.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
		
		if(mapConstructor.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false)
		{
			return true;
		}
		
		if(angleAlpha + angleBeta > 180f)
		{
			Debug.Log (triangles.Count + " initial");

			Triangle newTriA = new Triangle ();
			newTriA.points.Add (unsharedPointOne);
			newTriA.points.Add (unsharedPointTwo);
			newTriA.points.Add (sharedPointA);
			newTriA.lines.Add (mapConstructor.ABCLineEquation (newTriA.points[0].transform.position, newTriA.points[1].transform.position));
			newTriA.lines.Add (mapConstructor.ABCLineEquation (newTriA.points[1].transform.position, newTriA.points[2].transform.position));
			newTriA.lines.Add (mapConstructor.ABCLineEquation (newTriA.points[2].transform.position, newTriA.points[0].transform.position));
			triangles.Add (newTriA);
			
			Triangle newTriB = new Triangle ();
			newTriB.points.Add (unsharedPointOne);
			newTriB.points.Add (unsharedPointTwo);
			newTriB.points.Add (sharedPointB);
			newTriB.lines.Add (mapConstructor.ABCLineEquation (newTriB.points[0].transform.position, newTriB.points[1].transform.position));
			newTriB.lines.Add (mapConstructor.ABCLineEquation (newTriB.points[1].transform.position, newTriB.points[2].transform.position));
			newTriB.lines.Add (mapConstructor.ABCLineEquation (newTriB.points[2].transform.position, newTriB.points[0].transform.position));
			triangles.Add (newTriB);
			
			triangles.Remove(triOne);
			triangles.Remove(triTwo);

			flips++;

			Debug.Log (triangles.Count + " final");

			return false;
		}
		
		return true;
	}

	private void SimpleTriangulation()
	{
		CacheNearestStars ();

		Triangle newTri = new Triangle();
		newTri.points.Add (unvisitedStars [0]);
		newTri.points.Add (unvisitedStars [1]);
		newTri.points.Add (unvisitedStars [2]);
		newTri.lines.Add (mapConstructor.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position));
		newTri.lines.Add (mapConstructor.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
		newTri.lines.Add (mapConstructor.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));
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

		bool isDelaunay = false;

		while(isDelaunay == false)
		{
			isDelaunay = TriangulationToDelaunay();
		}

		CheckForDupes ();

		Debug.Log (triangles.Count + " | " + flips);
	}
	
	public void CreateVoronoiCells()
	{
		SimpleTriangulation ();

		/*
		for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
		{
			VoronoiCellVertices newCell = new VoronoiCellVertices(); //Create new voronoi cell

			for(int i = 0; i < triangles.Count; ++i) //For all systems
			{
				if(triangles[i].points.Contains(systemListConstructor.systemList[j].systemObject))
				{
					bool looped = false;


					Vector3 systemA = triangles[i].points[0]; //System A equals this system

					Vector3 systemB = triangles[i].points[1];

					Vector3 systemC = triangles[i].points[2];

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
			}

			voronoiVertices.Add (newCell);
		}*/


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