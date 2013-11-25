using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigTester : MonoBehaviour
{

	#region Properties
	public int BoxMargin = 10;	// Horizontal margin between each box
	public int BoxWidth = 200;	// Width of each box in pixels

	public int RandomSeed = 1;

	private LugusConfigDefault _config = null;
	private string _newProfileName = "Enter a name";

	private int _profileEditMode = 0;
	private string _profileKeyString = "Key";
	private string _profileStringValue = "Value";
	private float _profilenumericalValue = 0.0f;
	private bool _profileOverwrite = true;
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

		// Add a profile-box
		yPos += 20;
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 60), "Add Profile");

		yPos += 20;
		_newProfileName = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _newProfileName);

		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Add Profile"))
			_config.AllProfiles.Add(new LugusConfigProfileDefault(_newProfileName));

		// Save and load-box
		yPos += 40;
		GUI.Box(new Rect(xPos, yPos, BoxWidth, 60), "Load and Store");

		// Reloads all the profiles found in the Config-folder when pressing the button
		yPos += 20;
		if (GUI.Button(new Rect(xPos, yPos, BoxWidth, 20), "Reload profiles"))
			_config.ReloadDefaultProfiles();

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

			// Draw the actions for the profile
			yPos += 20;
			GUI.Box(new Rect(xPos, yPos, BoxWidth, 120), "Edit profile");

			yPos += 20;
			string[] editModeStrings = {"String", "Numerical"};
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

	}

	bool TestRandomValues()
	{

		bool testResult = true;
		Random.seed = RandomSeed;

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
			if (insertFloat - extractFloat != 0.0f)
			{
				testResult = false;
				Debug.Log(insertFloat);
				Debug.Log(extractFloat);
				Debug.LogError("Extracted float value did not match the inserted value. Inserted: " + insertFloat + " Extracted: " + extractFloat);
			}

			// test doubles
			double insertDouble = Random.Range(0.0f, 100.0f);
			key = "doubleValue" + i;
			profile.SetDouble(key, insertDouble);
			double extractDouble = profile.GetDouble(key, -1.0f);
			if (insertDouble != extractDouble)
			{
				testResult = false;
				Debug.Log(insertDouble);
				Debug.Log(extractDouble);
				Debug.LogError("Extracted double value did not match the inserted value. Inserted: " + insertDouble + " Extracted: " + extractDouble);
			}
		}

		return testResult;
	}
}
