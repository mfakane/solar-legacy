using System;

namespace Ignition
{
	/// <summary>
	/// 2 種類のイベント データを格納します。
	/// </summary>
	/// <typeparam name="T1">1 種類目のイベント データの型。</typeparam>
	/// <typeparam name="T2">2 種類目のイベント データの型。</typeparam>
	public class EventArgs<T1, T2> : EventArgs
	{
		/// <summary>
		/// 1 種類目のイベント データを取得または設定します。
		/// </summary>
		public T1 Value1
		{
			get;
			set;
		}

		/// <summary>
		/// 2 種類目のイベント データを取得または設定します。
		/// </summary>
		public T2 Value2
		{
			get;
			set;
		}

		/// <summary>
		/// 指定された 2 種類のイベント データで EventArgs&lt;T1, T2&gt; の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="value1">1 種類目のイベント データ。</param>
		/// <param name="value2">2 種類目のイベント データ。</param>
		public EventArgs(T1 value1, T2 value2)
		{
			this.Value1 = value1;
			this.Value2 = value2;
		}
	}
}
