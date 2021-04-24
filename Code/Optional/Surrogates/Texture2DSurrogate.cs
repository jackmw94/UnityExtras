using System.Runtime.Serialization;
using UnityEngine;

public sealed class Texture2DSurrogate : ISerializationSurrogate
{
    private const string TextureNameId = "textureName";
    private const string TextureFormatId = "textureFormat";
    private const string AnsioLevelId = "ansioLevel";
    private const string TextureSizeId = "textureSize";
    private const string TextureDataId = "textureData";

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Texture2D texture = (Texture2D) obj;
        AddTextureData(texture, ref info);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        return GetTextureData(info);
    }

    public static void AddTextureData(Texture2D texture, ref SerializationInfo info)
    {
        info.AddValue(TextureNameId, texture.name);
        info.AddValue(TextureFormatId, texture.format);
        info.AddValue(AnsioLevelId, texture.anisoLevel);
        info.AddValue(TextureSizeId,  new Vector2Int(texture.width, texture.height));
        info.AddValue(TextureDataId, texture.GetRawTextureData());
    }

    public static Texture2D GetTextureData(SerializationInfo info)
    {
        byte[] textureData = (byte[]) info.GetValue(TextureDataId, typeof(byte[]));

        string textureName = (string) info.GetValue(TextureNameId, typeof(string));
        TextureFormat textureFormat = (TextureFormat) info.GetValue(TextureFormatId, typeof(TextureFormat));
        int ansioLevel = (int) info.GetValue(AnsioLevelId, typeof(int));
        Vector2Int textureSize = (Vector2Int) info.GetValue(TextureSizeId, typeof(Vector2Int));

        Texture2D texture = new Texture2D(textureSize.x, textureSize.y, textureFormat, false)
        {
            name = textureName
        };

        texture.LoadRawTextureData(textureData);
        texture.anisoLevel = ansioLevel;
        texture.Apply();

        return texture;
    }
}