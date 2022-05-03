namespace Helpers.OpenWrt.Models;

public record CommandRequestObject(string Command)
	: RequestObject(1, "exec", Command);
