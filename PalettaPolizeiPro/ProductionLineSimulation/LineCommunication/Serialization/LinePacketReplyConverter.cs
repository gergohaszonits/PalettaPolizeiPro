using ProductionLineSimulation.Communication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ProductionLineSimulation.LineCommunication.Data;

namespace ProductionLineSimulation.LineCommunication.Serialization
{
    public class LinePacketReplyConverter : JsonConverter<LinePacketReply>
    {
        public override LinePacketReply Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                string typeName = root.GetProperty("Type").GetString();
                /// ez egy sebtapasz 
                if (DEBUG)
                {
                    typeName = typeName.Replace("ProductionLineSimulation,", "PalettaPolizeiPro,");
                }
                ///
                Type type = Type.GetType(typeName);

               
                if (type == null || !typeof(LinePacketReply).IsAssignableFrom(type))
                {
                    throw new JsonException($"Unknown type: {typeName}");
                }

                string json = root.GetRawText();

                // Hozunk létre egy új JsonSerializerOptions példányt, amelyből eltávolítjuk az aktuális konvertert
                var optionsWithoutConverter = new JsonSerializerOptions(options);
                optionsWithoutConverter.Converters.Remove(this); // Eltávolítjuk a jelenlegi konvertert, hogy ne hívja újra önmagát

                // Deszerializálás az új opciókkal, amelyek már nem tartalmazzák az önhivatkozó konvertert
                return (LinePacketReply)JsonSerializer.Deserialize(json, type, optionsWithoutConverter);
            }
        }

        public override void Write(Utf8JsonWriter writer, LinePacketReply value, JsonSerializerOptions options)
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
