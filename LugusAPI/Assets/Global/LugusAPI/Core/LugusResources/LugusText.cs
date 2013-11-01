using UnityEngine;
using System.Collections;

public class LugusText : MonoBehaviour 
{
	public string key = "";
	public TextMesh textMesh = null;
	
	public void FindReferences()
	{
		if( textMesh == null )
		{
			textMesh = GetComponent<TextMesh>();
			
			if( textMesh == null )
			{
				Debug.LogError(name + " : TextMesh was null!");
			}
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		FindReferences();
		
		if( string.IsNullOrEmpty(key) )
		{
			key = textMesh.text;
			
			Debug.LogError(name + " : key was empty! using TextMesh.text as key : " + key );
		}
		
		LugusResources.use.onResourcesReloaded += UpdateText;
		
		UpdateText();
	}
	
	protected void UpdateText()
	{
		FindReferences();
		
		textMesh.text = LugusResources.use.GetText(key);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
