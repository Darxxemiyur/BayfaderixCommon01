namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	/// <summary>
	/// Node network's next step information.
	/// </summary>
	public class StepInfo
	{
		/// <summary>
		/// Node network unit of work.
		/// </summary>
		/// <param name="previous">Previous node network step</param>
		/// <returns>Next node network step</returns>
		public delegate Task<StepInfo?> Delegate(StepInfo? previous);

		/// <summary>
		/// The original method delegate.
		/// </summary>
		public System.Delegate? OriginalMethod {
			get; protected set;
		}

		/// <summary>
		/// Arguments for OriginalMethod, if any.
		/// </summary>
		public IEnumerable<object?> Arguments {
			get; protected set;
		}

		/// <summary>
		/// Next step
		/// </summary>
		public Delegate? NextStep {
			get;
		}

		/// <summary>
		/// Data
		/// </summary>
		public object? Data {
			get;
		}

		/// <summary>
		/// </summary>
		/// <param name="nextStep"></param>
		/// <param name="data"></param>
		public StepInfo(Delegate? nextStep, object? data = null)
		{
			NextStep = nextStep;
			OriginalMethod = nextStep;
			Arguments = Array.Empty<object?>();
			Data = data;
		}
	}

	/// <summary>
	/// Node network's next step information with 1 argument.
	/// </summary>
	public class StepInfo<T1> : StepInfo
	{
		public new delegate Task<StepInfo?> Delegate(StepInfo? previous, T1 arg1);

		public StepInfo(Delegate? func, T1 arg1, object? data = null) : base(func != null ? (x) => func(x, arg1) : null, data)
		{
			OriginalMethod = func;
			Arguments = new object?[] { arg1 };
		}
	}

	/// <summary>
	/// Node network's next step information with 2 arguments.
	/// </summary>
	public class StepInfo<T1, T2> : StepInfo
	{
		public new delegate Task<StepInfo?> Delegate(StepInfo? previous, T1 arg1, T2 arg2);

		public StepInfo(Delegate? func, T1 arg1, T2 arg2, object? data = null) : base(func != null ? (x) => func(x, arg1, arg2) : null, data)
		{
			OriginalMethod = func;
			Arguments = new object?[] { arg1, arg2 };
		}
	}

	/// <summary>
	/// Node network's next step information with 3 arguments.
	/// </summary>
	public class StepInfo<T1, T2, T3> : StepInfo
	{
		public new delegate Task<StepInfo?> Delegate(StepInfo? previous, T1 arg1, T2 arg2, T3 arg3);

		public StepInfo(Delegate? func, T1 arg1, T2 arg2, T3 arg3, object? data = null) : base(func != null ? (x) => func(x, arg1, arg2, arg3) : null, data)
		{
			OriginalMethod = func;
			Arguments = new object?[] { arg1, arg2, arg3 };
		}
	}

	/// <summary>
	/// Node network's next step information with 4 arguments.
	/// </summary>
	public class StepInfo<T1, T2, T3, T4> : StepInfo
	{
		public new delegate Task<StepInfo?> Delegate(StepInfo? previous, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		public StepInfo(Delegate func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, object? data = null) : base(func != null ? (x) => func(x, arg1, arg2, arg3, arg4) : null, data)
		{
			OriginalMethod = func;
			Arguments = new object?[] { arg1, arg2, arg3, arg4 };
		}
	}
}
