using UnityEngine;
using System.Collections;

public class CoroutineTester : MonoBehaviour 
{
	public int testVar = 10;
	
	ILugusCoroutineHandle myHandle = null;
	
	// Use this for initialization
	void Start () 
	{
		//1. fire and forget start coroutine
		LugusCoroutines.use.StartRoutine( PrintRoutine("QuickNDirty") );
		
		//2. use a handle to be able to Stop a coroutine at will
		ILugusCoroutineHandle handle = LugusCoroutines.use.GetHandle();
		handle.StartRoutine( PrintRoutine("3secs") );
		handle.StopRoutineDelayed( 3.0f );
		
		//3. claim a handle to start multiple coroutines on it
		myHandle = LugusCoroutines.use.GetHandle();
		myHandle.Claim();
		myHandle.StartRoutine( PrintRoutine("ONE") );
		myHandle.StartRoutine( PrintRoutine("TWO") );
		// release the handle afterwards for re-usage
		// myHandle.Release()
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
		
	public IEnumerator PrintRoutine(string prefix)
	{
		int counter = 0;
		while( counter < 10 )
		{
			Debug.Log (name + " : " + prefix + " routine iteration " + counter);
			
			counter++;
			
			yield return new WaitForSeconds(1.0f);
		}
	}
}
