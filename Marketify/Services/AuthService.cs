using Marketify.Authentication;
using Marketify.Contracts.Authenthication;
using Marketify.Entites;
using Marketify.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Org.BouncyCastle.Crypto.Prng;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Marketify.Services
{
    public class AuthService(UserManager<ApplicationUser>usermanger,IJwtProvider jwtProvider,
        ILogger<AuthService> logger, IEmailSender emailSender) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManger = usermanger;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;


        public async Task<AuthResponse?> GetTokenAsync(string Email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManger.FindByEmailAsync(Email);
            
            if (user == null) return null;

           var isValid =  await _userManger.CheckPasswordAsync(user, password);
            if (!isValid) return null;
            //generate Jwt Token
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var emailBody = EmailBodyHelper.GenerateEmailBody("EmailUserLogIn",
                new Dictionary<string, string>
                {
            { "{{username}}", user.FirstName },
            { "{{login_time}}",time }
                }
            );

            await _emailSender.SendEmailAsync(user.Email!, "Marketify  : UserlogedIn ✅", emailBody);
            return new AuthResponse(user.Id,user.Email,user.FirstName,user.LastName,token , expiresIn*60); 
        }

        public async Task<string> RegisterAsync(RegisterRequestUser model, CancellationToken cancellationToken = default)
        {
            var userExists = await _userManger.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return "User Already Exists";
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
            };

            var result = await _userManger.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return result.Errors.FirstOrDefault()?.Description ?? "Registration Failed";
            }

            var code = await _userManger.GenerateTwoFactorTokenAsync(user, "Email");

            var emailBody = EmailBodyHelper.GenerateEmailBody("Emailconfirmation",
                new Dictionary<string, string>
                {
            { "{{name}}", user.FirstName },
            { "{{code}}",code }
                }
            );

            await _emailSender.SendEmailAsync(user.Email!, "Marketify  : EmailConfirmation ✅", emailBody);

            return string.Empty;
        }
        public async Task<string> ConfirmEmailAsync(ConfirmEmailRequest model)
        {
            var user = await _userManger.FindByEmailAsync(model.email);

            if (user == null)
                return "Invalid Email or User Not Found";

            if (user.EmailConfirmed)
                return "Email is already confirmed";

            var isValid = await _userManger.VerifyTwoFactorTokenAsync(user, "Email", model.code);

            if (!isValid)
            {
                return "Invalid or Expired verification code";
            }

            user.EmailConfirmed = true;
            var result = await _userManger.UpdateAsync(user);

            return result.Succeeded ? string.Empty : "An error occurred while confirming your email";

        }

        public async Task<string> ForgotPasswordAsync(ForgotPassword request)
        {
            var user = await _userManger.FindByEmailAsync(request.Email);
            string successMessage = "If your email exists, a 6-digit code has been sent to your inbox.";

            if (user == null)
                return successMessage;

            var code = await _userManger.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            var values = new Dictionary<string, string>
    {
        { "{{name}}", user.FirstName ?? "User" },
        { "{{code}}", code }
    };

            var emailBody = EmailBodyHelper.GenerateEmailBody("ResetPassword", values);

            try
            {
                await _emailSender.SendEmailAsync(user.Email!, "Marketify : Reset Password Code ✅", emailBody);
            }
            catch
            {
                return "Error sending email. Please try again later.";
            }

            return successMessage;
        }

        public async Task<string> ResetPasswordAsync(ResetPassword request)
        {
            var user = await _userManger.FindByEmailAsync(request.Email);
            if (user == null)
                return "Invalid request";

            var isValid = await _userManger.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                request.Token
            );

            if (!isValid)
                return "Invalid or expired code";

            var resetToken = await _userManger.GeneratePasswordResetTokenAsync(user);
            var result = await _userManger.ResetPasswordAsync(user, resetToken, request.NewPassword);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return "Password has been reset successfully ✅";
        }
    }
}
