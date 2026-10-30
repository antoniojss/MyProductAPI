using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyProductAPI.Converters
{
    public partial class StringConverter : JsonConverter<string>
    {

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // recebeu assim   =  "                nome              sobrenome                  "
            var value = reader.GetString()?.Trim();

            // converteu assim =  "nome          sobrenome"

            //precisa tirar este espaço do meio e como sabre que tem que deixar 1 espaco entre nome e 
            // sobremnome

            if (value is null)
                return value;

            // usar o regex para remover os espacos em branco 
            // value  - valor a ser trocado
            // @ onde define o que quer ser subistituido 
            // e o ultimo pelo que m no caso 1 espaço em branco 
            // formato antigo (3 pontos em baixo do regex diz como implementar um novo tipo de metodo)
            //return Regex.Replace(value, @"\s=", " ");

            return RemoveExtraBlankSpace().Replace(value, " ");

        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        [GeneratedRegex(@"\s+")] 
        private static partial Regex RemoveExtraBlankSpace();

    }
}