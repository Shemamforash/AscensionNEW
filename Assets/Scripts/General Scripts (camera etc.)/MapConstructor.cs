using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapConstructor : MasterScript
{
	private List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	public float distanceMax;

	public bool TestForIntersection(GameObject thisSystem, GameObject targetSystem)
	{
		Vector3 lineVector1 = targetSystem.transform.position - thisSystem.transform.position;

		for (int i = 0; i < coordinateList.Count; ++i) 
		{
			Vector3 lineVector2 = coordinateList[i].systemB.transform.position - coordinateList[i].systemA.transform.position;
			Vector3 lineVector3 = coordinateList[i].systemA.transform.position - thisSystem.transform.position;

			Vector3 crossVector1and2 = Vector3.Cross(lineVector1, lineVector2);
			Vector3 crossVector3and2 = Vector3.Cross(lineVector3, lineVector2);

			float planarFactor = Vector3.Dot (lineVector1, crossVector1and2);

			if (planarFactor >= 0.00001f || planarFactor <= -0.00001f) 
			{
				continue;
			}
			
			float s = Vector3.Dot (crossVector3and2, crossVector1and2)/crossVector1and2.sqrMagnitude;

			if(s>= 0.0f && s <= 1.0f)
			{
				Vector3 intersection = thisSystem.transform.position + (lineVector1 * s);

				float dot = Vector3.Dot (intersection, lineVector1);

				if(intersection.magnitude <= lineVector1.magnitude)
				{
					return false;
				}
			}
		}

		return true;
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
			
			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				int system = systemListConstructor.RefreshCurrentSystemA(linkedSystems[j]);
				
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(TestForIntersection(linkedSystems[j], unlinkedSystems[k]) == false)
					{
						continue;
					}
					
					float distance = Vector3.Distance(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position);
					
					if(distance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = distance;
						nearestSystem = systemListConstructor.RefreshCurrentSystemA(unlinkedSystems[k]);
						thisSystem = system;
					}
				}
			}
			
			if(nearestSystem != -1 && thisSystem != -1)
			{
				linkedSystems.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add nearest unlinked system to linkedsystems list
				unlinkedSystems.Remove (systemListConstructor.systemList[nearestSystem].systemObject);
				
				AddPermanentSystem(thisSystem, nearestSystem);
			}
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].permanentConnections.Count;
		}

		AssignMaximumConnections ();
	}
	
	private void AddPermanentSystem(int thisSystem, int nearestSystem)
	{
		systemListConstructor.systemList[thisSystem].permanentConnections.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add target system to current systems permanent connections
		systemListConstructor.systemList [nearestSystem].permanentConnections.Add (systemListConstructor.systemList [thisSystem].systemObject);
		
		ConnectionCoordinates connection = new ConnectionCoordinates ();
		
		connection.systemA = systemListConstructor.systemList [thisSystem].systemObject;
		connection.systemB = systemListConstructor.systemList [nearestSystem].systemObject;
		
		coordinateList.Add (connection);
	}
	
	private int WeightedConnectionFinder(int randomInt)
	{
		if(randomInt < 9)
		{
			return 1;
		}
		if(randomInt >= 9 && randomInt < 24)
		{
			return 2;
		}
		if(randomInt >= 24 && randomInt < 49)
		{
			return 3;
		}
		if(randomInt >= 49 && randomInt < 74)
		{
			return 4;
		}
		if(randomInt >= 74 && randomInt < 89)
		{
			return 5;
		}
		if(randomInt >= 89)
		{
			return 6;
		}
		
		return 0;
	}
	
	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			int randomInt = WeightedConnectionFinder(Random.Range (0,99)); //Generate number
			
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
				
				if(distance < distanceMax) //If distance is less than maximum distance, add it to temporary connections
				{
					Node nearbySystem = new Node();
					
					nearbySystem.targetSystem = systemListConstructor.systemList[j].systemObject;
					nearbySystem.targetDistance = distance;
					
					systemListConstructor.systemList[i].tempConnections.Add (nearbySystem);
				}
			}
			
			SortNearestConnections();
			
			if(systemListConstructor.systemList[i].tempConnections.Count < (systemListConstructor.systemList[i].numberOfConnections - systemListConstructor.systemList[i].permanentConnections.Count))
			{
				systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].tempConnections.Count +  systemListConstructor.systemList[i].permanentConnections.Count;
			}
		}

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
			
			if(systemListConstructor.systemList[i].tempConnections.Count > (systemListConstructor.systemList[i].numberOfConnections - systemListConstructor.systemList[i].permanentConnections.Count))
			{
				int tempInt = systemListConstructor.systemList[i].tempConnections.Count - (systemListConstructor.systemList[i].numberOfConnections - systemListConstructor.systemList[i].permanentConnections.Count);
				
				for(int j = 0; j < tempInt; ++j)
				{
					systemListConstructor.systemList[i].tempConnections.RemoveAt(systemListConstructor.systemList[i].numberOfConnections - systemListConstructor.systemList[i].permanentConnections.Count);
				}
			}
		}
	}
	
	private void ConnectSystems()
	{
		for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all systems
		{
			if(systemListConstructor.systemList[j].numberOfConnections == systemListConstructor.systemList[j].permanentConnections.Count) //If the number of assigned systems equals the maximum number of systems, continue
			{
				continue;
			}
			
			for(int l = 0; l < systemListConstructor.systemList[j].tempConnections.Count; ++l) //For all tempconnections
			{
				if(systemListConstructor.systemList[j].numberOfConnections == systemListConstructor.systemList[j].permanentConnections.Count)
				{
					break;
				}

				bool skipConnection = false;

				int targetSystem = systemListConstructor.RefreshCurrentSystemA(systemListConstructor.systemList[j].tempConnections[l].targetSystem); //Get target system
				
				if(systemListConstructor.systemList[targetSystem].permanentConnections.Count < systemListConstructor.systemList[targetSystem].numberOfConnections) //If target/this system's connections are already full, continue
				{
					for(int m = 0; m < systemListConstructor.systemList[j].permanentConnections.Count; ++m) //If connections has already been made, continue
					{
						if(systemListConstructor.systemList[j].tempConnections[l].targetSystem == systemListConstructor.systemList[j].permanentConnections[m])
						{
							skipConnection = true;
						}
					}

					for(int m = 0; m < systemListConstructor.systemList[targetSystem].permanentConnections.Count; ++m)
					{
						if(systemListConstructor.systemList[targetSystem].permanentConnections[m] == systemListConstructor.systemList[j].systemObject)
						{
							skipConnection = true;
						}
					}
					
					if(TestForIntersection(systemListConstructor.systemList[j].systemObject, systemListConstructor.systemList[targetSystem].systemObject) == true && skipConnection == false)
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
	}
}

public class ConnectionCoordinates
{
	public GameObject systemA, systemB;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}
