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
		handle.StartRoutine( PrintRoutine() );
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( LugusInput.use.KeyDown(KeyCode.T) )
		{
			handle.Toggle();
		}
	}
		
	public IEnumerator PrintRoutine()
	{
		int counter = 0;
		while( true )
		{
			Debug.Log (name + " : routine iteration " + counter);
			
			counter++;
			
			yield return new WaitForSeconds(1.0f);
		}
	}
}
