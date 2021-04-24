using System;
using System.Runtime.Serialization;
using UnityEngine;

public sealed class Vector2Surrogate : ISerializationSurrogate
{
    private const string XId = "x";
    private const string YId = "y";
    
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2 v2 = (Vector2) obj;
        info.AddValue(XId, v2.x);
        info.AddValue(YId, v2.y);
    }
    
    public System.Object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector2 v2 = (Vector2) obj;
        v2.x = (float) info.GetValue(XId, typeof(float));
        v2.y = (float) info.GetValue(YId, typeof(float));
        obj = v2;
        return obj;
    }
}

public sealed class Vector2IntSurrogate : ISerializationSurrogate
{
    private const string XId = "x";
    private const string YId = "y";
    
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2Int v2 = (Vector2Int) obj;
        info.AddValue(XId, v2.x);
        info.AddValue(YId, v2.y);
    }
    
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector2Int v2 = (Vector2Int) obj;
        v2.x = (int) info.GetValue(XId, typeof(int));
        v2.y = (int) info.GetValue(YId, typeof(int));
        obj = v2;
        return obj;
    }
}

public sealed class Vector3Surrogate : ISerializationSurrogate
{
    private const string XId = "x";
    private const string YId = "y";
    private const string ZId = "z";
    
    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj,
        SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3) obj;
        info.AddValue(XId, v3.x);
        info.AddValue(YId, v3.y);
        info.AddValue(ZId, v3.z);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj,
        SerializationInfo info, StreamingContext context,
        ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3) obj;
        v3.x = (float) info.GetValue(XId, typeof(float));
        v3.y = (float) info.GetValue(YId, typeof(float));
        v3.z = (float) info.GetValue(ZId, typeof(float));
        obj = v3;
        return obj;
    }
}

public sealed class ColorSurrogate : ISerializationSurrogate
{
    private const string RId = "r";
    private const string GId = "g";
    private const string BId = "b";
    private const string AId = "a";
    
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Color color = (Color) obj;
        info.AddValue(RId, color.r);
        info.AddValue(GId, color.g);
        info.AddValue(BId, color.b);
        info.AddValue(AId, color.a);
    }
    
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Color color = (Color) obj;
        color.r = (float) info.GetValue(RId, typeof(float));
        color.g = (float) info.GetValue(GId, typeof(float));
        color.b = (float) info.GetValue(BId, typeof(float));
        color.a = (float) info.GetValue(AId, typeof(float));
        obj = color;
        return obj;
    }
}

public sealed class AudioClipSurrogate : ISerializationSurrogate
{
    private const string ClipNameId = "clipName";
    private const string ClipIsNullId = "clipIsNull";
    private const string SampleCountId = "sampleCount";
    private const string NumChannelsId = "numChannels";
    private const string FrequencyId = "frequency";
    private const string SampleDataId = "sampleData";

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        AudioClip audioClip = (AudioClip) obj;
        
        bool clipIsNull = !audioClip;
        info.AddValue(ClipIsNullId, clipIsNull);
        
        if (clipIsNull)
        {
            return;
        }
        
        float[] audioSamples = new float[audioClip.samples];
        audioClip.GetData(audioSamples, 0);
        
        byte[] audioSampleData = new byte[audioClip.samples * 4];
        Buffer.BlockCopy(audioSamples, 0, audioSampleData, 0, audioSampleData.Length);

        info.AddValue(ClipNameId, audioClip.name);
        info.AddValue(SampleCountId, audioClip.samples);
        info.AddValue(NumChannelsId, audioClip.channels);
        info.AddValue(FrequencyId, audioClip.frequency);
        info.AddValue(SampleDataId, audioSampleData);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        AudioClip audioClip = (AudioClip) obj;

        bool clipIsNull = (bool) info.GetValue(ClipIsNullId, typeof(bool));
        if (clipIsNull)
        {
            return null;
        }

        byte[] sampleData = (byte[]) info.GetValue(SampleDataId, typeof(byte[]));
        string clipName = (string) info.GetValue(ClipNameId, typeof(string));
        int sampleCount = (int) info.GetValue(SampleCountId, typeof(int));
        int numChannels = (int) info.GetValue(NumChannelsId, typeof(int));
        int audioFrequency = (int) info.GetValue(FrequencyId, typeof(int));
        
        if (sampleCount > 0)
        {
            audioClip = AudioClip.Create(clipName, sampleCount, numChannels, audioFrequency, false);
            float[] audioSamples = new float[sampleCount];
            Buffer.BlockCopy(sampleData, 0, audioSamples, 0, sampleData.Length);
            audioClip.SetData(audioSamples, 0);
        }

        obj = audioClip;
        return obj;
    }
}

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