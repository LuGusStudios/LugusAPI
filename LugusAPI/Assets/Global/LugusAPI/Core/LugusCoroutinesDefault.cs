using UnityEngine;
using System.Collections;

public class LugusCoroutines : LugusSingletonRuntime<LugusCoroutinesDefault>
{

}

public class LugusCoroutinesDefault : MonoBehaviour
{
	
	protected LugusCoroutineHandle CreateHandle()
	{
		GameObject handleBase = new GameObject("LugusCoroutineHandle");
		handleBase.transform.parent = this.transform;
		
		return handleBase.AddComponent<LugusCoroutineHandle>();
	}
	
	public LugusCoroutineHandle GetHandle()
	{
		return CreateHandle();
	}
}
