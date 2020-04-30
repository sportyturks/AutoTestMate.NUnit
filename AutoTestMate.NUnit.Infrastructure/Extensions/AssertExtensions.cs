using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoTestMate.MsTest.Infrastructure.Extensions
{
	public static class AssertExtensions
	{
		public static void All(this Assert assert, params Action[] assertions)
		{
			var errors = new List<string>();
			var inconclusives = new List<string>();
			foreach (var assertion in assertions)
			{
				try
				{
					assertion();
				}
				catch (AssertFailedException ex)
				{
					errors.Add(ex.Message);
				}
				catch (AssertInconclusiveException ex)
				{
					inconclusives.Add(ex.Message);
				}
			}

			int errCount = errors.Count;
			int inconCount = inconclusives.Count;

			var message = errCount > 0
				? $"{errCount} assertion(s) failed: {Environment.NewLine} - {string.Join(Environment.NewLine + " - ", errors)}"
				: "";

			if (inconCount > 0)
				message += $"{(errCount > 0 ? Environment.NewLine + Environment.NewLine : "")} {inconCount}  assertion(s) were inconclusive: {Environment.NewLine} - {string.Join(Environment.NewLine + " - ", inconclusives)}";

			if (errCount > 0)
			{
				throw new AssertFailedException(message);
			}

			if (inconCount > 0)
			{
				throw new AssertInconclusiveException(message);
			}
		}

	}
}
