namespace Helpers.OpenWrt.Models;

public record LoginRequestObject(string Username, string Password)
	: RequestObject(1, "login", Username, Password);
