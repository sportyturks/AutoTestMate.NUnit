namespace AutoTestMate.NUnit.Web.Core
{
	public interface IDriverCleanup
	{
		void Initialise();
		void Dispose();

		IProcess Process { get; }
	}
}