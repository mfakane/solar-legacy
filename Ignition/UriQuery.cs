using System;
using System.Collections.Generic;
using System.Linq;

namespace Ignition
{
	public class UriQuery : Dictionary<string, string>
	{
		public UriQuery()
			: base()
		{
		}

		public UriQuery(Uri uri)
			: base()
		{
			foreach (var i in uri.Query.TrimStart('?').Split('&'))
			{
				var idx = i.IndexOf('=');

				if (idx == -1)
					this.Add(i, null);
				else
					this.Add(i.Substring(0, idx), i.Substring(idx + 1));
			}
		}

		public override string ToString()
		{
			return string.Join("&", this.Where(_ => !string.IsNullOrEmpty(_.Key)).Select(_ => _.Key + "=" + _.Value).ToArray());
		}
	}
}
