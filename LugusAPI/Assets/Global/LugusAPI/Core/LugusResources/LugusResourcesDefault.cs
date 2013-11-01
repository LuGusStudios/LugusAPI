using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LugusResourcesDelegates
{
	public delegate void OnResourcesReloaded();
}

public interface ILugusResources
{
	event LugusResourcesDelegates.OnResourcesReloaded onResourcesReloaded;
	
	string LanguageID { get; set; }
	
	Texture2D GetTexture(string key);
	AudioClip GetAudio(string key);
	string GetText(string key);
	
	
}

public class LugusResources : LugusSingletonRuntime<LugusResourcesDefault>
{
	/*
	private static ILugusResources _instance = null;
	
	public static ILugusResources use 
	{ 
		get 
		{
			if ( _instance == null )
			{
				_instance = new LugusResourcesDefault();
			}
			
			
			return _instance; 
		}
	}
	
	public static void Change(ILugusResources newInstance)
	{
		_instance = newInstance;
	}
	*/
}

public class LugusResourcesDefault : MonoBehaviour, ILugusResources
{
	public event LugusResourcesDelegates.OnResourcesReloaded onResourcesReloaded;
	
	public List<ILugusResourceProvider> providers = null;
	
	protected LugusResourceProviderDisk diskProvider = null;
	protected LugusResourceHelperText textHelper = null;
	
	protected string sharedBaseURL = "Shared/";
	protected string languagesBaseURL = "Languages/";
	protected string languagesBaseLanguage = "en";
	
	public string LanguageID
	{
		get{ return languagesBaseLanguage; }
		set{ SetBaseLanguage(value); }
	}
	
	protected void FindReferences()
	{ 
		if( this.diskProvider == null )
		{
			this.diskProvider = gameObject.AddComponent<LugusResourceProviderDisk>(); 
		}
		
		if( this.textHelper == null )
		{
			this.textHelper = new LugusResourceHelperText();
		}
		
		if( this.providers == null || this.providers.Count == 0 )
		{
			providers = new List<ILugusResourceProvider>();
			providers.Add( diskProvider );
		}
	}
	
	public void SetBaseLanguage(string langID)
	{
		FindReferences();
		
		languagesBaseLanguage = langID;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			provider.BaseURL = languagesBaseURL + languagesBaseLanguage + "/";
		}
		
		// TODO: now we're loading all text files from disk. If this is no longer the case, we should update this part
		textHelper.Parse( diskProvider.GetText("texts") );
		
		if( onResourcesReloaded != null )
		{
			onResourcesReloaded();
		}
	}
	
	
	
	public void Awake()
	{
		FindReferences();
		SetBaseLanguage( languagesBaseLanguage );
	}
	
	public Texture2D GetTexture(string key)
	{	
		Texture2D output = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			output = provider.GetTexture(key);
			if( output != null )
				break;
		}
		
		if( output == null )
		{
			Debug.LogError(name + " : Texture " + key + " was not found!");
			output = diskProvider.GetTexture(sharedBaseURL, "error"); 
		}
		
		return output;
	}
	
	
	public AudioClip GetAudio(string key)
	{
		AudioClip output = null;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			output = provider.GetAudio(key);
			if( output != null )
				break;
		}
		
		if( output == null )
		{
			Debug.LogError(name + " : AudioClip " + key + " was not found!");
			output = diskProvider.GetAudio(sharedBaseURL, "error"); 
		}
		
		return output;
	}
	
	public string GetText(string key)
	{
		return textHelper.Get ( key );
	}

}
