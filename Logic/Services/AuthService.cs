﻿using Logic.IServices;
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
		string uri = BuildGatewayUri("/user/User/login");
		HttpClient client = new HttpClient();
		try
		{
			var payload = request;
			var response = await client.PostAsJsonAsync(uri, payload);

			return response.IsSuccessStatusCode;
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send login request: {e.Message}.");
			return false;
		}
	}

	public async Task<(bool, Guid)> EmailAuthorize(EmailAuthRequest request)
	{
		string uri = BuildGatewayUri("/user/User/email-login");

		try
		{
			// Send the request using the injected HttpClient
			var response = await _httpClient.PostAsJsonAsync(uri, request);

			if (response.IsSuccessStatusCode)
			{
				// Deserialize the response content to extract the userId
				var responseContent = await response.Content.ReadFromJsonAsync<Guid>();
				if (responseContent != null && responseContent != Guid.Empty)
				{
					return (true, responseContent);
				}
			}

			Console.WriteLine($"Email authorization failed with status code: {response.StatusCode}");
			return (false, Guid.Empty);
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send the email login request: {e.Message}\n{e.StackTrace}");
			return (false, Guid.Empty);
		}
	}


	public async Task<(bool, Guid)> Register(RegisterRequest request)
	{
		string uri = BuildGatewayUri("/user/User");
		HttpClient client = new HttpClient();
		User requestUser = new User
		{
			Email = request.Email,
			Id = Guid.Empty,
			Password = request.Password,
			Username = request.Username
		};

		try
		{
			var payload = requestUser;
			var response = await client.PostAsJsonAsync(uri, payload);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadFromJsonAsync<User>();
				if (responseContent != null && responseContent.Id != Guid.Empty)
				{
					return (true, responseContent.Id);
				}
			}

			return (false, Guid.Empty);
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"An error occurred while trying to send register request: {e.Message}.");
			return (false, Guid.Empty);
		}
	}

}
