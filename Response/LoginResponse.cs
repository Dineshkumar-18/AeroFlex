namespace AeroFlex.Response
{
    public record class LoginResponse(bool flag,string message,string token=null!,string refreshtoken = null!);
}
