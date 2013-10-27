using UnityEngine;
using System.Collections;

public class LugusCoroutineHandleHelper : MonoBehaviour 
{
	public LugusCoroutineHandle handle = null;
	
	[SerializeField] // NOTE: this is just to make it show up in the inspector. Can be removed without problem.
	protected bool _running = false;
	public bool Running
	{
		get{ return _running; }
	}
	
	
	public IEnumerator RoutineRunner( IEnumerator routine )
	{
		if( handle == null )
		{
			Debug.LogError(name + " : handle was null!");
			yield break;
		}
		
		_running = true;
		
		yield return StartCoroutine( routine );
		
		_running = false;
	}
	
	public void StopRoutine()
	{
		StopAllCoroutines();
		
		_running = false;
	}
	
	
	void Awake()
	{
		_running = false;
	}
}
