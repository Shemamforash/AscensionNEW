using UnityEngine;
using System.Collections;

public class CameraFunctions : MasterScript
{
	//This class contains all functions related to camera behaviour. This includes panning and zooming of the main camera, as well as using raycasts to update the current selected object.
	//It also includes mouse functions (double click).
	
	public Camera cameraMain;
	public float zoomSpeed, minZoom, maxZoom, panSpeed, zPosition, t;
	[HideInInspector]
	public GameObject selectedSystem;
	[HideInInspector]
	public bool doubleClick = false, coloniseMenu = false, openMenu = false, moveCamera = false, lightFading, zoom;
	
	private float leftBound = 0.0f, rightBound = 90.0f, upperBound = 90.0f, lowerBound = 0.0f;
	private float timer = 0.0f;
	private float updatedX, updatedY;
	private GameObject thisObject;
	private TechTreeGUI techTreeGUI;

	private Vector3 initPos, finalPos;

	void Start()
	{
		techTreeGUI = GameObject.Find ("GUIContainer").GetComponent<TechTreeGUI> ();
		minZoom = (0.1333333f * systemListConstructor.mapSize) - 29f;
		zoomSpeed = (maxZoom - minZoom) / -5f;
		zPosition = minZoom;
	}

	void Update()
	{
		if(zoom == true)
		{
			gameObject.transform.position = Vector3.Lerp(initPos, finalPos, t);
			t += Time.deltaTime / 0.2f;

			if(timer < Time.time)
			{
				transform.position = finalPos;
				t = 0f;
				zoom = false;
				timer = 0f;
			}
		}

		if(openMenu != true && zoom == false)
		{
			ZoomCamera();

			PanCamera();

			ClickSystem ();
		}
		
		if(Input.GetKeyDown ("escape")) //Used to close all open menus, and to reset doubleclick
		{
			CloseAllWindows();
		}
	}

	private void CloseAllWindows()
	{
		coloniseMenu = false;
		openMenu = false;
		doubleClick = false;
		heroGUI.openHeroLevellingScreen = false;
		invasionGUI.openInvasionMenu = false;
		
		NGUITools.SetActive (heroGUI.heroDetailsContainer, false);
		
		if(techTreeGUI.techTree.activeInHierarchy == true)
		{
			techTreeGUI.ShowTechTree();
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
					systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();

					if(systemSIMData.guardedBy == playerTurnScript.playerRace)
					{
						coloniseMenu = true; //Shows the colonise button on single click
					}
				}

				if(doubleClick == true && heroResource.improvementScreen.activeInHierarchy == false)
				{
					if(systemListConstructor.systemList[i].systemOwnedBy == playerTurnScript.playerRace)
					{
						openMenu = true; //Opens system menu on double click
					}
					if(systemListConstructor.systemList[i].systemOwnedBy != playerTurnScript.playerRace && systemListConstructor.systemList[i].systemOwnedBy != null)
					{
						systemDefence = selectedSystem.GetComponent<SystemDefence>();

						bool openInvMenu = false;

						for(int j = 0; j < playerTurnScript.playerOwnedHeroes.Count; ++j)
						{
							if(playerTurnScript.playerOwnedHeroes[j] == heroGUI.currentHero)
							{
								heroScript = playerTurnScript.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();

								if(heroScript.heroLocation == systemListConstructor.systemList[i].systemObject)
								{
									if(heroScript.heroType == "Infiltrator")
									{
										openInvMenu = true;
									}

									if(systemDefence.canEnter == true)
									{
										openInvMenu = true;
									}
									
									if(openInvMenu == true)
									{
										CloseAllWindows();
										invasionGUI.openInvasionMenu = true;
										invasionGUI.OpenPlanetInvasionScreen();
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void PanCamera() //Used to pan camera
	{		
		updatedX = transform.position.x;
		updatedY = transform.position.y;

		if(Input.GetAxis ("Horizontal") > 0)
		{
			moveCamera = false;

			updatedX += panSpeed;
			
			if(updatedX > rightBound) //Prevent scrolling over screen edge
			{
				updatedX = rightBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Horizontal") < 0)
		{
			moveCamera = false;

			updatedX -= panSpeed;
			
			if(updatedX < leftBound) //Prevent scrolling over screen edge	
			{
				updatedX = leftBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") > 0)
		{
			moveCamera = false;

			updatedY += panSpeed;
			
			if(updatedY > upperBound) //Prevent scrolling over screen edge	
			{
				updatedY = upperBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") < 0)
		{
			moveCamera = false;

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
		zPosition = transform.position.z;

		if(Input.GetAxis ("Mouse ScrollWheel") < 0) //Zoom out
		{
			moveCamera = false;

			zPosition -= zoomSpeed;
		
			if(zPosition < maxZoom)
			{
				zPosition = maxZoom;
			}

			initPos = transform.position;
			finalPos = new Vector3(transform.position.x, transform.position.y, zPosition);
			timer = Time.time + 0.2f;
			zoom = true;
		}

		if(Input.GetAxis ("Mouse ScrollWheel") > 0) //Zoom in
		{
			moveCamera = false;

			zPosition += zoomSpeed;
			
			if(zPosition > minZoom)
			{
				zPosition = minZoom;
			}

			initPos = transform.position;
			finalPos = new Vector3(transform.position.x, transform.position.y, zPosition);
			timer = Time.time + 0.2f;
			zoom = true;
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
				
				moveCamera = false; //Stop moving camera
				
				timer = 0.0f; //Reset timer
			}

			updatedX = cameraMain.transform.position.x;
			
			updatedY = cameraMain.transform.position.y;
			
			zPosition = cameraMain.transform.position.z;
			
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
