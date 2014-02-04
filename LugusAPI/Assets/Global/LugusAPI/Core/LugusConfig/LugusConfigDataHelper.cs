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
		return TinyXmlReader.DictionaryFromXMLString( rawdata ); 
	}

	public string ParseTo(Dictionary<string, string> data)
	{
		return TinyXmlReader.DictionaryToXMLString( data, "Config" );
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
