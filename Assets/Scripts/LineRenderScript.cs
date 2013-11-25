using UnityEngine;
using System.Collections;

public class LineRenderScript : MonoBehaviour 
{
	[HideInInspector]
	public bool showText, owned = false;
	[HideInInspector]
	public GUISystemDataScript guiPlanScript;
	[HideInInspector]
	public MainGUIScript mainGUIScript;
	[HideInInspector]
	public GUITextScript guiTextScript;
	[HideInInspector]
	public TurnInfo turnInfoScript;
	public GUIText activeGUI;
	public GameObject[] connections = new GameObject[4];
	[HideInInspector]
	public GameObject[] connectedLines = new GameObject[4];
	[HideInInspector]
	public Vector3[,] lineRotationsScalePosition = new Vector3[4,3];
	public GameObject quadA;
	public GameObject ownedQuad;
	[HideInInspector]
	public int i = 0;
	[HideInInspector]
	private GameObject objectB;

	void Start()
	{	
		turnInfoScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<TurnInfo>();
		guiTextScript = GameObject.FindGameObjectWithTag("SystemOverlay").GetComponent<GUITextScript>();
		guiPlanScript = gameObject.GetComponent<GUISystemDataScript>();
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();

		foreach (GameObject connectedObject in connections)
		{
			if(connectedObject == null)
			{
				break;
			}
			
			objectB = connectedObject;

			OrientationBuild();
		}

		BuildLine(quadA);
	}

	void Update()
	{
		if(owned == false)
		{
			foreach(GameObject system in turnInfoScript.ownedSystems)
			{
				if(gameObject == system && system != null)
				{
					BuildLine(ownedQuad);
					owned = true;
					break;
				}
			}
		}
	}

	void BuildLine(GameObject aQuad)
	{
		for(i = 0; i < 4; ++i)
		{
			if(owned == false)
			{
				Destroy (connectedLines[i]);
			}

			Vector3 midPoint = lineRotationsScalePosition[i,2];

			Quaternion directQuat = new Quaternion();

			directQuat.eulerAngles = lineRotationsScalePosition[i,0];

			Vector3 scale = lineRotationsScalePosition[i,1];

			GameObject clone = (GameObject)Instantiate (aQuad, midPoint, directQuat);

			clone.transform.localScale = scale;

			connectedLines[i] = clone;
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

		lineRotationsScalePosition[i,0] = rotation;
		
		lineRotationsScalePosition[i,1] = scale;
		
		lineRotationsScalePosition[i,2] = midPoint;

		++i;
	}
		
	void OnMouseEnter()
	{
		guiTextScript.highlightedSystemName = gameObject.name;

		string systemSize = guiPlanScript.numPlanets.ToString ();

		guiTextScript.highlightedSystemSize = systemSize;

		guiTextScript.highlightedSystem = gameObject;
	}
	
	void OnMouseExit()
	{
		guiTextScript.highlightedSystemName = null;
		guiTextScript.highlightedSystemSize = null;
	}
}