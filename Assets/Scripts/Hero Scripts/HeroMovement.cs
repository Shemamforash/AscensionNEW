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

	private Vector3 targetPosition, currentPosition;

	void Start () 
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	void Update()
	{
		if(pathfindTarget != null && heroScript.movementPoints > 0)
		{
			heroIsMoving = true;
			RefreshHeroLocation();
		}

		if(pathfindTarget == null || heroScript.movementPoints == 0)
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

		queuedVertices.Enqueue (heroScript.heroLocation);
		
		while(queuedVertices.Count > 0)
		{
			GameObject currentObject = queuedVertices.Dequeue ();
			
			if(currentObject == pathfindTarget)
			{
				tempRoute.Add (pathfindTarget);
				
				while(currentObject != heroScript.heroLocation)
				{
					for(int i = 0; i < nodeTiers.Count; ++i)
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
			
			for(int j = 0; j < systemListConstructor.systemList[k].permanentConnections.Count; ++j)
			{
				GameObject tempObject = systemListConstructor.systemList[k].permanentConnections[j];
				
				if(visitedVertices.Contains(tempObject) == false)
				{
					visitedVertices.Add (tempObject);
					queuedVertices.Enqueue(tempObject);
					
					PathFindingNodes node = new PathFindingNodes();
					
					node.precededBy = currentObject;
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

			if(TestForProximity(currentPosition, targetPosition) == true) //If current hero position is equal to the next system on route
			{				
				++currentVertex; //Update current system
				
				heroScript.heroLocation = pathVertices [currentVertex]; //Set herolocation to current system
				
				--heroScript.movementPoints;

				heroShip = gameObject.GetComponent<HeroShip>();

				heroShip.ShipAbilities();
			}

			gameObject.transform.position = Vector3.MoveTowards (currentPosition, targetPosition, 20 * Time.deltaTime);
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
		Vector3 position = new Vector3 ();

		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		if(heroScript.heroOwnedBy == playerTurnScript.playerRace)
		{
			position.x = location.transform.position.x;
			position.y = location.transform.position.y + 1.5f;
		}
		
		if(heroScript.heroOwnedBy == enemyOneTurnScript.playerRace)
		{
			position.x = location.transform.position.x + 0.75f;
			position.y = location.transform.position.y - 0.5f;
		}
		
		if(heroScript.heroOwnedBy == enemyTwoTurnScript.playerRace)
		{
			position.x = location.transform.position.x - 0.75f;
			position.y = location.transform.position.y - 0.5f;
		}

		position.z = location.transform.position.z;
		
		return position;
	}
}

public class PathFindingNodes
{
	public GameObject precededBy, thisNode;
}
