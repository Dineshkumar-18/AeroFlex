using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AeroFlex.Response
{
    public class CommonResponse<T>
    {
        public bool Flag { get; set; }
        public string Message { get; set; }
        public T? Model { get; set; }

        public CommonResponse(bool flag, string message, T? model)
        {
            Flag = flag;
            Message = message;
            Model = model;
        }
    }
}
