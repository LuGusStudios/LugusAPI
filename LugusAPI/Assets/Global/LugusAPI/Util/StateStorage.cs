using UnityEngine;
using System.Collections;

public class StateStorage : MonoBehaviour 
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	
	public bool visible = true; 
	
	public void Restore(Transform target)
	{
		target.position = position;
		target.rotation = rotation;
		target.localScale = scale;
		
		//Debug.LogError("Restoring " + target.name + " to scale " + scale );
		
		if( target.renderer != null )
			target.renderer.enabled = visible;
	}
	
	public void Store(Transform origin)
	{
		position = origin.position;
		rotation = origin.rotation;
		scale = origin.localScale;
		
		if( origin.renderer != null )
			visible = origin.renderer.enabled;
	}
	
	public void Restore()
	{
		Restore(transform);
	}
	
	public void Store()
	{
		Store(transform);
	}
	
	public static Vector3 StoredPosition(Transform element)
	{
		StateStorage storage = element.gameObject.GetComponent<StateStorage>();
		if( storage != null )
			return storage.position;
		else
			return Vector3.zero;
	}
	
	
	public static void AddTo( Transform obj )
	{
		AddTo( obj.gameObject );
	}
	
	public static void AddTo( GameObject obj )
	{
		StateStorage storage = obj.GetComponent<StateStorage>();
		
		if( storage == null )
		{
			storage = obj.AddComponent<StateStorage>();
		}
		
		storage.Store();
	}
	
	public static void AddToChildren(GameObject parent)
	{
		AddToChildren( parent.transform );
	}
	
	public static void AddToChildren(Transform parent)
	{
		foreach( Transform child in parent )
			StateStorage.AddTo( child.gameObject );
	}
	
	public static void RestoreChildren(Transform parent)
	{
		foreach( Transform child in parent )
		{
			StateStorage storage = child.GetComponent<StateStorage>();
			storage.Restore();
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public static class StorageExtensions
{
	public static void Store(this Transform t)
	{
		StateStorage storer = t.GetComponent<StateStorage>();
		if( storer == null )
			storer = t.gameObject.AddComponent<StateStorage>();
		
		storer.Store();
	}
	
	public static StateStorage Revert(this Transform t)
	{
		StateStorage storer = t.GetComponent<StateStorage>();
		
		if( storer == null )
		{
			storer = t.gameObject.AddComponent<StateStorage>();
			storer.Store(); 
		}
		
		
		return storer;
	}
	
}
