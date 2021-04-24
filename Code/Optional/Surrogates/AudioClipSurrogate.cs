using System;
using System.Runtime.Serialization;
using UnityEngine;

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