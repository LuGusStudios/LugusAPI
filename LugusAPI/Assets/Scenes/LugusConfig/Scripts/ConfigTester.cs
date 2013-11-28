using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigTester : MonoBehaviour
{

	#region Properties
	public int BoxMargin = 10;	// Horizontal margin between each box
	public int BoxWidth = 200;	// Width of each box in pixels

	protected LugusConfigDefault _config = null;
	protected string _newProfileName = "Enter a name";

	protected int _profileEditMode = 0;
	protected string _profileKeyString = "Key";
	protected string _profileStringValue = "Value";
	protected float _profilenumericalValue = 0.0f;
	protected bool _profileOverwrite = true;
	#endregion

	// Use this for initialization
	void Start()
	{
		_config = LugusConfig.use;
		_config.ReloadDefaultProfiles();
	}

	void OnGUI()
	{
		int xPos = 10, yPos = 10;
		GUITestBox(xPos, yPos);

		xPos += BoxMargin + BoxWidth;
		GUIProfilesBox(xPos, yPos);

		xPos += BoxMargin + BoxWidth;
		GUIProfileBox(xPos, yPos, _config.User);
	}

	// Draws a box to test the set and get methods
	void GUITestBox(int xPos, int yPos)
	{
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 40), "Test Methods");

		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Start test"))
		{
			if (TestRandomValues())
				Debug.Log("All tests passed.");
			else
				Debug.Log("Not all tests passed.");
		}
	}

	// Draws the layout of all available profiles in LugusConfig
	void GUIProfilesBox(int xPos, int yPos)
	{

		// Display all profiles
		int totalItems = _config.AllProfiles.Count + 1;

		GUI.Box(new Rect(xPos, yPos, BoxWidth, totalItems * 20), "All Profiles");

		yPos += 20;
		foreach (ILugusConfigProfile profile in _config.AllProfiles)
		{
			if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), profile.Name))
				_config.User = profile;

			yPos += 20;
		}

		AddOrEditProfileBox(xPos, yPos);

	}

	void AddOrEditProfileBox(int xPos, int yPos)
	{
		// Add or edit profile-box
		yPos += 20;
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 60), "Add or edit profile");

		yPos += 20;
		_newProfileName = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _newProfileName);

		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth / 2, 20), "Add Profile"))
			_config.AllProfiles.Add(new LugusConfigProfileDefault(_newProfileName));

		if ((GUI.Button(new Rect(xPos + BoxWidth / 2, yPos, BoxWidth / 2, 20), "Edit Profile") && (_config.User != null)))
			_config.User.Name = _newProfileName;

		LoadAndStoreProfilesBox(xPos, yPos);
	}

	void LoadAndStoreProfilesBox(int xPos, int yPos)
	{
		// Save and load-box
		yPos += 40;
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 100), "Load and Store");

		// Reloads all the profiles found in the Config-folder when pressing the button
		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Reload profiles"))
			_config.ReloadDefaultProfiles();

		// Reload all profiles from XML format
		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Reload profiles from XML"))
			ReloadProfilesXML();

		// Reload all profiles from XML format
		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Reload profiles from JSON"))
			ReloadProfilesJSON();

		// Saves all the profiles to the Config-folder when pressing the button
		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Store profiles"))
			_config.SaveProfiles();
	}

	// Draws the layout of the system profile
	void GUIProfileBox(int xPos, int yPos, ILugusConfigProfile profile)
	{
		// Draw the properties of the system profile
		int totalItems = 1;
		string userName = string.Empty;
		LugusConfigProfileDefault profileDefault = null;

		if (profile != null)
		{
			profileDefault = profile as LugusConfigProfileDefault;
			if (profileDefault != null)
			{
				totalItems += profileDefault.Data.Count;
				userName = profile.Name;
			}
		}

		GUI.Box(new Rect(xPos, yPos, BoxWidth, totalItems * 20), "Current profile: " + userName);

		// Draw the system properties
		if (profile != null)
		{
			yPos += 20;
			foreach (KeyValuePair<string, string> pair in profileDefault.Data)
			{
				if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), pair.Key + ": " + pair.Value))
					_profileKeyString = pair.Key;

				yPos += 20;
			}

			GUIEditProfileBox(xPos, yPos, profile);
		}

	}

	void GUIEditProfileBox(int xPos, int yPos, ILugusConfigProfile profile)
	{
		// Draw the actions for the profile
		yPos += 20;
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 120), "Edit profile");

		yPos += 20;
		string[] editModeStrings = { "String", "Numerical" };
		_profileEditMode = GUI.Toolbar(new Rect(xPos, yPos, BoxWidth, 20), _profileEditMode, editModeStrings);

		yPos += 20;
		_profileKeyString = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _profileKeyString);

		// Either allow a string value-field, or a numerical one
		switch (_profileEditMode)
		{
			case 0:
				yPos += 20;
				_profileStringValue = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _profileStringValue);
				break;
			case 1:
				yPos += 20;
				_profilenumericalValue = GUI.HorizontalSlider(new Rect(xPos, yPos, BoxWidth / 2, 20), _profilenumericalValue, 0.0f, 100.0f);
				GUI.Label(new Rect(xPos + BoxWidth / 2 + 20, yPos, BoxWidth / 2, 20), _profilenumericalValue.ToString());
				break;
		}

		yPos += 20;
		_profileOverwrite = GUI.Toggle(new Rect(xPos, yPos, BoxWidth, 20), _profileOverwrite, "Overwrite?");

		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth / 2, 20), "Add"))
		{
			switch (_profileEditMode)
			{
				case 0:
					profile.SetString(_profileKeyString, _profileStringValue, _profileOverwrite);
					break;
				case 1:
					profile.SetFloat(_profileKeyString, _profilenumericalValue, _profileOverwrite);
					break;
			}
		}

		if (GUI.Button(new Rect(xPos + BoxWidth / 2, yPos, BoxWidth / 2, 20), "Remove"))
			profile.Remove(_profileKeyString);
	}

	// Tests values in the profile by inserting and extracting
	// random values and comparing them. Will output a debug
	// error message when two values are not considered equal.
	bool TestRandomValues()
	{

		bool testResult = true;
		Random.seed = (int)Time.time;

		// A series of random test to put and pull values from a profile
		LugusConfigProfileDefault profile = new LugusConfigProfileDefault("Test");

		for (int i = 0; i < 10; i++)
		{
			string key = string.Empty;
			float random;
	
			// Test booleans
			random = Random.value;
			bool insertBool;
			if (random < 0.5f)
				insertBool = false;
			else
				insertBool = true;

			key = "boolValue" + i;
			profile.SetBool(key, insertBool);
			bool extractBool = profile.GetBool(key);
			if (insertBool != extractBool)
			{
				testResult = false;
				Debug.LogError("Extracted bool value did not match the inserted value. Inserted: " + insertBool + " Extracted: " + extractBool);
			}

			// Test integers
			int insertInt = Random.Range(0, 100);
			key = "intValue" + i;
			profile.SetInt(key, insertInt);
			int extractInt = profile.GetInt(key, -1);
			if (insertInt != extractInt)
			{
				testResult = false;
				Debug.LogError("Extracted int value did not match the inserted value. Inserted: " + insertInt + " Extracted: " + extractInt);
			}

			// Test floats
			float insertFloat = Random.Range(0.0f, 100.0f);
			key = "floatValue" + i;
			profile.SetFloat(key, insertFloat);
			float extractFloat = profile.GetFloat(key, -1.0f);
			if (!Mathf.Approximately(insertFloat, extractFloat))
			{
				testResult = false;
				Debug.LogError("Extracted float value did not match the inserted value. Inserted: " + insertFloat + " Extracted: " + extractFloat);
			}
		}

		return testResult;
	}

	private void ReloadProfilesXML()
	{

		// Temporarily replace the list of providers with XML providers
		LugusConfigProviderDefault xmlProvider = new LugusConfigProviderDefault(Application.dataPath + "/Config/", new LugusConfigDataHelperXML());
		List<ILugusConfigProvider> xmlProviders = new List<ILugusConfigProvider>();
		xmlProviders.Add(xmlProvider);

		foreach (ILugusConfigProfile iprofile in _config.AllProfiles)
		{
			LugusConfigProfileDefault profile = iprofile as LugusConfigProfileDefault;

			if (profile == null)
				break;

			List<ILugusConfigProvider> originalProviders = profile.Providers;
			profile.Providers = xmlProviders;

			profile.Load();

			profile.Providers = originalProviders;
		}
	}

	private void ReloadProfilesJSON()
	{
		// Temporarily replace the list of providers with JSON providers
		LugusConfigProviderDefault jsonProvider = new LugusConfigProviderDefault(Application.dataPath + "/Config/", new LugusConfigDataHelperXML());
		List<ILugusConfigProvider> jsonProviders = new List<ILugusConfigProvider>();
		jsonProviders.Add(jsonProvider);

		foreach (ILugusConfigProfile iprofile in _config.AllProfiles)
		{
			LugusConfigProfileDefault profile = iprofile as LugusConfigProfileDefault;

			if (profile == null)
				break;

			List<ILugusConfigProvider> originalProviders = profile.Providers;
			profile.Providers = jsonProviders;

			profile.Load();

			profile.Providers = originalProviders;
		}
	}
}
