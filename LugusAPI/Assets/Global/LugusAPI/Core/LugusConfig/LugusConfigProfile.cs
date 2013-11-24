using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


// The ILugusConfigProfile saves the properties of a user or a set of system variables
public interface ILugusConfigProfile
{

	string Name { get; set; }

	void Load();

	void Store();

	bool Exists(string key);

	void Remove(string key);

	#region Getters
	bool GetBool(string key, bool defaultValue);

	int GetInt(string key, int defaultValue);

	float GetFloat(string key, float defaultValue);

	double GetDouble(string key, double defaultValue);

	string GetString(string key, string defaultValue);

	T Get<T>(string key, T defaultValue);
	#endregion

	#region Setters
	void SetBool(string key, bool value, bool overwrite);

	void SetInt(string key, int value, bool overwrite);

	void SetFloat(string key, float value, bool overwrite);

	void SetDouble(string key, double value, bool overwrite);

	void SetString(string key, string value, bool overwrite);

	void Set<T>(string key, T value, bool overwrite);
	#endregion
}

public class LugusConfigProfileDefault : ILugusConfigProfile
{

	#region Properties
	public virtual string Name
	{
		get
		{
			return _profileName;
		}
		set
		{
			_profileName = value;
		}
	}
	public Dictionary<string, string> Data = null;
	public List<ILugusConfigProvider> Providers = null;

	[SerializeField]
	private string _profileName = "";

	private bool _changed = false;	// True if the data has changed by using one of the setter methods. Used to save time if no changes have been made to the profile.
	#endregion

	// Add a default provider pointing at the config folder. Doesn't load a profile from file.
	public LugusConfigProfileDefault(string name)
	{
		_profileName = name;
		Data = new Dictionary<string, string>();
		Providers = new List<ILugusConfigProvider>();
		_changed = true;

		LugusConfigProviderDefault provider = new LugusConfigProviderDefault(Application.dataPath + "/Config/");
		Providers.Add(provider);
	}

	// Add a default provider pointing at a folder, and loads the profile
	// from the file %path%/Config/%name%.xml or %path%/Config/%name%.json.
	public LugusConfigProfileDefault(string name, string path)
	{
		_profileName = name;
		Data = new Dictionary<string, string>();
		Providers = new List<ILugusConfigProvider>();
		_changed = false;
		LugusConfigProviderDefault provider = new LugusConfigProviderDefault(path);
		Providers.Add(provider);

		Load();
	}

	// Add the provider and load the profile from file.
	public LugusConfigProfileDefault(string name, ILugusConfigProvider provider)
	{
		_profileName = name;
		Data = new Dictionary<string, string>();
		Providers = new List<ILugusConfigProvider>();
		_changed = false;

		Providers.Add(provider);
		Load();

	}

	// Add a list of providers to load the profile data from.
	public LugusConfigProfileDefault(string name, List<ILugusConfigProvider> providers)
	{
		_profileName = name;
		Data = new Dictionary<string, string>();
		_changed = false;

		this.Providers = providers;
		Load();
	}

	public void Load()
	{
		// Populate the configuration with values found in the providers
		foreach (ILugusConfigProvider p in Providers)
		{
			if (p == null)
				break;

			// Load the dictionary from the provider
			Dictionary<string, string> providerData = p.Load(Name);
			List<string> keys = providerData.Keys.ToList();
			List<string> values = providerData.Values.ToList();

			// Add the data from the provider to the profile
			for (int i = 0; i < providerData.Count; ++i)
				Data.Add(keys[i], values[i]);
		}
	}

	public void Store()
	{
		if (!_changed)
			return;

		foreach (ILugusConfigProvider provider in Providers)
			provider.Store(Data, Name);

		_changed = false;
	}

	public bool Exists(string key)
	{
		return Data.ContainsKey(key);
	}

	public void Remove(string key)
	{
		if (Data.ContainsKey(key))
			Data.Remove(key);
	}

	#region Getters
	public bool GetBool(string key, bool defaultValue = false)
	{
		return Get<bool>(key, defaultValue);
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		return Get<int>(key, defaultValue);
	}

	public float GetFloat(string key, float defaultValue = 0.0f)
	{
		return Get<float>(key, defaultValue);
	}

	public double GetDouble(string key, double defaultValue = 0.0)
	{
		return Get<double>(key, defaultValue);
	}

	public string GetString(string key, string defaultValue = "")
	{
		return Get<string>(key, defaultValue);
	}

	public T Get<T>(string key, T defaultValue)
	{
		if (Data.ContainsKey(key))
		{
			// Check for known types with built-in parsing
			if (typeof(T) == typeof(bool))
				return (T)(object)StringToBool(Data[key], (bool)(object)defaultValue);
			else if (typeof(T) == typeof(int))
				return (T)(object)int.Parse(Data[key]);
			else if (typeof(T) == typeof(float))
				return (T)(object)float.Parse(Data[key]);
			else if (typeof(T) == typeof(double))
				return (T)(object)double.Parse(Data[key]);
			else if (typeof(T) == typeof(string))
				return (T)(object)Data[key];
			else
				return (T)(object)Data[key];

		}
		else
			return defaultValue;

	}
	#endregion

	#region Setters
	public void SetBool(string key, bool value, bool overwrite = true)
	{
		Set<bool>(key, value, overwrite);
	}

	public void SetInt(string key, int value, bool overwrite = true)
	{
		Set<int>(key, value, overwrite);
	}

	public void SetFloat(string key, float value, bool overwrite = true)
	{
		Set<float>(key, value, overwrite);
	}

	public void SetDouble(string key, double value, bool overwrite = true)
	{
		Set<double>(key, value, overwrite);
	}

	public void SetString(string key, string value, bool overwrite = true)
	{
		Set<string>(key, value, overwrite);
	}

	public void Set<T>(string key, T value, bool overwrite = true)
	{

		if (Data.ContainsKey(key))
		{
			if (overwrite)
			{
				Data[key] = value.ToString();
				_changed = true;
			}
		}
		else
		{
			Data.Add(key, value.ToString());
			_changed = true;
		}
	}
	#endregion

	// Convert a string to a boolean value.
	// Current string values that evaluate to true are: "true", "t" and "1".
	// Current string values that evaluate to false are: "false", "f" and "0".
	protected virtual bool StringToBool(string value, bool defaultValue)
	{

		// Make all letters lower values,
		// so we don't have to check all possible upper-lower combinations.
		string lower = value.ToLower();
		bool result = defaultValue;

		switch (lower)
		{
			case "true":
			case "t":
			case "1":
				result = true;
				break;
			case "false":
			case "f":
			case "0":
				result = false;
				break;
		}

		return result;
	}

}
