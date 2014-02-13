using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusRandom 
{
	private static LugusRandomDefault _instance = null;
	
	public static LugusRandomDefault use 
	{ 
		get 
		{
			if ( _instance == null )
			{
				_instance = new LugusRandomDefault();
			}

			return _instance; 
		}
	}
	
	public static void Change(LugusRandomDefault newInstance)
	{
		_instance = newInstance;
	}
}

public class LugusRandomDefault
{
	protected LugusRandomGeneratorDistribution _distribution;
	public LugusRandomGeneratorDistribution Distr
	{
		get
		{
			if(_distribution == null)
			{
				_distribution = new LugusRandomGeneratorDistribution(Distribution.Gaussian);
			}
			return _distribution;
		}
	}
	protected LugusRandomGeneratorDistribution _gaussian;
	public LugusRandomGeneratorDistribution Gaussian
	{
		get
		{
			if(_gaussian == null)
			{
				_gaussian = new LugusRandomGeneratorDistribution(Distribution.Gaussian);
			}
			return _gaussian;
		}
	}
	protected LugusRandomGeneratorDistribution _exponential;
	public LugusRandomGeneratorDistribution Exponential
	{
		get
		{
			if(_exponential == null)
			{
				_exponential = new LugusRandomGeneratorDistribution(Distribution.Exponential);
			}
			return _exponential;
		}
	}
	protected LugusRandomGeneratorDistribution _triangular;
	public LugusRandomGeneratorDistribution Triangular
	{
		get
		{
			if(_triangular == null)
			{
				_triangular = new LugusRandomGeneratorDistribution(Distribution.Triangular);
			}
			return _triangular;
		}
	}
	protected LugusRandomGeneratorDistribution _doubleGauss;
	public LugusRandomGeneratorDistribution DoubleGaussian
	{
		get
		{
			if(_doubleGauss == null)
			{
				_doubleGauss = new LugusRandomGeneratorDistribution(Distribution.DoubleGaussian);
			}
			return _doubleGauss;
		}
	}
	protected LugusRandomGeneratorSequence _sequence;
	public LugusRandomGeneratorSequence Sequence
	{
		get
		{
			if(_sequence == null)
			{
				_sequence = new LugusRandomGeneratorSequence(0,10);
			}
			return _sequence;
		}
	}
	protected LugusRandomGeneratorGrid _grid;
	public LugusRandomGeneratorGrid Grid
	{
		get
		{
			if(_grid == null)
			{
				_grid =  new LugusRandomGeneratorGrid(10,10,10,10);
			}
			return _grid;
		}
	}
	protected LugusRandomGeneratorPerlin _perlin;
	public LugusRandomGeneratorPerlin Perlin
	{
		get
		{
			if(_perlin == null)
			{
				_perlin =  new LugusRandomGeneratorPerlin();
			}
			return _perlin;
		}
	}
	protected LugusRandomGeneratorGoldenRatio _goldenRatio;
	public LugusRandomGeneratorGoldenRatio GoldenRatio
	{
		get
		{
			if(_goldenRatio == null)
			{
				_goldenRatio = new LugusRandomGeneratorGoldenRatio();
			}
			return _goldenRatio;
		}
	}
	protected LugusRandomGeneratorUniform _uniform;
	public LugusRandomGeneratorUniform Uniform
	{
		get
		{
			if(_uniform == null)
			{
				_uniform = new LugusRandomGeneratorUniform();
			}
			return _uniform;
		}
	}
	//protected List<ILugusRandomGenerator> rndList;
	public LugusRandomDefault()
	{
		//rndList = new Light<ILugusRandomGenerator>();
	}

	public float Next()
	{
		return _uniform.Next();
	}
	public float Next(float max)
	{
		return _uniform.Next(max);
	}
	public float Next(float min, float max)
	{
		return _uniform.Next(min, max);
	}
	public void Reset()
	{
		_uniform.Reset();
	}
	public void ResetAll()
	{
//		_uniform = null;
//		_distribution
	}
	public void SetSeed(int seed)
	{
		_uniform.SetSeed(seed);
	}
	public void SetRange(DataRange dr)
	{
		_uniform.Range = dr;
		Reset();
	}
}

