using UnityEngine;

#if !UNITY_WEBPLAYER
using System.IO;
#endif
using System.Text;
using System.Collections;
using System.Collections.Generic;

public interface ILugusConfigProvider
{

	string URL { get; set; }

	Dictionary<string, string> Load(string key);

	void Store(Dictionary<string, string> data, string key);
}

#if !UNITY_WEBPLAYER
public class LugusConfigProviderDefault : ILugusConfigProvider
{

	#region Properties
	public virtual string URL
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}
	public List<ILugusConfigDataHelper> Parsers
	{
		get
		{
			return _parsers;
		}
		set
		{
			_parsers = value;
		}
	}

	[SerializeField]
	protected string _url = "";

	protected List<ILugusConfigDataHelper> _parsers = null;
	#endregion

	// Adds an XML parser as standard parser.
	public LugusConfigProviderDefault(string url)
	{
		_url = url;

		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(new LugusConfigDataHelperXML());
		//_parsers.Add(new LugusConfigDataHelperJSON());
	}

	// Use the given parser to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, ILugusConfigDataHelper parser)
	{
		_url = url;
		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(parser);

	}

	// Use the given list of parsers to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, List<ILugusConfigDataHelper> parsers)
	{
		_url = url;
		_parsers = parsers;
	}

	public Dictionary<string, string> Load(string key)
	{
		// Find config files that can be parsed by the parsers in the list.
		// Data parsed by multiple parsers will be merged.
		// Data with the same key from different parsers will result in the value of the parser that parsed the key last.
		// So the order in which the parsers are added is important!

		Dictionary<string, string> data = new Dictionary<string, string>();

		foreach (ILugusConfigDataHelper parser in _parsers)
		{
			string fullpath = URL + key + parser.FileExtension;

			if (!Directory.Exists(URL))
				return data;

			// Read the raw data out of the file
			if (File.Exists(fullpath))
			{
				StreamReader reader = new StreamReader(fullpath, Encoding.Default);
				string rawdata = reader.ReadToEnd();
				reader.Close();

				Dictionary<string, string> partialData = parser.ParseFrom(rawdata);

				// Merge with data and resolve duplicate keys
				foreach (KeyValuePair<string, string> entry in partialData)
					data.Add(entry.Key, entry.Value);
			}
		}
		return data;
	}

	public void Store(Dictionary<string, string> data, string key)
	{
		if (!Directory.Exists(URL))
		{
			try
			{
				Directory.CreateDirectory(URL);
			}
			catch (System.Exception)
			{
				return;
			}
		}

		foreach (ILugusConfigDataHelper parser in _parsers)
		{
			string rawData = parser.ParseTo(data);

			// Compose the full path to write away to profile
			string fullpath = URL + key + parser.FileExtension;

			// Write the raw data out to a file
			StreamWriter writer = new StreamWriter(fullpath);
			writer.Write(rawData);
			writer.Close();
		}
	}

}
#endif

public class LugusConfigProviderPlayerPrefs : ILugusConfigProvider
{
	
	#region Properties
	public virtual string URL
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}
	public List<ILugusConfigDataHelper> Parsers
	{
		get
		{
			return _parsers;
		}
		set
		{
			_parsers = value;
		}
	}
	
	[SerializeField]
	protected string _url = "";
	
	protected List<ILugusConfigDataHelper> _parsers = null;
	#endregion
	
	// Adds an XML parser as standard parser.
	public LugusConfigProviderPlayerPrefs(string url) 
	{
		_url = url;
		
		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(new LugusConfigDataHelperXML());
		//_parsers.Add(new LugusConfigDataHelperJSON());
		
	}
	
	// Use the given parser to parse data loaded by the provider.
	public LugusConfigProviderPlayerPrefs(string url, ILugusConfigDataHelper parser)
	{
		_url = url;
		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(new LugusConfigDataHelperXML());

		// for now we only accept XML serialiation
		//_parsers.Add(parser);
		
	}
	
	// Use the given list of parsers to parse data loaded by the provider.
	public LugusConfigProviderPlayerPrefs(string url, List<ILugusConfigDataHelper> parsers)
	{
		_url = url;
		_parsers = new List<ILugusConfigDataHelper>();
		_parsers.Add(new LugusConfigDataHelperXML());

		//_parsers = parsers;
	}
	
	public Dictionary<string, string> Load(string key)
	{
		// Find config files that can be parsed by the parsers in the list.
		// Data parsed by multiple parsers will be merged.
		// Data with the same key from different parsers will result in the value of the parser that parsed the key last.
		// So the order in which the parsers are added is important!
		
		Dictionary<string, string> data = new Dictionary<string, string>();

		Debug.LogWarning("PlayerPrefs load " + key + " at " + PlayerPrefs.GetString(URL) );

		if( PlayerPrefs.HasKey(URL) )
		{
			//foreach (ILugusConfigDataHelper parser in _parsers)
			//{
				Dictionary<string, string> partialData = _parsers[0].ParseFrom( PlayerPrefs.GetString(URL) );
				
				// Merge with data and resolve duplicate keys
				foreach (KeyValuePair<string, string> entry in partialData)
					data.Add(entry.Key, entry.Value);
			//}
		}

		return data;
	}
	
	public void Store(Dictionary<string, string> data, string key)
	{
		Debug.LogWarning("PlayerPrefs : storing " + key + " as " + _parsers[0].ParseTo(data) );
		PlayerPrefs.SetString(URL, _parsers[0].ParseTo(data) );
	}
	
}