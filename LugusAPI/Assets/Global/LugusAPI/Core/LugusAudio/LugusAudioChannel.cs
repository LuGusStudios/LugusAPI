using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusAudioChannel 
{
	
	protected float _volume = 1.0f;
	public float VolumePercentage
	{
		get{ return _volume; }
		set{ UpdateVolume(value); }
	}
	
	// if any of the tracks on this channel is playing
	public bool IsPlaying
	{
		get
		{
			bool playing = false;
			
			foreach( ILugusAudioTrack t in _tracks )
			{
				if( t.Playing )
				{
					playing = true;
					break;
				}
			}
			
			return playing; 
		}
	}
	
	protected Lugus.AudioChannelType _channelType = Lugus.AudioChannelType.NONE;
	public Lugus.AudioChannelType ChannelType
	{
		get{ return _channelType; }
		set{ _channelType = value; }
	}
	
	// TODO: FIXME: this doesn't work properly
	// if we've already adjusted the percentage and applied to running tracks
	// if we then do it again, tracks are adjusted again by percentage...
	// possible fix: tracks should keep original volume at start and re-use that in subsequent calculations
	protected void UpdateVolume(float newValue)
	{
		float changeFactor = newValue / _volume;
		_volume = newValue;
		
		foreach( ILugusAudioTrack track in _tracks )
		{
			track.Source.volume *= changeFactor; 
		}
	}
	
	public LugusAudioChannel(Lugus.AudioChannelType type)
	{
		this.ChannelType = type;
		
		// TODO : create a number of tracks at the beginning to act as a Pool
	}
	
	
	// TODO: store channel-wide base settings in LugusConfig and re-load them at app startup
	// this way, we can easily create a settings-screen to manipulate individual channels and keep user config between sessions
	protected LugusAudioTrackSettings _baseTrackSettings = null;
	public LugusAudioTrackSettings BaseTrackSettings
	{
		get{ return _baseTrackSettings; }
		set{ _baseTrackSettings = value; }
	}
	
	
	protected List<ILugusAudioTrack> _tracks = new List<ILugusAudioTrack>();
	public List<ILugusAudioTrack> Tracks
	{
		get{ return _tracks; }
	}
	
	protected Transform trackParent = null;
	
	protected void FindReferences()
	{
		if( trackParent == null )
		{
			GameObject p = GameObject.Find("_ILugusAudioTracks");
			if( p == null )
			{
				p = new GameObject("_ILugusAudioTracks");
				p.transform.parent = LugusCamera.game.transform;
			}
			
			trackParent = p.transform;
		}
	}
	
	
	protected ILugusAudioTrack CreateTrack()
	{
		FindReferences();
		
		GameObject trackGO = new GameObject(this._channelType.ToString() + "_Track_" + (_tracks.Count + 1));
		
		trackGO.AddComponent<AudioSource>();
		ILugusAudioTrack track = trackGO.AddComponent<LugusAudioTrack>();
		track.Channel = this;
		
		trackGO.transform.parent = trackParent;
		
		_tracks.Add ( track );
		
		return track;
	}
	
	// get next available track in the pool (not playing, not claimed, not paused)
	public ILugusAudioTrack GetTrack() 
	{
		ILugusAudioTrack output = null;
		
		foreach( ILugusAudioTrack t in _tracks )
		{
			// only if the track is not claimed by someone,
			// and it's not currently playing
			// and it hasn't been paused (which means it's still playing but unity's AudioSource.IsPlaying is false...)
			if( !t.Claimed && !t.Playing && !t.Paused )
			{
				output = t;
				break;
			}
		}
		
		if( output == null )
		{
			output = CreateTrack();
		}
		
		return output; 
	}
	
	// returns the first found track that is currently playing
	// usefull in situations where we have 1 main track (ex. backgroundmusic) and want to fade to a new track
	public ILugusAudioTrack GetActiveTrack()
	{	
		foreach( ILugusAudioTrack t in _tracks )
		{
			if( t.Playing )
			{
				return t;
			}
		}
		
		return null;
	}
	
	public ILugusAudioTrack Play(AudioClip clip, bool stopOthers = false, LugusAudioTrackSettings trackSettings = null )
	{
		// TODO: maybe upgrade this option to allow PauseOthers or MuteOthers as well? 
		if( stopOthers )
		{
			StopAll ();
		}
		
		ILugusAudioTrack track = GetTrack();
		
		track.Play( clip, trackSettings );
		
		return track;
	}
	
	public ILugusAudioTrack Play(AudioClip clip, bool stopOthers, Vector3 position )
	{
		return Play (clip, stopOthers, new LugusAudioTrackSettings().Position(position) );
	}
	
	// fades out and stops all active tracks and fades in a new track with the new clip
	public ILugusAudioTrack CrossFade(AudioClip clip, float duration, LugusAudioTrackSettings settings = null )
	{
		foreach( ILugusAudioTrack t in _tracks )
		{
			if( t.Playing && !t.Paused )
			{
				t.FadeOut(duration, true);
			}
		}
		
		ILugusAudioTrack newTrack = GetTrack();
		newTrack.FadeIn( clip, duration, settings );
		
		return newTrack;
	}
	
	public void StopAll()
	{
		foreach( ILugusAudioTrack t in _tracks )
		{
			t.Stop();
		}
	}
}
