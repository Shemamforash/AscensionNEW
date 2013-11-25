using UnityEngine;
using System.Collections;

public class LineRenderScript : MonoBehaviour 
{
	[HideInInspector]
	public bool showText, owned = false;
	[HideInInspector]
	public GUISystemDataScript guiPlanScript;
	public MainGUIScript mainGUIScript;
	public GUITextScript guiTextScript;
	public TurnInfo turnInfoScript;
	public GUIText activeGUI;
	public GameObject[] connections = new GameObject[4];
	public GameObject[] connectedLines = new GameObject[4];
	public Vector3[,] lineRotationsScalePosition = new Vector3[4,3];
	public GameObject quadA;
	public GameObject ownedQuad;
	public int i;
	
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

			BuildLineUnowned();
		}
	}

	void Update()
	{
		if(owned == false)
		{
			foreach(GameObject system in turnInfoScript.ownedSystems)
			{
				if(gameObject == system && system != null)
				{
					BuildLineOwned();
					break;
				}
			}
		}
	}

	void BuildLineOwned()
	{
		for(i = 0; i < 4; ++i)
		{
			Destroy (connectedLines[i]);

			Vector3 midPoint = lineRotationsScalePosition[i,2];

			Quaternion directQuat = new Quaternion();

			directQuat.eulerAngles = lineRotationsScalePosition[i,0];

			Vector3 scale = lineRotationsScalePosition[i,1];

			GameObject clone = (GameObject)Instantiate (ownedQuad, midPoint, directQuat);

			connectedLines[i] = clone;

			clone.transform.localScale = scale;
		}

		owned = true;
	}

	void BuildLineUnowned()
	{			
		float distance = Vector3.Distance(gameObject.transform.position, objectB.transform.position);
		
		float rotationZRad = Mathf.Acos ((objectB.transform.position.y-gameObject.transform.position.y)/distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(gameObject.transform.position.x < objectB.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		Quaternion directQuat = new Quaternion();
		
		directQuat.eulerAngles = rotation;
		
		Vector3 midPoint = (gameObject.transform.position + objectB.transform.position)/2;
		
		GameObject clone = (GameObject)Instantiate (quadA, midPoint, directQuat);

		Vector3 scale = new Vector3(0.2f, distance, 0.0f);
		
		clone.transform.localScale = scale;

		connectedLines[i] = clone;

		lineRotationsScalePosition[i,0] = rotation;

		lineRotationsScalePosition[i,1] = scale;

		lineRotationsScalePosition[i,2] = midPoint;

		i++;
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