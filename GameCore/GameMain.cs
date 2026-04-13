using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameCore;
using Microsoft.Xna.Framework;
public class GameMain : Game
{
    public GameMain()
    {
        GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

        // Typically you would load a config here...
        gdm.PreferredBackBufferWidth = 512;
        gdm.PreferredBackBufferHeight = 512;
        gdm.IsFullScreen = false;
        gdm.SynchronizeWithVerticalRetrace = true;

        // All content loaded will be in a "Content" folder
        Content.RootDirectory = "Content";
    }

    byte r = 0;
    byte g = 0;
    byte b = 0;
    DateTime lastUpdate = DateTime.UnixEpoch;
    int updateCount = 0;
    private SpriteBatch batch;
    
    private Texture2D texture;
    private SoundEffect sound;
    private KeyboardState keyboardPrev = new KeyboardState();

    protected override void Initialize()
    {
        /* This is a nice place to start up the engine, after
         * loading configuration stuff in the constructor
         */
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Load textures, sounds, and so on in here...
        // Create the batch...
        batch = new SpriteBatch(GraphicsDevice);
        texture = Content.Load<Texture2D>("images/popsicle");
        sound = Content.Load<SoundEffect>("sounds/sfx_jump");
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        // Clean up after yourself!
        batch.Dispose();
        texture.Dispose();
        sound.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Run game logic in here. Do NOT render anything here!
        base.Update(gameTime);
        updateCount++;
        DateTime now = DateTime.UtcNow;
        if ((now - lastUpdate).TotalSeconds > 1.0)
        {
            Console.WriteLine($"Main loop still running at: {now}; {Math.Round(updateCount / (now - lastUpdate).TotalSeconds, MidpointRounding.AwayFromZero)} UPS");
            lastUpdate = now;
            updateCount = 0;
        }
        KeyboardState keyboardCur = Keyboard.GetState();

        if (keyboardCur.IsKeyDown(Keys.Space) && keyboardPrev.IsKeyUp(Keys.Space))
        {
            sound.Play();
        }

        keyboardPrev = keyboardCur;

        // loop colors
        r++;
        if (r == 255) { r = 0;}
        g++;
        if (g == 255) { g = 0;}
        b++;
        if (b == 255) { b = 0;}
        
    }

    protected override void Draw(GameTime gameTime)
    {
        // Render stuff in here. Do NOT run game logic in here!
        GraphicsDevice.Clear(new Color(r, g, b));
         // Draw the texture to the corner of the screen
        batch.Begin();
        batch.Draw(texture, Vector2.Zero, Color.White);
        batch.End();
        base.Draw(gameTime);
    }
}