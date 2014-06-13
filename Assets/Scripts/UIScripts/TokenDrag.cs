using UnityEngine;
using System.Collections;

public class TokenDrag : TokenBehaviour 
{
	private Transform tokenContainer;
	private Vector3 originalPosition;
	private float proximity = 20f;
	private bool posSnap, clicked = false, removeEvent;
	private GameObject currentContainer;
	private TokenBehaviour tokenBehaviour;

	void Start()
	{
		tokenBehaviour = GameObject.Find ("GUIContainer").GetComponent<TokenBehaviour> ();
		tokenContainer = GameObject.Find ("Token Container").transform.Find("BG").transform; //Assign Container
		originalPosition = transform.position;
	}

	void Update () 
	{
		if(Input.GetKeyDown("escape"))
		{
			transform.position = originalPosition;
			gameObject.GetComponent<UIButton>().isEnabled = true;
			clicked = false;
			removeEvent = true;
		}

		if(removeEvent == true && clicked == false) //If the button events should be changed and the button is not following the mouse
		{
			EventDelegate.Add (gameObject.GetComponent<UIButton>().onClick, ButtonClicked); //Add button clicked
			removeEvent = false; //Prevent this being called again
		}
		
		if(clicked == true) //If object is following mouse
		{
			if(Input.GetMouseButtonDown(0)) //If mouse clicked
			{
				AttachToContainer(); //Check to attach it to a container
			}
			
			if(removeEvent == true && clicked == true) //If button events should be changed
			{
				EventDelegate.Remove(gameObject.GetComponent<UIButton>().onClick, ButtonClicked); //Remove button clicked
				removeEvent = false; //Prevent this being called again
			}
			
			if(CheckForContainer() == false)
			{
				FollowMouse();
			}
		}
	}

	public void ButtonClicked() //This method is called if the button is not already following the mouse
	{
		clicked = true; //Allow it to follow the mouse
		removeEvent = true; //Change button events (these are done later)
		gameObject.GetComponent<UIButton> ().isEnabled = false;

		if(transform.parent.name == "Defence Token" || transform.parent.name == "Secondary Token" || transform.parent.name == "Primary Token") //If it already has a parent (is already in a container)
		{
			UILabel label = transform.parent.Find ("Label").gameObject.GetComponent<UILabel>(); //Decrease the container's value
			int j = int.Parse (label.text);
			label.text = (j - 1).ToString();
		}
	}

	private void UpdateParent(GameObject container) //This method is used to increase the container's value
	{
		gameObject.transform.parent = container.transform; //And to set it's parent
		UILabel label = container.transform.Find ("Label").gameObject.GetComponent<UILabel>();
		int j = int.Parse (label.text);
		label.text = (j + 1).ToString();
		clicked = false; //And to prevent the object following the mouse
		removeEvent = true; //And to change the button events
		gameObject.GetComponent<UIButton> ().isEnabled = true;
		currentContainer = null;
	}
	
	public void AttachToContainer() //This method is called if the button is clicked and the object is already following the mouse
	{
		if(currentContainer != null) //If it is snapped to a container
		{
			switch (currentContainer.name) //If the container name matches the active object
			{
			case "Defence Token":
				if(gameObject.GetComponent<UISprite>().spriteName == "Defence Pressed")
				{
					UpdateParent(currentContainer); //UpdateParent
				}
				break;
			case "Secondary Token":
				if(gameObject.GetComponent<UISprite>().spriteName == "Secondary Weapon Pressed")
				{
					UpdateParent(currentContainer);
				}
				break;
			case "Primary Token":
				if(gameObject.GetComponent<UISprite>().spriteName == "Primary Weapon Pressed")
				{
					UpdateParent(currentContainer);
				}
				break;
			default:
				break;
			}
		}
	}

	private bool CheckForContainer()
	{
		Vector3 mousePos = Input.mousePosition; //Position of mouse
		
		foreach(Transform child in tokenContainer) //For all children of token container
		{
			if(child.tag == "TokenContainer") //If it IS a token container
			{
				Vector3 childPos = systemPopup.uiCamera.WorldToScreenPoint(child.position); //Get position of container in screen coordinates
				
				if(mousePos.x < childPos.x + proximity && mousePos.x > childPos.x - proximity && mousePos.y < childPos.y + proximity && mousePos.y > childPos.y - proximity) //If mouseposition is within distance of container
				{
					gameObject.transform.position = child.position; //Snap to position
					currentContainer = child.gameObject; //Set nearby container
					return true; //Nearby container found return true
				}
			}
		}

		currentContainer = null; //Object has no nearby container
		return false; //So return false
	}

	private void FollowMouse()
	{
		Vector3 position = Camera.main.ScreenToViewportPoint (Input.mousePosition); //New position is mouse position in viewport coordinates
		
		position = systemPopup.uiCamera.ViewportToWorldPoint (position); //Use uicamera to convert viewport coordinates to world coordinates
		
		Vector3 newPosition = new Vector3(position.x, position.y, -37.0f); //Set position
		
		gameObject.transform.position = newPosition;
	}
}
