using UnityEngine;
using System.Collections;

public class CameraFunctions : MasterScript
{
	//This class contains all functions related to camera behaviour. This includes panning and zooming of the main camera, as well as using raycasts to update the current selected object.
	//It also includes mouse functions (double click).
	
	public Camera cameraMain;
	public float zoomSpeed, minZoom, maxZoom, panSpeed, zPosition;
	[HideInInspector]
	public GameObject selectedSystem;
	[HideInInspector]
	public bool doubleClick = false, coloniseMenu = false, openMenu = false, moveCamera = false, lightFading;
	
	private float leftBound = -40, rightBound = 40, upperBound = 40, lowerBound = -40;
	private float timer = 0.0f;
	private float updatedX, updatedY;
	private GameObject thisObject;

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
			systemGUI.spendMenu = false;
			systemGUI.openImprovementList = false;
			tier3HeroScript.openSystemLinkScreen = false;
			heroGUI.openHeroLevellingScreen = false;
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
				if(hit.collider.gameObject.tag == "StarSystem")
				{
					selectedSystem = hit.collider.gameObject;
				}

				int i = RefreshCurrentSystem(selectedSystem);

				if(systemListConstructor.systemList[i].systemOwnedBy == null)
				{
					coloniseMenu = true; //Shows the colonise button on single click
				}

				if(doubleClick == true)
				{
					bool canViewSystem = false;

					for(int j = 0; j < 3; ++j)
					{
						if(systemListConstructor.systemList[i].heroesInSystem[j] == null)
						{
							continue;
						}

						heroScript = systemListConstructor.systemList[i].heroesInSystem[j].GetComponent<HeroScriptParent>();

						if(heroScript.isInvisible == true && heroScript.heroOwnedBy == playerTurnScript.playerRace)
						{
							canViewSystem = true;
							break;
						}
					}

					if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace || canViewSystem == true)
					{
						openMenu = true; //Opens system menu on double click
					}
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
		Vector3 cameraPositionNew = new Vector3(cameraMain.transform.position.x, cameraMain.transform.position.y, zPosition);
		
		if(Input.GetAxis ("Mouse ScrollWheel") < 0) //Zoom in
		{
			zPosition -= zoomSpeed * Time.deltaTime;
		
			if(zPosition < maxZoom)
			{
				zPosition = maxZoom;
				
				cameraMain.transform.position = cameraPositionNew;
			}
			else
			{
				cameraMain.transform.position = cameraPositionNew;
			}
		}

		if(Input.GetAxis ("Mouse ScrollWheel") > 0) //Zoom out
		{
			zPosition += zoomSpeed * Time.deltaTime;
			
			if(zPosition > minZoom)
			{
				zPosition = minZoom;
				
				cameraMain.transform.position = cameraPositionNew;
			}
			else
			{
				cameraMain.transform.position = cameraPositionNew;
			}
		}
	}

	public void CentreCamera() //Used to centre the camera over the last selected object, or the home planet if on first turn.
	{
		if(Input.GetKeyDown("f") && selectedSystem != null)
		{
			moveCamera = true;
			
			timer = Time.time;
			
			thisObject = selectedSystem;
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
