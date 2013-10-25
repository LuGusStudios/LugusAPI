using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour 
{
	public int queue = 1000;
	private bool started = false; 
	public bool update = false; 
	
	void Update () 
	{		
		if (started && !update)
			return;
		
		gameObject.renderer.sharedMaterial.renderQueue = queue;
		
		started = true;
		update = false;
	}
	
}
