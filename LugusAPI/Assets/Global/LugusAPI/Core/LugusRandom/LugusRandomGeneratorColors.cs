using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//source: http://devmag.org.za/2012/07/29/how-to-choose-colours-procedurally-algorithms/#more-4948
public class LugusRandomGeneratorColors : ILugusRandomGenerator
{
	public LugusRandomGeneratorColors():this(System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorColors(int seed)
	{
		_dr = new DataRange(0,255);
		SetSeed(seed);
	}
	public new Color Next ()
	{
		return new Color(base.Next(), base.Next(), base.Next());
	}
	//source: http://wiki.unity3d.com/index.php/HSBColor
	public Color ColorFromHSV(float h, float s, float v, float a = 1)
	{
		float r = v;
		float g = v;
		float b = v;
		if (s != 0)
		{
			float max = v;
			float dif = v * s;
			float min = v - dif;
			
			if (h < 60f)
			{
				r = max;
				g = h * dif / 60f + min;
				b = min;
			}
			else if (h < 120f)
			{
				r = -(h - 120f) * dif / 60f + min;
				g = max;
				b = min;
			}
			else if (h < 180f)
			{
				r = min;
				g = max;
				b = (h - 120f) * dif / 60f + min;
			}
			else if (h < 240f)
			{
				r = min;
				g = -(h - 240f) * dif / 60f + min;
				b = max;
			}
			else if (h < 300f)
			{
				r = (h - 240f) * dif / 60f + min;
				g = min;
				b = max;
			}
			else if (h <= 360f)
			{
				r = max;
				g = min;
				b = -(h - 360f) * dif / 60 + min;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
		
		return new Color(Mathf.Clamp01(r),Mathf.Clamp01(g),Mathf.Clamp01(b),a);
	}
	public List<Color> GenerateColorsUniform(int colorCount)
	{
		List<Color> colors = new List<Color>();
		for (int i = 0; i < colorCount; i++)
		{
			Color newColor = new Color(NextByte(),NextByte(),NextByte());
			colors.Add(newColor);
		}
		return colors;
	}
	public List<Color> GenerateColorsHarmonyAnalogous(int colorCount,float rangeAngle,float saturation, float luminance, float saturationRange = 0, float luminanceRange = 0 )
	{
		return GenerateColorsHarmony(colorCount,0,0,rangeAngle,0,0,saturation,luminance,saturationRange,luminanceRange);
	}
	public List<Color> GenerateColorsHarmonyComplementary(int colorCount,float offsetAngle1,float offsetAngle2,float rangeAngle0,float rangeAngle1,float saturation, float luminance, float saturationRange = 0, float luminanceRange = 0)
	{
		return GenerateColorsHarmony(colorCount,180,0,rangeAngle0,rangeAngle1,0,saturation,luminance,saturationRange,luminanceRange);
	}
	public List<Color> GenerateColorsHarmonyTriad(int colorCount,float offsetAngle1,float offsetAngle2,float rangeAngle0,float rangeAngle1,float rangeAngle2,float saturation, float luminance, float saturationRange = 0, float luminanceRange = 0)
	{
		return GenerateColorsHarmony(colorCount,120,240,rangeAngle0,rangeAngle1,rangeAngle2,saturation,luminance,saturationRange,luminanceRange);
	}
	public List<Color> GenerateColorsHarmony(int colorCount,float offsetAngle1,float offsetAngle2,float rangeAngle0,float rangeAngle1,float rangeAngle2,float saturation, float luminance, float saturationRange = 0, float luminanceRange = 0)
	{
		List<Color> colors = new List<Color>();
		float referenceAngle = base.Next() * 360;
		for (int i = 0; i < colorCount+1; i++)
		{
			float randomAngle = base.Next(rangeAngle0 + rangeAngle1 + rangeAngle2);
			if (randomAngle > rangeAngle0)
			{
				if (randomAngle < rangeAngle0 + rangeAngle1)
				{
					randomAngle += offsetAngle1;
				}
				else
				{
					randomAngle += offsetAngle2;
				}
			}
			float newSaturation = saturation + (base.Next() - 0.5f) * saturationRange;
			float newLuminance = luminance + +(base.Next() - 0.5f) * luminanceRange;
			Color color = ColorFromHSV((referenceAngle + randomAngle) % 360.0f, newSaturation, newLuminance);
			colors.Add(color);
		}
		return colors;
	}
}
