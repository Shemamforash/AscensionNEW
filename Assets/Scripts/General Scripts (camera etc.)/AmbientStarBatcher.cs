using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]

public class AmbientStarBatcher : MonoBehaviour 
{
	public void BatchChildren () 
	{
		MeshFilter[] filters = GetComponentsInChildren<MeshFilter> ();
		CombineInstance[] combine = new CombineInstance[filters.Length - 1];

		int index = 0;
		for(int i = 0; i < filters.Length; ++i)
		{
			if(filters[i].sharedMesh == null)
			{
				continue;
			}

			combine[index].mesh = filters[i].sharedMesh;
			combine[index++].transform = filters[i].transform.localToWorldMatrix;
			filters[i].renderer.enabled = false;
		}

		gameObject.GetComponent<MeshFilter> ().mesh = new Mesh ();
		gameObject.GetComponent<MeshFilter> ().mesh.CombineMeshes (combine);
		gameObject.renderer.material = filters [1].renderer.sharedMaterial;
	}
}
