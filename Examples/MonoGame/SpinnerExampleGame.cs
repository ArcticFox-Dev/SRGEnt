using ECS_Mono_Example;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SRGEnt.Generated;

namespace MonoExample;

public class SpinnerExampleGame : Game
{
    private readonly SpriteBatchDomain _domain;
    private readonly int _spritesCount;
    private RotatorSystem _rotator;
    private RendererSystem _renderer;

    public SpinnerExampleGame(int spritesCount)
    {
        _spritesCount = spritesCount;
        
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 800;
        graphics.PreferredBackBufferHeight = 600;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _domain = new SpriteBatchDomain(_spritesCount + 1);
        _domain.CircleRadius = 200; // Radius of the entity circle in pixels.
    }

    protected override void Initialize()
    {
        const int spriteSize = 15;
        for(var i = 0; i < _spritesCount; i++)
        {
            var spriteEntity = _domain.CreateEntity();
            var color = new Color(255 * i / _spritesCount, 255 * i / _spritesCount, 255 * i / _spritesCount);
            spriteEntity.Color = color;
            spriteEntity.Angle = 360.0f / _spritesCount * i;
            spriteEntity.Size = new Point(spriteSize, spriteSize);
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _rotator = new RotatorSystem(_domain, .33f);
        _renderer = new RendererSystem(_domain, GraphicsDevice);
    }

    protected override void UnloadContent()
    {
        _renderer.DisposeTexture();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _domain.ElapsedTime = gameTime.ElapsedGameTime.Milliseconds;
        _rotator.Execute();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _renderer.Execute();
        
        base.Draw(gameTime);
    }
}
