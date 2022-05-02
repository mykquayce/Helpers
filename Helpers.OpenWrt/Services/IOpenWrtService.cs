﻿namespace Helpers.OpenWrt.Services;

public interface IOpenWrtService
{
	Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix prefix);
	Task AddBlackholesAsync(IEnumerable<Helpers.Networking.Models.AddressPrefix> prefixes);
	IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync();
	Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix blackhole);
}
