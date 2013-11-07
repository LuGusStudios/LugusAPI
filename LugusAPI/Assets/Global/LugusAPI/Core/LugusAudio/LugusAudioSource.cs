using UnityEngine;
using System.Collections;

public class LugusAudioSource : MonoBehaviour 
{
	public string key = "";
	public Lugus.AudioChannelType channelType = Lugus.AudioChannelType.NONE;
	public bool stopOthers = false;
	
	public bool preload = true;
	
	protected void AssignKey()
	{
		if( string.IsNullOrEmpty(key) )
		{
			if( this.audio != null )
			{
				if( this.audio.clip != null )
				{
					key = this.audio.clip.name;
				}
			}
			
			Debug.LogWarning(name + " : key was empty! using material.mainTexture name : " + key );
		}
	}
	
	protected AudioClip clip = null;
	
	public void Play()
	{
		if( clip == null )
		{
			FetchClip(); // here it's really needed, so no check for preload there
		}
		
		if( clip == null )
		{
			Debug.LogError(name + " : audioClip " + key + " not found");
			return;
		}
		
		LugusAudioChannel channel = LugusAudio.use.GetChannel( channelType );
		// TODO: best cache the GetAudio result and only re-fetch (and re-cache) when we receive callback from LugusResources) 
		channel.Play( clip, this.stopOthers ); 
	}
	
	// Use this for initialization
	void Start () 
	{
		AssignKey();
		
		LugusResources.use.onResourcesReloaded += UpdateClip;
		
		if( preload )
			UpdateClip();
	}
	
	public void UpdateClip()
	{
		if( preload )
			FetchClip();
	}
	
	protected void FetchClip()
	{
		clip = LugusResources.use.GetAudio(key);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
