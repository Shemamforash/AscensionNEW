using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Triangulation : MasterScript 
{
	public List<Triangle> triangles = new List<Triangle> ();
	private List<Triangle> tempTri = new List<Triangle>();
	private List<GameObject> unvisitedStars = new List<GameObject> ();
	private List<GameObject> externalPoints = new List<GameObject> ();
	private Triangle activeTriangle;
	private float timer = 0;
	private int number = 0;
	private bool loaded = false, triangulated = false;

	void Update()
	{
		if(systemListConstructor.loaded == true && loaded == false)
		{
			CacheNearestStars ();
			
			Triangle newTri = new Triangle();
			newTri.points.Add (unvisitedStars [0]);
			newTri.points.Add (unvisitedStars [1]);
			newTri.points.Add (unvisitedStars [2]);
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));
			externalPoints.Add (unvisitedStars [0]);
			externalPoints.Add (unvisitedStars [1]);
			externalPoints.Add (unvisitedStars [2]);
			triangles.Add (newTri);
		
			voronoiGenerator.DrawDebugLine(unvisitedStars [0].transform.position, unvisitedStars [1].transform.position, turnInfoScript.selkiesMaterial);
			voronoiGenerator.DrawDebugLine(unvisitedStars [2].transform.position, unvisitedStars [1].transform.position, turnInfoScript.selkiesMaterial);
			voronoiGenerator.DrawDebugLine(unvisitedStars [0].transform.position, unvisitedStars [2].transform.position, turnInfoScript.selkiesMaterial);
			
			unvisitedStars.RemoveRange (0, 3);

			timer = Time.time;

			loaded = true;
		}

		if(loaded == true)
		{
			if(timer + 1.0f < Time.time && number < unvisitedStars.Count)
			{
				if(externalPoints.Contains (unvisitedStars[23]) == true)
				{
					Debug.Log ("isee");
				}

				LinkPointToTris(number);
				CacheTempTris(number);
				++number;

				if(number == unvisitedStars.Count)
				{
					triangulated = true;
				}

				timer = Time.time;
			}
		}

		if(triangulated == true)
		{
			if(timer + 1.0f < Time.time)
			{
				bool isDelaunay = false;

				while(isDelaunay == false)
				{
					isDelaunay = voronoiGenerator.TriangulationToDelaunay();
				}

				for(int i = 0; i < triangles.Count; ++i)
				{
					voronoiGenerator.DrawDebugLine(triangles[i].points[0].transform.position, triangles[i].points [1].transform.position, turnInfoScript.nereidesMaterial);
					voronoiGenerator.DrawDebugLine(triangles[i].points [2].transform.position, triangles[i].points [1].transform.position, turnInfoScript.nereidesMaterial);
					voronoiGenerator.DrawDebugLine(triangles[i].points [0].transform.position, triangles[i].points [2].transform.position, turnInfoScript.nereidesMaterial);
				}
			}
		}
	}

	public void SimpleTriangulation()
	{
		CacheNearestStars ();
		
		Triangle newTri = new Triangle();
		newTri.points.Add (unvisitedStars [0]);
		newTri.points.Add (unvisitedStars [1]);
		newTri.points.Add (unvisitedStars [2]);
		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position));
		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));
		externalPoints.Add (unvisitedStars [0]);
		externalPoints.Add (unvisitedStars [1]);
		externalPoints.Add (unvisitedStars [2]);
		triangles.Add (newTri);

		Debug.Log (unvisitedStars [0] + " | " + unvisitedStars [1] + " | " + unvisitedStars [2] + " | " + unvisitedStars[3]);

		unvisitedStars.RemoveRange (0, 3);

		for(int i = 0; i < unvisitedStars.Count; ++i) //For all unchecked points
		{
			LinkPointToTris(i);
			CacheTempTris(i);
		}

		Debug.Log (triangles.Count);
	
		bool isDelaunay = false;

		//while(isDelaunay == false)
		//{
			//isDelaunay = voronoiGenerator.TriangulationToDelaunay ();
		//}

		CheckForNonDelaunayTriangles ();
	}

	private void CacheNearestStars()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //Add all systems to a list of unvisited nodes
		{
			unvisitedStars.Add (systemListConstructor.systemList[i].systemObject);
		}
		
		for(int i = 0; i < 12; ++i)
		{
			float angle = i * 30f;
			
			float xPos = (float)(Mathf.Cos(angle) * (-60f) - Mathf.Sin(angle) * (-50f) + 50f);
			float yPos = (float)(Mathf.Sin(angle) * (-60f) + Mathf.Cos(angle) * (-50f) + 50f);
			
			Vector3 newPos = new Vector3 (xPos, yPos, 0f);
			
			GameObject edge = new GameObject();
			edge.name = i.ToString();
			edge.transform.position = newPos;
			unvisitedStars.Add (edge); //This adds bounds to the voronoi diagram (forces it to be a circle);
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

	private void LinkPointToTris(int curPoint) //Send the current unvisited star as the seed point NEED TO DO EXT TO EXT TRIANGULATION THIS IS CLEARLY NOT WORKING
	{
		for(int i = 0; i < externalPoints.Count; ++i) //For all the external points
		{
			int nextPoint = i + 1; //Assign the next point

			if(nextPoint == externalPoints.Count) //If the next point
			{
				nextPoint = 0;
			}

			Vector3 lineCurToExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[i].transform.position); //Create a line between the external point and the unvisited star

			if(IsIllegalIntersection(i, curPoint, lineCurToExternal) == true)
			{
				continue;
			}

			Vector3 lineCurToNextExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[nextPoint].transform.position);

			if(IsIllegalIntersection(nextPoint, curPoint, lineCurToNextExternal) == true)
			{
				continue;
			}

			for(int j = 0; j < triangles.Count; ++j)
			{
				if(triangles[j].isInternal == true)
				{
					continue;
				}

				if(triangles[j].points.Contains(externalPoints[i]) == true && triangles[j].points.Contains(externalPoints[nextPoint]) == true)
				{
					GameObject pointA = externalPoints[i];
					GameObject pointB = externalPoints[nextPoint];
					GameObject pointC = unvisitedStars[curPoint];

					Triangle newTri = new Triangle();
					newTri.points.Add (pointA);
					newTri.points.Add (pointB);
					newTri.points.Add (pointC);
					newTri.lines.Add (MathsFunctions.ABCLineEquation (pointA.transform.position, pointB.transform.position));
					newTri.lines.Add (MathsFunctions.ABCLineEquation (pointB.transform.position, pointC.transform.position));
					newTri.lines.Add (MathsFunctions.ABCLineEquation (pointC.transform.position, pointA.transform.position));
					tempTri.Add (newTri);

					voronoiGenerator.DrawDebugLine(pointA.transform.position, pointB.transform.position, turnInfoScript.selkiesMaterial);
					voronoiGenerator.DrawDebugLine(pointC.transform.position, pointB.transform.position, turnInfoScript.selkiesMaterial);
					voronoiGenerator.DrawDebugLine(pointA.transform.position, pointC.transform.position, turnInfoScript.selkiesMaterial);

				}				  
			}
		}
	}

	public Vector2 CheckIfSharesSide(Triangle triOne, Triangle triTwo)
	{
		List<int> sharedPointsA = new List<int> ();
		List<int> sharedPointsB = new List<int> ();

		for(int i = 0; i < 3; ++i) //For all points in tri one
		{
			for(int j = 0; j < 3; ++j) //For all points in tri two
			{
				if(triOne.points[i] == triTwo.points[j]) //If tri one shares a point with tri two
				{
					sharedPointsA.Add (i); //Add it to the shared points list
					sharedPointsB.Add (j);
					break; //Skip to the next point in tri one
				}
			}
		}

		if(sharedPointsA.Count == 2) //If there are 2 shared point (i.e a line)
		{
			int lineA = 0, lineB = 0;

			if(sharedPointsA[0] == 0 && sharedPointsA[1] == 1|| sharedPointsA[0] == 1 && sharedPointsA[1] == 0)
			{
				lineA = 0;
			}
			if(sharedPointsA[0] == 2 && sharedPointsA[1] == 1 || sharedPointsA[0] == 1 && sharedPointsA[1] == 2)
			{
				lineA = 1;
			}
			if(sharedPointsA[0] == 0 && sharedPointsA[1] == 2 || sharedPointsA[0] == 2 && sharedPointsA[1] == 0)
			{
				lineA = 2;
			}
			if(sharedPointsB[0] == 0 && sharedPointsB[1] == 1 || sharedPointsB[0] == 1 && sharedPointsB[1] == 0)
			{
				lineB = 0;
			}
			if(sharedPointsB[0] == 2 && sharedPointsB[1] == 1 || sharedPointsB[0] == 1 && sharedPointsB[1] == 2)
			{
				lineB = 1;
			}
			if(sharedPointsB[0] == 0 && sharedPointsB[1] == 2 || sharedPointsB[0] == 2 && sharedPointsB[1] == 0)
			{
				lineB = 2;
			}

			return new Vector2(lineA, lineB);
		}

		return new Vector2 (-1, -1);
	}

	private void CheckForNonDelaunayTriangles()
	{
		List<Triangle> numberofnondelaunay = new List<Triangle>();

		for(int i = 0; i < triangles.Count; ++i)
		{
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}

				Vector2 sharedSides = CheckIfSharesSide(triangles[i], triangles[j]);

				if(sharedSides != new Vector2(-1, -1))
				{
					GameObject sharedPointA = new GameObject ();
					GameObject sharedPointB = new GameObject ();
					GameObject unsharedPointA = new GameObject ();
					GameObject unsharedPointB = new GameObject ();
					float angleAlpha = 0f, angleBeta = 0f;
					
					if(sharedSides.x == 0f)
					{
						sharedPointA = triangles[i].points[0];
						sharedPointB = triangles[i].points[1];
						unsharedPointA = triangles[i].points[2];
					}
					if(sharedSides.x == 1f)
					{
						sharedPointA = triangles[i].points[1];
						sharedPointB = triangles[i].points[2];
						unsharedPointA = triangles[i].points[0];
					}
					if(sharedSides.x == 2f)
					{
						sharedPointA = triangles[i].points[0];
						sharedPointB = triangles[i].points[2];
						unsharedPointA = triangles[i].points[1];
					}
					
					angleAlpha = MathsFunctions.AngleBetweenLineSegments (unsharedPointA.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);
					
					if(sharedSides.y == 0f)
					{
						unsharedPointB = triangulation.triangles[j].points[2];
					}
					if(sharedSides.y == 1f)
					{
						unsharedPointB = triangulation.triangles[j].points[0];
					}
					if(sharedSides.y == 2f)
					{
						unsharedPointB = triangulation.triangles[j].points[1];
					}
					
					angleBeta = MathsFunctions.AngleBetweenLineSegments (unsharedPointB.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);
					
					Vector3 sharedPointLine = triangles[i].lines[(int)sharedSides.x];
					Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position);
					Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
					
					if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //Is non-convex
					{
						continue;
					}
				
					if(angleBeta + angleAlpha > 180)
					{
						numberofnondelaunay.Add (triangles[i]);
					}
				}
			}
		}

		Debug.Log (numberofnondelaunay.Count);

		for(int i = 0; i < numberofnondelaunay.Count; ++i)
		{
			for(int j = 0; j < 3; ++j)
			{
				int a = j;
				int b = j + 1;

				if(j + 1 == 3)
				{
					b = 0;
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
		SortExternalPoints ();
		tempTri.Clear ();
	}

	private void SortExternalPoints()
	{
		for(int j = externalPoints.Count; j > 0; --j) //For all unvisited stars
		{
			bool swapsMade = false;
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float angleK = MathsFunctions.RotationOfLine(new Vector3(45f,45f,0), externalPoints[k].transform.position);
				float angleKMinus1 = MathsFunctions.RotationOfLine(new Vector3(45f,45f,0), externalPoints[k - 1].transform.position);

				if(angleK < angleKMinus1) //Sort smallest to largest
				{
					GameObject tempExternal = externalPoints[k];
					externalPoints[k] = externalPoints[k - 1];
					externalPoints[k - 1] = tempExternal;
					swapsMade = true;
				}
			}
			
			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
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
				
				Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(curToExt, triangles[j].lines[k]); //Get the intersection of the line with the current star to external point line
				
				if(MathsFunctions.CheckPointIsCloseToPoint(intersection, new Vector2(externalPoints[external].transform.position.x, externalPoints[external].transform.position.y))) //If the intersection is on the external point
				{
					continue; //This is not illegal so keep going
				}
				
				if(MathsFunctions.PointLiesOnLine(externalPoints[external].transform.position, unvisitedStars[point].transform.position, intersection) == true) //If the point lies elsewhere on the line
				{
					if(MathsFunctions.PointLiesOnLine(pointA, pointB, intersection) == true)
					{
						return true; //This IS an illegal intersection so return that, otherwise keep checking
					}
				}
			}
		}

		for(int i = 0; i < externalPoints.Count; ++i)
		{
			if(i == external)
			{
				continue;
			}

			if(MathsFunctions.PointLiesOnLine(externalPoints[i].transform.position, unvisitedStars[point].transform.position, externalPoints[external].transform.position) == true)
			{
				return false;
			}
		}
		
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
				if(triangles[j].isInternal == true)
				{
					continue;
				}

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

						tempAngle += MathsFunctions.AngleBetweenLineSegments(triangles[j].points[k].transform.position, triangles[j].points[a].transform.position, triangles[j].points[b].transform.position);

						break;
					}
				}
			}

			if(tempAngle > 359)
			{
				pointsToRemove.Add (i);
			}
		}

		for(int j = 0; j < triangles.Count; ++j) //For all triangles
		{
			bool isInternalTri = true; //Say its internal
			
			for(int k = 0; k < 3; ++k) //For all points in that triangle
			{
				if(externalPoints.Contains (triangles[j].points[k])) //If one of them is external
				{
					isInternalTri = false; //The tri isn't internal
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
		
		for(int i = 0; i < externalPoints.Count; ++i)
		{
			if(externalPoints[i] == null)
			{
				Debug.Log ("whu");
			}
		}
	}
}

public class Triangle
{
	public List<GameObject> points = new List<GameObject>(); //A, B, C points
	public List<Vector3> lines = new List<Vector3>(); //AB, BC, CA line equations
	public bool isInternal = false; //This allows me to ignore any triangles with no external points
}
