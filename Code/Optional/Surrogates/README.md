# Serialization Surrogates

Maybe a little niche for this repository but these were the product of a lot of serialization frustration so thought I'd keep them here to save other people from the same fate.

Surrogates are used for handling the serialization and deserialization of types not natively supported by serializers such as the binary formatter. Surrgates get called when you serialize or deserialize and allow you to specify the data you store or retrieve from the serialized data - this was great for my attempts to serialize audio clips, I can set it to write the raw data, the number of channels and the frequency in order to reconstruct the same audio clip on deserialization.

If you want to write a surrogate then it'll implement GetObjectData and SetObjectData, the former gets called when you serialize an object and the latter on deserialization. In GetObjectData, the main goal is to add the data required to reconstruct the object to the SerializationInfo object. E.g. for the Vector3 this is just the X, Y and Z components. Then in SetObjectData you'll want to retrieve that same data from the SerializationInfo to reconstruct the object and return it.