using UnityEngine;
using System.Collections;

public class ToolTip : MasterScript
{	
	public GameObject tooltip;
	public UIWidget tooltipWidget;
	public UIPanel rootPanel;
	private float timer, xDif, yDif;
	private bool showTooltip = false;

	void Start()
	{
		NGUITools.SetActive(tooltip, false);
	}

	public void Update()
	{
		if(UICamera.hoveredObject != null)
		{
			if(UICamera.hoveredObject.tag == "Improvement" || UICamera.hoveredObject.tag == "Improvement Slot")
			{
				if(showTooltip == false)
				{
					timer = Time.time;
					showTooltip = true;
				}

				if(showTooltip == true && timer + 0.5f <= Time.time)
				{
					for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
					{
						if(UICamera.hoveredObject.gameObject.name == systemListConstructor.basicImprovementsList[i].name)
						{
							NGUITools.SetActive(tooltip, true);

							tooltip.GetComponent<UILabel>().text = systemListConstructor.basicImprovementsList[i].details;

							yDif = tooltipWidget.height / 2;
							xDif = 0.0f;

							if(Input.mousePosition.x + 250 > rootPanel.width)
							{
								xDif = -250f;
							}

							Vector3 position = new Vector3(Input.mousePosition.x + xDif, Input.mousePosition.y + yDif, 0.0f); //TODO

							position = overlayGUI.mainCamera.ScreenToViewportPoint(position);
												
							position = overlayGUI.uiCamera.ViewportToWorldPoint (position);
							
							tooltip.transform.position = new Vector3(position.x, position.y, tooltip.transform.position.z);
						}
					}
				}
			}
		}

		else
		{
			if(tooltip.activeInHierarchy == true)
			{
				tooltip.GetComponent<UILabel>().text = "";
				NGUITools.SetActive(tooltip, false);
				showTooltip = false;
			}
		}
	}
}
