using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// keeps a history of data so it can be processed afterwards
// primarily made for use in signal processing (ex. extracting average value over a series of frames)
public class DataInterval<T>
{
	public LinkedList<T> internalList;
	
	public DataInterval(int intervalSize)
	{
		this.intervalSize = intervalSize;
		
		internalList = new LinkedList<T>();
	}
	
	public DataInterval()
	{	
		internalList = new LinkedList<T>();
	}
	
	public int intervalSize = 30;
	
	public int Count
	{
		get
		{
			return internalList.Count;
		}
	}
	
	public void Add(T val)
	{
		if( this.Count >= intervalSize )
			internalList.RemoveLast();
		
		internalList.AddFirst(val);
	}
	
	public bool IsPrevalentValue( T val)
	{
		return IsPrevalentValue(intervalSize, val, 0.6f);
	}
	
	public bool IsPrevalentValue(int sampleSize, T val)
	{
		return IsPrevalentValue(sampleSize, val, 0.6f);
	}
	
	public bool IsPrevalentValue(int sampleSize, T val, float percentage)
	{
		if( sampleSize > intervalSize )
		{
			Debug.LogError("DataInterval:IsPrevalentValue : sampleSize > intervalSize! " + sampleSize + " > " + intervalSize );
			sampleSize = intervalSize;
		}
		
		int equalCount = 0;
		
		int counter = 0;
		foreach( T el in internalList )
		{
			if( counter > sampleSize )
				break;
			
			if( EqualityComparer<T>.Default.Equals(el, val) )
				equalCount++;
			
			counter++;
		}
		
		if( (equalCount / (sampleSize)) > percentage )
			return true;
		else
			return false;
	}
	
	public bool ValuesGreaterThan(int sampleSize, T val)
	{
		// are most values in the interval greater than the value?
		if( sampleSize > intervalSize )
		{
			Debug.LogError("DataInterval:ValuesGreaterThan : sampleSize > intervalSize! " + sampleSize + " > " + intervalSize );
			sampleSize = intervalSize;
		}
		
		int greaterCount = 0;
		int smallerCount = 0;
		
		int counter = 0;
		foreach( T el in internalList )
		{
			if( counter > sampleSize )
				break;
			
			
			
			if( Comparer<T>.Default.Compare(el, val) > 0 )
				greaterCount++;
			else 
				smallerCount++;
			
			counter++;
		}
		
		if( greaterCount > smallerCount )
			return true;
		else
			return false;
		
	}
	
	public bool ValuesSmallerThan(int sampleSize, T val)
	{
		// are most values in the interval greater than the value?
		if( sampleSize > intervalSize )
		{
			Debug.LogError("DataInterval:ValuesSmallerThan : sampleSize > intervalSize! " + sampleSize + " > " + intervalSize );
			sampleSize = intervalSize;
		}
		
		int greaterCount = 0;
		int smallerCount = 0;
		
		int counter = 0;
		foreach( T el in internalList )
		{
			if( counter > sampleSize )
				break;
			
			if( Comparer<T>.Default.Compare(el, val) >= 0 )
				greaterCount++;
			else 
				smallerCount++;
			
			counter++;
		}
		
		if( greaterCount < smallerCount )
			return true;
		else
			return false;
		
	}
}
