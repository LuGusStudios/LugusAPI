using UnityEngine;
using System.Collections;

public class ResourcesTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		UpdateTextAsset();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Transform t = LugusInput.use.RayCastFromMouseDown(LugusCamera.game);
		if( t == this.transform )
		{
			Debug.Log (name + " : clicked texture");
			
			
			if( LugusResources.use.Localized.LangID == "en" )
			{
				LugusResources.use.Localized.LangID = "nl";
			}
			else
			{
				LugusResources.use.Localized.LangID = "en";
			}

			UpdateTextAsset();
		}
		else if( t == GameObject.Find ("AudioButton").transform )
		{
			LugusAudioSource src = GameObject.Find ("AudioButton").GetComponent<LugusAudioSource>();
			src.Play();
		}
	}

	protected void UpdateTextAsset()
	{
		GameObject.Find ("TextAsset").GetComponent<TextMesh>().text = LugusResources.use.Localized.GetTextAsset("test").text;
	}
}
