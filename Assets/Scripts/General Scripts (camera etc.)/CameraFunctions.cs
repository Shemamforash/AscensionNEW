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
	
	private float leftBound = 0f, rightBound = 90f, upperBound = 90f, lowerBound = 0f;
	private float timer = 0.0f;
	private float updatedX, updatedY;
	private GameObject thisObject;
	private TechTreeGUI techTreeGUI;

	private Vector3 initPos, finalPos;

	public GameObject invasionButton, invasionLine, currentPointer = null, currentLine;
	private Vector3 anchorPoint, currentPoint;

	void Start()
	{
		techTreeGUI = GameObject.Find ("GUIContainer").GetComponent<TechTreeGUI> ();
		minZoom = (0.1333333f * systemListConstructor.mapSize) - 29f;
		zoomSpeed = (maxZoom - minZoom) / -5f;
		zPosition = minZoom;
	}

	private void RotateLine()
	{
		float distance = Vector3.Distance (anchorPoint, currentPoint);

		if(distance != 0)
		{
			float rotationZRad = Mathf.Acos ((currentPoint.y - anchorPoint.y) / distance);
			
			float rotationZ = rotationZRad * Mathf.Rad2Deg;
			
			if(anchorPoint.x < currentPoint.x)
			{
				rotationZ = -rotationZ;
			}
			
			Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);

			Quaternion rotation = new Quaternion ();

			rotation.eulerAngles = vectRotation;

			currentLine.transform.rotation = rotation;
		}
	}

	private void ResizeLine()
	{
		Vector3 midpoint = (currentPoint + anchorPoint) / 2;

		float distance = Vector3.Distance (anchorPoint, currentPoint);

		currentLine.transform.localScale = new Vector3 (0.2f, distance, 0f);

		currentLine.transform.position = midpoint;
	}

	private void UpdateInvadeLine()
	{
		if(Input.GetKeyDown("i") && selectedSystem != null)
		{
			if(systemListConstructor.systemList[RefreshCurrentSystem(selectedSystem)].systemOwnedBy == playerTurnScript.playerRace)
			{
				bool ignore = false;

				if(currentPointer != null)
				{
					GameObject.Destroy (currentPointer);
					GameObject.Destroy (currentLine);
					ignore = true;
				}

				if(currentPointer == null && ignore == false)
				{
					anchorPoint = selectedSystem.transform.position;
					currentPointer = (GameObject)GameObject.Instantiate(invasionButton, anchorPoint, Quaternion.identity);
					currentLine = (GameObject)GameObject.Instantiate(invasionLine, anchorPoint, Quaternion.identity);
				}
			}
		}

		if(currentPointer != null)
		{
			Ray temp = Camera.main.ScreenPointToRay(Input.mousePosition);
			float angle = Vector3.Angle (Vector3.forward, temp.direction);
			float hyp = Camera.main.transform.position.z / (Mathf.Cos(angle * Mathf.Deg2Rad));
			currentPoint = temp.origin - temp.direction * hyp;
			currentPoint = new Vector3(currentPoint.x, currentPoint.y, 0f);

			for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
			{
				Vector3 sysPos = systemListConstructor.systemList[i].systemObject.transform.position;

				if(currentPoint.x < sysPos.x + 3f && currentPoint.x > sysPos.x - 3f)
				{
					if(currentPoint.y < sysPos.y + 3f && currentPoint.y > sysPos.y - 3f)
					{
						currentPoint = sysPos;
						break;
					}
				}
			}

			currentPointer.transform.position = currentPoint;
			ResizeLine();
			RotateLine();
		}
	}

	void Update()
	{
		UpdateInvadeLine ();

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
						if(galaxyGUI.planetSelectionWindow.activeInHierarchy == false)
						{
							openMenu = true; //Opens system menu on double click
						}
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
