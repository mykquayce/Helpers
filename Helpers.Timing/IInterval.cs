using Microsoft.Extensions.Options;

namespace Helpers.Timing;

public interface IInterval : IOptions<Interval>
{
	double Count { get; init; }
	Units Unit { get; init; }

	void Deconstruct(out Units Unit, out double Count);
}
