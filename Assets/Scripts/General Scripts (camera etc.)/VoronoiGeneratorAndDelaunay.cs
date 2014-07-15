using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiGeneratorAndDelaunay : MasterScript 
{
	public Transform boundaryContainer;
	public Material humansMat, selkiesMat, nereidesMat;
	private List<VoronoiCellVertices> voronoiVertices = new List<VoronoiCellVertices> ();
	private int flips = 0;

	private class VoronoiCellVertices
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<float> vertexAngles = new List<float> ();
	}

	public bool CheckIsDelaunay(int triOne, int triTwo)
	{
		Vector2 sharedSides = triangulation.CheckIfSharesSide(triangulation.triangles[triOne], triangulation.triangles[triTwo]);
		
		if(sharedSides != new Vector2(-1f, -1f))
		{
			GameObject sharedPointA = new GameObject ();
			GameObject sharedPointB = new GameObject ();
			GameObject unsharedPointA = new GameObject ();
			GameObject unsharedPointB = new GameObject ();
			float angleAlpha = 0f, angleBeta = 0f;

			if(sharedSides.x == 0f)
			{
				sharedPointA = triangulation.triangles[triOne].points[0];
				sharedPointB = triangulation.triangles[triOne].points[1];
				unsharedPointA = triangulation.triangles[triOne].points[2];
			}
			if(sharedSides.x == 1f)
			{
				sharedPointA = triangulation.triangles[triOne].points[1];
				sharedPointB = triangulation.triangles[triOne].points[2];
				unsharedPointA = triangulation.triangles[triOne].points[0];
			}
			if(sharedSides.x == 2f)
			{
				sharedPointA = triangulation.triangles[triOne].points[0];
				sharedPointB = triangulation.triangles[triOne].points[2];
				unsharedPointA = triangulation.triangles[triOne].points[1];
			}

			angleAlpha = MathsFunctions.AngleBetweenLineSegments (unsharedPointA.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);

			if(sharedSides.y == 0f)
			{
				unsharedPointB = triangulation.triangles[triTwo].points[2];
			}
			if(sharedSides.y == 1f)
			{
				unsharedPointB = triangulation.triangles[triTwo].points[0];
			}
			if(sharedSides.x == 2f)
			{
				unsharedPointB = triangulation.triangles[triTwo].points[1];
			}

			angleBeta = MathsFunctions.AngleBetweenLineSegments (unsharedPointB.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);

			float angleOne = MathsFunctions.AngleBetweenLineSegments (sharedPointA.transform.position, unsharedPointA.transform.position, unsharedPointB.transform.position);
			float angleTwo = MathsFunctions.AngleBetweenLineSegments (sharedPointB.transform.position, unsharedPointA.transform.position, unsharedPointB.transform.position);

			Vector3 sharedPointLine = triangulation.triangles[triOne].lines[(int)sharedSides.x];
			Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position);
			Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
			
			if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //Is non-convex
			{
				//Instantiate(systemInvasion.invasionQuad, intersection, Quaternion.identity);
				//DrawDebugLine(unsharedPointA.transform.position, unsharedPointB.transform.position, turnInfoScript.humansMaterial);
				return true;
			}

			float potential = angleOne +angleTwo;

			if(angleAlpha + angleBeta > 180f) //DUPLICATES ARE MADE HERE!!!
			{
				Debug.Log ("bacon");

				triangulation.triangles[triOne].points[0] = unsharedPointA;
				triangulation.triangles[triOne].points[1] = unsharedPointB;
				triangulation.triangles[triOne].points[2] = sharedPointA;
				triangulation.triangles[triOne].lines[0] = MathsFunctions.ABCLineEquation (triangulation.triangles[triOne].points[0].transform.position, triangulation.triangles[triOne].points[1].transform.position);
				triangulation.triangles[triOne].lines[1] = MathsFunctions.ABCLineEquation (triangulation.triangles[triOne].points[1].transform.position, triangulation.triangles[triOne].points[2].transform.position);
				triangulation.triangles[triOne].lines[2] = MathsFunctions.ABCLineEquation (triangulation.triangles[triOne].points[2].transform.position, triangulation.triangles[triOne].points[0].transform.position);
				
				triangulation.triangles[triTwo].points[0] = unsharedPointA;
				triangulation.triangles[triTwo].points[1] = unsharedPointB;
				triangulation.triangles[triTwo].points[2] = sharedPointB;
				triangulation.triangles[triTwo].lines[0] = MathsFunctions.ABCLineEquation (triangulation.triangles[triTwo].points[0].transform.position, triangulation.triangles[triTwo].points[1].transform.position);
				triangulation.triangles[triTwo].lines[1] = MathsFunctions.ABCLineEquation (triangulation.triangles[triTwo].points[1].transform.position, triangulation.triangles[triTwo].points[2].transform.position);
				triangulation.triangles[triTwo].lines[2] = MathsFunctions.ABCLineEquation (triangulation.triangles[triTwo].points[2].transform.position, triangulation.triangles[triTwo].points[0].transform.position);

				++flips;

				return false;
			}
		}

		return true;
	}

	public bool TriangulationToDelaunay()
	{
		bool isDelaunay = true;
		flips = 0;

		for(int i = 0; i < triangulation.triangles.Count; ++i)
		{
			for(int j = 0; j < triangulation.triangles.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}
		
				if(CheckIsDelaunay(i, j) == false)
				{
					return false;//i = -1; j = 0;
					break;
				}
			}
		}

		if(flips > 0)
		{
			return false;
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

				if(start == end)
				{
					continue;
				}

				//DrawDebugLine(start, end, turnInfoScript.nereidesMaterial);

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

	public void DrawDebugLine(Vector3 start, Vector3 end, Material mat)
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
			width = 0.2f;
		}
		
		line.transform.localScale = new Vector3(width, distance, 0f);
	}
}