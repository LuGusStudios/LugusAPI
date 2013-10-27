using UnityEngine;
using System.Collections;
using System;


/*
 * This class acts as a dummy preface for the actual MonoBehaviour executing the routine.
 * This is handy in the following ways:
 * - Clean interface for the user : only Start and Stop, no MonoBehaviour stuff
 * - We can swap the internal _helper depending on availability
 *     (ex. if the _helper is already taken by another handle, we can look up an available one, so the user can keep re-using the same handle even if not running a coroutine on it constantly)
 */ 
[Serializable]
public class LugusCoroutineHandle 
{
	[SerializeField]
	protected LugusCoroutineHandleHelper _helper = null;
	public LugusCoroutineHandleHelper Helper
	{
		set
		{
			_helper = value; 
			_helper.handle = this;
		}
		get
		{
			return _helper; 
		}
	}
	
	public void Start( IEnumerator subject )
	{
		if( subject == null ) 
		{
			Debug.LogError("LugusCoroutineHandle : method for coroutine was null!");
			return;	
		}
		
		// TODO: check if _helper.Running is still false
		// if not: get a new Helper (from the Singleton maybe?) and start it on that?
		// as long as this is not fixed, and 2 routines are running on the same helper, Stop() will stop both routines!
		// Note though: this might be expected behaviour... ex. launching multiple routines that belong together on 1 handle...
		// possibly handle that with a flag? or even separate Singleton interface? (ex. GetHandle vs GetGroupHandle)
		
		if( _helper.Running )
		{
			Debug.LogWarning("LugusCoroutineHandle : there was already a routine running on this Handle! Starting new one anyway...");
		}
		
		_helper.StartCoroutine( _helper.RoutineRunner(subject) );
	}
	
	public void Stop()
	{
		_helper.StopRoutine();
	}
}
