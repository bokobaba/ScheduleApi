using System.Text.Json;

namespace ScheduleApi.Models {
    public class ErrorResponse {
        public string Message { get; set; }
        public bool Success { get; set; } = true;
    }
}
