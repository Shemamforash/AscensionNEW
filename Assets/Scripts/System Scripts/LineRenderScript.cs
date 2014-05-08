using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LineRenderScript : MasterScript 
{
	[HideInInspector]
	public List<ConnectorLine> connectorLines = new List<ConnectorLine>();
	public GameObject line, clone;
	[HideInInspector]
	public int thisSystem;
	[HideInInspector]
	public Material opaqueMaterial;
	public Transform connectorLineContainer;
	private Quaternion rotation;
	private Vector3 midPoint, scale;
	private float pixelWidth, pixelHeight, systemPixelSize;
	private CollisionDetection collisionDetection;

	public void StartUp()
	{	
		collisionDetection = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<CollisionDetection> ();
		connectorLineContainer = GameObject.Find ("LineContainer").transform;
		systemSIMData = gameObject.GetComponent<SystemSIMData>();
		thisSystem = RefreshCurrentSystem (gameObject);

		CreateLines ();
	}

	void Update()
	{
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == playerTurnScript.playerRace)
		{
			ViewNearbySystems();
		}

		for(int i = 0; i < connectorLines.Count; ++i)
		{
			UpdateLine(systemListConstructor.systemList[thisSystem].permanentConnections[i], i);
		}
	}

	private void CreateLines()
	{
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
		{
			ConnectorLine newLine = new ConnectorLine ();

			GameObject clone = NGUITools.AddChild(connectorLineContainer.gameObject, line);

			newLine.thisLine = clone;
			
			newLine.sprite = newLine.thisLine.transform.Find ("Sprite").GetComponent<UISprite>();

			newLine.widget = newLine.thisLine.GetComponent<UIWidget>();

			connectorLines.Add (newLine);
		}
	}

	private void SetPosition(GameObject target, int i)
	{
		midPoint = (gameObject.transform.position + target.transform.position) / 2;
		
		midPoint = new Vector3 (midPoint.x, midPoint.y, 0.0f);
		
		Vector3 position = systemPopup.mainCamera.WorldToViewportPoint (midPoint);
		
		position = systemPopup.uiCamera.ViewportToWorldPoint (position);
		
		position = new Vector3(position.x, position.y, -37.0f);
		
		connectorLines[i].thisLine.transform.position = position;
	}

	private void SetRotation(GameObject target, int i)
	{
		if(target == null)
		{
			Debug.Log (target);
		}

		float distance = Vector3.Distance (gameObject.transform.position, target.transform.position);

		float rotationZRad = Mathf.Acos ((target.transform.position.y - gameObject.transform.position.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(gameObject.transform.position.x < target.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		rotation.eulerAngles = vectRotation;
		
		connectorLines[i].thisLine.transform.rotation = rotation;
	}


	private void UpdateLine(GameObject target, int i)
	{
		SetPosition (target, i);
		SetRotation (target, i);
		
		connectorLines[i].sprite.spriteName = SetRaceLineColour();

		Vector3 start = systemPopup.mainCamera.WorldToScreenPoint (gameObject.transform.position);
		Vector3 end = systemPopup.mainCamera.WorldToScreenPoint (target.transform.position);

		Vector3 left = new Vector3 (gameObject.transform.position.x - systemListConstructor.systemScale / 1.5f, 0f, 0f);
		Vector3 right = new Vector3 (gameObject.transform.position.x + systemListConstructor.systemScale / 1.5f, 0f, 0f);

		left = systemPopup.mainCamera.WorldToScreenPoint (left);
		right = systemPopup.mainCamera.WorldToScreenPoint (right);

		systemPixelSize = Vector3.Distance (left, right);

		pixelHeight = Vector3.Distance(start, end);
		
		pixelWidth = (0.16f * systemPopup.mainCamera.transform.position.z) + 10f;
		
		connectorLines[i].widget.width = Convert.ToInt32(pixelWidth);
	
		connectorLines[i].widget.height = Convert.ToInt32(pixelHeight - (systemPixelSize / 2));
	}

	public string SetRaceLineColour()
	{
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Humans")
		{
			return "PlayerOwnedLineMaterial";
		}
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Selkies")
		{
			return "EnemyOwnedLineMaterial";
		}
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Nereides")
		{
			return "SelkiesOwnedLine";
		}

		return "Empty Line"; //Should be "Empty Line"
	}

	private void ViewNearbySystems()
	{
		int system = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int tempSystem = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

			if(systemListConstructor.systemList[tempSystem].systemOwnedBy == null)
			{
				systemListConstructor.systemList[system].permanentConnections[i].renderer.material = opaqueMaterial;
			}
		}
	}
}

public class ConnectorLine
{
	public Quaternion rotation;
	public Vector3 midPoint;
	public GameObject thisLine;
	public UISprite sprite;
	public UIWidget widget;
}