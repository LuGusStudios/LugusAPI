using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// we didn't use LugusSingletons here, since LugusCoroutinesDefault is not a MonoBehaviour but a POC
public class LugusCoroutines
{
	private static LugusCoroutinesDefault _instance = null;
	
	public static LugusCoroutinesDefault use 
	{ 
		get 
		{
			if ( _instance == null )
			{
				_instance = new LugusCoroutinesDefault();
			}
			
			
			return _instance; 
		}
	}
	
	public static void Change(LugusCoroutinesDefault newInstance)
	{
		_instance = newInstance;
	}
}

[Serializable]
public class LugusCoroutinesDefault
{
	public List<ILugusCoroutineHandle> handles = new List<ILugusCoroutineHandle>();
	
	protected Transform handleHelperParent = null;
	
	public LugusCoroutinesDefault()
	{
		// TODO: find all LugusCoroutineHandles in the scene and add their handles to this.handles
		// TODO : create a number of handles at the beginning to act as a Pool
	}
	
	protected void FindReferences()
	{
		if( handleHelperParent == null )
		{
			GameObject p = GameObject.Find("_LugusCoroutines");
			if( p == null )
			{
				p = new GameObject("_LugusCoroutines");
			}
			
			handleHelperParent = p.transform;
		}
	}
	
	protected ILugusCoroutineHandle CreateHandle()
	{
		FindReferences();
		
		GameObject handleGO = new GameObject("LugusCoroutineHandle");
		ILugusCoroutineHandle handle = handleGO.AddComponent<LugusCoroutineHandleDefault>();
		
		handleGO.transform.parent = handleHelperParent;

		return handle;
	}
	
	public ILugusCoroutineHandle GetHandle() 
	{
		// TODO: make sure the handles are recycled / that we use a Pool of handles that is initialized at the beginning
		// loop over this.handles to find the next handle that has .Running == false
		// if none can be found -> only then use CreateHandle()
		
		return CreateHandle(); 
	}
	
	public ILugusCoroutineHandle StartRoutine( IEnumerator routine )
	{
		ILugusCoroutineHandle handle = GetHandle();
		handle.StartRoutine( routine );
		
		return handle;
	}
	
	public void StopAllRoutines()
	{
		foreach( ILugusCoroutineHandle handle in handles )
		{
			handle.StopRoutine ();
		}
	}
}
