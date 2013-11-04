using UnityEngine;
using System.Collections;

public class LugusAudioSource : MonoBehaviour 
{
	public string key = "";
	public Lugus.AudioChannelType channelType = Lugus.AudioChannelType.NONE;
	public bool stopOthers = false;
	
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
	
	public void Play()
	{
		LugusAudioChannel channel = LugusAudio.use.GetChannel( channelType );
		// TODO: best cache the GetAudio result and only re-fetch (and re-cache) when we receive callback from LugusResources) 
		channel.Play( LugusResources.use.GetAudio("test.audio.default"), this.stopOthers );
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
