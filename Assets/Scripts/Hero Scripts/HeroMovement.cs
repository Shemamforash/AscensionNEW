using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroMovement : MasterScript 
{
	public bool heroIsMoving;
	public GameObject pathfindTarget;
	private int currentVertex;
	private List<GameObject> pathVertices = new List<GameObject>();
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
			UpdatePosition();
		}
		if(pathfindTarget == null || heroScript.movementPoints == 0)
		{
			heroIsMoving = false;
		}
	}

	/*
	private void StartHeroMovement(GameObject hero, int targetSystem)
	{
		if(heroIsMoving == false)
		{
			if(heroScript.merchantLine != null)
			{
				Destroy (heroScript.merchantLine);
			}
			
			heroScript = hero.GetComponent<HeroScriptParent> ();
			
			Destroy (heroScript.invasionObject);
			
			heroScript.isInvading = false;
			
			systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData>();
			
			systemSIMData.underInvasion = false;
			
			int k = RefreshCurrentSystem(heroScript.heroLocation);
			
			for(int i = 0; i < 3; ++i)
			{
				if(systemListConstructor.systemList[k].heroesInSystem[i] == null)
				{
					continue;
				}
				
				if(systemListConstructor.systemList[k].heroesInSystem[i] == hero)
				{
					systemListConstructor.systemList[k].heroesInSystem[i] = null;
				}
			}
			
			heroScript.heroLocation = systemListConstructor.systemList[targetSystem].systemObject;
			
			k = RefreshCurrentSystem(heroScript.heroLocation);
			
			for(j = 0; j < 3; ++j)
			{
				if(systemListConstructor.systemList[targetSystem].heroesInSystem[j] == null)
				{
					timer = Time.time;
					
					heroScript.thisHeroNumber = j;
					
					systemListConstructor.systemList[k].heroesInSystem[j] = hero;
					
					heroIsMoving = true;
					
					break;
				}
			}
		}
	}
	*/

	public void FindPath()
	{
		currentVertex = 1;
		
		Queue<GameObject> queuedVertices = new Queue<GameObject> ();
		List<GameObject> visitedVertices = new List<GameObject> ();
		
		List<GameObject> tempRoute = new List<GameObject> ();
		
		List<PathFindingNodes> nodeTiers = new List<PathFindingNodes> ();
		
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
		int system = RefreshCurrentSystem (heroScript.heroLocation);
		
		systemListConstructor.systemList [system].heroesInSystem.Remove (gameObject);

		heroScript.heroLocation = pathVertices [currentVertex]; //Set herolocation to current system

		system = RefreshCurrentSystem (heroScript.heroLocation);

		systemListConstructor.systemList [system].heroesInSystem.Add (gameObject);

		if(pathVertices[currentVertex] != pathfindTarget) //If current system does not equal the destination system
		{
			if(gameObject.transform.position == pathVertices[currentVertex + 1].transform.position) //If current hero position is equal to the next system on route
			{
				++currentVertex; //Update current system

				--heroScript.movementPoints;
			}

			currentPosition = gameObject.transform.position; //Current hero position is updated

			targetPosition = HeroPositionAroundStar (pathVertices[currentVertex + 1]); //Target position is set
		}

		if(gameObject.transform.position == HeroPositionAroundStar(pathfindTarget))
		{
			pathfindTarget = null;
		}
	}

	public void UpdatePosition()
	{
		gameObject.transform.position = Vector3.MoveTowards (currentPosition, targetPosition, 20 * Time.deltaTime);

		if(currentPosition == targetPosition)
		{
			RefreshHeroLocation();
		}
	}

	public Vector3 HeroPositionAroundStar(GameObject location)
	{
		Vector3 position = new Vector3 ();

		/*
		if(heroScript.thisHeroNumber == 0)
		{
			position.x = location.transform.position.x;
			position.y = location.transform.position.y + 1.5f;
		}
		
		if(heroScript.thisHeroNumber == 1)
		{
			position.x = location.transform.position.x + 0.75f;
			position.y = location.transform.position.y - 0.5f;
		}
		
		if(heroScript.thisHeroNumber == 2)
		{
			position.x = location.transform.position.x - 0.75f;
			position.y = location.transform.position.y - 0.5f;
		}
		*/

		position.x = location.transform.position.x;
		position.y = location.transform.position.y + 1.5f;
		position.z = location.transform.position.z;
		
		return position;
	}
}

public class PathFindingNodes
{
	public GameObject precededBy, thisNode;
}
