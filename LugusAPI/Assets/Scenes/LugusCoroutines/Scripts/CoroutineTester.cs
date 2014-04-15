using UnityEngine;
using System.Collections;

public class CoroutineTester : MonoBehaviour 
{
	public int testVar = 10;
	
	ILugusCoroutineHandle myHandle = null;
	
	// Use this for initialization
	void Start () 
	{
		//LugusCoroutines.use.StartRoutine(); // returns a ILugusCoroutineHandle. Cannot be directly used in yield return
		//LugusCoroutines.use.StartRoutine().Coroutine; // this way, it can be used in a yield return ;)
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

	protected void PrintRoutines()
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

	protected IEnumerator DelayRoutine( string prefix, float delay )
	{
		yield return new WaitForSeconds( delay );
		
		Debug.Log (Time.time + " Delayroutine : " + prefix + " finished after delay " + delay);
	}

	protected IEnumerator LongestRoutinesCustom()
	{
		Debug.Log (Time.time + " LongestRoutinesCustom : started");

		ILugusCoroutineHandle routine1 = LugusCoroutines.use.StartRoutine( DelayRoutine("three", 3) );
		ILugusCoroutineHandle routine2 = LugusCoroutines.use.StartRoutine( DelayRoutine("five", 5) );

		while( routine1.Running || routine2.Running )
		{
			yield return null;
		}

		Debug.Log (Time.time + " LongestRoutinesCustom : finished");

		yield break;
	}

	protected IEnumerator LongestRoutinesAPI()
	{
		Debug.Log (Time.time + " LongesRoutinesAPI : started");

		yield return LugusCoroutineUtil.WaitForFinish( DelayRoutine("four", 4), DelayRoutine("two", 2) ).Coroutine;
		
		Debug.Log (Time.time + " LongesRoutinesAPI : finished first (IEnumerator)");

		ILugusCoroutineHandle routine1 = LugusCoroutines.use.StartRoutine( DelayRoutine("three", 3) );
		ILugusCoroutineHandle routine2 = LugusCoroutines.use.StartRoutine( DelayRoutine("five", 5) );

		yield return LugusCoroutineUtil.WaitForFinish( routine1, routine2 ).Coroutine;
		
		Debug.Log (Time.time + " LongesRoutinesAPI : finished second (ILugusCoroutineHandle)");
		
		yield return LugusCoroutineUtil.WaitForFinish( DelayRoutine("four", 4), DelayRoutine("two", 2), DelayRoutine("one", 1)  ).Coroutine;
		
		Debug.Log (Time.time + " LongesRoutinesAPI : finished third (IEnumerator)");
	}
	
	protected IEnumerator CustomHandleGameObject()
	{
		Debug.Log (Time.time + " CustomHandleGameObject : started");

		yield return this.gameObject.StartLugusRoutine( DelayRoutine("two", 2) ).Coroutine;
		
		Debug.Log (Time.time + " CustomHandleGameObject : finished first");

		ILugusCoroutineHandle handle2 = this.gameObject.StartLugusRoutine( DelayRoutine("four", 4) );
		yield return handle2.Coroutine;

		Destroy ( handle2.Component ); // alternative: Destroy( this.gameObject.GetComponent<LugusCoroutineHandleDefault>() );
		
		Debug.Log (Time.time + " CustomHandleGameObject : finished second : wait 3 seconds. Meanwhile: see if the handle is removed from the GO");

		yield return new WaitForSeconds( 3.0f );
		
		ILugusCoroutineHandle routine1 = this.gameObject.StartLugusRoutine( DelayRoutine("three", 3) );
		ILugusCoroutineHandle routine2 = this.gameObject.StartLugusRoutine( DelayRoutine("five", 5) );
		
		yield return LugusCoroutineUtil.WaitForFinish( routine1, routine2 ).Coroutine;
		
		Debug.Log (Time.time + " CustomHandleGameObject : finished third");
	}

	protected IEnumerator PauseRoutines()
	{
		Debug.Log (Time.time + " PauseRoutines : started");

		ILugusCoroutineHandle handle = LugusCoroutines.use.StartRoutine( PrintRoutine("pauseRoutine1 ") );

		yield return new WaitForSeconds(3.0f);
		handle.Paused = true;
		
		Debug.Log (Time.time + " PauseRoutines : paused afer 3 secs. Start again in another 3 secs.");
		
		yield return new WaitForSeconds(3.0f);

		Debug.Log (Time.time + " PauseRoutines : starting again. Will stop after 5 more seconds;");

		handle.Paused = false;

		yield return new WaitForSeconds(5.0f);

		handle.StopRoutine();

		
		Debug.Log (Time.time + " PauseRoutines : stopped");
	}

	public void OnGUI()
	{
		GUILayout.BeginArea( new Rect(Screen.width - 300, 0, 300, 500) );

		if( GUILayout.Button("Print routines") )
		{
			PrintRoutines();
		}

		if( GUILayout.Button ("Longest routines Custom (Bad)") )
		{
			StartCoroutine( LongestRoutinesCustom() );
		}

		if( GUILayout.Button ("Longest routines LugusAPI (Good)") )
		{
			StartCoroutine( LongestRoutinesAPI() );
		}
		
		if( GUILayout.Button ("Handle on this GameObject") )
		{
			StartCoroutine( CustomHandleGameObject() );
		}

		if( GUILayout.Button ("Pause routines") )
		{
			StartCoroutine( PauseRoutines() );
		}
		
		GUILayout.EndArea();
	}
}
