using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRGEnt.Generated;

namespace ECS_Mono_Example;

public class RendererSystem : SpriteBatchExecuteSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _texture;

    public RendererSystem(SpriteBatchDomain domain, GraphicsDevice graphicsDevice, bool shouldSort = false) : base(domain, shouldSort)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _texture = new Texture2D(graphicsDevice, 1, 1);
        Color[] data = { Color.White };
        _texture.SetData(data);
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
        _spriteBatch.Begin();
        foreach (var entity in entities)
        {
            var angle = MathHelper.ToRadians(entity.Angle);
            var position = new Point(400 + (int)(MathF.Sin(angle) * 200), 300 +(int)(MathF.Cos(angle) * 200));
            _spriteBatch.Draw(_texture,new Rectangle(position,entity.Size),entity.Color);
        }
        _spriteBatch.End();
    }
}