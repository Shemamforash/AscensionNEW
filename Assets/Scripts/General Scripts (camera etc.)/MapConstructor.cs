using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapConstructor : MasterScript
{
	public List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	public float distanceMax;
	public bool connected = false;
	public List<int> allIntersections = new List<int>();

	public bool TestForIntersection(Vector3 thisSystem, Vector3 targetSystem, bool includeIntersections)
	{
		allIntersections.Clear ();
		bool intersects = false;

		float A1 = targetSystem.y - thisSystem.y;
		float B1 = thisSystem.x - targetSystem.x;
		float C1 = (A1 * thisSystem.x) + (B1 * thisSystem.y);

		for (int i = 0; i < coordinateList.Count; ++i) 
		{
			float A2 = coordinateList [i].systemB.y - coordinateList [i].systemA.y;
			float B2 = coordinateList [i].systemA.x - coordinateList [i].systemB.x;
			float C2 = (A2 * coordinateList [i].systemA.x) + (B2 * coordinateList [i].systemA.y);

			float determinant = (A1 * B2) - (A2 * B1);

			if (determinant == 0.0f) 
			{
				continue;
			}

			float x = (B2 * C1 - B1 * C2) / determinant;
			float y = (A1 * C2 - A2 * C1) / determinant;

			Vector2 intersection = new Vector2(x, y);

			if(PointLiesOnLine(thisSystem, targetSystem, intersection))
			{
				if(PointLiesOnLine(coordinateList[i].systemA, coordinateList[i].systemB, intersection))
				{
					if(includeIntersections == true)
					{
						allIntersections.Add (i);
					}
					intersects = true;
				}
			}
		}

		if(intersects == true)
		{
			return true;
		}

		return false;
	}

	public bool TestForAngle(GameObject thisSystem, GameObject targetSystem)
	{
		int current = RefreshCurrentSystem (thisSystem);
		int target = RefreshCurrentSystem (targetSystem);

		Vector3 directionVector1 = targetSystem.transform.position - thisSystem.transform.position;

		for(int i = 0; i < systemListConstructor.systemList[current].permanentConnections.Count; ++i)
		{
			Vector3 directionVector2 = systemListConstructor.systemList[current].permanentConnections[i].transform.position - thisSystem.transform.position;

			float angle = Vector3.Angle(directionVector1, directionVector2);

			if(angle <= 30.0f)
			{
				return false;
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[target].permanentConnections.Count; ++i)
		{
			Vector3 directionVector2 = systemListConstructor.systemList[target].permanentConnections[i].transform.position - targetSystem.transform.position;
			
			float angle = Vector3.Angle(directionVector1, directionVector2);
			
			if(angle <= 30.0f)
			{
				return false;
			}
		}

		return true;
	}

	public bool PointLiesOnLine(Vector3 systemA, Vector3 systemB, Vector2 intersection)
	{
		Vector2 point1 = new Vector2(systemA.x, systemA.y);
		Vector2 point2 = new Vector2(systemB.x, systemB.y);
		Vector2 lineVector = point2.normalized - point1.normalized;
		Vector2 pointVector = intersection.normalized - point1.normalized;
		
		float dotProduct = Vector2.Dot (pointVector, lineVector);
		
		if(dotProduct > 0)
		{
			if(pointVector.magnitude < lineVector.magnitude)
			{
				return true;
			}
		}

		return false;
	}
	
	public void DrawMinimumSpanningTree() //Working
	{
		List<GameObject> linkedSystems = new List<GameObject> (); //Create empty list of linkedsystems
		List<GameObject> unlinkedSystems = new List<GameObject> ();
		
		linkedSystems.Add (systemListConstructor.systemList [0].systemObject); //Add initial system to list

		for (int i = 1; i < systemListConstructor.systemList.Count; ++i) 
		{
			unlinkedSystems.Add (systemListConstructor.systemList[i].systemObject);
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			int nearestSystem = -1;
			int thisSystem = -1;
			bool toLink = false;
			
			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				int system = systemListConstructor.RefreshCurrentSystemA(linkedSystems[j]);
				
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(TestForIntersection(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position, false) == true && TestForAngle(linkedSystems[j], unlinkedSystems[k]) == false)
					{
						continue;
					}
					
					float distance = Vector3.Distance(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position);
					
					if(distance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = distance;
						nearestSystem = systemListConstructor.RefreshCurrentSystemA(unlinkedSystems[k]);
						thisSystem = system;
						toLink = true;
					}
				}
			}
			
			if(toLink == true)
			{
				AddPermanentSystem(thisSystem, nearestSystem);

				linkedSystems.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add nearest unlinked system to linkedsystems list
				unlinkedSystems.Remove (systemListConstructor.systemList[nearestSystem].systemObject);
			}
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].permanentConnections.Count;

			SortConnectionsByAngle(i);
		}

		AssignMaximumConnections ();
	}

	private void SortConnectionsByAngle (int i)
	{
		Vector3 zeroVector = systemListConstructor.systemList [i].permanentConnections [0].transform.position;

		for(int j = systemListConstructor.systemList[i].permanentConnections.Count; j > 0; --j)
		{
			bool swapsMade = false;

			for(int k = 2; k < j; ++k)
			{
				float angleA = Vector3.Angle(systemListConstructor.systemList[i].permanentConnections[k].transform.position, zeroVector);

				if(Vector3.Cross(systemListConstructor.systemList[i].permanentConnections[k].transform.position, zeroVector).y < 0)
				{
					angleA = -angleA;
				}

				float angleB = Vector3.Angle(systemListConstructor.systemList[i].permanentConnections[k - 1].transform.position, zeroVector);

				if(Vector3.Cross(systemListConstructor.systemList[i].permanentConnections[k - 1].transform.position, zeroVector).y < 0)
				{
					angleB = -angleB;
				}

				if(angleB > angleA)
				{
					GameObject temp = systemListConstructor.systemList[i].permanentConnections[k];
					systemListConstructor.systemList[i].permanentConnections[k] = systemListConstructor.systemList[i].permanentConnections[k - 1];
					systemListConstructor.systemList[i].permanentConnections[k - 1] = temp;
					swapsMade = true;
				}
			}

			if(swapsMade == false)
			{
				break;
			}
		}
	}

	private void AddPermanentSystem(int thisSystem, int nearestSystem)
	{
		systemListConstructor.systemList[thisSystem].permanentConnections.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add target system to current systems permanent connections
		systemListConstructor.systemList [nearestSystem].permanentConnections.Add (systemListConstructor.systemList [thisSystem].systemObject);
		
		ConnectionCoordinates connection = new ConnectionCoordinates ();

		connection.systemOne = systemListConstructor.systemList [thisSystem].systemObject;
		connection.systemTwo = systemListConstructor.systemList [nearestSystem].systemObject;
		connection.systemA = systemListConstructor.systemList [thisSystem].systemObject.transform.position;
		connection.systemB = systemListConstructor.systemList [nearestSystem].systemObject.transform.position;
		
		coordinateList.Add (connection);

		bool thisSystemRemove = false;
		bool targetSystemRemove = false;

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].tempConnections.Count; ++i)
		{
			if(systemListConstructor.systemList[thisSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[nearestSystem].systemObject && thisSystemRemove == false)
			{
				systemListConstructor.systemList[thisSystem].tempConnections.RemoveAt(i);
				thisSystemRemove = true;
				continue;
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[nearestSystem].tempConnections.Count; ++i)
		{
			if(systemListConstructor.systemList[nearestSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[thisSystem].systemObject && targetSystemRemove == false)
			{
				systemListConstructor.systemList[nearestSystem].tempConnections.RemoveAt (i);
				targetSystemRemove = true;
				continue;
			}
		}
	}
	
	private int WeightedConnectionFinder(int randomInt)
	{
		if(randomInt < 20)
		{
			return 1;
		}
		if(randomInt >= 20 && randomInt < 30)
		{
			return 2;
		}
		if(randomInt >= 40 && randomInt < 60)
		{
			return 3;
		}
		if(randomInt >= 60 && randomInt < 80)
		{
			return 4;
		}
		if(randomInt >= 80 && randomInt < 90)
		{
			return 5;
		}
		if(randomInt >= 90)
		{
			return 6;
		}
		
		return 0;
	}
	
	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			int randomInt = WeightedConnectionFinder(Random.Range (0,100)); //Generate number
			
			if(systemListConstructor.systemList[i].systemName == "Samael" || systemListConstructor.systemList[i].systemName == "Midgard" || systemListConstructor.systemList[i].systemName == "Nepthys")
			{
				randomInt = WeightedConnectionFinder(Random.Range (49, 99));
			}
			
			if(systemListConstructor.systemList[i].numberOfConnections < randomInt) //If number of connections is lower than number
			{
				systemListConstructor.systemList[i].numberOfConnections = randomInt; //Increase number of connections
			}
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all systems
			{
				if(i == j)
				{
					continue;
				}
				
				bool skipSystem = false;
				
				float distance = Vector3.Distance (systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position); //Assign distance
				
				for(int k = 0; k < systemListConstructor.systemList[i].permanentConnections.Count; ++k) //For all of this systems permanent connections
				{
					if(systemListConstructor.systemList[i].permanentConnections[k] == systemListConstructor.systemList[j].systemObject) //If target systems is already in permanent connections, continue;
					{
						skipSystem = true;
						break;
					}
				}
				
				if(skipSystem == true)
				{
					continue;
				}

				if(distance < distanceMax)
				{
					Node nearbySystem = new Node();
						
					nearbySystem.targetSystem = systemListConstructor.systemList[j].systemObject;
					nearbySystem.targetDistance = distance;
						
					systemListConstructor.systemList[i].tempConnections.Add (nearbySystem);
				}

			}
		}

		SortNearestConnections();
		ConnectSystems();
	}
	
	private void SortNearestConnections()
	{
		GameObject tempObject;
		float tempFloat;
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			for(int j = systemListConstructor.systemList[i].tempConnections.Count - 1; j >= 0; --j)
			{
				bool swaps = false;
				
				for(int k = 1; k <= j; ++k)
				{
					tempObject = systemListConstructor.systemList[i].tempConnections[k-1].targetSystem;
					tempFloat = systemListConstructor.systemList[i].tempConnections[k-1].targetDistance;
					
					if(systemListConstructor.systemList[i].tempConnections[k-1].targetDistance > systemListConstructor.systemList[i].tempConnections[k].targetDistance)
					{
						systemListConstructor.systemList[i].tempConnections[k-1].targetSystem = systemListConstructor.systemList[i].tempConnections[k].targetSystem;
						systemListConstructor.systemList[i].tempConnections[k-1].targetDistance = systemListConstructor.systemList[i].tempConnections[k].targetDistance;
						
						systemListConstructor.systemList[i].tempConnections[k].targetSystem = tempObject;
						systemListConstructor.systemList[i].tempConnections[k].targetDistance = tempFloat;
						
						swaps = true;
					}
				}
				
				if(swaps == false)
				{
					break;
				}
			}
		}
	}
	
	private void ConnectSystems()
	{
		for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all systems
		{
			for(int l = 0; l < systemListConstructor.systemList[j].tempConnections.Count; ++l) //For all tempconnections
			{
				if(systemListConstructor.systemList[j].numberOfConnections == systemListConstructor.systemList[j].permanentConnections.Count)
				{
					break;
				}

				int targetSystem = systemListConstructor.RefreshCurrentSystemA(systemListConstructor.systemList[j].tempConnections[l].targetSystem); //Get target system

				if(systemListConstructor.systemList[targetSystem].numberOfConnections == systemListConstructor.systemList[targetSystem].permanentConnections.Count)
				{
					continue;
				}

				if(TestForIntersection(systemListConstructor.systemList[j].systemObject.transform.position, systemListConstructor.systemList[targetSystem].systemObject.transform.position, false) == false)
				{
					if(TestForAngle(systemListConstructor.systemList[j].systemObject, systemListConstructor.systemList[targetSystem].systemObject) == true)
					{
						AddPermanentSystem(j, targetSystem);
					}
				}
			}
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].permanentConnections.Count != systemListConstructor.systemList[i].numberOfConnections)
			{
				systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].permanentConnections.Count;
			}
		}

		connected = true;
	}
}

public class ConnectionCoordinates
{
	public GameObject systemOne, systemTwo;
	public Vector3 systemA, systemB;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}
