using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ILugusRandomGenerator
{
	protected System.Random _r;
	public System.Random RandomClass {
		get {
			return _r;
		}
	}
	protected int _seed;
	public int Seed {
		get {
			return _seed;
		}
	}
	protected DataRange _dr;
	public DataRange Range
	{
		get{return _dr;}
		set{_dr = value;}
	}
	public virtual int NextInt()
	{
		return _r.Next();
	}
	public virtual int NextInt(int max)
	{
		return _r.Next(max);
	}
	public virtual int NextInt(int min, int max)
	{
		return _r.Next(min, max);
	}
	public virtual float Next()
	{
		return (float)_r.NextDouble();
	}
	public virtual float Next(float max)
	{
		return (float)_r.NextDouble() * max;
	}
	public virtual float Next(float min, float max)
	{
		return (float)_r.NextDouble() * (max - min ) + min;
	}
	public virtual float Next(DataRange dr)
	{
		return (float)_r.NextDouble() * (dr.to - dr.from) + dr.to;
	}
	public virtual int NextByte()
	{
		return Mathf.Clamp((int)(_r.NextDouble()*256),0,255);
	}
	public virtual void Reset()
	{
		SetSeed(System.DateTime.Now.Millisecond);
	}
	public virtual void SetSeed(int seed)
	{
		_seed = seed;
		_r = new System.Random(seed);
	}
}

public class LugusRandomGeneratorUniform : ILugusRandomGenerator
{
	public LugusRandomGeneratorUniform(){}
	public LugusRandomGeneratorUniform(int seed)
	{
		SetSeed(seed);
	}
	public Vector2 InsideUnitCircle()
	{
		return new Vector2(Next(-1,1),Next(-1,1));
	}
	public Vector3 InsideUnitSphere()
	{
		return new Vector3(Next(-1,1),Next(-1,1),Next(-1,1));
	}
}

public class LugusRandomGeneratorPerlin : ILugusRandomGenerator
{
	protected float _scale;
	public float Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
		}
	}
	public LugusRandomGeneratorPerlin():this(10,10,1, System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorPerlin(float width, float height,float scale,int seed)
	{
		SetSeed(seed);
		_dr = new DataRange(width,height);
		_scale = scale;
	}
	public override float Next ()
	{
		return Perlin(base.Next()/_dr.from,base.Next()/_dr.to);
	}
	public float Perlin(float x, float y)
	{
		return Mathf.PerlinNoise(x/_dr.from*_scale,y/_dr.to*_scale);
	}
}
//Source: http://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/ 
public class LugusRandomGeneratorGoldenRatio : ILugusRandomGenerator
{
	private float _goldenRatio = 0.618033988749895f;
	private float _currentRandom;
	private int _iterator = 0;
	public LugusRandomGeneratorGoldenRatio():this(0.0f,1.0f,System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorGoldenRatio(float min,float max,int seed)
	{
		_dr = new DataRange(min,max);
		SetSeed(seed);
	}
	public override float Next()
	{
		float nextGolden = GoldenRatio(_dr.from, _dr.to, _iterator);
		_iterator++;
		return nextGolden;
	}
	public float GoldenRatio(float min, float max, int i)
	{
		return GoldenRatioNormalized(i) * (max - min ) + min;
	}
	public float GoldenRatioNormalized(int i)
	{
		return (_currentRandom + (_goldenRatio * i)) % 1;
	}
	public override void SetSeed(int seed)
	{
		base.SetSeed(seed);
		_currentRandom = base.Next();
		_iterator = 0;
	}
	public override void Reset()
	{
		SetSeed(System.DateTime.Now.Millisecond);
	}
}

public class LugusRandomGeneratorSequence : ILugusRandomGenerator
{
	protected List<float> _listRange;
	public List<float> ListRange {
		get
		{
			return _listRange;
		}
		set
		{
			_listRange = value;
			_iterator = Sequence();
		}
	}
	protected IEnumerator _iterator;
	protected int _maxShuffles = 50;
	public LugusRandomGeneratorSequence() : this(1){}
	public LugusRandomGeneratorSequence(int max) : this(0,max){}
	public LugusRandomGeneratorSequence(int min, int max) : this(min, max, System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorSequence(int min, int max,int seed)
	{
		SetSeed(seed);
		_dr = new DataRange(min,max);
		FillList(_dr);
	}
	public override float Next()
	{
		_iterator.MoveNext();
		return (float)_iterator.Current;
	}
	public IEnumerator Sequence()
	{
		while (true) 
		{
			float previous = _listRange[ListRange.Count-1];
			int x = 0;
			do
			{
				Shuffle(_listRange);
			}while(x > _maxShuffles || previous == _listRange[0]);

			for (int i = 0; i < _listRange.Count; i++) 
			{
				yield return (float)_listRange[i];
			}
		}
	}
	protected void FillList(DataRange dr)
	{
		_listRange = new List<float>();
		for (int i = (int)dr.from; i <= (int)dr.to; i++) 
		{
			_listRange.Add(i);
		}
		_iterator = Sequence();
	}
	public void Shuffle<T>(List<T> deck)
	{
		int n = deck.Count; 
		for (int i = 0; i < n; i++) 
		{
			int r = i + _r.Next( n-i);     // between i and N-1
			T temp = deck[i];
			deck[i] = deck[r];
			deck[r] = temp;
		}
	}
	public void Shuffle<T>(List<T> deck, int low, int high )
	{
		if(low < 0 || low > deck.Count || low > high || high > deck.Count)
		{
			Debug.LogError("low or high is falsely set");
		}
		else
		{
			for (int i = low; i <= high; i++) 
			{
				int r = i + _r.Next(high-i+1);     // between i and high
				T temp = deck[i];
				deck[i] = deck[r];
				deck[r] = temp;
			}
		}
	}
	public override void Reset()
	{
		base.Reset();
		FillList(_dr);
	}
}
