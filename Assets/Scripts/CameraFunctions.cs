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
	public bool doubleClick = false;
	[HideInInspector]
	public bool coloniseMenu = false;
	[HideInInspector]
	public bool openMenu = false;
	
	private float leftBound = -40;
	private float rightBound = 40;
	private float upperBound = 40;
	private float lowerBound = -40;
	private float timer = 0.0f;
	[HideInInspector]
	public float updatedX, updatedY;
	
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
			DoubleClick ();

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
