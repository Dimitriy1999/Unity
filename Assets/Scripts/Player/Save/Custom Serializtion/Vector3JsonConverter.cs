using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Net;

public class Vector3JsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
       return objectType == typeof(Vector3);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new Vector3((float)JToken.Load(reader), (float)JToken.Load(reader), (float)JToken.Load(reader));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JToken.FromObject((float)value).WriteTo(writer);
    }
    }
