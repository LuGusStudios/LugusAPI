using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/* 
	Source: http://wiki.unity3d.com/index.php?title=TinyXmlReader 
	See site for usage.
	Extended to handle comments and headers.
*/
using System.Linq;

public class TinyXmlReader
{
	private string xmlString = "";
	private int idx = 0;

	public TinyXmlReader(string newXmlString)
	{
		xmlString = newXmlString;
	}

	public enum TagType { OPENING = 0, CLOSING = 1, COMMENT = 2, HEADER = 3};

	public string tagName = "";
	public TagType tagType = TagType.OPENING;
	public string content = "";


	// properly looks for the next index of _c, without stopping at line endings, allowing tags to be break lines	
	int IndexOf(char _c, int _i)
	{
		int i = _i;
		while (i < xmlString.Length)
		{
			if (xmlString[i] == _c)
				return i;

			++i;
		}

		return -1;
	}

	int IndexOf(string _s, int _i)
	{
		if (string.IsNullOrEmpty(_s))
			return -1;

		int i = _i;
		while (i < (xmlString.Length - _s.Length))
		{
			if (xmlString.Substring(i, _s.Length) == _s)
				return i;

			++i;
		}

		return -1;
	}

	string ExtractCDATA(int _i)
	{
		return string.Empty;
	}

	public bool Read()
	{
		if (idx > -1)
			idx = xmlString.IndexOf("<", idx);

		if (idx == -1)
		{
			return false;
		}
		++idx;

		int endOfTag = IndexOf('>', idx);
		if (endOfTag == -1)
			return false;

		// All contents of the tag, incl. name and attributes
		string tagContents = xmlString.Substring(idx, endOfTag - idx);

		int endOfName = IndexOf(' ', idx);
		if ((endOfName == -1) || (endOfTag < endOfName))
			endOfName = endOfTag;

		tagName = xmlString.Substring(idx, endOfName - idx);
		idx = endOfTag;

		// Fill in the tag name
		if (tagName.StartsWith("/"))
		{
			tagType = TagType.CLOSING;
			tagName = tagName.Remove(0, 1);	// Remove the "/" at the front
		}
		else if (tagName.StartsWith("?"))
		{
			tagType = TagType.HEADER;
			tagName = tagName.Remove(0, 1);	// Remove the "?" at the front
		}
		else if(tagName.StartsWith("!--"))
		{
			tagType = TagType.COMMENT;
			tagName = string.Empty;	// A comment doesn't have a tag name
		}
		else
		{
			tagType = TagType.OPENING;
		}

		// Set the contents of the tag with respect to the type of the tag
		switch (tagType)
		{
			case TagType.OPENING:
				content = xmlString.Substring(idx + 1);

				int startOfCloseTag = IndexOf("<", idx);
				if (startOfCloseTag == -1)
					return false;
				
				// Check that the startOfCloseTag is not actually the start of a tag containing CDATA
				if (xmlString.Substring(startOfCloseTag, 9) == "<![CDATA[")
				{
					int startOfCDATA = startOfCloseTag;
					int endOfCDATA = IndexOf("]]>", startOfCDATA + 9);
					startOfCloseTag = IndexOf("<", endOfCDATA + 3);

					if (endOfCDATA == -1)
						return false;

					string CDATAContent = xmlString.Substring(startOfCDATA + 9, endOfCDATA - (startOfCDATA + 9));
					string preContent = xmlString.Substring(idx + 1, startOfCDATA - (idx + 1));
					string postContent = xmlString.Substring(endOfCDATA + 3, startOfCloseTag - (endOfCDATA + 3));

					content = preContent + CDATAContent + postContent;
					
					idx = startOfCloseTag;
				}
				else
				{
					content = xmlString.Substring(idx + 1, startOfCloseTag - idx - 1);
				}

				break;
			case TagType.COMMENT:
				if ((tagContents.Length - 5) < 0)
					return false;

				content = tagContents.Substring(3, tagContents.Length - 5);
				break;
			case TagType.HEADER:
				if ((tagContents.Length - 1) < 0)
					return false;

				content = tagContents.Substring(tagName.Length + 1, tagContents.Length - tagName.Length - 2);
				break;
			default:
				content = string.Empty;
				break;
		}

		return true;
	}

	// returns false when the endingTag is encountered
	public bool Read(string endingTag)
	{
		bool retVal = Read();
		if ((tagName == endingTag) && (tagType == TagType.CLOSING))
		{
			retVal = false;
		}
		return retVal;
	}

	
	public static Dictionary<string, string> DictionaryFromXMLString(string xmlString)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();

		TinyXmlReader xmlreader = new TinyXmlReader(xmlString);
		
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
			{
				if( !data.ContainsKey(xmlreader.tagName) )
				{
					data.Add(xmlreader.tagName, xmlreader.content);
				}
				else
				{
					Debug.LogWarning("Data already contained key " + xmlreader.tagName + " with value "+ data[ xmlreader.tagName ] +". Replacing value " + xmlreader.content);
					data[ xmlreader.tagName ] = xmlreader.content;
				}
			}
		}

		return data;
	}

	
	public static string DictionaryToXMLString(Dictionary<string, string> data, string root = "Root")
	{
		if (data == null)
			return string.Empty;
		
		List<string> keys = data.Keys.ToList();
		List<string> values = data.Values.ToList();
		
		string rawdata = string.Empty;
		rawdata += "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += "<"+root +">\r\n";
		
		for (int i = 0; i < data.Count; ++i)
		{
			string key = keys[i];
			string value = values[i];

			if (value.Contains('<') || value.Contains('>') || value.Contains('&'))
				value = "<![CDATA[" + value + "]]>"; 


			rawdata += "\t<" + key + ">" + value + "</" + key + ">\r\n";
		}
		
		rawdata += "</"+ root +">\r\n";
		return rawdata;
	}



}