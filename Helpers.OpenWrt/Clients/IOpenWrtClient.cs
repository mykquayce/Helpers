﻿namespace Helpers.OpenWrt.Clients;

public interface IOpenWrtClient
{
	Task<string> ExecuteCommandAsync(string command);
	Task<string> LoginAsync();
}
