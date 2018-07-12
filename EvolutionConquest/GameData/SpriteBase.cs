using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpriteBase
{
    private int _climateHeightPercent;
    private int _worldSize;
    private Texture2D _texture;
    private Vector2 _position;
    private Rectangle _bounds;
    private List<Point> _gridPositions;
    private int _BottomOfCold;
    private int _TopOfHot;

    public int WorldSize
    {
        get { return _worldSize; }
        set
        {
            _worldSize = value;
        }
    }
    public int ClimateHeightPercent
    {
        get
        {
            return _climateHeightPercent;
        }
        set
        {
            _climateHeightPercent = value;
            _BottomOfCold = (int)(WorldSize * (_climateHeightPercent * 0.01));
            _TopOfHot = WorldSize - (int)(WorldSize * (_climateHeightPercent * 0.01));
        }
    }
    public Texture2D Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            TextureCollideDistance = (int)Math.Ceiling(Math.Sqrt(_texture.Width * _texture.Width + _texture.Height * _texture.Height));
            CalculateBounds();
            _bounds.X = (int)(_position.X - (Texture.Width / 2));
            _bounds.Y = (int)(_position.Y - (Texture.Height / 2));
        }
    }
    public Vector2 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;

            if (_position.X < 0)
                _position.X = 0;
            if (_position.Y < 0)
                _position.Y = 0;
            if (_position.X > WorldSize)
                _position.X = WorldSize;
            if (_position.Y > WorldSize)
                _position.Y = WorldSize;

            if (Texture != null)
            {
                _bounds.X = (int)Math.Round((_position.X - (Texture.Width / 2)), 0);
                _bounds.Y = (int)Math.Round((_position.Y - (Texture.Height / 2)), 0);
            }
            if (!IsInCold && _position.Y <= _BottomOfCold)
            {
                IsInCold = true;
            }
            else if (!IsInHot && _position.Y >= _TopOfHot)
            {
                IsInHot = true;
            }
            else if (_position.Y > _BottomOfCold && _position.Y < _TopOfHot)
            {
                IsInCold = false;
                IsInHot = false;
            }

            OnPositionSet();
        }
    }
    public bool IsInCold { get; set; }
    public bool IsInHot { get; set; }
    public Vector2 Origin { get; set; }
    public Rectangle Bounds
    {
        get
        {
            return _bounds;
        }
        set
        {
            _bounds = value;
        }
    }
    public int TextureCollideDistance { get; set; }
    public List<Point> GridPositions
    {
        get
        {
            return _gridPositions;
        }
        set
        {
            OldGridPositions = _gridPositions;
            _gridPositions = value;
        }
    } //The list of grid positions for the creature
    public List<Point> OldGridPositions { get; set; } //The list of grid positions for the creature
    public string CurrentGridPositionsForCompare { get; set; }
    public string OldGridPositionsForCompare { get; set; }

    public SpriteBase()
    {
        _position = Vector2.Zero;
        GridPositions = new List<Point>();
        OldGridPositions = new List<Point>();
        CurrentGridPositionsForCompare = String.Empty;
        OldGridPositionsForCompare = String.Empty;
    }

    public void CalculateBounds()
    {
        Bounds = new Rectangle((int)(Position.X - (Texture.Width / 2)), (int)(Position.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);
    }
    public Vector2 CalculateGridPositionVector(int cellSize)
    {
        Vector2 pos = new Vector2();

        //Divide the position by the cell size then cast to int which does the same as Math.Floor. 
        //Multiply by the cell size to find the upper left cell corner. 
        //This only tells you the cell the object is in centered over not accounting for texture height/width
        pos.X = (int)(Position.X / cellSize) * cellSize;
        pos.Y = (int)(Position.Y / cellSize) * cellSize;

        return pos;
    }
    public Point CalculateGridPosition(int cellSize)
    {
        Point pos = new Point();
        int maxIndex = WorldSize / cellSize - 1;
        //Divide the position by the cell size then cast to int which does the same as Math.Floor. 
        //Multiply by the cell size to find the upper left cell corner. 
        //This only tells you the cell the object is in centered over not accounting for texture height/width
        pos.X = (int)(Position.X / cellSize);
        pos.Y = (int)(Position.Y / cellSize);

        if (pos.X < 0)
            pos.X = 0;
        if (pos.Y < 0)
            pos.Y = 0;
        if (pos.X > maxIndex)
            pos.X = maxIndex;
        if (pos.Y > maxIndex)
            pos.Y = maxIndex;

        return pos;
    }
    public List<Point> GetGridDeltaAdd()
    {
        List<Point> delta = new List<Point>();

        foreach (Point p in GridPositions)
        {
            bool found = false;

            foreach (Point o in OldGridPositions)
            {
                if (p.X == o.X && p.Y == o.Y)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                delta.Add(p);
            }
        }

        return delta;
    }
    public List<Point> GetGridDelta()
    {
        List<Point> delta = new List<Point>();

        foreach (Point p in OldGridPositions)
        {
            bool found = false;

            foreach (Point o in GridPositions)
            {
                if (p.X == o.X && p.Y == o.Y)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                delta.Add(p);
            }
        }

        return delta;
    }
    public void GetGridPositionsForSpriteBase(int gridCellSize, GameData _gameData)
    {
        List<Point> gridPositions = new List<Point>();

        //Find the exact grid position we are in then check the surrounding grid locations
        Point exactGridPos = CalculateGridPosition(gridCellSize);
        gridPositions.Add(exactGridPos);

        if (exactGridPos.X > 0 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X - 1, exactGridPos.Y].CellRectangle))
        {
            gridPositions.Add(new Point(exactGridPos.X - 1, exactGridPos.Y));
            if (exactGridPos.Y > 0 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X - 1, exactGridPos.Y - 1].CellRectangle))
            {
                gridPositions.Add(new Point(exactGridPos.X - 1, exactGridPos.Y - 1));
            }
            else if (exactGridPos.Y < _gameData.MapGridData.GetLength(1) - 1 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X - 1, exactGridPos.Y + 1].CellRectangle))
            {
                gridPositions.Add(new Point(exactGridPos.X - 1, exactGridPos.Y + 1));
            }
        }
        else if (exactGridPos.X < _gameData.MapGridData.GetLength(0) - 1 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X + 1, exactGridPos.Y].CellRectangle))
        {
            gridPositions.Add(new Point(exactGridPos.X + 1, exactGridPos.Y));
            if (exactGridPos.Y > 0 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X + 1, exactGridPos.Y - 1].CellRectangle))
            {
                gridPositions.Add(new Point(exactGridPos.X + 1, exactGridPos.Y - 1));
            }
            else if (exactGridPos.Y < _gameData.MapGridData.GetLength(1) - 1 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X + 1, exactGridPos.Y + 1].CellRectangle))
            {
                gridPositions.Add(new Point(exactGridPos.X + 1, exactGridPos.Y + 1));
            }
        }

        if (exactGridPos.Y > 0 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X, exactGridPos.Y - 1].CellRectangle))
        {
            gridPositions.Add(new Point(exactGridPos.X, exactGridPos.Y - 1));
        }
        else if (exactGridPos.Y < _gameData.MapGridData.GetLength(1) - 1 && Bounds.Intersects(_gameData.MapGridData[exactGridPos.X, exactGridPos.Y + 1].CellRectangle))
        {
            gridPositions.Add(new Point(exactGridPos.X, exactGridPos.Y + 1));
        }

        //Move the Current Grid position string to the Old
        string gridPosition = String.Empty;
        foreach (Point p in gridPositions)
        {
            gridPosition += p.X + "," + p.Y + " ";
        }

        OldGridPositionsForCompare = CurrentGridPositionsForCompare;
        CurrentGridPositionsForCompare = gridPosition;

        GridPositions = gridPositions;
    }
    public virtual void OnPositionSet()
    {
    }
}