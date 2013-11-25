using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigTester : MonoBehaviour
{

	#region Properties
	public int BoxMargin = 10;	// Horizontal margin between each box
	public int BoxWidth = 200;	// Width of each box in pixels

	private LugusConfigDefault _config = null;
	private string _newProfileName = "Enter a name";
	private string _profileKeyString = "Key";
	private string _profileValueString = "Value";
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
		GUIProfilesBox(xPos, yPos);

		xPos += BoxMargin + BoxWidth;
		GUIProfileBox(xPos, yPos, _config.User);
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
				GUI.Label(new Rect(xPos + 10, yPos, BoxWidth, 20), pair.Key + ": " + pair.Value);
				yPos += 20;
			}

			// Draw the actions for the profile
			yPos += 20;
			GUI.Box(new Rect(xPos, yPos, BoxWidth, 100), "Edit profile");

			yPos += 20;
			_profileKeyString = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _profileKeyString);

			yPos += 20;
			_profileValueString = GUI.TextField(new Rect(xPos, yPos, BoxWidth, 20), _profileValueString);

			yPos += 20;
			_profileOverwrite = GUI.Toggle(new Rect(xPos, yPos, BoxWidth, 20), _profileOverwrite, "Overwrite?");

			yPos += 20;
			if (GUI.Button(new Rect(xPos, yPos, BoxWidth / 2, 20), "Add"))
				profile.SetString(_profileKeyString, _profileValueString, _profileOverwrite);

			if (GUI.Button(new Rect(xPos + BoxWidth / 2, yPos, BoxWidth / 2, 20), "Remove"))
				profile.Remove(_profileKeyString);
		}

	}
}
