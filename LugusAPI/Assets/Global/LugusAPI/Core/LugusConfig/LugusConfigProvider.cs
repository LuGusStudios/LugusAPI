using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public interface ILugusConfigProvider
{

	string URL { get; set; }

	Dictionary<string, string> Load(string key);

	void Store(Dictionary<string, string> data, string key);
}

public class LugusConfigProviderDefault : ILugusConfigProvider
{

	#region Properties
	public virtual string URL
	{
		get
		{
			return _URL;
		}
		set
		{
			_URL = value;
		}
	}
	private List<ILugusConfigDataHelper> parsers = null;

	[SerializeField]
	private string _URL = "";
	#endregion

	// Adds an XML parser as standard parser.
	public LugusConfigProviderDefault(string url)
	{
		_URL = url;

		parsers = new List<ILugusConfigDataHelper>();
		parsers.Add(new LugusConfigDataHelperXML());

	}

	// Use the given parser to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, ILugusConfigDataHelper parser)
	{
		_URL = url;
		parsers = new List<ILugusConfigDataHelper>();
		parsers.Add(parser);

	}

	// Use the given list of parsers to parse data loaded by the provider.
	public LugusConfigProviderDefault(string url, List<ILugusConfigDataHelper> parsers)
	{
		_URL = url;
		this.parsers = parsers;
	}

	public Dictionary<string, string> Load(string key)
	{

		// Find config files that can be parsed by the parsers in the list.
		// Data parsed by multiple parsers will be merged.
		// Data with the same key value from different parsers will result in the value of the parser that parsed the key last.
		// So the order in which the parsers are used is important!

		Dictionary<string, string> data = new Dictionary<string, string>();

		foreach (ILugusConfigDataHelper parser in parsers)
		{
			
			string fullpath = URL + key + parser.FileExtension;

			// Read the raw data out of the file
			StreamReader reader = new StreamReader(fullpath, Encoding.Default);
			string rawdata = reader.ReadToEnd();
			reader.Close();

			Dictionary<string, string> partialData = parser.ParseFrom(rawdata);

			// Merge with main data
			foreach (KeyValuePair<string, string> entry in partialData)
				data.Add(entry.Key, entry.Value);
		}

		return data;
	}

	public void Store(Dictionary<string, string> data, string key)
	{
		foreach (ILugusConfigDataHelper parser in parsers)
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