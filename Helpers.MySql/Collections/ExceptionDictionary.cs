namespace Helpers.MySql.Collections;

public class ExceptionDictionary : SafeDictionary<ExceptionTypes, int>
{
	public ExceptionDictionary() : base(0) { }
}
