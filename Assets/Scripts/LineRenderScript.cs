using UnityEngine;
using System.Collections;

public class LineRenderScript : MonoBehaviour 
{
	[HideInInspector]
	public bool showText;
	[HideInInspector]
	public GUISystemDataScript guiPlanScript;
	public GUITextScript guiTextScript;
	public GUIText activeGUI;
	public GameObject[] connections = new GameObject[4];
	public GameObject quadA;
	
	private GameObject objectB;

	void Start()
	{	
		guiTextScript = GameObject.FindGameObjectWithTag("SystemOverlay").GetComponent<GUITextScript>();
		guiPlanScript = gameObject.GetComponent<GUISystemDataScript>();
		
		foreach (GameObject connectedObject in connections)
		{
			if(connectedObject == null)
			{
				break;
			}
			
			objectB = connectedObject;

			BuildLine();
		}
	}

	void BuildLine()
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