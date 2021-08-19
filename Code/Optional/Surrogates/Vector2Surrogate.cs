using System.Runtime.Serialization;
using UnityEngine;

namespace UnityExtras.Code.Optional.Surrogates
{
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
}