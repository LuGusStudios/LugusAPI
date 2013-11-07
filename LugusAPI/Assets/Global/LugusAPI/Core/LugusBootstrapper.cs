using UnityEngine;
using System.Collections;
using UnityEditor;

public class LugusBootstrapper : MonoBehaviour 
{
	[MenuItem ("Lugus/Bootstrap Scene")]
	static void Bootstrap()
	{
		GameObject JESUS = GameObject.Find ("JESUS");
		if( JESUS == null )
			JESUS = new GameObject("JESUS");
		
		if( LugusResources.use == null )
		{
			JESUS.AddComponent<LugusResourcesDefault>();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
