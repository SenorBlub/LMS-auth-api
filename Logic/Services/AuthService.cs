using Logic.IServices;
using Logic.Models;
using System.Configuration;
using System.Net.Http.Json;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace Logic.Services;

public class AuthService : IAuthService
{
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;
	public AuthService(IConfiguration configuration, HttpClient httpClient)
	{
		_configuration = configuration;
		_httpClient = httpClient;
	}
	public async Task<bool> Authorize(AuthRequest request)
	{
		string uri = Env.GetString("GATEWAY_IP") + ":" + Env.GetString("GATEWAY_PORT"); //!TODO Should be MQ when implemented
		uri += "/user/login";
		HttpClient client = new HttpClient();
		try
		{
			var response = client.PostAsJsonAsync(uri, request).Result;

			return response.IsSuccessStatusCode;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");

			return false;
		}
	}
}