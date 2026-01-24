namespace HireBase_Backend.DTOs
{
    public class UpdatePasswordDto
    {
        public Guid UserId { get; set; } // or string Email if you prefer
        public string FullName { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }



    public class UpdateApplicantPasswordDto
    {
        public Guid applicantId { get; set; } // or string Email if you prefer
        public string FullName { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }


    public class UpdateApplicantNameDto
    {
        public Guid applicantId { get; set; } // or string Email if you prefer
        public string FullName { get; set; } = string.Empty;
    }

}
