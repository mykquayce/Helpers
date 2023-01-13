using System.Text.Json;
using Xunit;

namespace Helpers.OldhamCouncil.Tests.Models;

public class AddressTests
{
	[Theory]
	[InlineData("""
[
    {
        "UPRN": null,
        "USRN": "",
        "SAON": null,
        "PAON": null,
        "STREET_DESCRIPTOR": null,
        "LOCALITY_NAME": null,
        "TOWN_NAME": null,
        "ADMINISTRATIVE_AREA": null,
        "POSTCODE": null,
        "X_COORDINATE": null,
        "Y_COORDINATE": null,
        "LAST_UPDATE_DATE": null,
        "LOGICAL_STATUS": null,
        "BLPU_CLASS": null,
        "POSTALLY_ADDRESSABLE": null,
        "FULL_ADDRESS": "-- Please select from the list --",
        "AddressNumber": 0
    },
    {
        "UPRN": "422000069073",
        "USRN": "29405110",
        "SAON": null,
        "PAON": "21  OLDHAM DELIVERY OFFICE",
        "STREET_DESCRIPTOR": "HAMILTON STREET",
        "LOCALITY_NAME": null,
        "TOWN_NAME": "OLDHAM",
        "ADMINISTRATIVE_AREA": "OLDHAM",
        "POSTCODE": "OL1 1AA",
        "X_COORDINATE": "3.93482E+5",
        "Y_COORDINATE": "4.04988E+5",
        "LAST_UPDATE_DATE": "20020411",
        "LOGICAL_STATUS": "1",
        "BLPU_CLASS": "Residential, Dwellings, Semi-detached House",
        "POSTALLY_ADDRESSABLE": "Y",
        "FULL_ADDRESS": "21  OLDHAM DELIVERY OFFICE HAMILTON STREET OLDHAM OL1 1AA",
        "AddressNumber": 0
    }
]
""", "422000069073", "21  OLDHAM DELIVERY OFFICE HAMILTON STREET OLDHAM OL1 1AA")]
	public void Deserialize(string json, string expectedUprn, string expectedFullAddress)
	{
		var addresses = JsonSerializer.Deserialize<IList<OldhamCouncil.Models.Address>>(json);

		Assert.NotNull(addresses);
		Assert.NotEmpty(addresses);
		Assert.DoesNotContain(default, addresses);
		Assert.Equal(2, addresses.Count);

		Assert.Equal(default, addresses[0].Uprn);
		Assert.Equal("-- Please select from the list --", addresses[0].FullAddress);

		Assert.Equal(expectedUprn, addresses[1].Uprn);
		Assert.Equal(expectedFullAddress, addresses[1].FullAddress);
	}
}
