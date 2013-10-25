using UnityEngine;
using System.Collections;

public class LugusCoroutineHandle : MonoBehaviour 
{
	protected bool _available = false;
	public bool Available
	{
		get{ return _available; }
	}
	
	protected bool _running = false;
	public bool Running
	{
		get{ return _running; }
	}
	
	
	public void StartRoutine( IEnumerator subject )
	{
		Debug.LogWarning("StartRoutine");
		if( subject == null )
		{
			Debug.LogError(name + " : method for coroutine was null!");
			return;	
		}
		
		_available = false;
		
		base.StartCoroutine( RoutineRunner(subject) );
	}
	
	public void StopRoutine()
	{
		StopAllCoroutines();
		
		_available = true;
	}
	
	// TODO: test this rigourously!!! : see if coroutine continues as expected after deactivate and re-activate afterwards
	public void SetRunning(bool running)
	{
		_running = running;
	}
	
	public void Toggle()
	{
		SetRunning( !_running );
	}
	
	protected IEnumerator RoutineRunner( IEnumerator routine )
	{
		_available = false;
		_running = true;
		
		//yield return StartCoroutine( subject );
		base.StartCoroutine( routine );
		
		while( routine.MoveNext() )
		{
			Debug.Log ("MOVE NEXT");
			while( !_running )
			{ 
				Debug.Log ("PAUSED"); 
				yield return null;
			}
			
			yield return null;
		}
		
		_available = true;
		_running = false;
	}
	
	protected new Coroutine StartCoroutine(IEnumerator subject)
	{
		Debug.LogError("Cannot start a normal coroutine in the routineHandle! use StartRoutine() function instead");
		return null;
	}
	
	protected new Coroutine StartCoroutine(string methodName )
	{
		Debug.LogError("Cannot start a normal coroutine in the routineHandle! use StartRoutine() function instead");
		return null;
	}
	
	protected new Coroutine StartCoroutine(string methodname, object value)
	{
		Debug.LogError("Cannot start a normal coroutine in the routineHandle! use StartRoutine() function instead");
		return null;
	}
	
	
	void Awake()
	{
		_available = true;
	}
}
