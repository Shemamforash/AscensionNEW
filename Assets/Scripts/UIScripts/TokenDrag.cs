using UnityEngine;
using System.Collections;

public class TokenDrag : MasterScript 
{
	public Transform tokenContainer;
	private float proximity = 20f;
	private bool posSnap;
	private GameObject currentContainer;

	void Start()
	{
		tokenContainer = GameObject.Find ("Token Container").transform.Find("BG").transform;
	}

	public void IncreaseTokenCounter()
	{
		if(currentContainer != null)
		{
			UILabel label = currentContainer.transform.Find ("Label").gameObject.GetComponent<UILabel>();
			int j = int.Parse (label.text);
			label.text = (j + 1).ToString();

			if(currentContainer.gameObject.GetComponent<UISprite>().spriteName == "Empty Line")
			{
				Debug.Log("Bacon");

				switch (currentContainer.gameObject.name)
				{
				case "Defence Token":
					currentContainer.gameObject.GetComponent<UISprite>().spriteName = "Defence Normal";
					break;
				case "Secondary Token":
					currentContainer.gameObject.GetComponent<UISprite>().spriteName = "Secondary Weapon Normal";
					break;
				case "Primary Token":
					currentContainer.gameObject.GetComponent<UISprite>().spriteName = "Primary Weapon Normal";
					break;
				default:
					break;
				}
			}
		}
	}

	void Update () 
	{
		Vector3 mousePos = Input.mousePosition; //Position of mouse

		foreach(Transform child in tokenContainer) //For all token containers
		{
			if(child.tag == "TokenContainer")
			{
				Vector3 childPos = systemPopup.uiCamera.WorldToScreenPoint(child.position); //Get position of container in screen coordinates

				if(mousePos.x < childPos.x + proximity)
				{
					if(mousePos.x > childPos.x - proximity)
					{
						if(mousePos.y < childPos.y + proximity)
						{
							if(mousePos.y > childPos.y - proximity) //If mouseposition is within distance of container
							{
								gameObject.transform.position = child.position; //Snap to position
								posSnap = true;
								currentContainer = child.gameObject;
								break;
							}
						}
					}
				}

				posSnap = false;
				currentContainer = null;
			}
		}

		if(posSnap == false) //If position snapping has no occured, follow the mouse
		{
			Vector3 position = Camera.main.ScreenToViewportPoint (Input.mousePosition); //New position is mouse position in viewport coordinates
			
			position = systemPopup.uiCamera.ViewportToWorldPoint (position); //Use uicamera to convert viewport coordinates to world coordinates
			
			Vector3 newPosition = new Vector3(position.x, position.y, -37.0f); //Set position
			
			gameObject.transform.position = newPosition;
		}
	}
}
