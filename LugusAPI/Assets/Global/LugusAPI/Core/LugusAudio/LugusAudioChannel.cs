using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusAudioChannel 
{
	protected float _volume = 100.0f;
	public float VolumePercentage
	{
		get{ return _volume; }
		set{ UpdateVolume(value); }
	}
	
	protected bool _playing = false;
	public bool IsPlaying
	{
		get{ return _playing; }
		set{ _playing = value; }
	}
	
	protected Lugus.AudioChannelType _channelType = Lugus.AudioChannelType.NONE;
	public Lugus.AudioChannelType ChannelType
	{
		get{ return _channelType; }
		set{ _channelType = value; }
	}
	
	protected void UpdateVolume(float newValue)
	{
		float changeFactor = newValue / _volume;
		_volume = newValue;
		
		foreach( LugusAudioTrack track in _tracks )
		{
			track.Source.volume *= changeFactor; 
		}
	}
	
	public LugusAudioChannel(Lugus.AudioChannelType type)
	{
		this.ChannelType = type;
		
		// TODO : create a number of tracks at the beginning to act as a Pool
	}
	
	
	protected List<LugusAudioTrack> _tracks = new List<LugusAudioTrack>();
	public List<LugusAudioTrack> Tracks
	{
		get{ return _tracks; }
	}
	
	protected Transform trackParent = null;
	
	protected void FindReferences()
	{
		if( trackParent == null )
		{
			GameObject p = GameObject.Find("_LugusAudioTracks");
			if( p == null )
			{
				p = new GameObject("_LugusAudioTracks");
			}
			
			trackParent = p.transform;
		}
	}
	
	
	protected LugusAudioTrack CreateTrack()
	{
		FindReferences();
		
		GameObject trackGO = new GameObject(this._channelType.ToString() + "_Track_" + (_tracks.Count + 1));
		
		trackGO.AddComponent<AudioSource>();
		LugusAudioTrack track = trackGO.AddComponent<LugusAudioTrack>();
		track.Channel = this;
		
		trackGO.transform.parent = trackParent;
		
		_tracks.Add ( track );
		
		return track;
	}
	
	public LugusAudioTrack GetTrack() 
	{
		// TODO: make sure the tracks are recycled / that we use a Pool of handles that is initialized at the beginning
		// loop over this.tracks to find the next handle that has .IsPlaying == false and Claimed == false
		// if none can be found -> only then use CreateTrack()
		
		LugusAudioTrack output = null;
		
		foreach( LugusAudioTrack t in _tracks )
		{
			// only if the track is not claimed by someone,
			// and it's not currently playing
			// and it hasn't been paused (which means it's still playing but unity's AudioSource.IsPlaying is false...)
			if( !t.Claimed && !t.IsPlaying && !t.Paused )
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
	
	
	public LugusAudioTrack Play(AudioClip clip, bool stopOthers = false, LugusAudioTrackSettings trackSettings = null )
	{
		// TODO: maybe upgrade this option to allow PauseOthers or MuteOthers as well? 
		if( stopOthers )
		{
			foreach( LugusAudioTrack t in _tracks )
			{
				t.Stop();
			}
		}
		
		LugusAudioTrack track = GetTrack();
		
		if( trackSettings != null && trackSettings.position != LugusUtil.DEFAULTVECTOR )
		{
			track.transform.position = trackSettings.position;
		}
		else
		{
			track.transform.position = LugusAudio.use.transform.position;
		}
		
		track.Play( clip, trackSettings );
		
		return track;
	}
	
	public LugusAudioTrack Play(AudioClip clip, bool stopOthers, Vector3 position )
	{
		LugusAudioTrackSettings settings = new LugusAudioTrackSettings();
		settings.position = position;
		
		return Play (clip, stopOthers, settings);
	}
}
