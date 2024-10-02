using ProductionLineSimulation.Communication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Serialization
{
    public class LinePacketConverter : JsonConverter<LinePacket>
    {
        public override LinePacket Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                string typeName = root.GetProperty("Type").GetString();
                Type type = Type.GetType(typeName);

                /*
                if (type == null || !typeof(LinePacket).IsAssignableFrom(type))
                {
                    throw new JsonException($"Unknown type: {typeName}");
                }
                 */

                string json = root.GetRawText();
                return (LinePacket)JsonSerializer.Deserialize(json, type, options);
            }
        }
        public override void Write(Utf8JsonWriter writer, LinePacket value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.GetType().AssemblyQualifiedName);

            // Write all properties of the object
            foreach (var property in value.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(value);
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propertyValue, propertyValue?.GetType() ?? typeof(object), options);
            }

            writer.WriteEndObject();
        }
    }
}
