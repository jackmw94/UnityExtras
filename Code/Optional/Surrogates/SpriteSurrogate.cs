using System.Runtime.Serialization;
using UnityEngine;

public sealed class SpriteSurrogate : ISerializationSurrogate
{
    private const string SpriteNameId = "spriteName";
    
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Sprite sprite = (Sprite) obj;
        info.AddValue(SpriteNameId, sprite.name);
        Texture2DSurrogate.AddTextureData(sprite.texture, ref info);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Texture2D textureData = Texture2DSurrogate.GetTextureData(info);
        Sprite sprite = Sprite.Create (textureData, new Rect(0,0,textureData.width, textureData.height), new Vector2(.5f, .5f));
        sprite.name = (string)info.GetValue(SpriteNameId, typeof(string));
        return sprite;
    }
}