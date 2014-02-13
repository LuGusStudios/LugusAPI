using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RandomTester : MonoBehaviour {
	private string strMin;
	private string strMax;
	public float min = 0f;
	public float max = 10f;
	public int minSeqInput;
	public int maxSeqInput;
	
	private string _bucketLength;
	private string _iterations;
	public int bucketLength = 11;
	public int iterations = 100;
	private int[] _buckets;
	private List<int> _listBuckets = new List<int>();
	private int _maxInBucket = 0;
	private string seqString = "";
	private string strDelta;
	private float _delta = 9999;

	//Graph for the distributions
	private LineRenderer lineRenderer;
	public float graphX = -10;
	public float graphY = -10;
	public float graphScaleX = 2;
	public float graphScaleY = 2;

	public float current ;

	//Particle System for the grid
	public Transform pointsObject;
	public Transform parentObject;
	private ParticleSystem.Particle[] points;
	private float width = 512;
	private float height = 512;
	private float depth = 1;
	private int rows = 20;
	private int cols = 20;
	private int stack = 1;
	private Vector3 rotation;

	//Perlin
	public float scalePerlin = 1.0F;
	private Texture2D noiseTex;
	private Color[] pix;
	public Transform perlinQuad;
	// Use this for initialization
	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer>();
		_buckets = new int[bucketLength];
		_buckets = Bucketting(min,max,_delta,bucketLength,Distribution.Gaussian,System.DateTime.Now.Millisecond);

		//Perlin
		noiseTex = new Texture2D(512, 512);
		pix = new Color[noiseTex.width * noiseTex.height];
		perlinQuad.renderer.material.mainTexture = noiseTex;
		//CalcPerlin();

		RenderGraph();
		MakeGrid(512,512,1,20,20,1);

		MakeRandomColors();

	}
	
	// Update is called once per frame
	void Update () 
	{
		pointsObject.particleSystem.SetParticles(points, points.Length);

	}

	void OnGUI()
	{
		DistributionGui();
		SequenceGui();
		PerlinGui();
		ScatterGui();
	}
	public void DistributionGui()
	{
		GUILayout.BeginArea (new Rect (0,0,700,200));
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				Distribution[] distType = Enum.GetValues(typeof(Distribution)) as Distribution[];
				foreach(Distribution dist in distType)
				{
					if(GUILayout.Button(dist.ToString()))
					{
						_buckets = Bucketting(min,max,_delta,bucketLength,dist,System.DateTime.Now.Millisecond);
						RenderGraph();
					}
				}
				if(GUILayout.Button("Golden Ratio"))
				{
					MakeGoldenRatio();
				}
				GUILayout.EndVertical();
			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("bracket length");
				_bucketLength = GUILayout.TextField(bucketLength.ToString());
				int.TryParse(_bucketLength,out bucketLength);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("iterations");
				_iterations = GUILayout.TextField(iterations.ToString());
				int.TryParse(_iterations,out iterations);
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("min");
				strMin = GUILayout.TextField(min.ToString());
				float.TryParse(strMin,out min);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("max");
				strMax = GUILayout.TextField(max.ToString());
				float.TryParse(strMax,out max);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("delta");
				strDelta = GUILayout.TextField(_delta.ToString());
				float.TryParse(strDelta,out _delta);
				GUILayout.EndHorizontal();
				if(GUILayout.Button("Default delta"))
				{
					_delta = 9999;
				}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				for (int i = 0; i < _buckets.Length; i++) 
				{
					GUILayout.Box(_buckets[i].ToString());
				}		
				GUILayout.EndHorizontal();
				if(GUILayout.Button("shuffle"))
				{
					LugusRandom.use.Sequence.Shuffle(_listBuckets);
				}
				GUILayout.BeginHorizontal();
				for (int i = 0; i < _buckets.Length; i++) 
				{
					GUILayout.Box(_listBuckets[i].ToString());
				}		
				GUILayout.EndHorizontal();
				if(GUILayout.Button("Test all distributions with same seed"))
				{
					CheckAllDistributions();
				}
			GUILayout.EndVertical();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	public void ScatterGui()
	{
		GUILayout.BeginArea (new Rect (170,220,150,200));
		GUILayout.BeginVertical();
		if(GUILayout.Button("Scatter"))
		{
			MakeGrid(width,height,depth,rows,cols,stack);
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("width");
		width = GUILayout.HorizontalSlider(width,1,512);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("height");
		height = GUILayout.HorizontalSlider(height,1,512);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("depth");
		depth = GUILayout.HorizontalSlider(depth,1,512);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("rows");
		rows = Mathf.RoundToInt( GUILayout.HorizontalSlider(rows,1,20));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("cols");
		cols = Mathf.RoundToInt( GUILayout.HorizontalSlider(cols,1,20));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("stack");
		stack = Mathf.RoundToInt( GUILayout.HorizontalSlider(stack,1,20));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("rotation");
		float yAxis = GUILayout.HorizontalSlider(rotation.y,0,360);
		rotation = new Vector3(0,-yAxis,0);
		parentObject.rotation = Quaternion.Euler( rotation);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	public void SequenceGui()
	{
		GUILayout.BeginArea (new Rect (0,220,150,250));
		GUILayout.BeginVertical();
		if(GUILayout.Button("new seq list"))
		{
			//3 options for new sequence: make new sequence object,change datarange property,change list prop
//			DataRange dr = new DataRange(minSeqInput,maxSeqInput);
//			LugusRandom.use.Sequence().Range = dr;

			List<float> list = new List<float>();
			list.Add(3);
			list.Add(2.2f);
			list.Add(9);
			list.Add(6);
			LugusRandom.use.Sequence.ListRange = list;
		}
		if(GUILayout.Button("Next random"))
		{
			current = LugusRandom.use.Sequence.Next();
		}
		GUILayout.Box(current.ToString());

		if(GUILayout.Button("Iterate 50 times"))
		{
			 seqString = IterateSequence(50);
		}
		GUILayout.Label(seqString);
		if(GUILayout.Button("Check for double values"))
		{
			if (DoubleValues()) 
			{
				Debug.LogWarning("Double values found");
			}
			else
			{
				Debug.Log("No double values found");
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	public void PerlinGui()
	{
		GUILayout.BeginArea (new Rect (350,220,150,200));
		GUILayout.BeginVertical();
		if(GUILayout.Button("Perlin"))
		{
			CalcPerlin();
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("scale");
		scalePerlin = GUILayout.HorizontalSlider(scalePerlin,1,100);
		GUILayout.EndHorizontal();
		if(GUILayout.Button("Random Colors"))
		{
			MakeRandomColors();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	public int[] Bucketting(float min, float max, float delta, int bracketLength, Distribution type, int seed)
	{
		int[] brackets = new int[bracketLength];
		float rand;
		float percent;
//		string listRand = type.ToString() + "\n";
		LugusRandom.use.Distr.DistType = type;
		LugusRandomGeneratorDistribution rGen = new LugusRandomGeneratorDistribution(type, min, max , delta, seed);
		DataRange dr = new DataRange(min,max);
		for (int i = 0; i < iterations; i++) 
		{
			//rand = dr.PercentageInInterval(LugusRandom.use.distribution.Next());
			//rand = rGen.Next();
			rand = rGen.Next();
//			listRand += rand + "\n";
			percent = dr.PercentageInInterval(rand);
			//rand = dr.PercentageInInterval(UnityEngine.Random.value); Random.value is uniform!
			for (int j = 1; j <= brackets.Length; j++) {
				if(percent<=(float)j/brackets.Length)
				{
					brackets[j-1]++;
					break;
				}
			}
		}
		_listBuckets.Clear();
		_maxInBucket = 0;
		for (int i = 0; i < brackets.Length; i++) {
			if (brackets[i] > _maxInBucket) {
				_maxInBucket = brackets[i];
			}
			_listBuckets.Add(brackets[i]);	
		}
//		Debug.Log(listRand);
		return brackets;
	}
	public void RenderGraph()
	{
		DataRange dr = new DataRange(0 , _maxInBucket);
		lineRenderer.SetVertexCount(bucketLength);
		for (int i = 0; i < bucketLength; i++) {
			lineRenderer.SetPosition(i, new Vector3((i + graphX) * graphScaleX / bucketLength ,graphY + dr.PercentageInInterval(_buckets[i]) * graphScaleY,0));
		}
	}
	public bool DoubleValues()
	{
		LugusRandomGeneratorSequence rndSeq = new LugusRandomGeneratorSequence(0,10);
		float prev = -1;
		for (int i = 0; i < iterations; i++) 
		{
			float curValue = rndSeq.Next();
			if(prev == curValue)
			{
				Debug.Log(prev + " " + curValue);
				return true; 
			}
			prev = curValue;
		}
		return false;
	}
	public string IterateSequence(int iterate)
	{
		string sequence = "";
		LugusRandomGeneratorSequence rndSeq = new LugusRandomGeneratorSequence(0,10);
		for (int i = 0; i < iterate; i++) 
		{
			if(i!=0 && i % 10 == 0)
			{
				sequence +="\n"; 
			}
			sequence += rndSeq.Next() + ",";
		}
		return sequence;
	}
	public void MakeGrid(float width, float height, float depth, int rows,int cols, int stack)
	{
		LugusRandomGeneratorGrid rGrid = new LugusRandomGeneratorGrid(width,height,depth,rows,cols,stack);

		int resolution = (int)rGrid.Range.to;
		points = new ParticleSystem.Particle[resolution];
		for (int i = 0; i < rGrid.Range.to; i++) {
			points[i].position = rGrid.Next();
			points[i].color = new Color(1f, 0f, 0f);
			points[i].size = 20f;
		}
		Vector3 position =  new Vector3(width*0.5f,height*0.5f);
		pointsObject.localPosition = -position;
	}
	public void MakeGoldenRatio()
	{
		float size = 20;
		float rangeTo = 700;
		//LugusRandom.use.GoldenRatio.Range = new DataRange(0,rangeTo);
		points = new ParticleSystem.Particle[iterations+2];
		for (int i = 0; i < iterations; i++) 
		{
			points[i].position = new Vector3(LugusRandom.use.GoldenRatio.GoldenRatio(0,rangeTo,i),0,0);
			points[i].color = new Color(0, 1f, 0);
			points[i].size = size;
		}
		for (int i = 0; i < 2; i++) 
		{
			points[iterations+i].position = new Vector3(rangeTo*i,0,0);
			points[iterations+i].color = new Color(0, 0, 0);
			points[iterations+i].size = size;
		}
	}
	public void CalcPerlin()
	{
		LugusRandom.use.Perlin.Scale = scalePerlin;
		LugusRandomGeneratorPerlin rndPerl = new LugusRandomGeneratorPerlin(noiseTex.width,noiseTex.height,scalePerlin,System.DateTime.Now.Millisecond);
		float sample;
		float y = 0.0F;
		while (y < noiseTex.height) {
			float x = 0.0F;
			while (x < noiseTex.width) {
				sample = rndPerl.Perlin(x,y); 
//				sample = LugusRandom.use.perlin.Perlin(x,y);
				pix[(int)(y * noiseTex.width + x)] = new Color(sample, sample, sample);
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}
	public void CheckAllDistributions()
	{
		bool difference = false;
		int[] bucket1 = new int[bucketLength];
		int[] bucket2 = new int[bucketLength];
		Distribution[] distType = Enum.GetValues(typeof(Distribution)) as Distribution[];
		foreach(Distribution dist in distType)
		{
			bucket1 = Bucketting(min,max,_delta,bucketLength,dist,10);
			bucket2 = Bucketting(min,max,_delta,bucketLength,dist,10);
			for (int i = 0; i < bucketLength; i++) 
			{
				if(bucket1[0] != bucket2[0])
				{
					difference = true;
					Debug.LogWarning("Difference found with " + dist.ToString ());
				}
			}
		}
		if(!difference)
		{
			Debug.Log("Test passed");
		}
	}
	public void MakeRandomColors()
	{
		LugusRandomGeneratorColors rndColors = new LugusRandomGeneratorColors();
		List<Color> clrList = new List<Color>();
		clrList = rndColors.GenerateColorsHarmony(8,15,30,15,15,15,1,1,2f,2f);
		//clrList = rndColors.GenerateColorsHarmony(24,15,30,15,15,15,1,1);
		Color[] clrArray = new Color[128];
		int y = 0;
		float colorWidth = (noiseTex.width / 8) * noiseTex.height; 
		for (int i = 0; i < noiseTex.width * noiseTex.height; i++) 
		{
			if(i%colorWidth == 0)
			{
				y++;
			}
			pix[i] = clrList[y];
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}
}
