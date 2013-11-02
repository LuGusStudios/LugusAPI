using UnityEngine;
using System.Collections;

public class LugusResourceProviderDisk : MonoBehaviour, ILugusResourceProvider
{
	[SerializeField]
	protected string _baseURL = "";
	public string BaseURL
	{
		get{ return _baseURL; }
		set{ _baseURL = value; }
	}
	
	public Texture2D GetTexture(string key)
	{
		return GetTexture (this.BaseURL, key);
	}
	
	public Texture2D GetTexture(string BaseURL, string key)
	{
		return Resources.Load( BaseURL + "Textures/" + key, typeof(Texture2D) ) as Texture2D;
	}
	
	public AudioClip GetAudio(string key)
	{
		return GetAudio (this.BaseURL, key);
	}
	
	public AudioClip GetAudio(string BaseURL, string key)
	{
		return Resources.Load( BaseURL + "Audio/" + key, typeof(AudioClip) ) as AudioClip;
	}
	
	public TextAsset GetText(string key)
	{
		return GetText (this.BaseURL, key);
	}
	
	public TextAsset GetText(string BaseURL, string key)
	{
		return Resources.Load( BaseURL + "Text/" + key, typeof(TextAsset) ) as TextAsset;
	}
}
