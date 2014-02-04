using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigUtilTester : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	public void OnGUI()
	{
		GUILayout.BeginArea( new Rect(Screen.width - 300, 0, 300, 500) );

		LugusConfigUtil.GUIAutoSave();
		LugusConfig.use.User.GUIStringInput("Robin");
		LugusConfig.use.User.GUIStringInput("Marx");
		LugusConfig.use.User.GUISlider("strength", 0, 100);
		LugusConfig.use.User.GUISlider("C-MINE", 0, 100, "50");

		GUILayout.EndArea();
	}

	public void OnApplicationQuit()
	{
		if( LugusConfigUtil.autosave )
		{
			LugusConfig.use.SaveProfiles();
		}
	}
	
	public void OnApplicationPause()
	{
		if( LugusConfigUtil.autosave )
		{
			LugusConfig.use.SaveProfiles();
		}
	}
}
