using System.Runtime.Serialization;
using UnityEngine;

namespace UnityExtras.Code.Optional.Surrogates
{
    public sealed class ColorSurrogate : ISerializationSurrogate
    {
        private const string RId = "r";
        private const string GId = "g";
        private const string BId = "b";
        private const string AId = "a";
    
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Color color = (Color) obj;
            info.AddValue(RId, color.r);
            info.AddValue(GId, color.g);
            info.AddValue(BId, color.b);
            info.AddValue(AId, color.a);
        }
    
        public System.Object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Color color = (Color) obj;
            color.r = (float) info.GetValue(RId, typeof(float));
            color.g = (float) info.GetValue(GId, typeof(float));
            color.b = (float) info.GetValue(BId, typeof(float));
            color.a = (float) info.GetValue(AId, typeof(float));
            return color;
        }
    }
}