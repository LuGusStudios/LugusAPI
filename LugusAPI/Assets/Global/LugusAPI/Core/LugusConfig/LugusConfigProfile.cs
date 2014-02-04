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

	string GetString(string key, string defaultValue);

	T Get<T>(string key, T defaultValue);
	#endregion

	#region Setters
	void SetBool(string key, bool value, bool overwrite);

	void SetInt(string key, int value, bool overwrite);

	void SetFloat(string key, float value, bool overwrite);

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
			return _name;
		}
		set
		{
			_name = value;
			_changed = true;
		}
	}
	public Dictionary<string, string> Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
			_changed = true;
		}
	}
	public List<ILugusConfigProvider> Providers
	{
		get
		{
			return _providers;
		}
		set
		{
			_providers = value;
			_changed = true;
		}
	}

	[SerializeField]
	protected string _name = "";
	protected Dictionary<string, string> _data = null;
	protected List<ILugusConfigProvider> _providers = null;

	protected bool _changed = false;	// True if the data has changed by using one of the setter methods. Used to save time if no changes have been made to the profile.
	#endregion

	// Add a default provider pointing at the config folder. Doesn't load a profile from file.
	public LugusConfigProfileDefault(string name)
	{
		_name = name;
		_data = new Dictionary<string, string>();
		_providers = new List<ILugusConfigProvider>();
		_changed = true;
#if !UNITY_WEBPLAYER
		Providers.Add(new LugusConfigProviderDefault(Application.dataPath + "/Config/"));
#else
		Providers.Add(new LugusConfigProviderPlayerPrefs(name));
#endif
	}

	// Add a default provider pointing at a folder specified in path.
	public LugusConfigProfileDefault(string name, string path)
	{
		_name = name;
		_data = new Dictionary<string, string>();
		_providers = new List<ILugusConfigProvider>();
		_changed = false;
		
#if !UNITY_WEBPLAYER
		Providers.Add(new LugusConfigProviderDefault(path));
#else
		Providers.Add(new LugusConfigProviderPlayerPrefs(name));
#endif
	}

	// Add a predefined provider for this profile instead of a default one.
	public LugusConfigProfileDefault(string name, ILugusConfigProvider provider)
	{
		_name = name;
		_data = new Dictionary<string, string>();
		_providers = new List<ILugusConfigProvider>();
		_changed = false;

		Providers.Add(provider);
	}

	// Add a predefined list of providers for this profile.
	public LugusConfigProfileDefault(string name, List<ILugusConfigProvider> providers)
	{
		_name = name;
		_data = new Dictionary<string, string>();
		_changed = false;

		_providers = providers;
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
			{
				if (_data.ContainsKey(keys[i]))
					_data[keys[i]] = values[i];
				else
					_data.Add(keys[i], values[i]);
			}
		}
	}

	public void Store()
	{
		if (!_changed)
			return;

		foreach (ILugusConfigProvider provider in _providers)
			provider.Store(Data, Name);

		_changed = false;
	}

	public bool Exists(string key)
	{
		return _data.ContainsKey(key);
	}

	public void Remove(string key)
	{
		if (_data.ContainsKey(key))
			_data.Remove(key);
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

	public string GetString(string key, string defaultValue = "")
	{
		return Get<string>(key, defaultValue);
	}

	public T Get<T>(string key, T defaultValue)
	{
		if (_data.ContainsKey(key))
		{
			// Check for known types with built-in parsing
			if (typeof(T) == typeof(bool))
				return (T)(object)StringToBool(_data[key], (bool)(object)defaultValue);
			else if (typeof(T) == typeof(int))
				return (T)(object)int.Parse(_data[key]);
			else if (typeof(T) == typeof(float))
				return (T)(object)float.Parse(_data[key]);
			else if (typeof(T) == typeof(string))
				return (T)(object)_data[key];
			else
				return (T)(object)_data[key];

		}
		else if (PlayerPrefs.HasKey(key))
		{
			if (typeof(T) == typeof(bool))
				return (T)(object)StringToBool(PlayerPrefs.GetString(key), (bool)(object)defaultValue);
			else if (typeof(T) == typeof(int))
				return (T)(object)PlayerPrefs.GetInt(key);
			else if (typeof(T) == typeof(float))
				return (T)(object)PlayerPrefs.GetFloat(key);
			else if (typeof(T) == typeof(string))
				return (T)(object)PlayerPrefs.GetString(key);
			else
				return (T)(object)PlayerPrefs.GetString(key);
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

	public void SetString(string key, string value, bool overwrite = true)
	{
		Set<string>(key, value, overwrite);
	}

	public void Set<T>(string key, T value, bool overwrite = true)
	{

		if (_data.ContainsKey(key))
		{
			if (overwrite)
			{
				_data[key] = value.ToString();
				_changed = true;
			}
		}
		else
		{
			_data.Add(key, value.ToString());
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
