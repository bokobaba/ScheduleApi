namespace ScheduleApi.Models {
    public class ServiceResponse {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class ServiceResponse<T> {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
