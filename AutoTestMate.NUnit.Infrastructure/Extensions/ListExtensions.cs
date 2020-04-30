using System;
using System.Collections.Generic;
using System.Linq;
using AutoTestMate.MsTest.Infrastructure.Helpers;

namespace AutoTestMate.MsTest.Infrastructure.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IEnumerable{T}"/> and its descendants and implementors...
	/// </summary>
	public static class ListExtensions
	{
		#region Get Random Value

		/// <summary>
		/// Gets a specified property value from a random item in the supplied list. If there
		/// are no items in the list, will return the default value for the return type. If there
		/// is one item only in the list, will return the property value of that item.
		/// <para>Note that this method enumerates the list, so be careful with large lists.</para>
		/// </summary>
		/// <typeparam name="T">The type of the objects in the list. In most cases this can be implied from the
		/// input parameters.</typeparam>
		/// <typeparam name="TResult">The type of the value to be returned. In most cases this can be implied
		/// from the input parameters.</typeparam>
		/// <param name="list">The list from which to retrieve the object value.</param>
		/// <param name="getValue">Function which is supplied with the selected item from the list, allowing the
		/// calling method to determine the value to retrieve.</param>
		/// <returns>The value of the specified property of the randomly chosen object in the list, or the default
		/// value of the return type, if there are no items in the list.</returns>
		public static TResult GetRandomValue<T, TResult>(this IEnumerable<T> list, Func<T, TResult> getValue)
		{
			TResult result;
			// Need to enumerate the list in order to get the count AND retrieve an item later on.
			List<T> internalList = list.ToList();
			switch (internalList.Count)
			{
				case 0:
					// Nothing in the list. Return the default value for the type. In most cases this will be null, but not all.
					result = default(TResult);
					break;

				case 1:
					// Only one item in the list, so just return that.
					result = internalList.Select(getValue).First();
					break;

				default:
					int index = Global.Random.Next(0, internalList.Count);
					result = internalList.Select(getValue).Skip(index).FirstOrDefault();
					break;
			}

			return result;
		}

		/// <summary>
		/// <para>Returns up to maxCount random values from a list.</para>
		/// <para>If the maxCount parameter is less than one, no items (empty list) will be returned.</para>
		/// <para>If the maxCount parameter is greater than or equal to the number of items in the list,
		/// the whole list will be returned in a random order.</para>
		/// <para>Note that this method enumerates the list, so be careful with large lists.</para>
		/// </summary>
		/// <typeparam name="T">The type of the list item.</typeparam>
		/// <typeparam name="TResult">The type of the returned value.</typeparam>
		/// <param name="list">The list of items from which to randomly select.</param>
		/// <param name="getValue">Function to retrieve the appropriate value from the selected item.</param>
		/// <param name="maxCount">The maximum number of items to return from the list.</param>
		/// <returns>Up to maxCount values from the supplied list.</returns>
		public static IEnumerable<TResult> GetRandomValues<T, TResult>(this IEnumerable<T> list, Func<T, TResult> getValue, int maxCount)
		{
			List<T> internalList = list.ToList();
			int count = 0;
			while (count < maxCount && internalList.Any())
			{
				int index = internalList.Count == 1 ? 0 : Global.Random.Next(0, internalList.Count);
				TResult value = getValue(internalList.ElementAt(index));
				internalList.RemoveAt(index);
				count++;
				yield return value;
			}
		}

		#endregion

		#region Random Order

		/// <summary>
		/// Returns the supplied list sorted in a random order.
		/// <para>Note that the max parameter refers to the maximum number used when generating
		/// a random number used for sorting. It is NOT the maximum number of items returned.</para>
		/// </summary>
		/// <typeparam name="T">The type of the item in the list.</typeparam>
		/// <param name="list">The original list.</param>
		/// <param name="max">Optional maximum number used when generating a random number used
		/// for sorting. Defaults to 100, but can be increased for larger lists if required.</param>
		/// <returns>The entire list, sorted randomly.</returns>
		public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> list, int max = 1000)
		{
			return list.OrderBy(i => Global.Random.Next(max));
		}

		#endregion

		#region Distinct By

		/// <summary>
		/// Performs a distinct operation on an IEnumerable list, using a function to generate
		/// a key which is used to determine distinct-ness.
		/// Simpler to use than LINQ's default Distinct method, which requires an equality
		/// comparer, and only works on the object in the list, not one (or more) of its properties.
		/// </summary>
		/// <typeparam name="TSource">The type of the items in the list.</typeparam>
		/// <typeparam name="TKey">The type of the key being compared for distinct-ness.</typeparam>
		/// <param name="list">The original list of items.</param>
		/// <param name="getKey">The function used to determine the key used in the comparison.</param>
		/// <returns>A list of distinct items, based on the key.</returns>
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> getKey)
		{
			HashSet<TKey> set = new HashSet<TKey>();
			foreach (TSource item in list)
			{
				if (set.Add(getKey(item)))
					yield return item;
			}
		}

		#endregion
	}
}
