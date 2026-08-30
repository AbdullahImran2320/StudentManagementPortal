namespace StudentAPI.Exceptions
{
    public class AppExceptions : Exception
    {
        public int StatusCode {  get; set; }
        public AppExceptions(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public class NotFoundException : AppExceptions{
            public NotFoundException(string resource , int id): base($"{resource} with {id} not found!", 404) { }
            public NotFoundException(string message): base (message, 404) { }
        }

        public class BadRequestException: AppExceptions
        {
            public BadRequestException(string message) :base(message, 400) { }

        }
    }
        public class ConflictException: AppExceptions
        {
            public ConflictException(string message) :base(message, 409) { }

        }
    }

