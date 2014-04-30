using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroMovement : MasterScript 
{
	public bool heroIsMoving;
	public GameObject pathfindTarget;
	private int currentVertex;

	private Queue<GameObject> queuedVertices = new Queue<GameObject> ();
	private List<GameObject> visitedVertices = new List<GameObject> ();
	private List<GameObject> tempRoute = new List<GameObject> ();
	private List<GameObject> pathVertices = new List<GameObject>();
	private List<PathFindingNodes> nodeTiers = new List<PathFindingNodes> ();

	private Vector3 targetPosition = Vector3.zero, currentPosition;

	void Start () 
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	void Update()
	{
		if(pathfindTarget != null)
		{
			heroIsMoving = true;
			RefreshHeroLocation();
		}

		if(pathfindTarget == null)
		{
			heroIsMoving = false;
		}
	}

	public void FindPath()
	{
		pathVertices.Clear ();
		queuedVertices.Clear ();
		visitedVertices.Clear ();
		tempRoute.Clear ();
		nodeTiers.Clear ();

		queuedVertices.Enqueue (heroScript.heroLocation); //Add hero's current location
		
		while(queuedVertices.Count > 0) //While queue has contents
		{
			GameObject currentObject = queuedVertices.Dequeue (); //Current object is first queue object
			
			if(currentObject == pathfindTarget) //If the current object is equal to the target
			{
				tempRoute.Add (pathfindTarget); //Add it to the temproute
				
				while(currentObject != heroScript.heroLocation) //While the current object doesnt equal the hero's location
				{
					for(int i = 0; i < nodeTiers.Count; ++i) //Recursively add the nodes to a list to form a route
					{
						if(nodeTiers[i].thisNode == currentObject)
						{
							tempRoute.Add (nodeTiers[i].precededBy);
							currentObject = nodeTiers[i].precededBy;
							break;
						}
					}
				}
				
				break;
			}
			
			int k = RefreshCurrentSystem(currentObject);
			
			for(int j = 0; j < systemListConstructor.systemList[k].permanentConnections.Count; ++j) //For all systems connected to this system
			{
				GameObject tempObject = systemListConstructor.systemList[k].permanentConnections[j]; //Get a reference to the system
				
				if(visitedVertices.Contains(tempObject) == false) //If the system has not been visited
				{
					visitedVertices.Add (tempObject); //Set it to visitied
					queuedVertices.Enqueue(tempObject); //Queue it up
					
					PathFindingNodes node = new PathFindingNodes(); //Add new pathfinding node
					
					node.precededBy = currentObject; //Set up node
					node.thisNode = tempObject;
					
					nodeTiers.Add (node);
				}
			}	
		}
		
		for(int i = tempRoute.Count - 1; i >= 0; --i)
		{
			pathVertices.Add (tempRoute[i]);
		}

		currentVertex = 0;
	}

	public void RefreshHeroLocation()
	{
		if(pathVertices[currentVertex] != pathfindTarget) //If current system does not equal the destination system
		{
			currentPosition = gameObject.transform.position; //Current hero position is updated

			targetPosition = HeroPositionAroundStar (pathVertices[currentVertex + 1]); //Target position is set

			Vector3 dir = targetPosition - currentPosition;

			if(TestForProximity(currentPosition, targetPosition) == true) //If current hero position is equal to the next system on route
			{	
				systemSIMData = pathVertices[currentVertex].GetComponent<SystemSIMData>();
				systemDefence = pathVertices[currentVertex].GetComponent<SystemDefence>();

				systemDefence.underInvasion = false;
				systemDefence.regenerateTimer = 3;
				heroScript.isInvading = false;
				systemListConstructor.systemList[currentVertex].enemyHero = null;

				++currentVertex; //Update current system
				
				heroScript.heroLocation = pathVertices [currentVertex]; //Set herolocation to current system

				heroShip = gameObject.GetComponent<HeroShip>();

				targetPosition = Vector3.zero;
			}

			else
			{
				gameObject.transform.position = Vector3.MoveTowards (currentPosition, targetPosition, (10 * heroScript.movementSpeed) * Time.deltaTime);

				Debug.Log ((10 * heroScript.movementSpeed) * Time.deltaTime);
			}
		}

		if(TestForProximity(currentPosition, HeroPositionAroundStar(pathfindTarget)) == true)
		{
			heroScript.heroLocation = pathfindTarget;
			pathfindTarget = null;
		}
	}

	public bool TestForProximity(Vector3 current, Vector3 target)
	{
		if(current.x <= target.x + 0.1f && current.x >= target.x - 0.1f)
		{
			if(current.y <= target.y + 0.1f && current.y >= target.y - 0.1f)
			{
				return true;
			}
		}

		return false;
	}

	public Vector3 HeroPositionAroundStar(GameObject location)
	{
		Vector3 position = new Vector3();

		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		switch(heroScript.heroOwnedBy)
		{
		case "Humans":
			position.x = location.transform.position.x;
			position.y = location.transform.position.y + 2.5f;
			break;
		case "Selkies":
			position.x = location.transform.position.x + 2.5f;
			position.y = location.transform.position.y - 1.66f;
			break;
		case "Nereides":
			position.x = location.transform.position.x - 2.5f;
			position.y = location.transform.position.y - 1.66f;
			break;
		default:
			break;
		}

		position.z = location.transform.position.z;
		
		return position;
	}
}

public class PathFindingNodes
{
	public GameObject precededBy, thisNode;
}
