using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Distribution
{
	Gaussian,
	DoubleGaussian,
	Exponential,
	InverseExponential,
	DoubleExponential,
	Triangular,
	DoubleTriangular
}

public class LugusRandomGeneratorDistribution : ILugusRandomGenerator {
	protected delegate float DistributionDelegate(float min, float max, float delta);
	protected DistributionDelegate DistributionMethod;
	protected float _delta;
	public float Delta
	{
		get {
			return _delta;
		}
		set {
			_delta = value;
		}
	}
	protected Distribution _distType;
	public Distribution DistType 
	{
		get {
			return _distType;
		}
		set {
			_distType = value;
			SetDistributionMethod(_distType);
		}
	}
	protected bool _firstOverlap;
	protected float _defaultValue = 9999;
	protected float _expLimit = 6;
	public LugusRandomGeneratorDistribution(Distribution type):this(type,0,1){}
	public LugusRandomGeneratorDistribution(Distribution type,float min, float max, float delta = 9999):this(type, min, max, delta, System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorDistribution(Distribution type,float min, float max, float delta, int seed)
	{
		SetDistributionMethod(type);
		SetSeed(seed);
		_delta = delta;
		_firstOverlap = true;
		_dr = new DataRange(min,max);
	}
	public override float Next()
	{
		return Next(_dr,_delta);
	}
	public override float Next(float min, float max)
	{
		return DistributionMethod(min,max,_delta);
	}
	public float Next(float min, float max, float delta)
	{
		return DistributionMethod(min, max, delta);
	}
	public override float Next (DataRange dr)
	{
		return DistributionMethod(dr.from,dr.to,_delta);
	}
	public float Next(DataRange dr, float delta)
	{
		return DistributionMethod(dr.from,dr.to,delta);
	}
	public float NextDoubleDist()
	{
		switch (_distType) 
		{
			case Distribution.Gaussian:
			case Distribution.Triangular:
				return DoubleDistribution(_dr.from,_dr.to,_delta,DistributionMethod);
			case Distribution.Exponential:
			case Distribution.InverseExponential:
				return DoubleExponential(_dr.from,_dr.to,_delta);
			case Distribution.DoubleGaussian:
			case Distribution.DoubleExponential:
			case Distribution.DoubleTriangular:
				return Next();
			default:
				return float.NaN;
		}
	}
	protected void SetDistributionMethod(Distribution distType)
	{
		_distType = distType;
		switch (distType) 
		{
			case Distribution.Gaussian:
				DistributionMethod = new DistributionDelegate(Gaussian);
				break;
			case Distribution.DoubleGaussian:
				DistributionMethod = new DistributionDelegate(DoubleGaussian);
				break;
			case Distribution.Exponential:
				DistributionMethod = new DistributionDelegate(Exponential);
				break;
			case Distribution.InverseExponential:
				DistributionMethod = new DistributionDelegate(InvExponential);
				break;
			case Distribution.DoubleExponential:
				DistributionMethod = new DistributionDelegate(DoubleExponential);
				break;
			case Distribution.Triangular:
				DistributionMethod = new DistributionDelegate(Triangular);
				break;
			case Distribution.DoubleTriangular:
				DistributionMethod = new DistributionDelegate(DoubleTriangular);
				break;
			default:
				break;
			}
	}
	
	//Algorithms from http://introcs.cs.princeton.edu/java/stdlib/StdRandom.java.html
	public float Gaussian(float minValue, float maxValue , float delta)
	{
		if(minValue>maxValue)
		{
			Debug.LogWarning("minValue is bigger than maxValue");
			return float.NaN;
		}
		if(delta != _defaultValue && delta <= 0)
		{
			Debug.LogError("Delta is less than 0. Using default delta.");
			delta = _defaultValue;
		}
		float mean = (maxValue + minValue) * 0.5f;
		float sigma = maxValue - mean;
		if(delta == _defaultValue)
		{
			sigma /= 3.0f;
		}
		else 
		{
			sigma /= delta;	
		}
		
		return Mathf.Clamp((NormalizedGaussian()*sigma) + mean , minValue, maxValue);
	}
	public float Gaussian(float[] values)
	{
		int max = values.Length;
		return values[(int)Gaussian(0,max,_delta)];
	}
	public float DoubleGaussian(float min, float max, float delta)
	{
//		if(delta != _defaultValue && delta > max - min)
//		{
//			Debug.LogError("Overlap is too big. Using default overlap");
//			delta = _defaultValue;
//		}
//		if(delta == _defaultValue)
//		{
//			delta = 1;
//		}
//		float mean = (max + min) * 0.5f;
//		_firstOverlap = !_firstOverlap;
//		if(_firstOverlap)
//		{
//			return Gaussian(min , mean + delta, _defaultValue);
//		}
//		else
//		{
//			return Gaussian(mean - delta , max ,_defaultValue);
//		}
		return DoubleDistribution(min,max,delta,new DistributionDelegate(Gaussian));
	}
	protected float NormalizedGaussian()
	{
		//http://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform#Polar_form
		float x1, x2, w;//, y1, y2;
		do {
			x1 = 2.0f * (float)_r.NextDouble() - 1.0f;
			x2 = 2.0f * (float)_r.NextDouble() - 1.0f;
			w = x1 * x1 + x2 * x2;
		} while ( w >= 1.0f );
		
		w = Mathf.Sqrt( (-2.0f * Mathf.Log( w ) ) / w );
		return x1 * w;
		//y2 = x2 * w;
	}
	public float Exponential(float min, float max, float delta)
	{
		//http://en.wikipedia.org/wiki/Exponential_distribution
		//Based on the article chosen limit 0->6
		if(delta != _defaultValue && delta <= 0)
		{
			Debug.LogError("Lambda is not bigger than 0. Using default value");
			delta = _defaultValue;
		}
		if(delta == _defaultValue)
		{
			delta = 1;
		}
		float exp = min + ExponentialNormalized(delta)  / _expLimit * (max - min);
		return Mathf.Clamp(exp,min,max);
	}
	public float InvExponential(float min, float max, float delta)
	{
		return max - (Exponential(min,max,delta) + min);
	}
	public float DoubleExponential(float min, float max, float delta)
	{
		//float mean = (max + min) * 0.5f;
		_firstOverlap = !_firstOverlap;
		if(_firstOverlap)
		{
			return Exponential(min , max, delta);
		}
		else
		{
			return InvExponential(min , max , delta);
		}
	}
	protected float ExponentialNormalized(float lambda) {
		return -Mathf.Log(1 - base.Next() / lambda);
	}
	public float Triangular(float min,float max,float delta) {
		//http://en.wikipedia.org/wiki/Triangular_distribution#Generating_Triangular-distributed_random_variates
		if(delta != _defaultValue && (delta < min || delta > max) )
		{
			Debug.LogError("Mode is not between min and max. Using default mode");
			delta = _defaultValue;
		}
		if(delta == _defaultValue)
		{
			delta = (max + min) / 2.0f;
		}
		float u = (float)_r.NextDouble();
		float f = (delta - min) / (max - min);
		if (u <= f)
			return min + Mathf.Sqrt(u * (max - min) * (delta - min));
		else
			return max - Mathf.Sqrt((1 - u) * (max - min) * (max - delta));
	}
	public float DoubleTriangular(float min, float max, float delta)
	{
//		if(delta != _defaultValue && delta > max - min)
//		{
//			Debug.LogError("Overlap is too big. Using default overlap");
//			delta = _defaultValue;
//		}
//		if(delta == _defaultValue)
//		{
//			delta = 1;
//		}
//		float mean = (max + min) * 0.5f;
//		_firstOverlap = !_firstOverlap;
//		if(_firstOverlap)
//		{
//			return Triangular(min , mean + delta, _defaultValue);
//		}
//		else
//		{
//			return Triangular(mean - delta , max ,_defaultValue);
//		}
		return DoubleDistribution(min,max,delta,new DistributionDelegate(Triangular));
	}
	protected float DoubleDistribution(float min, float max, float delta, DistributionDelegate method)
	{
		if(delta != _defaultValue && delta > max - min)
		{
			Debug.LogError("Overlap is too big. Using default overlap");
			delta = _defaultValue;
		}
		if(delta == _defaultValue)
		{
			delta = 1;
		}
		float mean = (max + min) * 0.5f;
		_firstOverlap = !_firstOverlap;
		if(_firstOverlap)
		{
			return method(min, mean + delta, _defaultValue);// Next(min , mean + delta, _defaultValue);
		}
		else
		{
			return method(mean - delta, max, _defaultValue);//Next(mean - delta , max ,_defaultValue);
		}
	}
}
