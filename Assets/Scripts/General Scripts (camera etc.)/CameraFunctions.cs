using UnityEngine;
using System.Collections;

public class CameraFunctions : MasterScript
{

	//This class contains all functions related to camera behaviour. This includes panning and zooming of the main camera, as well as using raycasts to update the current selected object.
	//It also includes mouse functions (double click).
	
	public Transform cameraMain;
	public float zoomSpeed, minZoom, maxZoom, panSpeed, zPosition;
	[HideInInspector]
	public string selectedSystem = "";
	[HideInInspector]
	public bool doubleClick = false, coloniseMenu = false, openMenu = false, moveCamera = false;
	
	private float leftBound = -40, rightBound = 40, upperBound = 40, lowerBound = -40;
	private float timer = 0.0f;
	private float updatedX, updatedY;
	private GameObject thisObject;

	void Awake()
	{
		mainGUIScript = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<MainGUIScript>();
	}

	void Update()
	{
		if(openMenu != true)
		{
			ZoomCamera();

			PanCamera();

			ClickSystem ();
		}
		
		if(Input.GetKeyDown ("escape")) //Used to close all open menus, and to reset doubleclick
		{
			coloniseMenu = false;
			openMenu = false;
			doubleClick = false;
			mainGUIScript.spendMenu = false;
			mainGUIScript.openImprovementList = false;
		}
	}

	private void ClickSystem()
	{
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(doubleClick == false)
			{
				DoubleClick ();
			}
			
			RaycastHit hit = new RaycastHit();
			
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				selectedSystem = hit.collider.name;
				coloniseMenu = true; //Shows the colonise button on single click
				
				if(doubleClick == true)
				{
					openMenu = true; //Opens system menu on double click
				}
			}
		}
	}
	
	public void PanCamera() //Used to pan camera
	{		
		if(Input.GetAxis ("Horizontal") > 0)
		{
			updatedX += panSpeed;
			
			if(updatedX > rightBound) //Prevent scrolling over screen edge
			{
				updatedX = rightBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Horizontal") < 0)
		{
			updatedX -= panSpeed;
			
			if(updatedX < leftBound) //Prevent scrolling over screen edge	
			{
				updatedX = leftBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") > 0)
		{
			updatedY += panSpeed;
			
			if(updatedY > upperBound) //Prevent scrolling over screen edge	
			{
				updatedY = upperBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") < 0)
		{
			updatedY -= panSpeed;
			
			if(updatedY < lowerBound) //Prevent scrolling over screen edge	
			{
				updatedY = lowerBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}
	}
	
	public void ZoomCamera() //Changes height of camera
	{		
		Vector3 cameraPositionNew = new Vector3(cameraMain.position.x, cameraMain.position.y, zPosition);
		
		if(Input.GetAxis ("Mouse ScrollWheel") < 0) //Zoom in
		{
			zPosition -= zoomSpeed/10;
		
			if(zPosition < maxZoom)
			{
				zPosition = maxZoom;
				
				cameraMain.position = cameraPositionNew;
			}
			else
			{
				cameraMain.position = cameraPositionNew;
			}
		}

		if(Input.GetAxis ("Mouse ScrollWheel") > 0) //Zoom out
		{
			zPosition += zoomSpeed/10;
			
			if(zPosition > minZoom)
			{
				zPosition = minZoom;
				
				cameraMain.position = cameraPositionNew;
			}
			else
			{
				cameraMain.position = cameraPositionNew;
			}
		}
	}

	public void CentreCamera() //Used to centre the camera over the last selected object, or the home planet if on first turn.
	{
		GameObject planetObject = GameObject.Find (selectedSystem);
		
		if(Input.GetKeyDown("f"))
		{
			moveCamera = true;
			
			timer = Time.time;
			
			thisObject = planetObject;
		}
		
		if(moveCamera == true)
		{
			Vector3 homingPlanetPosition = new Vector3(thisObject.transform.position.x, thisObject.transform.position.y, -30.0f); //Target position
			
			Vector3 currentPosition = cameraMain.transform.position;
			
			if(cameraMain.transform.position == homingPlanetPosition || Time.time > timer + 1.0f) //If lerp exceeds timer, camera position will lock to point at object
			{
				homingPlanetPosition = cameraMain.transform.position;

				updatedX = cameraMain.transform.position.x;

				updatedY = cameraMain.transform.position.y;
				
				moveCamera = false; //Stop moving camera
				
				timer = 0.0f; //Reset timer
			}
			
			cameraMain.transform.position = Vector3.Lerp (currentPosition, homingPlanetPosition, 0.1f);
		}
	}
	
	void DoubleClick() //Function for detecting double click
	{
		float delay = 0.2f;

		if(Input.GetMouseButtonDown (0))
		{
			if(Time.time < (timer + delay))
			{
				doubleClick = true;
			}
			
			else
			{
				timer = Time.time;
			}
		}
	}
}
