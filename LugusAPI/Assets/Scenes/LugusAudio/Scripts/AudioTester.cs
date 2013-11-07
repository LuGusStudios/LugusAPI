using UnityEngine;
using System.Collections;
using System;
using Lugus;

public class AudioTester : MonoBehaviour 
{
	public AudioClip[] ambient;
	public AudioClip[] music;
	public AudioClip[] sfx;
	public AudioClip[] speech;
	
	protected int ambientIndex = 0;
	protected int musicIndex = 0;
	protected int sfxIndex = 0;
	protected int speechIndex = 0;
	
	void Awake()
	{
		// for each channel, we can change the base settings for each track (ex. music automatically loops)
		// when using Play() or Load() etc. we can send in overwrites for these basic settings
		LugusAudio.use.Music().BaseTrackSettings = new LugusAudioTrackSettings().Loop(true).Volume(0.5f);
		LugusAudio.use.Ambient().BaseTrackSettings = new LugusAudioTrackSettings().Loop(true).Volume(0.5f);
		LugusAudio.use.Speech().BaseTrackSettings = new LugusAudioTrackSettings().Loop(false).Volume(1.0f);
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		Transform hit = LugusInput.use.RayCastFromMouseUp(LugusCamera.game);
		if( hit == GameObject.Find("BackgroundAmbient").transform )
		{
			Ambient ();
		}
		else if( hit == GameObject.Find("BackgroundMusic").transform )
		{
			Music ();
		}
		else if( hit == GameObject.Find("ForegroundSFX").transform )
		{
			SFX ();
		}
		else if( hit == GameObject.Find("ForegroundSpeech").transform )
		{
			Speech ();
		}
	}
	
	public void Ambient()
	{
		if( ambient.Length == 0 )
		{
			Debug.LogError("No ambient sounds loaded");
			return;
		}
		
		// disable Music track
		LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).StopAll();
		
		ambientIndex++;
		ambientIndex = ambientIndex % ambient.Length;
		
		// TODO: change this to FadeTo()
		LugusAudio.use.GetChannel(AudioChannelType.BackgroundAmbient).Play( ambient[ambientIndex], true );
	}
	
	public void Music()
	{		
		if( music.Length == 0 )
		{
			Debug.LogError("No music sounds loaded");
			return;
		}
		
		// disable Ambient track
		LugusAudio.use.Ambient().StopAll();
		
		musicIndex++;
		musicIndex = musicIndex % music.Length;
		
		// TODO: change this to FadeTo()
		//LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).VolumePercentage = 0.05f;
		
		// this will play directly and stop all other sounds on the channel (hard cut)
		if( LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).GetActiveTrack() == null )
		{
			LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).Play( music[musicIndex], true );
		}
		else
		{
			// This will fade out the current playing track and FadeIn a new track
			//LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).GetActiveTrack().FadeOut(2.0f);
			
			// method 1, more control over the fade
			/*
			LugusAudioTrack newTrack = LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).GetTrack();
	
			newTrack.Load(music[musicIndex], new LugusAudioTrackSettings().Volume(0.0f).Loop(true) );
			newTrack.FadeIn( 5.0f );
			newTrack.Play();
			*/
			
			// method 2 : shorter
			//LugusAudio.use.GetChannel(AudioChannelType.BackgroundMusic).GetTrack().FadeIn ( music[musicIndex], 2.0f, new LugusAudioTrackSettings().Loop(true) );
		
			
			// method 3 : shortest
			LugusAudio.use.Music().CrossFade( music[musicIndex], 2.0f );
		}
	}
	
	public void SFX()
	{		
		if( sfx.Length == 0 )
		{
			Debug.LogError("No sfx sounds loaded");
			return;
		} 
		
		sfxIndex++;
		sfxIndex = sfxIndex % sfx.Length;
		
		// TODO: adjust volume if speech is playing 
		
		LugusAudio.use.GetChannel(AudioChannelType.ForegroundSFX).Play( sfx[sfxIndex] );
	}
	
	public void Speech()
	{		
		if( speech.Length == 0 )
		{
			Debug.LogError("No speech sounds loaded");
			return;
		}
		
		if( LugusAudio.use.GetChannel(AudioChannelType.ForegroundSpeech).IsPlaying )
		{
			Debug.Log ("Speech was still busy. Not executing new speech");
			return;
		}
		
		
		speechIndex++;
		speechIndex = speechIndex % speech.Length;
		
		LugusAudio.use.GetChannel(AudioChannelType.ForegroundSpeech).Play( speech[speechIndex] );
	}
	
	
	void OnGUI()
	{
		GUILayout.BeginArea( new Rect(0, 0, 400, Screen.height) );
		GUILayout.BeginVertical();
		
		GUILayout.Label ("Global AudioListener volume : " + AudioListener.volume);
		
		Lugus.AudioChannelType[] channels = Enum.GetValues(typeof(Lugus.AudioChannelType)) as Lugus.AudioChannelType[];
		foreach( Lugus.AudioChannelType channelType in channels )
		{
			if( channelType == Lugus.AudioChannelType.NONE )
				continue;
			
			LugusAudioChannel channel = LugusAudio.use.GetChannel( channelType );
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("" + channelType.ToString() + " : ", GUILayout.Width(150));
			
			// how many tracks the channel contains at this time (pool-size)
			GUILayout.Label(" # " + channel.Tracks.Count );
			
			
			// how many tracks are playing and are claimed
			int playingCount = 0;
			int claimedCount = 0;
			foreach( LugusAudioTrack track in channel.Tracks )
			{
				if( track.Playing )
					playingCount++;
				
				if( track.Claimed )
					claimedCount++;
			}
			
			GUILayout.Label(" playing # " + playingCount );
			GUILayout.Label(" claimed # " + claimedCount );
			
			// allow user to mute / unmute entire channel 
			bool wasMuted = (channel.VolumePercentage == 0);
			bool mute = (GUILayout.Toggle(!wasMuted, "") == false);
			if( mute != wasMuted )
			{
				if( mute )
				{
					Debug.Log ("Volume to 0");
					channel.VolumePercentage = 0.0f;
				}
				else
				{
					Debug.Log ("Volume to 1");
					channel.VolumePercentage = 1.0f;
				}
			}
				
			
			if( GUILayout.Button("stop all") )
			{
				channel.StopAll();
			}
			
			
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
