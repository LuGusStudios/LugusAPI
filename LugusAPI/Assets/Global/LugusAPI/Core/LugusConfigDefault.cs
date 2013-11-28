/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/ *
 * TODO:
 * 
- meerdere user profiles!!!
- boolean terugkrijgen
- schrijven naar xml file ipv playerprefs (#config files)
- list all keys
 * 
 * / 

public class LugusConfig : LugusSingletonRuntime<LugusConfigDefault>
{
	public static void Test()
	{
		Debug.Log("DOE MIJ WEG");
	}
	
}

public class LugusConfigDefault : MonoBehaviour
{
	public Dictionary<string, object> data = new Dictionary<string, object>();
	
	public void ClearTempStorage()
	{
		foreach( KeyValuePair<string,object> entry in data )
		{
			Debug.LogError("Config contained : " + entry.Key + " -> " + entry.Value.ToString() );
		}
		
		data.Clear();
	}
	
	public void OnDisable()
	{
		// store values in scene transitions
		StoreAllInPlayerPrefs();
	}
	
	public void Set(Enum keyE, object val)
	{
		Set ( keyE.ToString(), val);
	}
	
	public void Set(Enum keyE, object val, bool overwrite)
	{
		Set ( keyE.ToString(), val, overwrite);
	}
	
	
	public void Set(string key, object val)
	{
		Set (key, val, true);
	}
	
	// if overwrite == true, an existing value will be overwritten. Otherwhise, the new value is discarded
	public void Set(string key, object val, bool overwrite)
	{
		if( overwrite )
			data[key] = val;
		else
		{
			if( !data.ContainsKey(key) )
				data[key] = val;
		}
	}
	
	public T Get<T>(Enum keyE, T defaultValue)
	{
		string key = keyE.ToString();
		return Get<T>(key, defaultValue);
	}
	
	public T Get<T>(string key, T defaultValue)
	{
		
		if( data.ContainsKey(key) )
		{
			//Debug.Log ("Config:Get : key from enum found : " + key);
			
			if( typeof(T) == typeof(float) )
				return (T) (object) float.Parse( "" + data[key] );
			else if( typeof(T) == typeof(int) )
				return (T) (object) int.Parse( "" + data[key] );
			
			return (T) data[key];
		}
		else if( PlayerPrefs.HasKey(key) )
		{
			//Debug.LogError ("Config:Get : key from playerprefs direct found : " + key);
			
			string val = PlayerPrefs.GetString(key);
			
			if( typeof(T) == typeof(float) )
				return (T) (object) float.Parse( "" + val );
			else if( typeof(T) == typeof(int) )
				return (T) (object) int.Parse( "" + val );
			
			return (T) (object) val;
		}
		else
		{
			//Debug.Log ("Config:Get : key not found : returning default : " + key);
			
			return defaultValue;
		}
	}
	
	public T GetEnum<T>(Enum keyE, T defaultValue)
	{
		string key = keyE.ToString();
		return GetEnum<T>(key, defaultValue);
	}
	
	public T GetEnum<T>(string key, T defaultValue )
	{
		if( data.ContainsKey(key) )
		{
			return (T) Enum.Parse(typeof(T), (string) data[key]);
		}
		else if( PlayerPrefs.HasKey(key) )
		{
			return (T) Enum.Parse(typeof(T), PlayerPrefs.GetString(key));
		}
		else
			return defaultValue;
	}
	
	public void FillFromPlayerPrefs(Enum keys)
	{
		string[] keyStrings = Enum.GetNames( keys.GetType() );
		
		foreach( string key in keyStrings )
		{
			string data = PlayerPrefs.GetString(key);
			if( !string.IsNullOrEmpty(data) )
				Set (key, data);
		}
	}
	
	public void StoreAllInPlayerPrefs()
	{
		foreach( string key in data.Keys )
			StoreInPlayerPrefs(key);
		
		PlayerPrefs.Save();
	}
	
	public void StoreInPlayerPrefs(string key)
	{
			//Debug.LogError("StoreInPlayerPrefs 1 : " + key);
		
		if( data.ContainsKey(key) )
		{
			//Debug.LogError("StoreInPlayerPrefs 2 : " + key + " / " + data[key]);
			PlayerPrefs.SetString( key, "" + data[key] );
		}
	}
	
	public void OnApplicationQuit()
	{
		StoreAllInPlayerPrefs();
	}
	
	public void OnApplicationPause()
	{
		StoreAllInPlayerPrefs(); 
	}
}
*/
