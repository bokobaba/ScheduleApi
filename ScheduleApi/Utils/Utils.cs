using System.Security.Claims;

namespace ScheduleApi.Utils {
    public static class Utils {
        public static string IdNotFoundMessage(string type, int id) {
            return string.Format("{0} with id = {1} not found", type, id);
        }

        public static Guid GetUserId(IHttpContextAccessor context) {
            return Guid.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
