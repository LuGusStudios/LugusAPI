using UnityEngine;
using System.Collections;

public class LugusTexture : MonoBehaviour 
{
	public string key = "";

	// Use this for initialization
	void Start () 
	{
		if( string.IsNullOrEmpty(key) )
		{
			key = renderer.material.mainTexture.name;
			
			Debug.LogError(name + " : key was empty! using material.mainTexture name : " + key );
		}
		
		LugusResources.use.onResourcesReloaded += UpdateTexture;
		
		UpdateTexture();
	}
	
	protected void UpdateTexture()
	{
		this.renderer.material.mainTexture = LugusResources.use.GetTexture(key);
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
