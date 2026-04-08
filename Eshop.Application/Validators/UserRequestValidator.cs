using Eshop.Application.Auth.Contracts.Requests;
using Eshop.Application.Common.Exceptions;
using Eshop.Application.Users.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Validators
{
    public class UserRequestValidator
    {
        public static void ValidateRegister(RegisterRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.UserName))
                errors.Add("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!request.Email.Contains("@"))
                errors.Add("Email is not valid.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required");

            if (!string.IsNullOrWhiteSpace(request.Password) &&
                request.Password.Length < 6)
                errors.Add("Password must be at least 6 characters long.");

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed.", errors);
            }
        }

        public static void ValidateLogin(LoginRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!request.Email.Contains("@"))
                errors.Add("Email is not valid.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed.", errors);
            }
        }

        public static void ValidateRefreshToken(RefreshTokenRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                errors.Add("Refresh token is required.");

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed.", errors);
            }
        }

        public static void ValidateRevokeRefreshToken(RevokeRefreshTokenRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                errors.Add("Refresh token is required.");

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed.", errors);
            }
        }
    }
}
