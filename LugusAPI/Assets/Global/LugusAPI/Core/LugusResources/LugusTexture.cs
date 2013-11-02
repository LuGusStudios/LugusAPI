using UnityEngine;
using System.Collections;

public class LugusTexture : MonoBehaviour 
{
	public string key = "";
	
	
	protected void AssignKey()
	{
		if( string.IsNullOrEmpty(key) )
		{
			key = renderer.material.mainTexture.name;
			
			Debug.LogWarning(name + " : key was empty! using material.mainTexture name : " + key );
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		AssignKey();
		
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
