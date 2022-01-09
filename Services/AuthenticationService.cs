using Blazored.LocalStorage;
using Domain.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Model.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Services;

public interface IAuthService {
    Task<bool> Login(string identifier, string password);
    Task Logout();
    User? GetAccount();
}

public class AuthenticationService : AuthenticationStateProvider, IAuthService {
    const string itemName = "AuthenticationToken";

    HttpClient _httpClient;
    ILocalStorageService _localStorage;
    IUserRepository _repository;

    AuthenticationState _authenticationState = new AuthenticationState(new ClaimsPrincipal());

    User? account;

    public AuthenticationService(HttpClient client, ILocalStorageService localStorage, IUserRepository repository) {
        _httpClient = client;
        _localStorage = localStorage;
        _repository = repository;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync() {
        try {
            var token = await _localStorage.GetItemAsync<string>(itemName);
            if(string.IsNullOrEmpty(token)) return await Task.FromResult(_authenticationState);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            _authenticationState = GetAuthentication(token);

            long id = Convert.ToInt64(Get("Identity"));
            account = await _repository.GetAccount(id);
        } catch(Exception ex) {
            System.Diagnostics.Debug.WriteLine("Authentication Error: " + ex.Message);
        }
        return await Task.FromResult(_authenticationState);
    }

    public async Task<bool> Login(string identifier, string password) {
        var user = await _repository.GetAccount(identifier, password);
        if(user == null) throw new Exception("Identifier or password incorrect.");
        else if(user.Codes.Any(c => c.Type == CodeType.EMAIL)) throw new Exception("Verify your email before you login.");
        var token = GenerateToken(new Dictionary<string, string> { { "Identity", $"{user.Id}" } });
        await Write(itemName, token);
        return await Task.FromResult(true);
    }

    public async Task Logout() => await Remove(itemName);

    public User? GetAccount() => account ?? null;

    private async Task Write(string item, string value) {
        await _localStorage.SetItemAsync(item, value);
    }
    private async Task Remove(string item) {
        await _localStorage.RemoveItemAsync(item);
    }
    private string Get(string type) => _authenticationState.User.Claims.Single(c => c.Type == type).Value;
    private AuthenticationState GetAuthentication(string token) => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "Authentication")));
    private string GenerateToken(Dictionary<string, string> claims) {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("keyyys21334567890sd987654"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "localhost",
            "localhost",
            claims.Select(c => new Claim(c.Key, c.Value)).ToArray(),
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

        if(roles != null) {
            if(roles.ToString().Trim().StartsWith("[")) {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                foreach(var parsedRole in parsedRoles) {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            } else {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }
    private byte[] ParseBase64WithoutPadding(string base64) {
        switch(base64.Length % 4) {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}