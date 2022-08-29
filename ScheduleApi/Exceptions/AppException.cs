using System.Globalization;

namespace ScheduleApi.Exceptions {
    public class AppException : Exception {
        public ExceptionType type = ExceptionType.BAD_REQUEST;
        public AppException() : base() { }

        public AppException(string message) : base(message) {
        }

        public AppException(string message, ExceptionType type) : base(message) {
            this.type = type;
        }

        public AppException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }

    public enum ExceptionType {
        BAD_REQUEST,
        FORBIDDEN,
    }
}
