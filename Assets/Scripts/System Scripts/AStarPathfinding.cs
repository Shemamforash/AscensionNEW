using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarPathfinding : MasterScript 
{
	private List<AStarNode> openList = new List<AStarNode> ();
	private List<AStarNode> closedList = new List<AStarNode> ();
	private List<GameObject> finalPath = new List<GameObject> ();
	private GameObject start, target;

	public void Pathfind(GameObject begin, GameObject end)
	{
		start = begin;
		target = end;

		AStarNode node = new AStarNode ();

		node.system = start;
		node.parent = null;
		node.f = 0f; 
		node.g = 0f; 
		node.h = Vector3.Distance(start.transform.position, target.transform.position);

		closedList.Add (node);
		finalPath.Add (start);

		int i = 0;

		while(finalPath.Contains(target) == false)
		{
			GetNearestNode(i);
			++i;
		}

		if(finalPath.Contains(target) == true)
		{
			for(int j = 0; j < finalPath.Count; ++j)
			{
				Debug.Log(finalPath[j].name);
			}
		}
	}

	private void GetNearestNode(int parNode)
	{
		int sys = RefreshCurrentSystem (closedList [parNode].system);

		for(int i = 0; i < systemListConstructor.systemList[sys].permanentConnections.Count; ++i)
		{
			bool skip = false;

			for(int j = 0; j < closedList.Count; ++j)
			{
				if(systemListConstructor.systemList[sys].permanentConnections[i] == closedList[j].system)
				{
					skip = true;
				}
			}

			if(skip == false)
			{
				AStarNode node = new AStarNode ();
				
				node.system = systemListConstructor.systemList[sys].permanentConnections[i];

				openList.Add (node);

				for(int j = 0; j < openList.Count; ++j)
				{
					if(openList[j].system == node.system && node.parent != null)
					{
						float gToParent = openList[j].g + Vector3.Distance(node.system.transform.position, node.parent.system.transform.position);
						float gThroughCurrent = closedList[parNode].g + Vector3.Distance(node.system.transform.position, node.parent.system.transform.position);

						if(gThroughCurrent < gToParent)
						{
							node.g = gThroughCurrent;
							node.h = Vector3.Distance(node.system.transform.position, target.transform.position);
							node.f = node.g + node.h;
							node.parent = closedList[parNode];
							openList[j] = node;
						}
					}
					else if(node.parent != null)
					{
						node.g = closedList[parNode].g + Vector3.Distance(node.system.transform.position, node.parent.system.transform.position); 
						node.h = Vector3.Distance(node.system.transform.position, target.transform.position);
						node.f = node.g + node.h; 
						node.parent = closedList[parNode];
					}
				}
			}
		}

		float temp = 10000f;
		int nodeToPick = -1;

		for(int i = 0; i < openList.Count; ++i)
		{
			if(openList[i].f < temp)
			{
				temp = openList[i].f;
				nodeToPick = i;
			}
		}

		closedList.Add (openList [nodeToPick]);
		finalPath.Add (openList [nodeToPick].system);
		openList.RemoveAt (nodeToPick);
	}

	private class AStarNode
	{
		public GameObject system;
		public AStarNode parent;
		public float f, g, h;
	}
}
