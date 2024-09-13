using AeroFlex.Dtos;

namespace AeroFlex.Response
{
    public class GeneralResponse<T>
    {
        public bool flag { get; set; }
        public string message { get; set; }
        public T data { get; set; }

        public GeneralResponse(bool Success, string Message, T Data = default)
        {
            flag = Success;
            message = Message;
            data = Data;
        }
    }
}
