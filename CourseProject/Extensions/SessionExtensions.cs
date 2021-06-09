using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Extensions {
    public static class SessionExtensions {

        public static void Set<T>(this ISession session, string key, T value) {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }

        public static T Get<T>(this ISession session, string key) {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

    }
}
