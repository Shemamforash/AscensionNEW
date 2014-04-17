using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRenderScript : MasterScript 
{
	[HideInInspector]
	public bool showText;
	[HideInInspector]
	public GUIText activeGUI;
	[HideInInspector]
	public List<GameObject> connectedSystems = new List<GameObject>();
	[HideInInspector]
	public List<ConnectorLine> connectorLines = new List<ConnectorLine>();
	public GameObject quadA, humansOwnedQuad, selkiesOwnedQuad, nereidesOwnedQuad;
	[HideInInspector]
	public int thisSystem;
	[HideInInspector]
	private GameObject objectB;
	public Material opaqueMaterial;
	private Transform connectorLineContainer;

	public void StartUp()
	{	
		connectorLineContainer = GameObject.Find ("Connector Lines Container").transform;
		systemSIMData = gameObject.GetComponent<SystemSIMData>();
		thisSystem = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
		{
			objectB = systemListConstructor.systemList[thisSystem].permanentConnections[i];

			OrientationBuild();
		}

		BuildLine(quadA);

		thisSystem = RefreshCurrentSystem(gameObject);
	}

	public void SetRaceLineColour(string thisRace)
	{
		GameObject quad = null;

		if(thisRace == "Humans")
		{
			quad = humansOwnedQuad;
		}
		if(thisRace == "Selkies")
		{
			quad = selkiesOwnedQuad;
		}
		if(thisRace == "Nereides")
		{
			quad = nereidesOwnedQuad;
		}
		if(thisRace == "None")
		{
			quad = quadA;
		}
	
		BuildLine(quad);

		if(thisRace == playerTurnScript.playerRace)
		{
			ViewNearbySystems();
		}
	}

	private void ViewNearbySystems()
	{
		int system = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int tempSystem = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

			if(systemListConstructor.systemList[tempSystem].systemOwnedBy == null)
			{
				systemListConstructor.systemList[system].permanentConnections[i].renderer.material = opaqueMaterial;
			}
		}
	}

	void BuildLine(GameObject aQuad)
	{
		int system = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < connectedSystems.Count; ++i)
		{
			Destroy (connectedSystems[i]);
		}

		connectedSystems.Clear ();

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			GameObject clone = (GameObject)Instantiate (aQuad, connectorLines[i].midPoint, connectorLines[i].rotation);

			clone.transform.parent = connectorLineContainer;

			clone.transform.localScale = connectorLines[i].scale;

			connectedSystems.Add(clone);
		}
	}

	void OrientationBuild()
	{
		float distance = Vector3.Distance(gameObject.transform.position, objectB.transform.position);
		
		float rotationZRad = Mathf.Acos ((objectB.transform.position.y-gameObject.transform.position.y)/distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(gameObject.transform.position.x < objectB.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);

		Quaternion rotation = new Quaternion ();

		rotation.eulerAngles = vectRotation;

		Vector3 midPoint = (gameObject.transform.position + objectB.transform.position)/2;

		Vector3 scale = new Vector3(0.2f * systemListConstructor.systemScale, distance, 0.0f);

		ConnectorLine newLine = new ConnectorLine ();

		newLine.rotation = rotation;
		
		newLine.scale = scale;
		
		newLine.midPoint = midPoint;

		connectorLines.Add (newLine);
	}

	public class ConnectorLine
	{
		public Quaternion rotation;
		public Vector3 scale, midPoint;
	}
}