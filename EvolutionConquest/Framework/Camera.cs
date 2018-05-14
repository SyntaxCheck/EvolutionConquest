using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Camera
{
    // Construct a new Camera class with standard zoom (no scaling)
    public Camera()
    {
        Zoom = 0.5f;
    }

    // Centered Position of the Camera in pixels.
    public Vector2 Position { get; private set; }
    // Current Zoom level with 1.0f being standard
    public float Zoom { get; private set; }
    private float ZoomMin = 0.25f;
    private float ZoomMax = 10.0f;
    // Current Rotation amount with 0.0f being standard orientation
    public float Rotation { get; private set; }

    // Height and width of the viewport window which we need to adjust
    // any time the player resizes the game window.
    public int ViewportWidth { get; set; }
    public int ViewportHeight { get; set; }

    // Center of the Viewport which does not account for scale
    public Vector2 ViewportCenter
    {
        get
        {
            return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
        }
    }

    // Create a matrix for the camera to offset everything we draw,
    // the map and our objects. since the camera coordinates are where
    // the camera is, we offset everything by the negative of that to simulate
    // a camera moving. We also cast to integers to avoid filtering artifacts.
    public Matrix TranslationMatrix
    {
        get
        {
            return Matrix.CreateTranslation(-(int)Position.X,
               -(int)Position.Y, 0) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
               Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
        }
    }

    // Call this method with negative values to zoom out
    // or positive values to zoom in. It looks at the current zoom
    // and adjusts it by the specified amount. If we were at a 1.0f
    // zoom level and specified -0.5f amount it would leave us with
    // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
    public void AdjustZoom(float amount)
    {
        float zoomAmount = 0f;
        if (amount > 0)
        {
            zoomAmount = (ZoomMax - Zoom) / 4;
        }
        else
        {
            zoomAmount = (Zoom - ZoomMin) / -4;
        }
        if (zoomAmount > 0.1f)
            zoomAmount = 0.1f;
        if (zoomAmount < -0.1f)
            zoomAmount = -0.1f;

        Zoom += zoomAmount;
        if (Zoom < ZoomMin)
        {
            Zoom = ZoomMin;
        }
        if (Zoom > ZoomMax)
        {
            Zoom = ZoomMax;
        }
    }

    // Move the camera in an X and Y amount based on the cameraMovement param.
    // if clampToMap is true the camera will try not to pan outside of the
    // bounds of the map.
    public void MoveCamera(int worldSize, Vector2 cameraMovement, bool clampToMap = false)
    {
        Vector2 newPosition = Position + cameraMovement;

        if (clampToMap)
        {
            Position = MapClampedPosition(worldSize, newPosition);
        }
        else
        {
            Position = newPosition;
        }
    }

    public Rectangle ViewportWorldBoundry()
    {
        Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
        Vector2 viewPortBottomCorner =
           ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

        return new Rectangle((int)viewPortCorner.X,
           (int)viewPortCorner.Y,
           (int)(viewPortBottomCorner.X - viewPortCorner.X),
           (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
    }

    // Center the camera on specific pixel coordinates
    public void CenterOn(Vector2 position)
    {
        Position = position;
    }

    // Center the camera on a specific cell in the map
    //public void CenterOn(Cell cell)
    //{
    //    Position = CenteredPosition(cell, true);
    //}

    //private Vector2 CenteredPosition(Cell cell, bool clampToMap = false)
    //{
    //    var cameraPosition = new Vector2(cell.X * Global.SpriteWidth,
    //       cell.Y * Global.SpriteHeight);
    //    var cameraCenteredOnTilePosition =
    //       new Vector2(cameraPosition.X + Global.SpriteWidth / 2,
    //           cameraPosition.Y + Global.SpriteHeight / 2);
    //    if (clampToMap)
    //    {
    //        return MapClampedPosition(cameraCenteredOnTilePosition);
    //    }

    //    return cameraCenteredOnTilePosition;
    //}

    // Clamp the camera so it never leaves the visible area of the map.
    private Vector2 MapClampedPosition(int worldSize, Vector2 position)
    {
        int clampOverrage = 1000;

        var cameraMax = new Vector2(
            worldSize - (ViewportWidth / Zoom / 2) + clampOverrage,
            worldSize - (ViewportHeight / Zoom / 2) + clampOverrage
        );

        Vector2 clamped = new Vector2();
        clamped = Vector2.Clamp(position, new Vector2((ViewportWidth / Zoom / 2) - clampOverrage, (ViewportHeight / Zoom / 2) - clampOverrage), cameraMax);
        return clamped;
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, TranslationMatrix);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition,
            Matrix.Invert(TranslationMatrix));
    }

    // Move the camera's position based on input
    public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer, ref GameData gameData)
    {
        Vector2 cameraMovement = Vector2.Zero;
        float cameraMovementAmount = 0.1f / Zoom;
        PlayerIndex playerIndex;

        if (inputState.IsScrollLeft(controllingPlayer))
        {
            cameraMovement.X = cameraMovementAmount  * - 1;
        }
        else if (inputState.IsScrollRight(controllingPlayer))
        {
            cameraMovement.X = cameraMovementAmount;
        }
        if (inputState.IsScrollUp(controllingPlayer))
        {
            cameraMovement.Y = cameraMovementAmount * -1;
        }
        else if (inputState.IsScrollDown(controllingPlayer))
        {
            cameraMovement.Y = cameraMovementAmount;
        }
        if (inputState.IsZoomIn(controllingPlayer))
        {
            AdjustZoom(0.25f);
        }
        else if (inputState.IsZoomOut(controllingPlayer))
        {
            AdjustZoom(-0.25f);
        }
        if (inputState.IsNewKeyPress(Keys.PageDown, controllingPlayer, out playerIndex))
        {
            if (gameData.FocusIndex < 0)
            {
                gameData.FocusIndex = 0;
            }

            gameData.SetIndexPositionsForCreatures(); //Need to call this to fix the FocusIndex value. Since we are removing creatures from the list the Index gets stale over time
            if (inputState.IsKeyPressed(Keys.LeftShift, controllingPlayer, out playerIndex))
            {
                if (gameData.FocusIndex > gameData.Creatures.Count - 1)
                {
                    gameData.FocusIndex = gameData.Creatures.Count - 1;
                }
                else
                {
                    int speciesId = gameData.Creatures[gameData.FocusIndex].SpeciesId;

                    if (speciesId > 0)
                    {
                        speciesId--;
                    }
                    else
                    {
                        speciesId = gameData.Creatures.Max(t => t.SpeciesId);
                    }

                    bool idExists = false;
                    while (!idExists)
                    {
                        if (gameData.Creatures.Where(t => t.SpeciesId == speciesId).Count() <= 0)
                        {
                            if (speciesId > 0)
                            {
                                speciesId--;
                            }
                            else
                            {
                                speciesId = gameData.Creatures.Max(t => t.SpeciesId);
                            }
                        }
                        else
                        {
                            idExists = true;
                        }
                    }

                    for (int i = 0; i < gameData.Creatures.Count; i++)
                    {
                        if (gameData.Creatures[i].SpeciesId == speciesId)
                        {
                            gameData.FocusIndex = i;
                            gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                        }
                    }
                }
            }
            else if (inputState.IsKeyPressed(Keys.LeftControl, controllingPlayer, out playerIndex))
            {
                if (gameData.FocusIndex >= 0 && gameData.Creatures[gameData.FocusIndex].IsAlive)
                {
                    int speciesId = gameData.Creatures[gameData.FocusIndex].SpeciesId;
                    int searchIndex = gameData.FocusIndex;

                    bool found = false;
                    while (!found)
                    {
                        if (searchIndex > 0)
                            searchIndex--;
                        else
                            searchIndex = gameData.Creatures.Count - 1;

                        if (gameData.Creatures[searchIndex].SpeciesId == speciesId)
                        {
                            found = true;
                            gameData.FocusIndex = searchIndex;
                            gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                        }
                    }
                }
            }
            else
            {
                if (gameData.FocusIndex > gameData.Creatures.Count - 1)
                {
                    gameData.FocusIndex = gameData.Creatures.Count - 1;
                }

                if (gameData.Focus != null && gameData.FocusIndex >= 0)
                {
                    if (gameData.FocusIndex == 0)
                    {
                        gameData.FocusIndex = gameData.Creatures.Count - 1;
                    }
                    else
                    {
                        gameData.FocusIndex--;
                    }
                    gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                }
                else
                {
                    gameData.FocusIndex = 0;
                    gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                }
            }
        }
        else if (inputState.IsNewKeyPress(Keys.PageUp, controllingPlayer, out playerIndex))
        {
            if (gameData.FocusIndex < 0)
            {
                gameData.FocusIndex = gameData.Creatures.Count - 1;
            }

            gameData.SetIndexPositionsForCreatures(); //Need to call this to fix the FocusIndex value. Since we are removing creatures from the list the Index gets stale over time
            if (inputState.IsKeyPressed(Keys.LeftShift, controllingPlayer, out playerIndex))
            {
                if (gameData.FocusIndex > gameData.Creatures.Count - 1)
                {
                    gameData.FocusIndex = gameData.Creatures.Count - 1;
                }
                else
                {
                    int speciesId = gameData.Creatures[gameData.FocusIndex].SpeciesId;
                    int speciesIdMax = gameData.Creatures.Max(t => t.SpeciesId);
                    if (speciesId < speciesIdMax)
                    {
                        speciesId++;
                    }
                    else
                    {
                        speciesId = 0;
                    }

                    bool idExists = false;
                    while (!idExists)
                    {
                        if (gameData.Creatures.Where(t => t.SpeciesId == speciesId).Count() <= 0)
                        {
                            if (speciesId < speciesIdMax)
                            {
                                speciesId++;
                            }
                            else
                            {
                                speciesId = 0;
                            }
                        }
                        else
                        {
                            idExists = true;
                        }
                    }

                    for (int i = 0; i < gameData.Creatures.Count; i++)
                    {
                        if (gameData.Creatures[i].SpeciesId == speciesId)
                        {
                            gameData.FocusIndex = i;
                            gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                        }
                    }
                }
            }
            else if (inputState.IsKeyPressed(Keys.LeftControl, controllingPlayer, out playerIndex))
            {
                int speciesId = gameData.Creatures[gameData.FocusIndex].SpeciesId;
                int searchIndex = gameData.FocusIndex;

                bool found = false;
                while (!found)
                {
                    if (searchIndex < gameData.Creatures.Count - 1)
                        searchIndex++;
                    else
                        searchIndex = 0;

                    if (gameData.Creatures[searchIndex].SpeciesId == speciesId)
                    {
                        found = true;
                        gameData.FocusIndex = searchIndex;
                        gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                    }
                }
            }
            else
            {
                if (gameData.FocusIndex > gameData.Creatures.Count - 1)
                {
                    gameData.FocusIndex = gameData.Creatures.Count - 1;
                }

                if (gameData.Focus != null && gameData.FocusIndex >= 0)
                {
                    if (gameData.FocusIndex == gameData.Creatures.Count - 1)
                    {
                        gameData.FocusIndex = 0;
                    }
                    else
                    {
                        gameData.FocusIndex++;
                    }
                    gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                }
                else
                {
                    gameData.FocusIndex = 0;
                    gameData.Focus = gameData.Creatures[gameData.FocusIndex];
                }
            }
        }
        else if (inputState.IsNewKeyPress(Keys.H, controllingPlayer, out playerIndex)) //Focus on top Herbavore
        {
            List<Creature> tmpList = gameData.Creatures.Where(t => t.IsHerbavore && t.Herbavore > t.Carnivore && t.Herbavore > t.Scavenger).ToList();
            if (tmpList.Count > 0)
            {
                float highestHerbavore = tmpList.Max(t => t.Herbavore);
                List<Creature> creatures = gameData.Creatures.Where(t => t.IsHerbavore && t.Herbavore == highestHerbavore).ToList();

                if (creatures.Count > 0)
                {
                    gameData.Focus = creatures[creatures.Count - 1];
                    gameData.SetIndexPositionsForCreatures();
                }
            }
        }
        else if (inputState.IsNewKeyPress(Keys.C, controllingPlayer, out playerIndex)) //Focus on top Herbavore
        {
            List<Creature> tmpList = gameData.Creatures.Where(t => t.IsCarnivore && t.Carnivore > t.Herbavore && t.Carnivore > t.Scavenger).ToList();
            if (tmpList.Count > 0)
            {
                float highestCarnivore = tmpList.Max(t => t.Carnivore);
                List<Creature> creatures = gameData.Creatures.Where(t => t.IsCarnivore && t.Carnivore == highestCarnivore).ToList();

                if (creatures.Count > 0)
                {
                    gameData.Focus = creatures[creatures.Count - 1];
                    gameData.SetIndexPositionsForCreatures();
                }
            }
        }
        else if (inputState.IsNewKeyPress(Keys.V, controllingPlayer, out playerIndex)) //Focus on top Herbavore
        {
            List<Creature> tmpList = gameData.Creatures.Where(t => t.IsScavenger && t.Scavenger > t.Carnivore && t.Scavenger > t.Herbavore).ToList();
            if (tmpList.Count > 0)
            {
                float highestScavenger = tmpList.Max(t => t.Scavenger);
                List<Creature> creatures = gameData.Creatures.Where(t => t.IsScavenger && t.Scavenger == highestScavenger).ToList();

                if (creatures.Count > 0)
                {
                    gameData.Focus = creatures[creatures.Count - 1];
                    gameData.SetIndexPositionsForCreatures();
                }
            }
        }
        else if (inputState.IsNewKeyPress(Keys.O, controllingPlayer, out playerIndex)) //Focus on top Omnivore
        {
            List<Creature> tmpList = gameData.Creatures.Where(t => t.IsOmnivore && t.Omnivore > t.Carnivore && t.Omnivore > t.Herbavore && t.Omnivore > t.Scavenger).ToList();
            if (tmpList.Count > 0)
            {
                float highestOmnivore = tmpList.Max(t => t.Omnivore);
                List<Creature> creatures = gameData.Creatures.Where(t => t.IsOmnivore && t.Omnivore == highestOmnivore).ToList();

                if (creatures.Count > 0)
                {
                    gameData.Focus = creatures[creatures.Count - 1];
                    gameData.SetIndexPositionsForCreatures();
                }
            }
        }

        MouseState mouseState;
        if (inputState.IsNewLeftMouseClick(out mouseState))
        {
            Vector2 worldPosition = Vector2.Transform(new Vector2(mouseState.Position.X, mouseState.Position.Y), Matrix.Invert(Global.Camera.TranslationMatrix));

            bool found = false;

            if (!found)
            {
                for (int i = 0; i < gameData.Eggs.Count; i++)
                {
                    if (gameData.Eggs[i].Position.X - (gameData.Eggs[i].Texture.Width / 2) < worldPosition.X &&
                    gameData.Eggs[i].Position.X + (gameData.Eggs[i].Texture.Width / 2) > worldPosition.X &&
                    gameData.Eggs[i].Position.Y - (gameData.Eggs[i].Texture.Height / 2) < worldPosition.Y &&
                    gameData.Eggs[i].Position.Y + (gameData.Eggs[i].Texture.Height / 2) > worldPosition.Y)
                    {
                        //Set the gameData focus to follow
                        gameData.Focus = gameData.Eggs[i].Creature;
                        gameData.FocusIndex = 0;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                for (int i = 0; i < gameData.Creatures.Count; i++)
                {
                    //If the object is not an egg help with clicking by expanding the radius
                    if (gameData.Creatures[i].Position.X - (gameData.Creatures[i].Texture.Width / 1.5) < worldPosition.X &&
                        gameData.Creatures[i].Position.X + (gameData.Creatures[i].Texture.Width / 1.5) > worldPosition.X &&
                        gameData.Creatures[i].Position.Y - (gameData.Creatures[i].Texture.Height / 1.5) < worldPosition.Y &&
                        gameData.Creatures[i].Position.Y + (gameData.Creatures[i].Texture.Height / 1.5) > worldPosition.Y)
                    {
                        //Set the gameData focus to follow
                        gameData.Focus = gameData.Creatures[i];
                        gameData.FocusIndex = i;
                        found = true;
                        break;
                    }
                }
            }

            //If we have focus on a creature remove that focus if we click anywhere other than on a creature
            if (gameData.Focus != null && !found)
            {
                gameData.Focus = null;
            }
        }

        // When using a controller, to match the thumbstick behavior,
        // we need to normalize non-zero vectors in case the user
        // is pressing a diagonal direction.
        if (cameraMovement != Vector2.Zero)
        {
            cameraMovement.Normalize();
        }

        // scale our movement to move 25 pixels per second
        cameraMovement *= 25f;

        MoveCamera(gameData.Settings.WorldSize, cameraMovement, true);
    }
}