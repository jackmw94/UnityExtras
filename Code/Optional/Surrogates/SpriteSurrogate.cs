using System.Runtime.Serialization;
using UnityEngine;

namespace UnityExtras.Code.Optional.Surrogates
{
    public sealed class SpriteSurrogate : ISerializationSurrogate
    {
        private const string SpriteNameId = "spriteName";
    
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Sprite sprite = (Sprite) obj;

            if (!sprite)
            {
                Texture2DSurrogate.AddTextureData(null, ref info);
                return;
            }

            Texture2DSurrogate.AddTextureData(sprite.texture, ref info);
            info.AddValue(SpriteNameId, sprite.name);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Texture2D textureData = Texture2DSurrogate.GetTextureData(info);

            if (!textureData)
            {
                return null;
            }
        
            Sprite sprite = Sprite.Create (textureData, new Rect(0,0,textureData.width, textureData.height), new Vector2(.5f, .5f));
            sprite.name = (string)info.GetValue(SpriteNameId, typeof(string));
            return sprite;
        }
    }
}