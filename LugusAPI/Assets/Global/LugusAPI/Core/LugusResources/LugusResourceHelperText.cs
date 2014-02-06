using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LugusResourceHelperText 
{
	public Dictionary<string, string> texts = new Dictionary<string, string>();
	
	public string delimiterLines = "\n";
	public string delimiterFields = "@@@";

	public bool HasText(string key)
	{
		if( texts.Count == 0 )
			return false;
		else
		{
			return texts.ContainsKey(key);
		}
	}

	public string Get(string key, string backupKey)
	{
		if( texts.Count == 0 )
		{
			Debug.LogError("No texts loaded! " + key);
			return "[" + key + " // " + backupKey +"]";
		}
		
		if( texts.ContainsKey(key) )
		{
			return texts[key];
		}
		else
		{
			if( texts.ContainsKey(backupKey) )
			{
				return texts[backupKey];
			}
			else
			{
				Debug.LogError("No entry found for key " + key + " or backup " + backupKey);
				return "[" + key + " // " + backupKey + "]";
			}
		}
	}
	
	public string Get(string key)
	{
		if( texts.Count == 0 )
		{
			Debug.LogError("No texts loaded! " + key);
			return "[" + key + "]";
		}
		
		if( texts.ContainsKey(key) )
		{
			return texts[key];
		}
		else
		{
			Debug.LogError("No entry found for key " + key);
			return "[" + key + "]";
		}
	}
	
	
	
	public void Parse( string text )
	{
		texts.Clear();

		if( string.IsNullOrEmpty(text) )
		{
			Debug.LogError("text source was null or empty!");
			return;
		}

		// ex.
		// <root>
		// <key>value</key>\n
		//</root>
		texts = TinyXmlReader.DictionaryFromXMLString( text );

		/*
		// code for parsing format "key@@@value\n"
		string[] delimterFields2 = new string[1];
		delimterFields2[0] = delimiterFields;

		
		String[] lines = text.Split(delimiterLines[0]);
		
		foreach( String line in lines )
		{
			String[] parts = line.Split(delimterFields2, StringSplitOptions.None);
			
			// completely empty line, skip
			if( parts.Length == 1 )
			{
				continue;
			}
			
			// more than 2 fields in a single line is not supported at this time
			if( parts.Length != 2 )
			{
				Debug.LogError("Line didn't contain 2 but "+ parts.Length +" fields "+ delimiterFields +" : " + line);
				continue;
			}
			
			// make sure key or content is not empty!
			if( parts[0] == "" || parts[1] == "" )
			{
				Debug.LogError("Line key or value is empty : " + line);
				continue;
			}
			
			texts[ parts[0].Trim() ] = parts[1].Trim();
		}
		*/
		
	}
	
	public void Parse( TextAsset text )
	{
		if( text == null )
		{
			texts.Clear();
			return;
		}
		
		Parse ( text.text );
	}
}
