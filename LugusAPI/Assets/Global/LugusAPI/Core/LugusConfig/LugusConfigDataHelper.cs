using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public interface ILugusConfigDataHelper
{

	string FileExtension { get; set; }

	Dictionary<string, string> ParseFrom(string rawdata);

	string ParseTo(Dictionary<string, string> data);

}

public class LugusConfigDataHelperXML : ILugusConfigDataHelper
{

	#region Properties
	public virtual string FileExtension
	{
		get
		{
			return _fileExtension;
		}
		set
		{
			_fileExtension = value;
		}
	}

	[SerializeField]
	private string _fileExtension = ".xml";
	#endregion

	// Parse flat xml data of the form: <key>value</key>
	public Dictionary<string, string> ParseFrom(string rawdata)
	{

		Dictionary<string, string> data = new Dictionary<string, string>();

		TinyXmlReader xmlreader = new TinyXmlReader(rawdata);

		// While still reading valid data
		while (xmlreader.Read())
		{

			// Read the contents of the element only when an opening tag is found
			if (xmlreader.isOpeningTag)
				data.Add(xmlreader.tagName, xmlreader.content);

		}

		return data;

	}

	public string ParseTo(Dictionary<string, string> data)
	{

		if (data == null)
			return string.Empty;

		List<string> keys = data.Keys.ToList();
		List<string> values = data.Values.ToList();
		string rawdata = string.Empty;

		for (int i = 0; i < data.Count; ++i)
		{
			string key = keys[i], value = values[i];
			rawdata += "<" + key + ">" + value + "</" + key + ">\n";
		}

		return rawdata;
	}

}

public class LugusConfigDataHelperJSON : ILugusConfigDataHelper
{

	#region Properties
	public virtual string FileExtension
	{
		get
		{
			return _fileExtension;
		}
		set
		{
			_fileExtension = value;
		}
	}

	[SerializeField]
	private string _fileExtension = ".json";
	#endregion

	public Dictionary<string, string> ParseFrom(string rawdata)
	{

		Dictionary<string, string> data = new Dictionary<string, string>();

		return data;
	}

	public string ParseTo(Dictionary<string, string> data)
	{
		string rawdata = string.Empty;

		return rawdata;
	}

}
