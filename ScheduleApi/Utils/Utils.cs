using System.Security.Claims;

namespace ScheduleApi.Utils {
    public static class Utils {
        public static string IdNotFoundMessage(string type, int id) {
            return string.Format("{0} with id = {1} not found", type, id);
        }

        public static string GetUserId(IHttpContextAccessor context) {
            return context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

    }
}
