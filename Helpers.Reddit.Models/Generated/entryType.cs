using Dawn;

namespace Helpers.Reddit.Models.Generated;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public partial class entryType
{
	private EntryType? _entryType;
	private long? _id;

	public EntryType Type => _entryType ??= GetEntryType();
	public long Id => _id ??= GetId();

	private long GetId()
	{
		PopulateValues();
		return _id!.Value;
	}

	private EntryType GetEntryType()
	{
		PopulateValues();
		return _entryType!.Value;
	}

	private void PopulateValues()
	{
		Guard.Argument(this.id).IsId();
		var (typeString, idString) = this.id.Split('_', count: 2);
		_entryType = (EntryType)byte.Parse(typeString[1..]);
		_id = Converters.Base36Converter.FromString<long>(idString);
	}
}
