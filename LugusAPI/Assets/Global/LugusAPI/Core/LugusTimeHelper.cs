using UnityEngine;
using System.Collections;

public class LugusTimeHelper : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnEnable()
	{
		LugusTime.timeHelperInternal = this;
	}
	
	public void OnDisable()
	{
		LugusTime.timeHelperInternal = null;
	}
	
	public float lastTime = 0.0f;
	public float deltaTime = 0.0f;
	
	// Update is called once per frame
	void Update () 
	{
		// the first frame (no lastTime yet), we return the Time.deltaTime to prevent huge deltaTimes 
		if( lastTime != 0.0f )
			deltaTime = Time.realtimeSinceStartup - lastTime;
		else
			deltaTime = Time.deltaTime;
		
		lastTime = Time.realtimeSinceStartup; 
	}
	
	
}
