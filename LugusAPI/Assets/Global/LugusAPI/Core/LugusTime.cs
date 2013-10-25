using UnityEngine;
using System.Collections;

public class LugusTime 
{
	public static float deltaTime
	{
		get{ return Time.deltaTime; }
	}
	
	// this is a timescale independent DeltaTime
	
	public static LugusTimeHelper timeHelperInternal = null;
	public static float realDeltaTime
	{
		get
		{ 
			// timeHelperInternal is set in the TimeHelper script itself
			if( timeHelperInternal != null )
				return timeHelperInternal.deltaTime;
			else
				return Time.deltaTime; 
		}
	}
}
