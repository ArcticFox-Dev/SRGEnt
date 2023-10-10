using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRGEnt.Generated;

namespace ECS_Mono_Example;

/// <summary>
/// This is a renderer system which is of the ExecuteSystem type.
/// SpriteBatchExecuteSystem class is auto generated and saves the
/// user the hassle of declaring generic types for the ExecuteSystem class. 
///
/// The system iterates over all entities that have the Angle, Size and Color components
/// and draws them to the screen as a single sprite batch.
/// </summary>
public class RendererSystem : SpriteBatchExecuteSystem
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _texture;

    public RendererSystem(SpriteBatchDomain domain, GraphicsDevice graphicsDevice, bool shouldSort = false) : base(domain, shouldSort)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _texture = new Texture2D(graphicsDevice, 1, 1);
        Color[] data = { Color.White };
        _texture.SetData(data);
    }

    public void DisposeTexture()
    {
        _texture.Dispose();
    }

    protected override void SetMatcher(ref SpriteBatchMatcher matcher)
    {
        matcher.Requires
            .Angle()
            .Size()
            .Color();
    }

    protected override void Execute(ReadOnlySpan<SpriteBatchEntity> entities)
    {
        var halfWidth = _graphicsDevice.Viewport.Width / 2;
        var halfHeight = _graphicsDevice.Viewport.Height / 2;
        var circleRadius = _domain.CircleRadius;
        _spriteBatch.Begin();
        foreach (var entity in entities)
        {
            var angle = MathHelper.ToRadians(entity.Angle);
            var position = new Point(halfWidth + (int)(MathF.Sin(angle) * circleRadius),
                halfHeight +(int)(MathF.Cos(angle) * circleRadius));
            _spriteBatch.Draw(_texture,new Rectangle(position,entity.Size),entity.Color);
        }
        _spriteBatch.End();
    }
}