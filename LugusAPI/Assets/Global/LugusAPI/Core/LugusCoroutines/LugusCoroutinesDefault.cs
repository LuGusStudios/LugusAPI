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
	
	protected ILugusCoroutineHandle CreateHandle(GameObject runner = null)
	{
		FindReferences();
		
		GameObject handleGO = runner;

		if( handleGO == null )
		{
			handleGO = new GameObject("LugusCoroutineHandle");
			handleGO.transform.parent = handleHelperParent;
		}

		ILugusCoroutineHandle handle = handleGO.AddComponent<LugusCoroutineHandleDefault>();

		handles.Add( handle );

		return handle;
	}
	
	public ILugusCoroutineHandle GetHandle(GameObject runner = null) 
	{
		// TODO: make sure the handles are recycled / that we use a Pool of handles that is initialized at the beginning
		// loop over this.handles to find the next handle that has .Running == false
		// if none can be found -> only then use CreateHandle()

		// if runner != null, we probably have to make a new one though... or at most re-use the Component's on the runner object that are no longer running
		
		return CreateHandle(runner); 
	}
	
	public ILugusCoroutineHandle StartRoutine( IEnumerator routine, GameObject runner = null )
	{
		ILugusCoroutineHandle handle = GetHandle(runner);
		Coroutine croutine = handle.StartRoutine( routine );
		handle.Coroutine = croutine;
		
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
