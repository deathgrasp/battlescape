using System;

namespace Assets.Utils
{
	public class Range
	{
		public static void For(int count, Action action)
		{
			for (var i = 0; i < count; i++)
				action();
		}
		public static void For(int start, int count, Action action)
		{
			for (var i = start; i < start + count; i++)
				action();
		}
		public static void For(int count, Action<int> action)
		{
			for (var i = 0; i < count; i++)
				action(i);
		}
		public static void For(int start, int count, Action<int> action)
		{
			for (var i = start; i < start + count; i++)
				action(i);
		}

		public static void ForFor(int count, int count1, Action action)
		{
			for (var i = 0; i < count; i++)
				for (var j = 0; j < count1; j++)
					action();
		}
		public static void ForFor(int start, int count, int start1, int count1, Action action)
		{
			for (var i = start; i < start + count; i++)
				for (var j = start; j < start1 + count1; j++)
					action();
		}
		public static void ForFor(int count, int count1, Action<int> action)
		{
			for (var i = 0; i < count; i++)
				for (var j = 0; j < count1; j++)
					action(i);
		}
		public static void ForFor(int start, int count, int start1, int count1, Action<int> action)
		{
			for (var i = start; i < start + count; i++)
				for (var j = start; j < start1 + count1; j++)
				
					action(i);
		}
		public static void ForFor(int count, int count1, Action<int, int> action)
		{
			for (var i = 0; i < count; i++)
				for (var j = 0; j < count1; j++)
					action(i, j);
		}
		public static void ForFor(int start, int count, int start1, int count1, Action<int, int> action)
		{
			for (var i = start; i < start + count; i++)
				for (var j = start; j < start1 + count1; j++)
					action(i, j);
		}

		public static void ReverseFor(int count, Action action)
		{
			for (var i = count; i--> 0;)
				action();
		}
		public static void ReverseFor(int start, int count, Action action)
		{
			for (var i = start; i--> start - count;)
				action();
		}
		public static void ReverseFor(int count, Action<int> action)
		{
			for (var i = count; i-- > 0; )
				action(i);
		}
		public static void ReverseFor(int start, int count, Action<int> action)
		{
			for (var i = start; i-- > start - count; )
				action(i);
		}

		/// <summary>
		/// Performs an action in the innermost loop
		/// </summary>
		/// <param name="action"></param>
		/// <param name="count"></param>
		public static void NestedFor(Action action, params int[] count)
		{
			NestedForHelper(action, 0, count);
		}
		private static void NestedForHelper(Action action, int index, params int[] count)
		{
			for (var i = 0; i < count[index]; i++)
			{
				if (index + 1 < count.Length)
					NestedForHelper(action, index + 1, count);
				else
					action();
			}
		}
	}
}
