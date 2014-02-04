using UnityEngine;
using System.Collections;

// TODO:
// OnEnable and OnDisable implementation so Singleton links are re-made on re-compilation
// see LugusSingletonExisting:OnEnable for example

// file loosely inspired by http://framebunker.com/blog/unity-singletons/

// use this for a singleton that has to exist in the scene before runtime (that cannot just be made through script)
public class LugusSingletonExisting<T> : MonoBehaviour where T : MonoBehaviour 
{
	private static T _instance = null;
	
	public static T use 
	{ 
		get 
		{
			if ( !CheckInstance() )
			{
				Debug.LogError ("LuGusSingletonExisting:use : " + typeof (T).Name + " does not exist!");
			}
			
			
			return _instance; 
		}
	}
	
	protected static bool CheckInstance()
	{
		if( _instance != null )
			return true;
		
		T[] instances = (T[]) GameObject.FindObjectsOfType( typeof(T) );
		if( instances.Length == 0 )
		{
			Debug.LogError ("LuGusSingletonExisting:CheckInstance : " + typeof (T).Name + " does not exist!");
			return false;
		}
		if( instances.Length > 1 )
		{
			Debug.LogError ("LuGusSingletonExisting:CheckInstance : " + typeof (T).Name + " exists multiple times! " + instances.Length);
		}
		
		_instance = instances[0];
		return true;
	}
	
	public void Change(T newInstance)
	{
		_instance = newInstance;
	}

	public static bool Exists()
	{
		if( _instance != null )
			return true;
		
		
		T[] instances = (T[]) GameObject.FindObjectsOfType( typeof(T) );
		return instances.Length != 0;
	}
	
	/*
	void OnEnable()
	{
		// could be used to survive re-compilation... not tested though
		_instance = null;	
	}
	*/
}

// use this for a singleton that can just be created through script if it's not set before runtime
public class LugusSingletonRuntime<T> : MonoBehaviour where T : MonoBehaviour 
{
	private static T _instance = null;
	
	public static T use 
	{ 
		get 
		{
			if ( !CheckInstance() )
			{
				Debug.LogError ("LuGusSingletonRuntime:use : " + typeof (T).Name + " does not exist!");
			}
			
			
			return _instance; 
		}
	}
	
	protected static bool CheckInstance()
	{
		if( _instance != null )
			return true;
		
		T[] instances = (T[]) GameObject.FindObjectsOfType( typeof(T) );
		if( instances.Length == 0 )
		{
			Debug.LogWarning ("LuGusSingletonRuntime:CheckInstance : " + typeof (T).Name + " does not exist! Creating it...");
			
			GameObject JESUS = GameObject.Find("JESUS");
			if( JESUS == null )
			{
				JESUS = new GameObject("JESUS"); 
			}
			
			_instance = JESUS.AddComponent<T>();
			
			return true;
		}
		if( instances.Length > 1 )
		{
			Debug.LogError ("LuGusSingletonRuntime:CheckInstance : " + typeof (T).Name + " exists multiple times! " + instances.Length);
		}
		
		_instance = instances[0];
		return true;
	}
	
	
	public void Change(T newInstance)
	{
		_instance = newInstance;
	}
}


// use this for a singleton that you don't want to store in a static var. Instead, it searches the singleton in the scene every time use is called.
// ATTENTION: can be very unperformant. Only use in specific cases!
public class LugusSingletonVolatile<T> : MonoBehaviour where T : MonoBehaviour 
{
	public static T use 
	{ 
		get
		{
			return (T) GameObject.FindObjectOfType( typeof(T) ); 
		}
	}
}