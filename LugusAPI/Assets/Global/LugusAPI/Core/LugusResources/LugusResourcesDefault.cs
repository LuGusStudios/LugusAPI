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
		this.diskProvider = gameObject.AddComponent<LugusResourceProviderDisk>(); 
		
		providers = new List<ILugusResourceProvider>();
		providers.Add( diskProvider );
		
		SetBaseLanguage( languagesBaseLanguage );
	}
	
	public void SetBaseLanguage(string langID)
	{
		languagesBaseLanguage = langID;
		
		foreach( ILugusResourceProvider provider in providers )
		{
			provider.BaseURL = languagesBaseURL + languagesBaseLanguage + "/";
		}
		
		if( onResourcesReloaded != null )
		{
			onResourcesReloaded();
		}
	}
	
	
	
	public void Awake()
	{
		FindReferences();
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
		return null;
	}

}
