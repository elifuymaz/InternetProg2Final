namespace emlak.api.DTOs
{
    public class ResultDto<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ResultDto<T> Success(T data, string message = "İşlem başarılı")
        {
            return new ResultDto<T>
            {
                Status = true,
                Message = message,
                Data = data
            };
        }

        public static ResultDto<T> Error(string message)
        {
            return new ResultDto<T>
            {
                Status = false,
                Message = message,
                Data = default
            };
        }
    }
}