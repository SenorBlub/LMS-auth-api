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

			return true;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");

			return false;
		}
	}

	public async Task<(bool, Guid)> Authorize(EmailAuthRequest request)
	{
		string uri = Env.GetString("GATEWAY_IP") + ":" + Env.GetString("GATEWAY_PORT"); //!TODO Should be MQ when implemented
		uri += "/user/email-login";
		HttpClient client = new HttpClient();
		try
		{
			var response = client.PostAsJsonAsync(uri, request).Result;

			return (true, Guid.Parse(response.ReasonPhrase));
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");

			return (false, Guid.Empty);
		}
	}

	public async Task<bool> Register(RegisterRequest request)
	{
		string uri = Env.GetString("GATEWAY_IP") + ":" + Env.GetString("GATEWAY_PORT"); //!TODO Should be MQ when implemented
		uri += "/user";
		HttpClient client = new HttpClient();
		try
		{
			var response = client.PostAsJsonAsync(uri, request).Result;
			if (response.IsSuccessStatusCode)
			{
				return true;
			}

			return false;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send register request: {e.Message}.");

			return false;
		}
	}
}