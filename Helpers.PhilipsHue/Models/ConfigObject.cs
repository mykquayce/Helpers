namespace Helpers.PhilipsHue.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record ConfigObject(
	string name, int zigbeechannel, string bridgeid, string mac, bool dhcp, string ipaddress,
	string netmask, string gateway, string proxyaddress, int proxyport, DateTime UTC, DateTime localtime,
	string timezone, string modelid, string datastoreversion, string swversion, string apiversion,
	ConfigObject.SoftwareUpdateObject swupdate, ConfigObject.SoftwareUpdateObject2 swupdate2, bool linkbutton,
	bool portalservices, string portalconnection, ConfigObject.PortalStateObject portalstate,
	ConfigObject.InternetServicesObject internetservices, bool factorynew, object replacesbridgeid,
	ConfigObject.BackupObject backup, string starterkitid,
	IReadOnlyDictionary<string, ConfigObject.WhitelistObject> whitelist)
{
	public record SoftwareUpdateObject(
		int updatestate, bool checkforupdate, SoftwareUpdateObject.DeviceTypesObject devicetypes,
		string url, string text, bool notify)
	{
		public record DeviceTypesObject(bool bridge, IReadOnlyList<string> lights, IReadOnlyList<object> sensors);
	}

	public record SoftwareUpdateObject2(
		bool checkforupdate, DateTime lastchange, SoftwareUpdateObject2.BridgeObject bridge,
		string state, SoftwareUpdateObject2.AutoInstallObject autoinstall)
	{
		public record BridgeObject(string state, DateTime lastinstall);
		public record AutoInstallObject(string updatetime, bool on);
	}

	public record PortalStateObject(bool signedon, bool incoming, bool outgoing, string communication);
	public record InternetServicesObject(string internet, string remoteaccess, string time, string swupdate);
	public record BackupObject(string status, int errorcode);
	public record WhitelistObject(DateTime lastusedate, DateTime createdate, string name);
}
