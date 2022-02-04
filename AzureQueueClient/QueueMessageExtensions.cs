using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ShedLab {
    // Two sets of Extension method classes to serialise/deserialise JSON to strong typed object
    // One for PeekedMessages (when developing), one for QueueMessages

    public static class PeekedMessageExtensions {
        public static string AsString(this PeekedMessage message) {
            byte[] data = Convert.FromBase64String(message.MessageText);
            return Encoding.UTF8.GetString(data);
        }

        public static T As<T>(this PeekedMessage message) where T : class {
            byte[] data = Convert.FromBase64String(message.MessageText);
            string json = Encoding.UTF8.GetString(data);
            return Deserialize<T>(json, true);
        }

        private static T Deserialize<T>(string json, bool ignoreMissingMembersInObject) where T : class {
            T deserializedObject = null;
            MissingMemberHandling missingMemberHandling = MissingMemberHandling.Error;
            if (ignoreMissingMembersInObject)
                missingMemberHandling = MissingMemberHandling.Ignore;


            try {
                deserializedObject = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MissingMemberHandling = missingMemberHandling, });
            } catch (Exception ex) {
                Console.WriteLine("Deserialise exception: " + ex.Message);
                Environment.Exit(1);
            }

            return deserializedObject;
        }
    }

    public static class QueueMessageExtensions {
        public static string AsString(this QueueMessage message) {
            byte[] data = Convert.FromBase64String(message.MessageText);
            return Encoding.UTF8.GetString(data);
        }

        public static T As<T>(this QueueMessage message) where T : class {
            byte[] data = Convert.FromBase64String(message.MessageText);
            string json = Encoding.UTF8.GetString(data);
            return Deserialize<T>(json, true);
        }

        private static T Deserialize<T>(string json, bool ignoreMissingMembersInObject) where T : class {
            T deserializedObject;
            MissingMemberHandling missingMemberHandling = MissingMemberHandling.Error;
            if (ignoreMissingMembersInObject)
                missingMemberHandling = MissingMemberHandling.Ignore;
            deserializedObject = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { MissingMemberHandling = missingMemberHandling, });
            return deserializedObject;
        }
    }
}
