namespace HireBase_Backend.DTOs
{

    public class SaveJobRequest
    {
        public Guid ApplicantId { get; set; }
        public Guid JobId { get; set; }
    }
}