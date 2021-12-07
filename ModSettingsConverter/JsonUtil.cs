using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ModSettingsConverter
{
    class JsonUtil
    {
        private enum PropertyTreeType : byte
        {
            None = 0,
            Bool = 1,
            Number = 2,
            String = 3,
            List = 4,
            Dictionary = 5
        }

        private static void ReadPropertyTree(Deserialiser input, JsonWriter jsonWriter)
        {
            // Type
            PropertyTreeType type = (PropertyTreeType)input.LoadByte();
            // Any-type flag
            input.LoadBool();

            // Read the value
            switch (type)
            {
                case PropertyTreeType.None: break;
                case PropertyTreeType.Bool:
                    jsonWriter.WriteValue(input.LoadBool());
                    break;
                case PropertyTreeType.Number:
                    jsonWriter.WriteValue(input.LoadDouble());
                    break;
                case PropertyTreeType.String:
                    jsonWriter.WriteValue(input.LoadString());
                    break;
                case PropertyTreeType.List:
                case PropertyTreeType.Dictionary:
                    // Count
                    uint count = input.LoadUInt();
                    jsonWriter.WriteStartObject();

                    for (uint i = 0; i < count; ++i)
                    {
                        // Key
                        jsonWriter.WritePropertyName(input.LoadString());

                        // Value
                        ReadPropertyTree(input, jsonWriter);
                    }

                    jsonWriter.WriteEndObject();
                    break;
                default:
                    throw new Exception("Unknown type: " + type);
            }
        }

        private static PropertyTreeType ParseTokenType(JTokenType type)
        {
            switch (type)
            {
                case JTokenType.Object: return PropertyTreeType.Dictionary;
                case JTokenType.Array: return PropertyTreeType.List;
                case JTokenType.Integer: return PropertyTreeType.Number;
                case JTokenType.Float: return PropertyTreeType.Number;
                case JTokenType.String: return PropertyTreeType.String;
                case JTokenType.Boolean: return PropertyTreeType.Bool;
                default:
                    throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(JTokenType));
            }
        }

        private static void WritePropertyTree(Serialiser output, JToken token)
        {
            PropertyTreeType type = ParseTokenType(token.Type);

            // Type
            output.Write((byte)type);
            // Any-type flag
            output.Write(false);

            // Write the value
            switch (type)
            {
                case PropertyTreeType.None: break;
                case PropertyTreeType.Bool:
                    output.Write(token.Value<bool>());
                    break;
                case PropertyTreeType.Number:
                    output.Write(token.Value<double>());
                    break;
                case PropertyTreeType.String:
                    output.Write(token.Value<string>());
                    break;
                case PropertyTreeType.List:
                case PropertyTreeType.Dictionary:
                    // Count
                    output.Write((uint)token.Count());

                    foreach (var pair in token.Value<IDictionary<string, JToken>>())
                    {
                        // Key
                        output.Write(pair.Key);

                        // Value
                        WritePropertyTree(output, pair.Value);
                    }
                    break;
                default:
                    throw new Exception("Unknown type: " + type);
            }
        }

        public static string DataToJsonString(byte[] data)
        {
            ListReadStream stream = new ListReadStream(data.ToList());
            Deserialiser input = new Deserialiser(stream);

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            JsonWriter writer = new JsonTextWriter(stringWriter);

            ReadPropertyTree(input, writer);

            return stringWriter.ToString();
        }

        public static byte[] JsonStringToData(string json)
        {
            ListWriteStream stream = new ListWriteStream();
            Serialiser output = new Serialiser(stream, MapVersion.Latest);

            JObject result = JObject.Parse(json);

            WritePropertyTree(output, result);

            return stream.buffer.ToArray();
        }
    }
}
