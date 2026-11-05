using CampusPlacementManagement.DTOs.Auth;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusPlacementManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPlacementOfficerRepository _placementOfficerRepository;
        private readonly ICollegeRepository _collegeRepository;

        public AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IPlacementOfficerRepository placementOfficerRepository,
    ICollegeRepository collegeRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _placementOfficerRepository = placementOfficerRepository;
            _collegeRepository = collegeRepository;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return null;

            var passwordValid = await _userManager.CheckPasswordAsync(
                user, loginDto.Password);

            if (!passwordValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            // Check Placement Officer approval status
            var placementOfficer =
                await _placementOfficerRepository.GetByUserIdAsync(user.Id);

            if (placementOfficer != null)
            {
                if (placementOfficer.Status != AccountStatus.Approved)
                {
                    return null;
                }

                if (placementOfficer.College == null ||
                    placementOfficer.College.Status != CollegeStatus.Approved)
                {
                    return null;
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = Convert.ToDouble(
                jwtSettings["ExpiryInMinutes"]);

            var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Roles = roles,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}