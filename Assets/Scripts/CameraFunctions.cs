using UnityEngine;
using System.Collections;

public class CameraFunctions : MonoBehaviour 
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
        Application.targetFrameRate = 60;
    }
	
	void Update()
	{
		ZoomCamera();

		PanCamera();
		
		if(Input.GetKeyDown ("escape"))
		{
			coloniseMenu = false;
			openMenu = false;
			doubleClick = false;
		}
		
		if(Input.GetMouseButtonDown(0))
		{
			if(doubleClick == false)
			{
				DoubleClick ();
			}

			RaycastHit hit = new RaycastHit();
		
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				selectedSystem = hit.collider.name;
				coloniseMenu = true;

				if(doubleClick == true)
				{
					openMenu = true;
				}
			}
		}
	}
	
	public void PanCamera()
	{		
		if(Input.GetAxis ("Horizontal") > 0)
		{
			updatedX += panSpeed;
			
			if(updatedX > rightBound)	
			{
				updatedX = rightBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Horizontal") < 0)
		{
			updatedX -= panSpeed;
			
			if(updatedX < leftBound)	
			{
				updatedX = leftBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") > 0)
		{
			updatedY += panSpeed;
			
			if(updatedY > upperBound)	
			{
				updatedY = upperBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") < 0)
		{
			updatedY -= panSpeed;
			
			if(updatedY < lowerBound)	
			{
				updatedY = lowerBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}
	}
	
	public void ZoomCamera()
	{		
		Vector3 cameraPositionNew = new Vector3(cameraMain.position.x, cameraMain.position.y, zPosition);
		
		if(Input.GetAxis ("Mouse ScrollWheel") < 0)
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

		if(Input.GetAxis ("Mouse ScrollWheel") > 0)
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

	public void CentreCamera()
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
			Vector3 homingPlanetPosition = new Vector3(thisObject.transform.position.x, thisObject.transform.position.y, -30.0f);
			
			Vector3 currentPosition = cameraMain.transform.position;
			
			if(cameraMain.transform.position == homingPlanetPosition || Time.time > timer + 1.0f)
			{
				homingPlanetPosition = cameraMain.transform.position;

				updatedX = cameraMain.transform.position.x;

				updatedY = cameraMain.transform.position.y;
				
				moveCamera = false;
				
				timer = 0.0f;
			}
			
			cameraMain.transform.position = Vector3.Lerp (currentPosition, homingPlanetPosition, 0.1f);
		}
	}
	
	void DoubleClick()
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
