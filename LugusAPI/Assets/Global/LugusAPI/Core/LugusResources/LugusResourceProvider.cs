using UnityEngine;
using System.Collections;

namespace LugusResourceProvider
{
	public enum ResourceProviderType
	{
		NONE = 0,
		
		Disk = 1
	}
}

public interface ILugusResourceProvider
{
	
	//string BaseURL { get; set; } 
	
	//Texture2D GetTexture(string key);
	Texture2D GetTexture(string BaseURL, string key);
	
	//AudioClip GetAudio(string key);
	AudioClip GetAudio(string BaseURL, string key);
	
	//TextAsset GetText(string key);
	TextAsset GetText(string BaseURL, string key);
	
	//TextAsset GetSprite(string key);
	Sprite GetSprite(string BaseURL, string key);
}

