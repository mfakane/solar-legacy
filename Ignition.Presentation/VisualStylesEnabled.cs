using System;
using System.Windows.Markup;
using Forms = System.Windows.Forms;

namespace Ignition.Presentation
{
	public class VisualStylesEnabled : MarkupExtension
	{
		public VisualStylesEnabled()
		{
		}

		public bool RequiresVista
		{
			get;
			set;
		}

		public bool RequiresXP
		{
			get;
			set;
		}

		public bool Disabled
		{
			get;
			set;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this.Disabled && (!Forms.Application.RenderWithVisualStyles || !Forms.VisualStyles.VisualStyleInformation.IsEnabledByUser)
				|| !this.Disabled && (Forms.Application.RenderWithVisualStyles || Forms.VisualStyles.VisualStyleInformation.IsEnabledByUser)
				&& (this.RequiresXP && Environment.OSVersion.Version.Major == 5 || this.RequiresVista && Environment.OSVersion.Version.Major >= 6 || !this.RequiresXP && !this.RequiresVista);
		}

		public static void Initialize()
		{
			Forms.Application.EnableVisualStyles();
		}
	}
}
