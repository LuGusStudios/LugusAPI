using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LugusConfig : LugusSingletonExisting<LugusConfigDefault>
{

}

public class LugusConfigDefault : MonoBehaviour
{
	#region Properties

	public ILugusConfigProfile User
	{
		get
		{
			return _currentUser;
		}
		set
		{
			_currentUser = value;
		}
	}
	public ILugusConfigProfile System
	{
		get
		{
			return _systemProfile;
		}
		set
		{
			_systemProfile = value;
		}
	}
	public List<ILugusConfigProfile> AllProfiles
	{
		get
		{
			return _profiles;
		}
		set
		{
			_profiles = value;
		}
	}


	private ILugusConfigProfile _systemProfile = null;
	private ILugusConfigProfile _currentUser = null;
	private List<ILugusConfigProfile> _profiles = null;
	#endregion

	// Reload all profiles found in the Config folder.
	public void ReloadDefaultProfiles()
	{

		// Load the profiles found in the config folder of the application datapath and try to set the latest user as the current user.
		// If no profiles could be found in the folder, then create a default system and user profile.

		// Clear the current profiles registered
		_profiles = new List<ILugusConfigProfile>();
		_systemProfile = null;
		_currentUser = null;

		Debug.Log(Application.dataPath);

		string configpath = Application.dataPath + "Config/";
		string[] configFileNames = Directory.GetFiles(configpath, ".xml");


		if (configFileNames.Length > 0)
		{

			// Create and load profiles
			foreach (string configFileName in configFileNames)
			{
				string profileName = configFileName.Remove(configFileName.LastIndexOf(".xml"));
				LugusConfigProfileDefault profile = new LugusConfigProfileDefault(profileName);

				if (profileName == "System")
					_systemProfile = profile;

				_profiles.Add(profile);
			}

			// If a system profile was found, then search for the latest user
			if (_systemProfile != null)
			{
				string lastestUser = _systemProfile.GetString("User.Latest", string.Empty);
				if (!string.IsNullOrEmpty(lastestUser))
					_currentUser = _profiles.Find(profile => profile.Name == lastestUser);
			}
			else
				System = new LugusConfigProfileDefault("System");

			// If no current user is set, create a default user profile
			_currentUser = new LugusConfigProfileDefault("Player");
			_profiles.Add(_currentUser);

		}
		

	}

	public void SaveProfiles()
	{

		// Go over all profiles, store them, and build up the list of profile keys
		string profileKeys = string.Empty;
		for (int i = 0; i < _profiles.Count; ++i)
		{

			ILugusConfigProfile profile = _profiles[i];
			profile.Store();
			
			profileKeys += profile.Name;

			if (i < (_profiles.Count - 1))
				profileKeys += "@@@";

		}

		// Store the list of profile keys in PlayerPrefs
		PlayerPrefs.SetString("Profile.Users", profileKeys);

		// Save the profile key of the current user in PlayerPrefs
		PlayerPrefs.SetString("Profile.Latest", _currentUser.Name);

		// Save the system profile
		_systemProfile.Store();
	}

	public ILugusConfigProfile FindProfile(string name)
	{
		return _profiles.Find(profile => profile.Name == name);
	}

}