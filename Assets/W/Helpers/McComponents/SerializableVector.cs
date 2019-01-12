using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

[Serializable]
public struct SerializableVector4
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableVector4(float rX, float rY, float rZ, float rW)
    {
        x = rX;
        y = rY;
        z = rZ;
        w = rW;
    }

    public static implicit operator Vector4(SerializableVector4 rValue)
    {
        return new Vector4(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    public static implicit operator SerializableVector4(Vector4 rValue)
    {
        return new SerializableVector4(rValue.x, rValue.y, rValue.z, rValue.w);
    }
}


[Serializable]
public struct SerializableVector3
{
	public float x;
	public float y;
	public float z;

	public SerializableVector3(float rX, float rY, float rZ)
	{
		x = rX;
		y = rY;
		z = rZ;
	}

	public static implicit operator Vector3(SerializableVector3 rValue)
	{
		return new Vector3(rValue.x, rValue.y, rValue.z);
	}

	public static implicit operator SerializableVector3(Vector3 rValue)
	{
		return new SerializableVector3(rValue.x, rValue.y, rValue.z);
	}
}


sealed class Vector4SerializationSurrogate : ISerializationSurrogate
{

    // Method called to serialize a Vector4 object
    public void GetObjectData(System.Object obj,
                              SerializationInfo info, StreamingContext context)
    {

        Vector4 v4 = (Vector4)obj;
        info.AddValue("x", v4.x);
        info.AddValue("y", v4.y);
        info.AddValue("z", v4.z);
        info.AddValue("w", v4.w);
    }

    // Method called to deserialize a Vector4 object
    public System.Object SetObjectData(System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector)
    {

        Vector4 v4 = (Vector4)obj;
        v4.x = (float)info.GetValue("x", typeof(float));
        v4.y = (float)info.GetValue("y", typeof(float));
        v4.z = (float)info.GetValue("z", typeof(float));
        v4.w = (float)info.GetValue("w", typeof(float));
        obj = v4;
        return obj;   // Formatters ignore this return value //Seems to have been fixed!
    }
}


sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{

	// Method called to serialize a Vector3 object
	public void GetObjectData(System.Object obj,
							  SerializationInfo info, StreamingContext context)
	{

		Vector3 v3 = (Vector3)obj;
		info.AddValue("x", v3.x);
		info.AddValue("y", v3.y);
		info.AddValue("z", v3.z);
	}

	// Method called to deserialize a Vector3 object
	public System.Object SetObjectData(System.Object obj,
									   SerializationInfo info, StreamingContext context,
									   ISurrogateSelector selector)
	{

		Vector3 v3 = (Vector3)obj;
		v3.x = (float)info.GetValue("x", typeof(float));
		v3.y = (float)info.GetValue("y", typeof(float));
		v3.z = (float)info.GetValue("z", typeof(float));
		obj = v3;
		return obj;   // Formatters ignore this return value //Seems to have been fixed!
	}
}