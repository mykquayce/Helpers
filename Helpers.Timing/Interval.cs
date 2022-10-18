using System.Text.Json.Serialization;

namespace Helpers.Timing;

public record Interval(
	[property: JsonConverter(typeof(JsonStringEnumConverter))] Units Unit,
	double Count)
	: IInterval
{
	public const Units DefaultUnit = Units.Day;
	public const double DefaultCount = 1;

	public Interval() : this(DefaultUnit, DefaultCount) { }

	#region ioptions implementation
	public Interval Value => this;
	#endregion ioptions implementation
}
