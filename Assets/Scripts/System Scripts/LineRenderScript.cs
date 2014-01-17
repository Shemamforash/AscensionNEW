using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRenderScript : MasterScript 
{
	[HideInInspector]
	public bool showText;
	public GUITextScript guiTextScript;
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
	private Light thisLight;

	void Start()
	{	
		guiTextScript = GameObject.FindGameObjectWithTag("SystemOverlay").GetComponent<GUITextScript>();
		guiPlanScript = gameObject.GetComponent<GUISystemDataScript>();
		thisLight = gameObject.GetComponent<Light> ();
		thisSystem = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].numberOfConnections; ++i)
		{
			objectB = systemListConstructor.systemList[thisSystem].connectedSystems[i];

			OrientationBuild();
		}

		BuildLine(quadA);

		thisSystem = RefreshCurrentSystem(gameObject);
	}

	void Update()
	{
		if(cameraFunctionsScript.selectedSystem == gameObject)
		{
			thisLight.intensity = 4.0f;
		}
		if(cameraFunctionsScript.selectedSystem != gameObject)
		{
			thisLight.intensity = 0.0f;
		}
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
	}

	void BuildLine(GameObject aQuad)
	{
		int system = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < connectedSystems.Count; ++i)
		{
			Destroy (connectedSystems[i]);
		}

		connectedSystems.Clear ();

		//Debug.Log (connectorLines.Count + " | " + systemListConstructor.systemList [system].systemName + " | " + systemListConstructor.systemList [system].numberOfConnections);

		for(int i = 0; i < systemListConstructor.systemList[system].numberOfConnections; ++i)
		{
			Quaternion directQuat = new Quaternion();

			directQuat.eulerAngles = connectorLines[i].rotation;

			GameObject clone = (GameObject)Instantiate (aQuad, connectorLines[i].midPoint, directQuat);

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
		
		Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);

		Vector3 midPoint = (gameObject.transform.position + objectB.transform.position)/2;

		Vector3 scale = new Vector3(0.2f, distance, 0.0f);

		ConnectorLine newLine = new ConnectorLine ();

		newLine.rotation = rotation;
		
		newLine.scale = scale;
		
		newLine.midPoint = midPoint;

		connectorLines.Add (newLine);
	}

	public class ConnectorLine
	{
		public Vector3 rotation, scale, midPoint;
	}
		
	void OnMouseEnter()
	{
		guiTextScript.highlightedSystemName = gameObject.name;

		string systemSize = systemListConstructor.systemList[thisSystem].systemSize.ToString ();

		guiTextScript.highlightedSystemSize = systemSize;

		guiTextScript.highlightedSystem = gameObject;
	}
	
	void OnMouseExit()
	{
		guiTextScript.highlightedSystemName = null;
		guiTextScript.highlightedSystemSize = null;
	}
}