using UnityEngine;
using System.Collections;

public static class Common{

	public static float Approach(float start, float end, float shift)
	{
		if (start < end)
			return Mathf.Min(start + shift, end);
		else
			return Mathf.Max(start - shift, end);
	}

	public static float ApproachWeighted (float targetValue, float currentValue, float tagetWeight, float threshold = 0.01f)
	{
		float newValue;

		newValue = tagetWeight * targetValue + (1 - tagetWeight) * currentValue;
		if (Mathf.Abs (currentValue) < threshold)
			currentValue = 0.0f;
		if (Mathf.Abs (targetValue) < threshold)
			currentValue = targetValue;

		return newValue;
	}

	public static float ApproachUniform (float targetValue, float currentValue, float changeRatio)
	{
		float sign = Mathf.Sign (targetValue - currentValue);
		currentValue += changeRatio * sign;
		if ( currentValue > sign * targetValue)
			currentValue = targetValue;

		//currentSpeedY += gravity * Mathf.Sign (targetSpeedY - currentSpeedY);
		//if ( Mathf.Abs (currentSpeedY) > maxSpeedY)
		//	currentSpeedY = targetSpeedY;

		return currentValue;
	}
}
