using UnityEngine;
using System.Collections;

public interface ILugusAudioTrack
{
	LugusAudioChannel Channel { get; set; }
	
	bool Playing{ get; }
	bool Paused { get; }
	
	float Volume { get; set; }
	bool Loop{ get; set; }
	
	
	// Load the clip and apply the settings, but don't start playing the clip just yet
	bool Load(AudioClip clip, LugusAudioTrackSettings settings = null);
	// Play a Load()ed clip or re-start the current clip
	void Play();
	// Directly Load() and afterwards Play() a clip
	void Play(AudioClip clip, LugusAudioTrackSettings settings = null);
	void Stop();
	
	void Pause();
	void UnPause();
	
	// FadeIn the current clip (to be used after Load())
	void FadeIn(float duration);
	// Directly start playing the clip with a fade in
	void FadeIn(AudioClip clip, float duration, LugusAudioTrackSettings settings = null);
	// Fade out the volume and optionally start the clip afterwards
	void FadeOut(float duration, bool stopAfter = false);
	// Lerps the volume directly
	void FadeTo(float targetVolume, float duration);
	
	// TODO:
	// Mute() and UnMute () : set volume to 0.0f and previousVolume
	
	bool Claimed{ get; }
	void Claim();
	void Claim(string name);
	void Release();
	
	
	AudioSource Source { get; }
}

public class LugusAudioTrack : MonoBehaviour, ILugusAudioTrack
{
	#region properties
	protected LugusAudioChannel _channel = null;
	public LugusAudioChannel Channel
	{
		get{ return _channel; }
		set{ _channel = value; }
	}
	
	public float Volume
	{
		get{ return Source.volume * (1 / _channel.VolumePercentage); }
		set{ Source.volume = _channel.VolumePercentage * value; }
	}
	
	public bool Loop
	{
		get{ return Source.loop; }
		set{ Source.loop = value; }
	}
	
	public bool Playing
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
	
	#endregion
	
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
		this.transform.name = Channel.ChannelType.ToString() + "_Track_" + "released";
	}
	
	#region load and play
	public bool Load(AudioClip clip, LugusAudioTrackSettings settings = null)
	{
		if( clip == null )
		{
			Debug.LogError(name + " : clip was null! ignoring...");
			return false;
		}
		
		if( Channel == null )
		{
			Debug.LogError(name + " : Channel was null! ignoring...");
			return false;
		}
		
		Source.clip = clip;
		Source.time = 0.0f;
		
		// reset the settings 
		// otherwhise, if the previous PLay had for example Loop set
		// but the baseSettings didn't have loop set... loop would remain set on the next Play
		// TODO: make this more decent...
		this.Loop = false;
		this.Volume = 1.0f;
		
		if( settings != null )
		{
			settings.Merge( _channel.BaseTrackSettings );
			
		}
		else
		{
			settings = _channel.BaseTrackSettings;
		}
		
		if( settings != null )
		{
			settings.ApplyTo( this );
			
			if( settings.Position() == LugusUtil.DEFAULTVECTOR )
			{
				transform.localPosition = Vector3.zero;//position = LugusCamera.game.transform.position;//LugusAudio.use.transform.position;
			}
		}
		
		return true;
	}
	
	public void Play(AudioClip clip, LugusAudioTrackSettings settings = null)
	{
		bool loaded = Load (clip, settings);
		if( loaded )
			Play ();
	}
	
	// To be used after Load() or to re-start the clip
	public void Play()
	{
		Stop ();
		
		_paused = false;
		
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
	
	#endregion
	
	#region fade
	public void FadeOut(float duration, bool stopAfterwards = true)
	{
		FadeTo (0.0f, duration);
		
		if( stopAfterwards )
			LugusCoroutines.use.StartRoutine( StopDelayedRoutine(duration) );
	}
	
	public void FadeIn(float duration)
	{
		Volume = 0.0f;
		FadeTo (1.0f, duration);
	}
	
	public void FadeIn(AudioClip clip, float duration, LugusAudioTrackSettings settings = null)
	{
		Load ( clip, settings );
		FadeIn (duration);
		Play ();
	}
	
	public void FadeTo(float targetVolume, float duration)
	{ 
		LugusCoroutines.use.StartRoutine( FadeRoutine(targetVolume, duration) );
	}
	
	public IEnumerator FadeRoutine(float targetVolume, float duration)
	{	
		float timeCount = 0.0f;
		float startingVolume = Source.volume * (1 / _channel.VolumePercentage);
		float temp = 0.0f;
		
		while( timeCount < duration )
		{
			temp = Mathf.Lerp( startingVolume, targetVolume, timeCount / duration );
			
			Source.volume = temp * _channel.VolumePercentage;
			
			timeCount += Time.deltaTime;
			
			yield return null;
		}
	}
	#endregion
	
	public IEnumerator StopDelayedRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		Stop ();
	}
}


public class LugusAudioTrackSettings
{
	protected bool position_set = false;
	protected Vector3 position = LugusUtil.DEFAULTVECTOR;
	public Vector3 Position() { return position; }
	
	protected bool volume_set = false;
	protected float volume = 1.0f;
	public float Volume() { return volume; }
	
	protected bool loop_set = false;
	protected bool loop = false;
	public bool Loop() { return loop; }
	
	public void ApplyTo(ILugusAudioTrack target)
	{
		if( volume_set )
			target.Volume = volume;
		
		if( loop_set )
			target.Loop = loop;
		
	}
	
	// TODO: possible better way of approaching this would be:
	// baseSettings.ApplyTo( track, alwaysApply = true )
	// customSettings.ApplyTo(track, false)
	// we will apply settings twice, but makes the API simpler + not yet another function besides ApplyTo that needs to keep track of al vars here
	
	// this can replace the current way of working:
	// customSettings.Merge( baseSettings )
	// customSettings.ApplyTo(track)
	public void Merge(LugusAudioTrackSettings baseSettings)
	{
		Debug.Log ("Mergin base settings 0"); 
		
		if( baseSettings == null )
			return;
		
		Debug.Log ("Mergin base settings");
		
		if( !volume_set )
		{
			Volume( baseSettings.Volume() );
		}
		
		if( !loop_set )
		{
			Loop ( baseSettings.Loop () );
		}
	}
	
	public static LugusAudioTrackSettings FromSource(AudioSource src)
	{
		// TODO : FIXME :  fill this in properly! and add other poperties of AudioSource to this class
		
		LugusAudioTrackSettings output = new LugusAudioTrackSettings();
		
		output.volume = src.volume;
		output.loop = src.loop;
		
		return output;
	}
	
	public LugusAudioTrackSettings Volume(float volume)
	{
		volume_set = true;
		this.volume = volume;
		return this;
	}
	
	public LugusAudioTrackSettings Loop(bool loop)
	{
		loop_set = true;
		this.loop = loop;
		return this;
	}
	
	public LugusAudioTrackSettings Position(Vector3 position)
	{
		position_set = true;
		this.position = position;
		return this;
	}
	
	public LugusAudioTrackSettings()
	{
		
	}
}