using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataPointAverager
{
	public enum Calculation
	{
		Average,
		Median
	}

	private List<float> _dataPoints;

	private int _maxPointCount;

	private int _currentIndex;

	private bool _isCachedAverageValid;

	private float _cachedAverage;

	private Calculation _calculation;

	public float Average
	{
		get
		{
			if (_isCachedAverageValid)
			{
				return _cachedAverage;
			}
			else
			{
				_cachedAverage = CalculateAverage();
				_isCachedAverageValid = true;
				return _cachedAverage;
			}
		}
	}

	public DataPointAverager(int inMaxPointCount, Calculation inCalculation)
	{
		_maxPointCount = inMaxPointCount;
		_calculation = inCalculation;
		ResetDataPoints();
	}

	public void AddDataPoint(float inDataPoint)
	{
		if (_dataPoints.Count + 1 <= _maxPointCount)
		{
			_dataPoints.Add(inDataPoint);
		}
		else
		{
			_dataPoints[_currentIndex] = inDataPoint;
		}

		_currentIndex = (_currentIndex + 1) % _maxPointCount;

		InvalidateCachedAverage();
	}

	public void ResetDataPoints()
	{
		_currentIndex = 0;
		_dataPoints = new List<float>();
		InvalidateCachedAverage();
	}

	private float CalculateAverage()
	{
		if (_dataPoints.Count > 0)
		{
			if (_calculation == Calculation.Average)
			{
				float average = _dataPoints.Sum() / _dataPoints.Count;
				return average;
			}
			else if (_calculation == Calculation.Median)
			{
				List<float> sortedDataPoints = new List<float>(_dataPoints);
				sortedDataPoints.Sort();
				int middleIndex = sortedDataPoints.Count / 2;
				float median = sortedDataPoints[middleIndex];
				return median;
			}
		}

		return 0;
	}

	private void InvalidateCachedAverage()
	{
		_isCachedAverageValid = false;
	}
}
