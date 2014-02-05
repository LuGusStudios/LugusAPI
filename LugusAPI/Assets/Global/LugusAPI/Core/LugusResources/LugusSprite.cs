using UnityEngine;
using System.Collections;

public class LugusSprite : MonoBehaviour
{
	public string key = "";
	
	
	protected void AssignKey()
	{
		if( string.IsNullOrEmpty(key) )
		{
			key = GetComponent<SpriteRenderer>().sprite.name;
			
			Debug.LogWarning(name + " : key was empty! using sprite.name : " + key );
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		AssignKey();
		
		LugusResources.use.onResourcesReloaded += UpdateSprite;
		
		UpdateSprite();
	}
	
	protected void UpdateSprite()
	{
		this.GetComponent<SpriteRenderer>().sprite = LugusResources.use.GetSprite(key);
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
