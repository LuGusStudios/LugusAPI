using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LugusLang : LugusSingletonExisting<LugusLangDefault> 
{
}

// LugusLangDefinition : Dictionary<> texts, Diciontary<> audio, Dictionary<> textures
// manager hefet dan langdefs en laadt ze in / switcht actieve
// LangDefs zelf laden de nodige dingen in (bijv. uit Resources, via WWW direct, via asset bundle) en geven ze terug

public class LugusLangDefault : MonoBehaviour
{
	public TextAsset textSource = null; 
	
	public Dictionary<string, string> texts = new Dictionary<string, string>();
	
	public string Get(string key)
	{
		if( texts.Count == 0 )
		{
			LoadTexts();
		}
		
		if( texts.ContainsKey(key) )
		{
			return texts[key];
		}
		else
		{
			Debug.LogError("LuGusLang:Get : no entry found for key " + key);
			return "[" + key + "]";
		}
	}
	
	public void LoadTexts()
	{
		if( textSource == null )
		{
			Debug.LogError("LuGusLang:LoadTexts : no textSource found! ");
			return;
		}
		
		String[] lines = textSource.text.Split("\n"[0]);
		Debug.LogWarning("LuGusLang:LoadTexts : found " + lines.Length + " lines");
		
		foreach( String line in lines )
		{
			String[] parts = line.Split(";"[0]);
			
			if( parts.Length != 2 )
				continue;
			
			// empty line
			if( parts[0] == "" )
				continue;
			
			Debug.LogWarning("LuGusLang:LoadTexts : adding " + parts[0] + " = " + parts[1] );
			texts[ parts[0].Trim() ] = parts[1].Trim();
		}
	}
	
	void Awake()
	{
		LoadTexts();
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
