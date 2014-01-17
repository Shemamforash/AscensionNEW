using UnityEngine;
using System.Collections;

public class LineRenderScript : MasterScript 
{
	[HideInInspector]
	public bool showText;
	public GUITextScript guiTextScript;
	[HideInInspector]
	public GUIText activeGUI;
	[HideInInspector]
	public GameObject[] connectedLines = new GameObject[5];
	[HideInInspector]
	public Vector3[,] lineRotationsScalePosition = new Vector3[5,3];
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

			OrientationBuild(i);
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

		for(int i = 0; i < systemListConstructor.systemList[system].numberOfConnections; ++i)
		{
			Destroy (connectedLines[i]);

			Vector3 midPoint = lineRotationsScalePosition[i,2];

			Quaternion directQuat = new Quaternion();

			directQuat.eulerAngles = lineRotationsScalePosition[i,0];

			Vector3 scale = lineRotationsScalePosition[i,1];

			GameObject clone = (GameObject)Instantiate (aQuad, midPoint, directQuat);

			clone.transform.localScale = scale;

			connectedLines[i] = clone;
		}
	}

	void OrientationBuild(int i)
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

		lineRotationsScalePosition[i,0] = rotation;
		
		lineRotationsScalePosition[i,1] = scale;
		
		lineRotationsScalePosition[i,2] = midPoint;
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