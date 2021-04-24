using System.Runtime.Serialization;
using UnityEngine;

public sealed class Vector2IntSurrogate : ISerializationSurrogate
{
    private const string XId = "x";
    private const string YId = "y";
    
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2Int v2 = (Vector2Int) obj;
        info.AddValue(XId, v2.x);
        info.AddValue(YId, v2.y);
    }
    
    public System.Object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector2Int v2 = (Vector2Int) obj;
        v2.x = (int) info.GetValue(XId, typeof(int));
        v2.y = (int) info.GetValue(YId, typeof(int));
        obj = v2;
        return obj;
    }
}