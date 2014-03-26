using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private LineRenderer lineRenderer;
	private List<Vector3> vertexPoints = new List<Vector3>();
	private Color white;
	private float width = 0.2f;
	private int numberOfPoints = 20, system;

	public void SetVertexPoints()
	{
		lineRenderer = gameObject.GetComponent<LineRenderer> ();
		white = Color.red;

		vertexPoints.Clear();
		system = RefreshCurrentSystem(GameObject.Find (playerTurnScript.homeSystem));
		float radius = 1000;
		float totalAngle = 0;

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			Vector3 midpoint = (systemListConstructor.systemList[system].systemObject.transform.position + systemListConstructor.systemList[system].permanentConnections[i].transform.position) / 2;
			float distance = Vector3.Distance(systemListConstructor.systemList[system].systemObject.transform.position, midpoint);

			if(i + 1 < systemListConstructor.systemList[system].permanentConnections.Count)
			{
				totalAngle += Vector3.Angle (systemListConstructor.systemList[system].permanentConnections[i].transform.position, systemListConstructor.systemList[system].permanentConnections[i + 1].transform.position);
			}

			if(radius > distance)
			{
				radius = distance;
			}

			vertexPoints.Add(midpoint);
		}

		float angle = 0;

		if(totalAngle < 180f && systemListConstructor.systemList[system].permanentConnections.Count > 1)
		{
			for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
			{
				angle += Vector3.Angle(Vector3.zero, systemListConstructor.systemList[system].permanentConnections[i].transform.position);
			}

			angle = - (angle / systemListConstructor.systemList[system].permanentConnections.Count);
		}

		if(systemListConstructor.systemList[system].permanentConnections.Count == 1)
		{
			angle = -Vector3.Angle(Vector3.zero, systemListConstructor.systemList[system].permanentConnections[0].transform.position);
		}
			
		float angleVPos = angle - 90f;
		float angleVNeg = angle + 90f;

		Vector3 perpendicularVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0.0f);

		Vector3 perpVPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleVPos), Mathf.Sin(Mathf.Deg2Rad * angleVPos), 0.0f);
		Vector3 perpVNeg = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleVNeg), Mathf.Sin(Mathf.Deg2Rad * angleVNeg), 0.0f);

		Debug.Log (perpVPos.normalized * radius);

		vertexPoints.Add (systemListConstructor.systemList[system].systemObject.transform.position + (perpVPos.normalized * radius));
		vertexPoints.Add (systemListConstructor.systemList[system].systemObject.transform.position + (perpendicularVector.normalized * radius));
		vertexPoints.Add (systemListConstructor.systemList[system].systemObject.transform.position + (perpVNeg.normalized * radius));

		lineRenderer.SetColors (white, white);
		lineRenderer.SetWidth (width, width);

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
				pointStep = 1f / (numberOfPoints - 1f);
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
