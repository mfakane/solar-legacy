using System.Collections.Generic;

namespace Ignition.Presentation
{
	public class MultiLineTextConverter : ValueConverter<IList<string>, string>
	{
		protected override string ConvertFromSource(IList<string> value, object parameter)
		{
			return value == null ? null : string.Join("\r\n", value);
		}

		protected override IList<string> ConvertToSource(string value, object parameter)
		{
			return value.Split("\r\n");
		}
	}
}
