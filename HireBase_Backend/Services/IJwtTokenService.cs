using System;
using System.Threading.Tasks;
using HireBase_Backend.Models;

namespace HireBase_Backend.Services
{
    public interface IJwtTokenService
    {

        Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)> CreateAccessTokenAsync(User user);


        Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)>
            CreateAccessTokenForApplicantAsync(ApplicantRegistration applicant);

        (string RefreshToken, DateTimeOffset ExpiresAt)
            CreateRefreshToken();
    }
}
