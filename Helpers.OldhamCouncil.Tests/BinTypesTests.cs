using Xunit;

namespace Helpers.OldhamCouncil.Tests;

public class BinTypesTests
{
	[Theory]
	[InlineData("Blue Bin", OldhamCouncil.Models.BinTypes.Blue)]
	[InlineData("Grey Bin", OldhamCouncil.Models.BinTypes.Grey)]
	[InlineData("1100 Litre Bin", OldhamCouncil.Models.BinTypes._1100)]
	public void GetBin(string description, OldhamCouncil.Models.BinTypes expected)
	{
		// Arrange
		var table = new OldhamCouncil.Models.Generated.tableType
		{
			thead = new OldhamCouncil.Models.Generated.theadType
			{
				tr = new OldhamCouncil.Models.Generated.trType
				{
					th = new OldhamCouncil.Models.Generated.thType[1]
					{
						new()
						{
							b = description,
						},
					},
				},
			},
		};

		// Act
		var actual = Concrete.Service.GetBin(table);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("05/05/2021", 2021, 5, 5)]
	[InlineData("12/05/2021", 2021, 5, 12)]
	[InlineData("19/05/2021", 2021, 5, 19)]
	public void DateParse(string dateString, int year, int month, int day)
	{
		// Arrange
		var table = new OldhamCouncil.Models.Generated.tableType
		{
			tbody = new OldhamCouncil.Models.Generated.trType[1]
			{
				new()
				{
					td = new OldhamCouncil.Models.Generated.tdType?[2]
					{
						default,
						new()
						{
							Text = new string[1]
							{
								dateString,
							},
						},
					},
				},
			},
		};

		// Act
		var actual = Concrete.Service.GetDateTime(table);

		// Assert
		Assert.Equal(year, actual.Year);
		Assert.Equal(month, actual.Month);
		Assert.Equal(day, actual.Day);
	}
}
