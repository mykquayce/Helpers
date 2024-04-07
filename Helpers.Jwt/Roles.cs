namespace Helpers.Jwt;

[Flags]
public enum Roles : byte
{
	Admin = 1,
	Customer = 2,
	User = 4,
}
