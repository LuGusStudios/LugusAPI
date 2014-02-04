using UnityEngine;
#if !UNITY_WEBPLAYER
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;

public class LugusConfig : LugusSingletonRuntime<LugusConfigDefault>
{
	
}

public class LugusConfigDefault : MonoBehaviour
{
	#region Properties
	public ILugusConfigProfile User
	{
		get
		{
			if (_currentUser == null)
				ReloadDefaultProfiles();

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
			if (_systemProfile == null)
				ReloadDefaultProfiles();

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
	#endregion

	#region Protected
	protected ILugusConfigProfile _systemProfile = null;	// Profile holding system variables, i.e. graphics and sound options.
	protected ILugusConfigProfile _currentUser = null;		// Profile holding user specific variables, i.e. character health and strength.
	protected List<ILugusConfigProfile> _profiles = new List<ILugusConfigProfile>();	// All profiles registered in this configuration, incl. system profile.
	#endregion

	#if !UNITY_WEBPLAYER
	// Reload all profiles found in the Config folder.
	public void ReloadDefaultProfiles()
	{
		_profiles = new List<ILugusConfigProfile>();
		_systemProfile = null;
		_currentUser = null;

		// Load the profiles found in the config folder of the application datapath
		// and try to set the latest user as the current user.
		// If no profiles could be found in the folder,
		// then create a default system and user profile.

		string configpath = Application.dataPath + "/Config/";
		DirectoryInfo directoryInfo = new DirectoryInfo(configpath);
		FileInfo[] files = directoryInfo.GetFiles("*.xml");

		if (files.Length > 0)
		{
			// Create and load profiles
			foreach (FileInfo fileInfo in files)
			{
				string profileName = fileInfo.Name.Remove(fileInfo.Name.LastIndexOf(".xml"));
				LugusConfigProfileDefault profile = new LugusConfigProfileDefault(profileName);
				profile.Load();

				if (profileName == "System")
					_systemProfile = profile;

				_profiles.Add(profile);
			}
		}

		if (_systemProfile == null)
		{
			LugusConfigProfileDefault sysProfile = new LugusConfigProfileDefault("System");
			this.System = sysProfile;
			_profiles.Add(sysProfile);
		}
		else
		{
			string lastestUser = _systemProfile.GetString("User.Latest", string.Empty);
			if (!string.IsNullOrEmpty(lastestUser))
				_currentUser = _profiles.Find(profile => profile.Name == lastestUser);
		}

		if (_currentUser == null)
		{
			_currentUser = new LugusConfigProfileDefault("Player");
			_profiles.Add(_currentUser);
		}
	}
#else

	// Reload all profiles found in the Config folder.
	public void ReloadDefaultProfiles()
	{
		_profiles = new List<ILugusConfigProfile>();
		_systemProfile = null;
		_currentUser = null;

		// TODO: in the case of playerprefs, we have to save a separate playerprefs key indicating which profiles are available
		// for now, we just take System

		_systemProfile = new LugusConfigProfileDefault("System", new LugusConfigProviderPlayerPrefs("System") );
		_systemProfile.Load();
		_profiles.Add( _systemProfile );

		string lastestUser = _systemProfile.GetString("User.Latest", string.Empty);
		if (!string.IsNullOrEmpty(lastestUser))
		{
			_currentUser = new LugusConfigProfileDefault(lastestUser, new LugusConfigProviderPlayerPrefs(lastestUser) );
		}
		else
		{
			_currentUser = new LugusConfigProfileDefault("Player", new LugusConfigProviderPlayerPrefs("Player") );
		}

		_currentUser.Load();

		_profiles.Add(_currentUser);
	}
#endif

	public void SaveProfiles()
	{

		if ((_systemProfile != null) && (_currentUser != null))
			_systemProfile.SetString("User.Latest", _currentUser.Name, true);

		foreach (ILugusConfigProfile profile in _profiles)
			profile.Store();
	}

	public ILugusConfigProfile FindProfile(string name)
	{
		return _profiles.Find(profile => profile.Name == name);
	}

}