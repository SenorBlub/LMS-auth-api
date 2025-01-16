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

	private string BuildGatewayUri(string endpoint)
	{
		string uri = Env.GetString("GATEWAY_IP") + ":" + Env.GetString("GATEWAY_PORT");
		return $"{uri}{endpoint}"; //change to MQ ip when implemented
	}

	private object WrapWithKey(object data)
	{
		return new { key = data };
	}

	public async Task<bool> Authorize(AuthRequest request)
	{
		string uri = BuildGatewayUri("/user/login");
		HttpClient client = new HttpClient();
		try
		{
			var payload = WrapWithKey(request);
			var response = await client.PostAsJsonAsync(uri, payload);

			return response.IsSuccessStatusCode;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");
			return false;
		}
	}

	public async Task<(bool, Guid)> Authorize(EmailAuthRequest request)
	{
		string uri = BuildGatewayUri("/user/email-login");
		HttpClient client = new HttpClient();
		try
		{
			var payload = WrapWithKey(request);
			var response = await client.PostAsJsonAsync(uri, payload);

			if (response.IsSuccessStatusCode)
			{
				Guid userId = Guid.Parse(response.ReasonPhrase);
				return (true, userId);
			}

			return (false, Guid.Empty);
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");
			return (false, Guid.Empty);
		}
	}

	public async Task<bool> Register(RegisterRequest request)
	{
		string uri = BuildGatewayUri("/user");
		HttpClient client = new HttpClient();
		try
		{
			var payload = WrapWithKey(request); 
			var response = await client.PostAsJsonAsync(uri, payload);

			return response.IsSuccessStatusCode;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send register request: {e.Message}.");
			return false;
		}
	}
}
