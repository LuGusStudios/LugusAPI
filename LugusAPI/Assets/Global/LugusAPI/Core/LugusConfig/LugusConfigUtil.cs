using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusConfigUtilHelper : LugusSingletonRuntime<LugusConfigUtilHelper>
{
	public ILugusConfigProfile _store = null;
	public ILugusConfigProfile dataStore
	{
		get
		{
			if( _store == null )
			{
				_store = new LugusConfigProfileDefault("temp");
			}

			return _store;
		}
	}
}

public static class LugusConfigUtil
{
	public static bool autosave = true;
	public static void GUIAutoSave()
	{
		GUILayout.BeginHorizontal();

		GUILayout.Label("AUTOSAVE?", GUILayout.Width(150));
		autosave = GUILayout.Toggle (autosave, ""); 

		GUILayout.EndHorizontal();
	}

	public static void GUIProfileSelect()
	{

	}

	public static bool GUIStringInput(this ILugusConfigProfile profile, string configName, string defaultValue = "")
	{
		//Debug.Log ("GUIStringInput for profile " + profile.Name);
		bool changed = false;

		GUILayout.BeginHorizontal();

		GUILayout.Label(configName, GUILayout.Width(50));

		string content = LugusConfigUtilHelper.use.dataStore.GetString(configName, profile.GetString(configName, defaultValue));
		string contentNew = GUILayout.TextField(content, GUILayout.Width(150));
		
		LugusConfigUtilHelper.use.dataStore.SetString(configName, contentNew, true);

		if( GUILayout.Button("set", GUILayout.Width(40)) )
		{
			profile.SetString(configName, contentNew, true);
			changed = true;
		}

		GUILayout.EndHorizontal();

		return changed;
	}
	
	public static bool GUISlider(this ILugusConfigProfile profile, string configName, float from, float to, string defaultValue = "0")
	{
		bool changed = false;

		GUILayout.BeginHorizontal();
		
		GUILayout.Label(configName, GUILayout.Width(50));
		
		string content = LugusConfigUtilHelper.use.dataStore.GetString(configName, profile.GetString(configName, defaultValue));
		/*string contentNew =*/ GUILayout.Label(content, GUILayout.Width(50) );

		float contentFloatNew = GUILayout.HorizontalSlider( Mathf.Clamp(float.Parse(content), from, to), from, to,  GUILayout.Width(100));
		
		LugusConfigUtilHelper.use.dataStore.SetString(configName, "" + contentFloatNew, true);
		
		if( GUILayout.Button("set", GUILayout.Width(40)) )
		{
			profile.SetString(configName, "" + contentFloatNew, true);
			changed = true;
		}
		
		GUILayout.EndHorizontal();
		
		return changed;
	}
}
