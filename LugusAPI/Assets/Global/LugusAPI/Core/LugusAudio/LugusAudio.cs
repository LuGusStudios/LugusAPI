using UnityEngine;
using System.Collections;
using System;

namespace Lugus
{	
	// http://stackoverflow.com/questions/8447/enum-flags-attribute
	// http://www.dotnetperls.com/enum-flags
	// DO NOT CHANGE INT VALUES IF YOU DO NOT KNOW WHAT YOU'RE DOING
	[Flags]
	public enum AudioChannelTypeExtended
	{
		NONE = 0, // 0
		
		BackgroundAmbient = 1, // 1 // ambient background sounds, usually looping
		BackgroundMusic = 2, // 2 // background music, usually looping
		
		BackgroundAll = 3, // 3 // convenience value, same as Ambient | Music
		
		ForegroundSFX = 4, // 4 // one-shot sound effects
		ForegroundSpeech = 8, // 8 // one-shot voice-overs or character speech
		
		ForegroundAll = 12, // 12 // convenience value, same as SFX | Speech
		
		Custom1 = 16, // 16 // Custom channel 1, to be used at will by the application
		Custom2 = 32, // 32 // Custom channel 2, to be used at will by the application
		Custom3 = 64, // 64 // Custom channel 3, to be used at will by the application
		
		CustomAll = 112, // 112
		
		All = 127 // 127 // convenience value, to address ALL channels
	}
	
	// These values indicate the actual individual channels (no convenience groupings)
	// when artists work with sound, they should only be presented these singular channels
	// only programmers should be able to access the grouped channels for things like fading, enable/disable, etc.
	// just make sure the int-values are the same across enums and you're good to go :)
	[Flags]
	public enum AudioChannelType
	{
		NONE = 0, // 0 
		
		BackgroundAmbient = 1, // 1 // ambient background sounds, usually looping
		BackgroundMusic = 2, // 2 // background music, usually looping
		
		ForegroundSFX = 4, // 4 // one-shot sound effects
		ForegroundSpeech = 8, // 8 // one-shot voice-overs or character speech
		
		Custom1 = 16, // 16 // Custom channel 1, to be used at will by the application
		Custom2 = 32, // 32 // Custom channel 2, to be used at will by the application
		Custom3 = 64, // 64 // Custom channel 3, to be used at will by the application
	}
}

public class LugusAudio : LugusSingletonRuntime<LugusAudioDefault>
{
}
