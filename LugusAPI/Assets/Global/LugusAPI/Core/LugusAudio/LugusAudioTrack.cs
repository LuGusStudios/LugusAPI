using UnityEngine;
using System.Collections;

public class LugusAudioTrack : MonoBehaviour 
{
	protected LugusAudioChannel _channel = null;
	public LugusAudioChannel Channel
	{
		get{ return _channel; }
		set{ _channel = value; }
	}
	
	
	public bool IsPlaying
	{
		get{ return Source.isPlaying; }
	}
	
	protected bool _paused = false;
	public bool Paused
	{
		get{ return _paused; }
		set{ _paused = value; }
	}
	
	public AudioSource Source
	{
		get{ return this.audio; } 
	}
	
	protected bool _claimed = false;
	public bool Claimed
	{
		get{ return _claimed; }
	}
	
	public void Claim()
	{
		_claimed = true;
	}
	
	public void Claim(string name)
	{
		Claim ();
		this.transform.name = name;
	}
	
	public void Release()
	{
		_claimed = false;
	}
	
	
	public void Play(AudioClip clip, LugusAudioTrackSettings settings = null)
	{
		Stop ();
		
		if( clip == null )
		{
			Debug.LogError(name + " : clip was null! ignoring...");
			return;
		}
		
		if( Channel == null )
		{
			Debug.LogError(name + " : Channel was null! ignoring...");
			return;
		}
		
		_paused = false;
		
		Source.clip = clip;
		Source.time = 0.0f;
		
		// TODO: apply all possible settings to our local AudioSource
		if( settings != null )
		{
			if( settings.copyFromSource != null )
			{
				Source.volume = settings.copyFromSource.volume;
			}
		}
			
		Source.volume *= _channel.VolumePercentage; 
			
			
		Source.Play();
	}
	
	public void Pause()
	{
		_paused = true;
		Source.Pause();
	}
	
	public void UnPause()
	{
		if( _paused )
		{
			_paused = false;
			Source.Play();
		}
	}
	
	public void Stop()
	{
		_paused = false;
		
		Source.Stop();
	}
}

// TODO: FIXME: make sure we can copy or at least auto-assign all settings from "copyFromSource" to another AudioSource
public class LugusAudioTrackSettings
{
	public Vector3 position = LugusUtil.DEFAULTVECTOR;
	
	public AudioSource copyFromSource = null;
	
	public LugusAudioTrackSettings()
	{
		
	}
}