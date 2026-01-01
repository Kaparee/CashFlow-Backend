namespace CashFlow.Application.DTO.Responses
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Nickname { get; set; }
        public required string Email { get; set; }
        public string? PhotoUrl { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}