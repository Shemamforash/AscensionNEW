using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MasterScript 
{
	float t;
	private Vector2 startA, endA, startB, endB, mp1, mp2;
	int startNo;

	private List<GameObject> unvisitedSystems = new List<GameObject> ();
	private List<GameObject> firmSystems = new List<GameObject> ();
	private List<GameObject> visitedSystems = new List<GameObject> ();
	private List<GameObject> currentSystems = new List<GameObject>();
	
	void Start()
	{
		t = 0f;
		startNo = 0;
	}

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			UpdateConnections (i);
		}

		/*
		t += Time.deltaTime;

		if(t >= 0.3f)
		{
			if(startNo >= systemListConstructor.systemList.Count)
			{
				startNo = 0;
			}

			for(int i = startNo; i < startNo + 5; ++i)
			{
				UpdateConnections (i);
			}

			t = 0f;

			startNo += 5;
		}
		*/
	}

	private void CheckVisitedSystems()
	{
		bool foundSystem = false;
		
		for(int i = 0; i < visitedSystems.Count; ++i) //For all current systems
		{
			firmSystems.Add (visitedSystems[i]);
			
			int sys = RefreshCurrentSystem (visitedSystems [i]);
			
			for(int j = 0; j < systemListConstructor.systemList[sys].permanentConnections.Count; ++j) //Check the systems permanent connections
			{
				if(visitedSystems.Contains(systemListConstructor.systemList[sys].permanentConnections[j]) || firmSystems.Contains(systemListConstructor.systemList[sys].permanentConnections[j])) //If the system is already in checked systems, ignore it
				{
					continue;
				}
				
				currentSystems.Add (systemListConstructor.systemList[sys].permanentConnections[j]);
				unvisitedSystems.Remove (systemListConstructor.systemList[sys].permanentConnections[j]);
				
				foundSystem = true;
			}
		}

		visitedSystems = currentSystems;
		currentSystems.Clear ();

		if(foundSystem == true)
		{
			CheckVisitedSystems();
		}
	}

	private void CheckForGraphComplete()
	{
		unvisitedSystems.Clear ();
		visitedSystems.Clear ();
		firmSystems.Clear ();
		currentSystems.Clear ();

		for(int i = 1; i < systemListConstructor.systemList.Count; ++i)
		{
			unvisitedSystems.Add (systemListConstructor.systemList[i].systemObject);
		}

		visitedSystems.Add (systemListConstructor.systemList[0].systemObject);

		CheckVisitedSystems ();

		if(firmSystems.Count != systemListConstructor.systemList.Count)
		{
			float distance = 10000f;
			int sysToJoinA = -1, sysToJoinB = -1;

			for(int i = 0; i < firmSystems.Count; ++i)
			{
				for(int j = 0; j < unvisitedSystems.Count; ++j)
				{
					float tempDistance = Vector3.Distance (firmSystems[i].transform.position, unvisitedSystems[j].transform.position);

					if(tempDistance < distance)
					{
						if(mapConstructor.TestForIntersection(firmSystems[i].transform.position, unvisitedSystems[j].transform.position, false) == false)
						{
							distance = tempDistance;
							sysToJoinA = i;
							sysToJoinB = j;
						}
					}
				}
			}

			Debug.Log (firmSystems.Count + " | " + unvisitedSystems.Count);

			sysToJoinA = RefreshCurrentSystem(firmSystems[sysToJoinA]);
			sysToJoinB = RefreshCurrentSystem(unvisitedSystems[sysToJoinB]);

			systemListConstructor.systemList[sysToJoinA].permanentConnections.Add (systemListConstructor.systemList[sysToJoinB].systemObject);
			systemListConstructor.systemList[sysToJoinA].numberOfConnections = systemListConstructor.systemList[sysToJoinA].permanentConnections.Count;
			systemListConstructor.systemList[sysToJoinB].permanentConnections.Add (systemListConstructor.systemList[sysToJoinA].systemObject);
			systemListConstructor.systemList[sysToJoinB].numberOfConnections = systemListConstructor.systemList[sysToJoinB].permanentConnections.Count;
			CreateNewLine(sysToJoinA);
			CreateNewLine(sysToJoinB);

			CheckForGraphComplete();
		}
	}

	private void UpdateConnections(int system)
	{
		startA = new Vector2(systemListConstructor.systemList[system].systemObject.transform.position.x, systemListConstructor.systemList[system].systemObject.transform.position.y);
		
		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			endA = new Vector2(systemListConstructor.systemList[system].permanentConnections[i].transform.position.x, systemListConstructor.systemList[system].permanentConnections[i].transform.position.y);
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
			{
				startB = new Vector2(systemListConstructor.systemList[j].systemObject.transform.position.x, systemListConstructor.systemList[j].systemObject.transform.position.y);
				
				for(int k = 0; k < systemListConstructor.systemList[j].permanentConnections.Count; ++k)
				{
					endB = new Vector2(systemListConstructor.systemList[j].permanentConnections[k].transform.position.x, systemListConstructor.systemList[j].permanentConnections[k].transform.position.y);
					
					if(startA == startB || startA == endB || endA == startB || endA == endB)
					{
						continue;
					}

					float distanceA = Vector2.Distance(startA, endA);
					float distanceB = Vector2.Distance(startB, endB);
					mp1 = (startA + endA) / 2;
					mp2 = (startB + endB) / 2;
					float distanceC = Vector2.Distance(mp1, mp2);

					if(distanceC * 2 > distanceA + distanceB)
					{
						continue;
					}

					if(LineIntersection(startA, endA, startB, endB) == true)
					{
						if(distanceA > distanceB)
						{
							if(i >= systemListConstructor.systemList[system].permanentConnections.Count)
							{
								continue;
							}

							GameObject target = systemListConstructor.systemList[system].permanentConnections[i];
							
							ReconnectSystems(systemListConstructor.systemList[system].systemObject, target);
							ReconnectSystems(target, systemListConstructor.systemList[system].systemObject);
							CheckForGraphComplete();
						}

						if(distanceB >= distanceA)
						{
							continue;
						}
					}
				}
			}
		}
	}

	private bool LineIntersection (Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		float M1 = (b.y - a.y) / (b.x - a.x);
		float C1 = a.y - (M1 * a.x);
		
		float M2 = (d.y - c.y) / (d.x - c.x);
		float C2 = c.y - (M2 * c.x);
		
		if(M2 == M1)
		{
			return false;
		}
		
		float xIntersect = (C2 - C1) / (M1 - M2);

		if(xIntersect > 95f || xIntersect < 0f)
		{
			return false;
		}

		float yOne = (M1 * xIntersect) + C1;
		float yTwo = (M2 * xIntersect) + C2;
		
		if(yOne == yTwo /*|| Mathf.Abs((yOne - yTwo) / 2) <= 2.0f*/)
		{
			if(yOne > 95f || yOne < 0f)
			{
				return false;
			}

			if(xIntersect < Mathf.Max (a.x, b.x) && xIntersect > Mathf.Min (a.x, b.x))
			{
				if(yOne < Mathf.Max (a.y, b.y) && yOne > Mathf.Min (a.y, b.y))
				{
					if(xIntersect < Mathf.Max (c.x, d.x) && xIntersect > Mathf.Min (c.x, d.x))
					{
						if(yOne < Mathf.Max (c.y, d.y) && yOne > Mathf.Min (c.y, d.y))
						{
							return true;
						}
					}
				}
			}
		}
		
		return false;
	}

	private void CreateNewLine(int system)
	{
		lineRenderScript = systemListConstructor.systemList[system].systemObject.GetComponent<LineRenderScript>();
		
		ConnectorLine newLine = new ConnectorLine ();
		
		GameObject clone = NGUITools.AddChild(lineRenderScript.connectorLineContainer.gameObject, lineRenderScript.line);
		
		newLine.thisLine = clone;
		
		newLine.sprite = newLine.thisLine.transform.Find ("Sprite").GetComponent<UISprite>();
		
		newLine.widget = newLine.thisLine.GetComponent<UIWidget>();
		
		lineRenderScript.connectorLines.Add (newLine);
	}
	
	public void ReconnectSystems(GameObject systemA, GameObject systemB)
	{
		int thisSystem = RefreshCurrentSystem (systemA);
		
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i) //For all permanent connections in target
		{
			if(systemListConstructor.systemList[thisSystem].permanentConnections[i] == systemB) //If connection equals original system
			{
				float distance = 100f; //Set max distance
				int newConnection = -1;
				
				for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all other systems
				{
					if(systemListConstructor.systemList[j].systemObject == systemA) //If system equals this system, continue
					{
						continue;
					}
					if(systemListConstructor.systemList[j].numberOfConnections > 7 && systemListConstructor.systemList[thisSystem].numberOfConnections > 0)
					{
						continue;
					}
					
					if(mapConstructor.TestForIntersection(systemA.transform.position, systemListConstructor.systemList[j].systemObject.transform.position, false) == false) //If there is no intersection between this system and other system
					{
						float tempDistance = Vector3.Distance (systemA.transform.position, systemListConstructor.systemList[j].systemObject.transform.position);
						
						if(tempDistance < distance) //Test if distance is less than current max distance
						{
							distance = tempDistance; //If it is
							newConnection = j; //Set this system as the new connection
						}
					}
				}
				
				if(newConnection != -1) //If the new connection is not -1
				{
					systemListConstructor.systemList[thisSystem].permanentConnections[i] = systemListConstructor.systemList[newConnection].systemObject; //The connection that was system b is now the new connection
					systemListConstructor.systemList[newConnection].permanentConnections.Add(systemListConstructor.systemList[thisSystem].systemObject); //The new connection is now connected to this system
					
					CreateNewLine(newConnection);
					
					systemListConstructor.systemList[thisSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
					systemListConstructor.systemList[newConnection].numberOfConnections = systemListConstructor.systemList[newConnection].permanentConnections.Count;
				}
				
				if(newConnection == -1)
				{
					int tempSystem = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);
					
					lineRenderScript = systemListConstructor.systemList[tempSystem].systemObject.GetComponent<LineRenderScript>();
					
					for(int j = 0; j < systemListConstructor.systemList[tempSystem].permanentConnections.Count; ++j)
					{
						if(systemListConstructor.systemList[tempSystem].permanentConnections[j] = systemListConstructor.systemList[thisSystem].systemObject)
						{
							systemListConstructor.systemList[tempSystem].permanentConnections.RemoveAt (j);
							NGUITools.Destroy(lineRenderScript.connectorLines[j].thisLine);
							lineRenderScript.connectorLines.RemoveAt (j);
							systemListConstructor.systemList[tempSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
						}
					}
					
					lineRenderScript = systemListConstructor.systemList[thisSystem].systemObject.GetComponent<LineRenderScript>();
					
					systemListConstructor.systemList[thisSystem].permanentConnections.RemoveAt (i);
					NGUITools.Destroy(lineRenderScript.connectorLines[i].thisLine);
					lineRenderScript.connectorLines.RemoveAt (i);
					
					systemListConstructor.systemList[thisSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
				}
				
				break;
			}
		}
	}
}
