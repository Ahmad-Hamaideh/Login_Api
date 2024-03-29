﻿using LoginApi.Helpers;
using LoginApi.Moudls;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginApi .Models
{
    //public class AuthService : IAuthService
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly RoleManager<IdentityRole> _roleManager;
    //    private readonly JWT _jwt;

    //    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
    //    {
    //        _userManager = userManager;
    //        _roleManager = roleManager;
    //        _jwt = jwt.Value;
    //    }

    //    public async Task<Response> RegisterAsync(Registration model)
    //    {
    //        if (await _userManager.FindByEmailAsync(model.UserEmail) is not null)
    //            return new Response { Message = "Email is already registered!" };

    //        if (await _userManager.FindByNameAsync(model.UserName) is not null)
    //            return new Response { Message = "Username is already registered!" };

    //        var user = new ApplicationUser
    //        {
    //            UserName = model.UserName,
    //            Email = model.UserEmail,
              
    //        };

    //        var result = await _userManager.CreateAsync(user, model.UserPassword);

    //        if (!result.Succeeded)
    //        {
    //            var errors = string.Empty;

    //            foreach (var error in result.Errors)
    //                errors += $"{error.Description},";

    //            return new AuthModel { Message = errors };
    //        }

    //        await _userManager.AddToRoleAsync(user, "User");

    //        var jwtSecurityToken = await CreateJwtToken(user);

    //        return new AuthModel
    //        {
    //            Email = user.Email,
    //            ExpiresOn = jwtSecurityToken.ValidTo,
    //            IsAuthenticated = true,
    //            Roles = new List<string> { "User" },
    //            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
    //            Username = user.UserName
    //        };
    //    }

    //    public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
    //    {
    //        var authModel = new AuthModel();

    //        var user = await _userManager.FindByEmailAsync(model.Email);

    //        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
    //        {
    //            authModel.Message = "Email or Password is incorrect!";
    //            return authModel;
    //        }

    //        var jwtSecurityToken = await CreateJwtToken(user);
    //        var rolesList = await _userManager.GetRolesAsync(user);

    //        authModel.IsAuthenticated = true;
    //        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    //        authModel.Email = user.Email;
    //        authModel.Username = user.UserName;
    //        authModel.ExpiresOn = jwtSecurityToken.ValidTo;
    //        authModel.Roles = rolesList.ToList();

    //        return authModel;
    //    }

       

    //    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    //    {
    //        var userClaims = await _userManager.GetClaimsAsync(user);
    //        var roles = await _userManager.GetRolesAsync(user);
    //        var roleClaims = new List<Claim>();

    //        foreach (var role in roles)
    //            roleClaims.Add(new Claim("roles", role));

    //        var claims = new[]
    //        {
    //            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //            new Claim(JwtRegisteredClaimNames.Email, user.Email),
    //            new Claim("uid", user.Id)
    //        }
    //        .Union(userClaims)
    //        .Union(roleClaims);

    //        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
    //        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

    //        var jwtSecurityToken = new JwtSecurityToken(
    //            issuer: _jwt.Issuer,
    //            audience: _jwt.Audience,
    //            claims: claims,
    //            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
    //            signingCredentials: signingCredentials);

    //        return jwtSecurityToken;
    //    }

    //    Task<Response> IAuthService.GetTokenAsync(TokenRequestModel model)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<string> AddRoleAsync(AddRoleModel model)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string GenerateJwtToken(ApplicationUser user)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}