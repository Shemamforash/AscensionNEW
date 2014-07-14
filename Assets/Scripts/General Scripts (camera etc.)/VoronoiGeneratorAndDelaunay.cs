using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiGeneratorAndDelaunay : MasterScript 
{
	public Transform boundaryContainer;
	public Material humansMat, selkiesMat, nereidesMat;
	private List<VoronoiCellVertices> voronoiVertices = new List<VoronoiCellVertices> ();

	private class VoronoiCellVertices
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<float> vertexAngles = new List<float> ();
	}

	public float AngleBetweenLinesOfTri(Triangle tri, int anglePoint) //Anglepoint is the point at which the angle needs to be found (this works)
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

	public bool CheckIsDelaunay(int lineA, int lineB, Triangle triOne, Triangle triTwo)
	{
		GameObject sharedPointA = new GameObject ();
		GameObject sharedPointB = new GameObject ();
		GameObject unsharedPointA = new GameObject ();
		GameObject unsharedPointB = new GameObject ();
		float angleAlpha = 0f, angleBeta = 0f;

		if(lineA == 0)
		{
			sharedPointA = triOne.points[0];
			sharedPointB = triOne.points[1];
			unsharedPointA = triOne.points[2];
			angleAlpha = AngleBetweenLinesOfTri(triOne, 2);

		}
		if(lineA == 1)
		{
			sharedPointA = triOne.points[1];
			sharedPointB = triOne.points[2];
			unsharedPointA = triOne.points[0];
			angleAlpha = AngleBetweenLinesOfTri(triOne, 0);

		}
		if(lineA == 2)
		{
			sharedPointA = triOne.points[0];
			sharedPointB = triOne.points[2];
			unsharedPointA = triOne.points[1];
			angleAlpha = AngleBetweenLinesOfTri(triOne, 1);
		}

		if(lineB == 0)
		{
			unsharedPointB = triTwo.points[2];
			angleBeta = AngleBetweenLinesOfTri(triTwo, 2);
		}
		if(lineB == 1)
		{
			unsharedPointB = triTwo.points[0];
			angleBeta = AngleBetweenLinesOfTri(triTwo, 0);
		}
		if(lineB == 2)
		{
			unsharedPointB = triTwo.points[1];
			angleBeta = AngleBetweenLinesOfTri(triTwo, 1);
		}
		
		Vector3 sharedPointLine = triOne.lines[lineA];
		Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position);
		Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
		
		if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //Is non-convex
		{
			return true;
		}
		
		if(angleAlpha + angleBeta > 180f) //DUPLICATES ARE MADE HERE!!!
		{
			Triangle newTriA = new Triangle ();
			newTriA.points.Add (unsharedPointA);
			newTriA.points.Add (unsharedPointB);
			newTriA.points.Add (sharedPointA);
			newTriA.lines.Add (MathsFunctions.ABCLineEquation (newTriA.points[0].transform.position, newTriA.points[1].transform.position));
			newTriA.lines.Add (MathsFunctions.ABCLineEquation (newTriA.points[1].transform.position, newTriA.points[2].transform.position));
			newTriA.lines.Add (MathsFunctions.ABCLineEquation (newTriA.points[2].transform.position, newTriA.points[0].transform.position));
			
			Triangle newTriB = new Triangle ();
			newTriB.points.Add (unsharedPointA);
			newTriB.points.Add (unsharedPointB);
			newTriB.points.Add (sharedPointB);
			newTriB.lines.Add (MathsFunctions.ABCLineEquation (newTriB.points[0].transform.position, newTriB.points[1].transform.position));
			newTriB.lines.Add (MathsFunctions.ABCLineEquation (newTriB.points[1].transform.position, newTriB.points[2].transform.position));
			newTriB.lines.Add (MathsFunctions.ABCLineEquation (newTriB.points[2].transform.position, newTriB.points[0].transform.position));

			triangulation.triangles.Remove(triOne);
			triangulation.triangles.Remove(triTwo);
		
			for(int i = 0; i < triangulation.triangles.Count; ++i)
			{
				if(triangulation.triangles[i].points.Contains(newTriA.points[0]) == true && triangulation.triangles[i].points.Contains(newTriA.points[1]) == true && triangulation.triangles[i].points.Contains(newTriA.points[2]) == true)
				{
					Debug.Log ("containstrione");
				}
				if(triangulation.triangles[i].points.Contains(newTriB.points[0]) == true && triangulation.triangles[i].points.Contains(newTriB.points[1]) == true && triangulation.triangles[i].points.Contains(newTriB.points[2]) == true)
				{
					Debug.Log ("containstritwo");
				}
			}

			triangulation.triangles.Add (newTriB);
			triangulation.triangles.Add (newTriA);

			return false;
		}
		
		return true;
	}

	public bool TriangulationToDelaunay(Triangle tri)
	{
		for(int j = 0; j < triangulation.triangles.Count; ++j) //For all other triangles
		{
			if(tri.points.Contains(triangulation.triangles[j].points[0]) == true && tri.points.Contains(triangulation.triangles[j].points[1]) == true && tri.points.Contains(triangulation.triangles[j].points[2]) == true)
			{
				continue;
			}
			
			for(int k = 0; k < 3; ++k)
			{
				for(int l = 0; l < 3; ++l)
				{
					if(tri.lines[k] == triangulation.triangles[j].lines[l]) //If the current triangle shares a side with another triangle
					{
						if(CheckIsDelaunay(k, l, tri, triangulation.triangles[j]) == false)
						{
							return false;
						}
					}
				}
			}
		}
		
		return true;
	}
	
	public void CreateVoronoiCells()
	{
		triangulation.SimpleTriangulation ();

		for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
		{
			VoronoiCellVertices newCell = new VoronoiCellVertices(); //Create new voronoi cell

			Vector3 voronoiCentre = systemListConstructor.systemList[j].systemObject.transform.position;

			for(int i = 0; i < triangulation.triangles.Count; ++i) //For all systems
			{
				if(triangulation.triangles[i].points.Contains(systemListConstructor.systemList[j].systemObject))
				{
					bool looped = false;

					Vector3 systemA = triangulation.triangles[i].points[0].transform.position; //System A equals this system

					Vector3 systemB = triangulation.triangles[i].points[1].transform.position;

					Vector3 systemC = triangulation.triangles[i].points[2].transform.position;

					Vector3 lineAB = MathsFunctions.PerpendicularLineEquation(systemA, systemB);
					Vector3 lineBC = MathsFunctions.PerpendicularLineEquation(systemB, systemC);

					Vector3 voronoiVertex = MathsFunctions.IntersectionOfTwoLines(lineAB, lineBC);

					//Instantiate (systemInvasion.invasionQuad, voronoiVertex, Quaternion.identity);

					float angle = MathsFunctions.RotationOfLine(voronoiVertex, voronoiCentre);

					newCell.vertexAngles.Add(angle);
					newCell.vertices.Add (voronoiVertex);

					if(looped)
					{
						break;
					}
				}
			}

			voronoiVertices.Add (newCell);
		}

		for(int i = 0; i < voronoiVertices.Count; ++i)
		{

			for(int j = voronoiVertices[i].vertexAngles.Count; j > 0; --j) //For all unvisited stars
			{
				bool swapsMade = false;
				
				for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
				{
					if(voronoiVertices[i].vertexAngles[k] < voronoiVertices[i].vertexAngles[k - 1]) //Sort smallest to largest
					{
						float tempAng = voronoiVertices[i].vertexAngles[k];
						Vector3 tempPos = voronoiVertices[i].vertices[k];
						voronoiVertices[i].vertexAngles[k] = voronoiVertices[i].vertexAngles[k - 1];
						voronoiVertices[i].vertices[k] = voronoiVertices[i].vertices[k - 1];
						voronoiVertices[i].vertexAngles[k - 1] = tempAng;
						voronoiVertices[i].vertices[k - 1] = tempPos;
						swapsMade = true;
					}
				}
				
				if(swapsMade == false) //If no swaps made, list must have been sorted
				{
					break; //So break
				}
			}
		}

		for(int i = 0; i < voronoiVertices.Count; ++i)
		{
			bool looped = false;

			for(int j = 0; j < voronoiVertices[i].vertices.Count; ++j)
			{
				Vector3 start = voronoiVertices[i].vertices[j];

				Vector3 end = Vector3.zero;

				if(start == end)
				{
					continue;
				}

				if(j + 1 == voronoiVertices[i].vertices.Count)
				{
					end = voronoiVertices[i].vertices[0];
					looped = true;
				}
				else
				{
					end = voronoiVertices[i].vertices[j + 1];
				}

				DrawDebugLine(start, end, turnInfoScript.nereidesMaterial);

				if(looped == true)
				{
					break;
				}
			}
		}


		for(int i = 0; i < triangulation.triangles.Count; ++i)
		{
			bool looped = false;
			
			for(int j = 0; j < 3; ++j)
			{
				Vector3 start = triangulation.triangles[i].points[j].transform.position;
				
				Vector3 end = Vector3.zero;
				
				if(start == end)
				{
					continue;
				}
				
				if(j + 1 >= 3)
				{
					end = triangulation.triangles[i].points[0].transform.position;
					looped = true;
				}
				else
				{
					end = triangulation.triangles[i].points[j + 1].transform.position;
				}

				DrawDebugLine(start, end, turnInfoScript.selkiesMaterial);
				
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

		Vector3 vectRotation = new Vector3(0.0f, 0.0f, MathsFunctions.RotationOfLine(start, end) - 90f);

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
			width = 0.05f;
		}
		
		line.transform.localScale = new Vector3(width, distance, 0f);
	}
}