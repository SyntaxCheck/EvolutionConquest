using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Slider
{
    private GraphicsDevice _device;
    private int _markerPosition;
    private Vector2 _markerTexturePosition;
    private float _currentValue;

    public bool Visible { get; set; }
    public Vector2 SliderPosition { get; set; } //Position is upper left corner
    public bool SliderActive { get; set; }
    public int BarWidth { get; set; }
    public int BarHeight { get; set; }
    public int MarkerWidth { get; set; }
    public int MarkerHeight { get; set; }
    public int MarkerPosition //This position is the Value 0 - 100 percent
    {
        get { return _markerPosition; }
        set
        {
            _markerPosition = value;
            if (_markerPosition < 0)
                _markerPosition = 0;
            if (_markerPosition > 100)
                _markerPosition = 100;

            float markerPercentToPixels = BarWidth / 100.0f;

            _markerTexturePosition = new Vector2((float)Math.Round(markerPercentToPixels * _markerPosition) - (MarkerWidth / 2) + SliderPosition.X, (BarHeight / 2) - (MarkerHeight / 2) + SliderPosition.Y);
            MarkerRectangle = new Rectangle((int)Math.Floor(_markerTexturePosition.X), (int)Math.Floor(_markerTexturePosition.Y), MarkerWidth, MarkerHeight);
            _currentValue = MinValue + ((MaxValue - MinValue) * (_markerPosition / 100.0f));
        }
    }
    public Vector2 MarkerTexturePosition
    {
        get { return _markerTexturePosition; }
        set
        {
            _markerTexturePosition = value;
            //Calculate the MarkerPosition based on the new X value
            float markerX = (_markerTexturePosition.X - SliderPosition.X);
            float percent = (float)Math.Round(markerX / BarWidth, 2);
            
            MarkerPosition = (int)(percent * 100);
            BuildBarTexture();
        }
    }
    public Rectangle MarkerRectangle { get; set; }
    public Texture2D BarTexture { get; set; }
    public Texture2D MarkerTexture { get; set; }
    public SpriteFont Font { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float CurrentValue
    {
        get
        {
            return _currentValue;
        }
        set
        {
            _currentValue = value;
            float adjustedPosition = _currentValue - MinValue;
            float range = MaxValue - MinValue;
            MarkerPosition = (int)Math.Round((adjustedPosition / range) * 100.0, 0);
        }
    }
    public bool ShowPercent { get; set; }
    public bool FillSlider { get; set; }

    public Slider()
    {
        SliderActive = false;
    }

    public void Initialize(GraphicsDevice device)
    {
        _device = device;
        BuildBarTexture();
        BuildMarkerTexture();
    }
    private void BuildBarTexture()
    {
        Texture2D texture = new Texture2D(_device, BarWidth, BarHeight);
        Color[] colors = new Color[BarWidth * BarHeight];
        Color borderColor = Color.Black;
        Color insideColor = Color.White;
        Color fillColor = Color.Red;
        int borderWidth = (int)Math.Ceiling(BarHeight * 0.1); //Make Border width 10% of the slider height
        List<Color> colorList = new List<Color>();

        for (int y = 0; y < BarHeight; y++)
        {
            //Adding Layers
            for (int x = 0; x < BarWidth; x++)
            {
                if (y >= borderWidth && y < BarHeight - borderWidth && x >= borderWidth && x < BarWidth - borderWidth)
                {
                    if (FillSlider)
                    {
                        if (x < BarWidth * (MarkerPosition / 100.0))
                        {
                            colorList.Add(fillColor);
                        }
                        else
                        {
                            colorList.Add(insideColor);
                        }
                    }
                    else
                    {
                        colorList.Add(insideColor);
                    }
                }
                else
                {
                    colorList.Add(borderColor);
                }
            }
        }
        colors = colorList.ToArray();

        texture.SetData(colors);
        BarTexture = texture;
    }
    private void BuildMarkerTexture()
    {
        Texture2D texture;
        Color markerColor = Color.Black;

        texture = new Texture2D(_device, MarkerWidth, MarkerHeight);
        Color[] colors = new Color[MarkerWidth * MarkerHeight];

        List<Color> colorList = new List<Color>();
        for (int y = 0; y < MarkerHeight; y++)
        {
            //Adding layers
            for (int x = 0; x < MarkerWidth; x++)
            {
                colorList.Add(markerColor);
            }
        }
        colors = colorList.ToArray();

        texture.SetData(colors);
        MarkerTexture = texture;
    }
    public void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(BarTexture, SliderPosition, Color.White);
        _spriteBatch.Draw(MarkerTexture, _markerTexturePosition, Color.White);
        if (ShowPercent)
        {
            _spriteBatch.DrawString(Font, CurrentValue.ToString(), new Vector2(SliderPosition.X + BarWidth + 10, SliderPosition.Y - 5), Color.Black);
        }
    }
}