using UnityEngine;
using System.Collections;

// Used to define a range of data between 2 numbers
// afterwards, we can work with percentages to get the corresponding real value within the interval (ex. 75% of the way in the interval [1,10] is 7.5)
// the other way around works too: find out to which percentage a certain value maps (ex. how far in the interval [1,10] is 5? )
public class DataRange 
{
	public float from = 0.0f;
	public float to = 0.0f;
	
	public DataRange(float from, float to)
	{
		this.from = from;
		this.to = to;
	}
	
	// returns the percentage in [0,1]
	public float PercentageInInterval(float val)
	{
		// -7 to -2 => 0 to 5
		// -7 to 2 => 0 to 9
		// 10 to 15 => 20 to 25
		
		float small = (this.from < this.to) ? this.from : this.to;
		float large = (this.from > this.to) ? this.from : this.to;
		
		if( val < small )
			return 0.0f;
		else if( val > large )
			return 1.0f;
		
		
		
		// transform the interval to all positive values
		// by adding the absolute of the smallest to all values
		float absSmall = Mathf.Abs( small );
		small += absSmall; // for negative numbers, small will become 0
		large += absSmall;
		val += absSmall;
		
		float intervalWidth = large - small; // this indicates how much is actually 100%
		float valueLocation = val - small; // how much along the interval we are from the starting point (small)
		
		float output = valueLocation / intervalWidth; // value / width
		
		return output;
	}
	
	public float ValueFromPercentage(float percentage)
	{
		// -0.7 -0.2 -> 0.5 => 0 to 5 en 0.5 * 5 = 2.5 + -7 = -4.5, which iz...correct!
		
		percentage = Mathf.Clamp01(percentage);
		
		// reverse is important here, as otherwhise we would get wrong values for intervals where from > to
		bool reverse = (this.from > this.to);  
		
		float small = (reverse) ? this.to : this.from; 
		float large = (reverse) ? this.from : this.to;
		
		percentage = (reverse) ? (1 - percentage) : percentage;
		
		// transform the interval to all positive values
		// by adding the absolute of the smallest to all values
		float absSmall = Mathf.Abs( small );
		small += absSmall; // for negative numbers, small will become 0
		large += absSmall;
		
		float intervalWidth = large - small;
		float output = percentage * intervalWidth;
		
		output += small;
		output -= absSmall;
		
		return output;
	}
	
}
