using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangulation : MasterScript 
{
	public List<Triangle> triangles = new List<Triangle> ();
	private List<Triangle> tempTri = new List<Triangle>();
	private List<GameObject> unvisitedStars = new List<GameObject> ();
	private List<GameObject> externalPoints = new List<GameObject> ();
	private Triangle activeTriangle;

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
		
		CheckForDupes ();
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
			
			float xPos = (float)(Mathf.Cos(angle) * (-60f) - Mathf.Sin(angle) * (-45f) + 45f);
			float yPos = (float)(Mathf.Sin(angle) * (-60f) + Mathf.Cos(angle) * (-45f) + 45f);
			
			Vector3 newPos = new Vector3 (xPos, yPos, 0f);
			
			GameObject edge= new GameObject();
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

	private void LinkPointToTris(int curPoint) //Send the current unvisited star as the seed point
	{
		for(int i = 0; i < externalPoints.Count; ++i) //For all the external points
		{
			Vector3 lineCurToExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[i].transform.position); //Create a line between the external point and the unvisited star
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
							if(IsIllegalIntersection(k, curPoint, MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[k].transform.position)) == false)
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
									newTri.lines.Add (MathsFunctions.ABCLineEquation (pointA.transform.position, pointB.transform.position));
									newTri.lines.Add (MathsFunctions.ABCLineEquation (pointB.transform.position, pointC.transform.position));
									newTri.lines.Add (MathsFunctions.ABCLineEquation (pointC.transform.position, pointA.transform.position));
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
		int lastTri = triangles.Count - 1;
		
		for(int i = 0; i < tempTri.Count; ++i)
		{
			triangles.Add (tempTri[i]);
		}
		
		for(int i = lastTri; i < triangles.Count; ++i)
		{
			bool isDelaunay = false;
			
			//if(voronoiGenerator.TriangulationToDelaunay(triangles[i]) == false)
			//{
				//i = lastTri - 1;
			//}
		}
		
		externalPoints.Add (unvisitedStars [curPoint]);
		CheckInteriorPoints ();
		tempTri.Clear ();
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
					Debug.Log (triangles[i].points[0] + " | " + triangles[i].points[1] + " | " + triangles[i].points[2]);
					++dupes;
				}
			}
		}
		
		Debug.Log (dupes);
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
						int line = 0;
						float angle = new float();
						
						if(k == 0)
						{
							line = 1;
						}
						if(k == 1)
						{
							line = 2;
						}
						if(k == 2)
						{
							line = 0;
						}

						angle = voronoiGenerator.AngleBetweenLinesOfTri(triangles[j], line);

						//Debug.Log (angle);

						tempAngle += angle;
					}
				}
			}

			Debug.Log (tempAngle);

			if(tempAngle > 360)
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
}

public class Triangle
{
	public List<GameObject> points = new List<GameObject>(); //A, B, C points
	public List<Vector3> lines = new List<Vector3>(); //AB, BC, CA line equations
	public bool isInternal = false; //This allows me to ignore any triangles with no external points
}
