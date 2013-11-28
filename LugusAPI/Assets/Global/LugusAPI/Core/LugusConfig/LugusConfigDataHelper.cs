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
	protected string _fileExtension = ".xml";
	#endregion

	// Parse flat xml data of the form: <key>value</key>
	// The data is considered to be found at depth level 1 (the root and header are found on depth level 0).
	public Dictionary<string, string> ParseFrom(string rawdata)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();

		TinyXmlReader xmlreader = new TinyXmlReader(rawdata);

		int depth = -1;

		// While still reading valid data
		while (xmlreader.Read())
		{

			if (xmlreader.tagType == TinyXmlReader.TagType.OPENING)
				++depth;
			else if (xmlreader.tagType == TinyXmlReader.TagType.CLOSING)
				--depth;

			// Useful data is found at depth level 1
			if ((depth == 1) && (xmlreader.tagType == TinyXmlReader.TagType.OPENING))
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
		rawdata += "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += "<Config>\r\n";

		for (int i = 0; i < data.Count; ++i)
		{
			string key = keys[i], value = values[i];
			rawdata += "\t<" + key + ">" + value + "</" + key + ">\r\n";
		}

		rawdata += "<Config>\r\n";
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
	protected string _fileExtension = ".json";
	#endregion

	public Dictionary<string, string> ParseFrom(string rawdata)
	{
		JSONObject jsonObj = new JSONObject(rawdata);
		Dictionary<string, string> data = jsonObj.ToDictionary();

		return data;
	}

	public string ParseTo(Dictionary<string, string> data)
	{
		string rawdata = string.Empty;

		JSONObject jsonObj = new JSONObject(data);
		rawdata = jsonObj.ToString();

		return rawdata;
	}

}
