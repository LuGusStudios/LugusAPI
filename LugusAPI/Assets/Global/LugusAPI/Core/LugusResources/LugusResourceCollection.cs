using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILugusResourceCollection
{	
	string URL { get; set; } 
	event Lugus.OnResourcesReloaded onResourcesReloaded;
	
	Texture2D GetTexture(string key);
	Sprite GetSprite(string key);
	AudioClip GetAudio(string key);
	string GetText(string key);
	string GetText(string key, string backupKey);
	
	void Reload();
}

// just loads an asset from Resources folder using BaseURL, easy peasy
public class LugusResourceCollectionDefault : ILugusResourceCollection
{
	
	public List<ILugusResourceProvider> providers = null;
	
	protected LugusResourceHelperText textHelper = null;
	public event Lugus.OnResourcesReloaded onResourcesReloaded;
	
	// Constructor
	public LugusResourceCollectionDefault(string url)
	{
		LoadResourceProviders();
		URL = url;
		Reload();
	}
	
	public LugusResourceCollectionDefault()
	{
		
	}
	
	protected virtual void LoadResourceProviders()
	{
		providers = new List<ILugusResourceProvider>();
		providers.Add( new LugusResourceProviderDisk() );
		
		textHelper = new LugusResourceHelperText();
	}
	
	
	[SerializeField]
	protected string _URL = "";
	public virtual string URL
	{
		get{ return _URL; }
		set{ _URL = value; }
	}
	
	public void Reload()
	{
		UpdateText();
		
		if( onResourcesReloaded != null )
			onResourcesReloaded();
	}
	
	public Texture2D GetTexture(string key)
	{	
		Texture2D output = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			output = provider.GetTexture(_URL, key);
			if( output != null )
				break;
		}
		
		if( output == null )
		{
			Debug.LogError(" : Texture " + _URL + " " + key + " was not found!");
			output = LugusResources.use.errorTexture; 
		}
		
		return output;
	}
	
	public Sprite GetSprite(string key)
	{	
		Sprite output = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			output = provider.GetSprite(_URL, key);
			if( output != null )
				break;
		}
		
		if( output == null )
		{
			Debug.LogError(" : Sprite " + _URL + " " + key + " was not found!");
			output = LugusResources.use.errorSprite; 
		}
		
		return output;
	}
	
	
	public AudioClip GetAudio(string key)
	{
		AudioClip output = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			output = provider.GetAudio(_URL, key);
			if( output != null )
				break;
		}
		
		if( output == null )
		{
			Debug.LogError(" : AudioClip " + _URL + " " + key + " was not found!");
			output = LugusResources.use.errorAudio;
		}
		
		return output;
	}
	
	protected void UpdateText()
	{
		TextAsset txt = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			txt = provider.GetText(_URL, "texts");
			if( txt != null )
				break;
		}
		
		textHelper.Parse( txt );
	}
	
	public string GetText(string key)
	{
		return textHelper.Get ( key );
	}
	public string GetText(string key, string backupKey)
	{
		return textHelper.Get ( key, backupKey );
	}
}

// very similar to Default, but allows changing of the BaseURL to accomodate a specific language
public class LugusResourceCollectionLocalized : LugusResourceCollectionDefault
{
	public LugusResourceCollectionLocalized(string url)
	{
		LoadResourceProviders();
		BaseURL = url;
	}
	
	protected string _langID = "en";
	public string LangID
	{
		get{ return _langID; }
		set
		{ 
			_langID = value; 
			_URL = _baseURL + _langID + "/"; 
			Reload(); 
			
			Debug.Log ("Changed language to " + _langID);
		}
	}
	
	protected string _baseURL = "";
	public string BaseURL
	{
		get{ return _baseURL + _langID; }
		set{ _baseURL = value; _URL = _baseURL + _langID + "/"; Reload(); }
	}
	
	public override string URL
	{
		get{ return _baseURL + _langID; }
		set{ BaseURL = value; }
	}
}

// loads an AssetBundle 
/*
public class LugusResourceCollectionBundle : ILugusResourceCollection
{
	
}
*/