using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpireBoundaries : MasterScript 
{
	private GameObject[] boundRings;
	public GameObject blankCircle;
	public Transform boundaryContainer;
	private float radius;
	public Material humansMat, selkiesMat, nereidesMat;

	public void SetArrSize()
	{
		boundRings = new GameObject[systemListConstructor.systemList.Count];
	}

	public void CalculateRadius(int system)
	{
		float tempRadius = 100f;

		for(int j = 0; j < systemListConstructor.systemList[system].permanentConnections.Count; ++j)
		{
			float tempDistance = Vector3.Distance(systemListConstructor.systemList[system].systemObject.transform.position, systemListConstructor.systemList[system].permanentConnections[j].transform.position);

			if(tempDistance < tempRadius)
			{
				tempRadius = tempDistance;
			}
		}

		radius = tempRadius;
	}

	private void CreateCircle (int system, Material material)
	{
		if(boundRings[system] != null)
		{
			Destroy(boundRings[system]);
		}

		CalculateRadius (system);

		GameObject temp = (GameObject)Instantiate (blankCircle, systemListConstructor.systemList [system].systemObject.transform.position, Quaternion.identity);

		temp.transform.parent = boundaryContainer;

		temp.transform.localScale = new Vector3 (radius, radius, radius);

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
					Destroy(boundRings[i]);
				}
				break;
			}
		}
	}
}
