using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusInput : LugusSingletonRuntime<LugusInputDefault>
{

}

public class LugusInputDefault : MonoBehaviour
{
	protected bool acceptInput = true;
	
	public bool dragging = false;
	public bool down = false;
	public bool up = false;
	
	public bool mouseMoving
	{
		get
		{
			if( dragging )
				return true;
			else
			{
				if( inputPoints.Count < 2 )
					return false;
				else
				{
					Vector3 previousPoint = inputPoints[ inputPoints.Count - 2 ];
					if( Vector3.Distance(lastPoint, previousPoint) > 2 )
						return true;
					else
						return false;
				}
			}
		}
	}
	
	public List<Vector3> inputPoints = new List<Vector3>();
	public Vector3 lastPoint;
	
	protected void OnMouseDown()
	{	
		inputPoints.Clear(); //= new List<Vector3>(); // reset the inputPoints array
		
		Vector3 inputPoint = Input.mousePosition;
		inputPoints.Add( inputPoint );
		
		lastPoint = inputPoint;
	}
	
	protected void OnMouseDrag()
	{
		Vector3 inputPoint = Input.mousePosition;
		
		
		// we're storing inputPoints even if we're not really dragging...
		// if the user then just leaves screen open for a long time, we will have a huge inputpoints array -> not good
		if( !dragging && inputPoints.Count > 1000 )
			inputPoints.Clear();
		
		
		//if( dragging )
			inputPoints.Add( inputPoint );
		
		lastPoint = inputPoint;
	}
	
	protected void OnMouseUp()
	{
		Vector3 inputPoint = Input.mousePosition;
		inputPoints.Add( inputPoint );
		
		lastPoint = inputPoint;
	}
	
	public Transform RayCastFromMouse(Camera camera)
	{
		if( inputPoints.Count == 0 )
			return RaycastFromScreenPoint( camera, lastPoint );
		else
			return RaycastFromScreenPoint( camera, inputPoints[ inputPoints.Count - 1 ] );
	}
	
	public Transform RaycastFromScreenPoint(Vector3 screenPoint)
	{
		return RaycastFromScreenPoint(LugusCamera.ui, screenPoint);
	}
	
	public Transform RaycastFromScreenPoint(Camera camera, Vector3 screenPoint)
	{
		
		//if( inputPoints.Count == 0 )
		//	return null;
		
		Ray ray = camera.ScreenPointToRay( screenPoint );
		RaycastHit hit;
		if ( Physics.Raycast(ray, out hit) )
		{
			return hit.collider.transform;
		}
		
		return null;
	}
	
	public Transform RayCastFromMouse()
	{
		return RayCastFromMouse(LugusCamera.ui);
	}
	
	public Vector3 ScreenTo3DPoint( Transform referenceObject )
	{
		return ScreenTo3DPoint( Input.mousePosition, referenceObject );
	}
	
	public Vector3 ScreenTo3DPoint( Vector3 screenPoint, Transform referenceObject )
	{
		return ScreenTo3DPoint( screenPoint, referenceObject.position );
	}
	
	public Vector3 ScreenTo3DPoint( Vector3 screenPoint, Vector3 referencePosition )
	{
		
		Ray ray = LugusCamera.ui.ScreenPointToRay( screenPoint );
		Vector3 output = ray.GetPoint( Vector3.Distance(referencePosition, LugusCamera.ui.transform.position) );
		return output.z(referencePosition.z);
		
		/*
		float distance = Vector3.Distance(referenceObject.position, LuGusCamera.ui.transform.position);
		
		Vector3 mousePos = new Vector3( Input.mousePosition.x, Input.mousePosition.y, distance - 5.0f);
		
		Debug.Log("ScreenTo3D : " + distance + " and " + mousePos ); 
		
		return LuGusCamera.ui.ScreenToWorldPoint( mousePos );
		*/	
	}
	
	public Vector3 ScreenTo3DPoint( Vector3 screenPoint, Vector3 referencePosition, Camera camera )
	{
		
		Ray ray = camera.ScreenPointToRay( screenPoint );
		Vector3 output = ray.GetPoint( Vector3.Distance(referencePosition, camera.transform.position) );
		return output.z(referencePosition.z);
		
		/*
		float distance = Vector3.Distance(referenceObject.position, LuGusCamera.ui.transform.position);
		
		Vector3 mousePos = new Vector3( Input.mousePosition.x, Input.mousePosition.y, distance - 5.0f);
		
		Debug.Log("ScreenTo3D : " + distance + " and " + mousePos ); 
		
		return LuGusCamera.ui.ScreenToWorldPoint( mousePos );
		*/	
	}
	
	
	
	public Vector3 ScreenTo3DPointOnPlane( Vector3 screenPoint, Plane plane)
	{
		float distance;
		Ray ray = LugusCamera.ui.ScreenPointToRay( screenPoint );
		
		if( plane.Raycast(ray, out distance) )
		{
			return ray.GetPoint(distance);
		}
		
		return Vector3.zero;
		
		/*
		float distance = Vector3.Distance(planeOrigin.position, LuGusCamera.ui.transform.position);
		
		Vector3 mousePos = new Vector3( screenPoint.x, screenPoint.y, distance);
		
		Debug.Log("ScreenTo3D : " + distance + " and " + mousePos ); 
		
		return LuGusCamera.ui.ScreenToWorldPoint( mousePos );
		*/
	}
	
	public Transform RayCastFromMouseDown()
	{
		if( down )
			return RayCastFromMouse();
		else
			return null;
	}
	
	public Transform RayCastFromMouseUp()
	{
		if( up )
			return RayCastFromMouse();
		else
			return null;
	}
	
	public Transform RayCastFromMouseDown(Camera camera)
	{
		if( down )
			return RayCastFromMouse(camera);
		else
			return null;
	}
	
	public Transform RayCastFromMouseUp(Camera camera)
	{
		if( up )
			return RayCastFromMouse(camera);
		else
			return null;
	}
	
	protected void ProcessMouse()
	{
		if( !acceptInput )
			return;
		
		down = false;
		up = false;
		
		// code for a single touch (mouse-like behaviour on touch)
		
		if( Input.touchCount == 1 )
		{
			Touch touch = Input.touches[0];
			
			if( touch.phase == TouchPhase.Began )
			{
				down = true;
				dragging = true;
				OnMouseDown();
			}
			
			if( touch.phase == TouchPhase.Ended )
			{
				up = true;
				dragging = false;
				OnMouseUp();
			}
			
			if( dragging )
			{
				if ( touch.deltaPosition == Vector2.zero ) 
				{
        			return;
   				}
				
				OnMouseDrag();
			}
		}
		else
		{
			if( Input.GetMouseButtonDown(0) ) // left click
			{
				down = true;
				dragging = true;
				OnMouseDown();
			}
				
			
			if( Input.GetMouseButtonUp(0) )
			{
				up = true;
				dragging = false;
				OnMouseUp();
			}
			
			
			//if( dragging )
			//{
				OnMouseDrag();
			//}
		}
	}		

	// Update is called once per frame
	void Update () 
	{
		ProcessMouse();
		
		if( Input.GetKeyDown(KeyCode.Tab) )
		{
			LugusDebug.debug = !LugusDebug.debug;
		}
	}
	
	public bool KeyDown(KeyCode key)
	{
		return Input.GetKeyDown(key); 
	}
	
	public bool KeyUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}
	
	public bool Key(KeyCode key)
	{
		return Input.GetKey(key);
	}
	
	
	void OnGUI()
	{	
		if( !LugusDebug.debug )
			return;
		
		if( GUI.Button( new Rect(200, Screen.height - 30, 200, 30), "Toggle debug") )
		{
			LugusDebug.debug = !LugusDebug.debug;
		}
	}
}
