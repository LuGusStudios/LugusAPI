using UnityEngine;
using System.Collections;

public class CoroutineTester : MonoBehaviour 
{
	public int testVar = 10;
	
	LugusCoroutineHandle handle = null;
	
	// Use this for initialization
	void Start () 
	{
		handle = LugusCoroutines.use.GetHandle();
		handle.Start( PrintRoutine("ONE") );
		
		// Test to see if we can start 2 coroutines on a single handle
		//handle.Start( PrintRoutine("TWO") );
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
		
	public IEnumerator PrintRoutine(string prefix)
	{
		int counter = 0;
		while( true )
		{
			Debug.Log (name + " : " + prefix + " routine iteration " + counter);
			
			counter++;
			
			yield return new WaitForSeconds(1.0f);
		}
	}
}
