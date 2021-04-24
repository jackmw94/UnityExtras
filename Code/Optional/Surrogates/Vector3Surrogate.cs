using System.Runtime.Serialization;
using UnityEngine;

public sealed class Vector3Surrogate : ISerializationSurrogate
{
    private const string XId = "x";
    private const string YId = "y";
    private const string ZId = "z";
    
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3) obj;
        info.AddValue(XId, v3.x);
        info.AddValue(YId, v3.y);
        info.AddValue(ZId, v3.z);
    }

    public System.Object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3) obj;
        v3.x = (float) info.GetValue(XId, typeof(float));
        v3.y = (float) info.GetValue(YId, typeof(float));
        v3.z = (float) info.GetValue(ZId, typeof(float));
        obj = v3;
        return obj;
    }
}