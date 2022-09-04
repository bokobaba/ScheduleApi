using System.Security.Claims;

namespace ScheduleApi.Utils {
    public static class Utils {
        public static string IdNotFoundMessage(string type, int id) {
            return string.Format("{0} with id = {1} not found", type, id);
        }

        public static string GetUserId(IHttpContextAccessor context) {
            string result = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("result: " + result);
            return result;
        }

        public static string GetUserAffiliation(IHttpContextAccessor context) {
            string result = context.HttpContext.User.FindFirstValue("user_metadata");
            Console.WriteLine("result: " + result);
            return result;
        }
    }
}
