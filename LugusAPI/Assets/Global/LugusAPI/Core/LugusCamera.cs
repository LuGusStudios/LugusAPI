using UnityEngine;
using System.Collections;

public class LugusCamera 
{
	protected static Camera _frontCamera = null;
	protected static Camera _gameCamera = null;
	
	public static Camera front
	{
		get
		{
            if (_frontCamera == null && GameObject.Find ("UICamera") != null )
                _frontCamera = GameObject.Find("UICamera").camera;
			
			if( _frontCamera == null )
				_frontCamera = game;
			
			return _frontCamera; 
		}
	}
	
	
	public static Camera game
	{
		get
		{
			if( _gameCamera == null )
				_gameCamera = Camera.main;
			
			return _gameCamera;
		}
	}
	
	public static Camera ui
	{
		get{ return front; } 
	}
}

public static class LugusCameraExtensions
{
	public enum ShakeAmount
	{
		NONE,
		SMALL,
		MEDIUM,
		LARGE 
	}
	
	public static void Shake(this Camera camera, ShakeAmount amount)
	{
		Vector3 displacement = Vector3.zero;
		if( amount == ShakeAmount.SMALL )
			
			displacement =  new Vector3(0.1f, 0.0f, 0.05f);
		else if( amount == ShakeAmount.MEDIUM )
			displacement =  new Vector3(0.2f, 0.0f, 0.2f);
		else if( amount == ShakeAmount.LARGE )
			displacement =  new Vector3(0.5f, 0.0f, 0.5f);
			
		
		iTween.ShakePosition(camera.transform.parent.gameObject, displacement, 0.3f );				
	}
}
