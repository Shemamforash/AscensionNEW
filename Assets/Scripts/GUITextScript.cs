using UnityEngine;
using System.Collections;

public class GUITextScript : MonoBehaviour 
{
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
