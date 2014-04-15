using UnityEngine;
using System.Collections;

public class LugusCoroutineUtil 
{
	public static ILugusCoroutineHandle WaitForFinish( IEnumerator c1, IEnumerator c2 )
	{
		return WaitForFinish( LugusCoroutines.use.StartRoutine(c1), LugusCoroutines.use.StartRoutine(c2) );
	}

	public static ILugusCoroutineHandle WaitForFinish( ILugusCoroutineHandle c1, ILugusCoroutineHandle c2 )
	{
		return LugusCoroutines.use.StartRoutine( WaitForFinishRoutine(c1, c2) );
	}

	protected static IEnumerator WaitForFinishRoutine( ILugusCoroutineHandle c1, ILugusCoroutineHandle c2 )
	{
		while( c1.Running || c2.Running )
		{
			yield return null;
		}

		yield break;
	}

	
	public static ILugusCoroutineHandle WaitForFinish( IEnumerator c1, IEnumerator c2, IEnumerator c3 )
	{
		return WaitForFinish( LugusCoroutines.use.StartRoutine(c1), LugusCoroutines.use.StartRoutine(c2), LugusCoroutines.use.StartRoutine(c3) );
	}
	
	public static ILugusCoroutineHandle WaitForFinish( ILugusCoroutineHandle c1, ILugusCoroutineHandle c2, ILugusCoroutineHandle c3 )
	{
		return LugusCoroutines.use.StartRoutine( WaitForFinishRoutine(c1, c2, c3) );
	}
	
	protected static IEnumerator WaitForFinishRoutine( ILugusCoroutineHandle c1, ILugusCoroutineHandle c2, ILugusCoroutineHandle c3 )
	{
		while( c1.Running || c2.Running || c3.Running )
		{
			yield return null;
		}
		
		yield break;
	}

	
	
	public static IEnumerator DelayRoutine( float delay )
	{
		if( delay > 0 )
		{
			yield return new WaitForSeconds(delay);
		}
		
		yield break;
	}
}

public static class LugusCoroutineExtensions
{
	// ex. this.gameObject.StartLugusRoutine( function() )
	// ex. yield return this.gameObject.StartLugusRoutine( function() ).Coroutine;
	public static ILugusCoroutineHandle StartLugusRoutine( this GameObject go, IEnumerator routine )
	{
		return LugusCoroutines.use.StartRoutine( routine, go );
	}
}