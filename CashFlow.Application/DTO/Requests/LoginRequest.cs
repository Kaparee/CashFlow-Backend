namespace CashFlow.Application.DTO.Requests
{
    public class LoginRequest
    {
        public string? EmailOrNickname { get; set; }
        public string? Password { get; set; }
    }
}
