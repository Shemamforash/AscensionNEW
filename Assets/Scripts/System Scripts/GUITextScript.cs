using UnityEngine;
using System.Collections;

public class GUITextScript : MonoBehaviour 
{
	//This class has been created simply to offset (with more control) the GUI text that appears when hovering over a system.

	public string highlightedSystemName;
	public string highlightedSystemSize;
	public GameObject highlightedSystem;
	
	void Update()
	{
		if(highlightedSystem != null)
		{
			transform.position = Camera.main.WorldToViewportPoint(highlightedSystem.transform.position);
			gameObject.guiText.text = highlightedSystemName + "\n" + highlightedSystemSize;
		}
	}
}
