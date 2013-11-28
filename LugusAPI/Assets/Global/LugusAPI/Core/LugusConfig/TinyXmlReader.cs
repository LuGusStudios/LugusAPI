using UnityEngine;
using System.Collections;

/* 
	Source: http://wiki.unity3d.com/index.php?title=TinyXmlReader 
	See site for usage. 	
*/

public class TinyXmlReader
{
	private string xmlString = "";
	private int idx = 0;

	public TinyXmlReader(string newXmlString)
	{
		xmlString = newXmlString;
	}

	public string tagName = "";
	public bool isOpeningTag = false;
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

	public bool Read()
	{
		if (idx > -1)
			idx = xmlString.IndexOf("<", idx);

		if (idx == -1)
		{
			return false;
		}
		++idx;

		// skip attributes, don't include them in the name!
		int endOfTag = IndexOf('>', idx);
		int endOfName = IndexOf(' ', idx);
		if ((endOfName == -1) || (endOfTag < endOfName))
		{
			endOfName = endOfTag;
		}

		if (endOfTag == -1)
		{
			return false;
		}

		tagName = xmlString.Substring(idx, endOfName - idx);

		idx = endOfTag;

		// check if a closing tag
		if (tagName.StartsWith("/"))
		{
			isOpeningTag = false;
			tagName = tagName.Remove(0, 1); // remove the slash
		}
		else
		{
			isOpeningTag = true;
		}

		// if an opening tag, get the content
		if (isOpeningTag)
		{
			int startOfCloseTag = xmlString.IndexOf("<", idx);
			if (startOfCloseTag == -1)
			{
				return false;
			}

			content = xmlString.Substring(idx + 1, startOfCloseTag - idx - 1);
			content = content.Trim();
		}

		return true;
	}

	// returns false when the endingTag is encountered
	public bool Read(string endingTag)
	{
		bool retVal = Read();
		if (tagName == endingTag && !isOpeningTag)
		{
			retVal = false;
		}
		return retVal;
	}
}