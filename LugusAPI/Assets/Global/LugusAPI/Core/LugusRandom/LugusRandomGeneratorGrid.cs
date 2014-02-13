using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusRandomGeneratorGrid : ILugusRandomGenerator
{
	protected float _xDir;
	protected float _yDir;
	protected float _zDir;
	protected float _width;
	protected float _height;
	protected float _depth;
	protected int _rows;
	public int Rows
	{
		get
		{
			return _rows;
		}
		set
		{
			_rows = value;
			_yDir = _height / _rows;
		}
	}
	protected int _cols;
	public int Colums
	{
		get
		{
			return _cols;
		}
		set
		{
			_cols = value;
			_xDir = _width/_cols;
		}
	}
	protected int _stack;
	public int Stack
	{
		get
		{
			return _stack;
		}
		set
		{
			_stack = value;
			_zDir = _depth / _stack;
		}
	}
	protected float _spread;
	public float Spread
	{
		get
		{
			return _spread;
		}
		set
		{
			_spread = value;
		}
	}
	protected Vector3[,,] _grid;
	public Vector3[,,] Grid
	{
		get
		{
			return _grid;
		}
	}
	protected int _currentX = 0;
	protected int _currentY = 0;
	protected int _currentZ = 0;
	public LugusRandomGeneratorGrid( float width, float height, int rows, int cols, float spread = 0.4f):this (width,height,1,rows,cols,1,spread,System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorGrid( float width, float height, float depth, int rows, int cols, int stack, float spread = 0.4f):this(width,height,depth,rows,cols,stack,spread,System.DateTime.Now.Millisecond){}
	public LugusRandomGeneratorGrid( float width, float height, float depth, int rows, int cols, int stack, float spread , int seed)
	{
		_dr = new DataRange(0,cols*rows*stack); 
		SetSeed(seed);
		_xDir = width / cols;
		_yDir = height / rows;
		_zDir = depth / stack;
		_width = width;
		_height = height;
		_depth = depth;
		_rows = rows;
		_cols = cols;
		_stack = stack;
		_spread = spread;
		_grid = CreateScatterGrid();
	}
	public new Vector3 Next ()
	{
		Vector3 nextValue = _grid[_currentX,_currentY,_currentZ];
		_currentX++;
		if (_currentX%_cols == 0) 
		{
			_currentX = 0;
			_currentY++;
			if (_currentY%_rows == 0) 
			{
				_currentY = 0;
				_currentZ++;
				if (_currentZ%_stack == 0) 
				{
					_currentZ=0;
				}
			}
		}
		return nextValue;
	}
	//source : http://www.gamasutra.com/view/feature/130071/random_scattering_creating_.php?page=2
	public Vector3[,] CreateScatter()
	{
		Vector3[,] grid = new Vector3[_cols,_rows];
		
		for (int ix = 0;ix <(_cols);ix++) 
		{
			for (int iy = 0;iy<(_rows);iy++) 
			{ 
				grid[ix,iy] = new Vector3(_xDir* ix + Next(-_spread,_spread) *_xDir,_yDir*iy+Next(-_spread,_spread)*_yDir);
			}
		}
		return grid;
	}
	public Vector3 [,,] CreateScatterGrid()
	{
		Vector3[,,] grid = new Vector3[_cols,_rows,_stack];
		for (int ix = 0;ix <_cols;ix++) 
		{
			for (int iy = 0;iy<_rows;iy++) 
			{ 
				for (int iz = 0; iz < _stack; iz++) 
				{
					grid[ix,iy,iz] = new Vector3(_xDir* ix + Next(-_spread,_spread) *_xDir,_yDir*iy+Next(-_spread,_spread)*_yDir,_zDir*iz+Next(-_spread,_spread)*_zDir);
				}
			}
		}
		return grid;
	}
	public override void SetSeed (int seed)
	{
		base.SetSeed (seed);
		ResetGrid();
	}
	public override void Reset ()
	{
		base.Reset ();
		ResetGrid();
	}
	public void ResetGrid()
	{
		_currentX=0;
		_currentY=0;
		_currentZ=0;
		_grid = CreateScatterGrid();
	}
}