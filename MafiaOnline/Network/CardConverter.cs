using MafiaOnline.RoleCards;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaOnline.Network
{
    internal class CardConverter : JsonConverter<Card>
    {
        public override Card ReadJson(JsonReader reader, Type objectType, Card existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            string asset = jObject["asset"].ToString();

            switch (asset)
            {
                case "citizen.png":
                    return jObject.ToObject<Citizen>();
                case "doctor.png":
                    return jObject.ToObject<Doctor>();
                case "mafia.png":
                    return jObject.ToObject<Mafia>();
                case "sheriff.png":
                    return jObject.ToObject<Sheriff>();
                default:
                    throw new ApplicationException($"The card type {asset} is not supported!");
            }
        }

        public override void WriteJson(JsonWriter writer, Card value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // Implement this if you need it
        }
    }

}
