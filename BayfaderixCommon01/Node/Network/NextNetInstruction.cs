namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	using Node = Node<NextNetworkInstruction>;

	public class NextNetworkInstruction
	{
		public readonly Node? NextStep;
		public readonly NextNetworkActions NextAction;
		public object? Payload => _payload;
		protected readonly object? _payload;

		public NextNetworkInstruction(NextNetworkInstruction parent)
		{
			NextStep = parent.NextStep;
			NextAction = parent.NextAction;
			_payload = parent.Payload;
		}

		public NextNetworkInstruction(Node nextStep, NextNetworkActions nextAction, object payload)
		{
			NextStep = nextStep;
			NextAction = nextAction;
			_payload = payload;
		}

		public NextNetworkInstruction(Node nextStep, object payload)
		{
			NextStep = nextStep;
			NextAction = NextNetworkActions.Continue;
			_payload = payload;
		}

		public NextNetworkInstruction(Node nextStep, NextNetworkActions nextAction)
		{
			NextStep = nextStep;
			NextAction = nextAction;
			_payload = null;
		}

		public NextNetworkInstruction(Node nextStep)
		{
			NextStep = nextStep;
			NextAction = nextStep != null ? NextNetworkActions.Continue : NextNetworkActions.Stop;
			_payload = null;
		}

		public NextNetworkInstruction(object payload)
		{
			NextStep = null;
			NextAction = NextNetworkActions.Stop;
			_payload = payload;
		}

		public NextNetworkInstruction()
		{
			NextStep = null;
			NextAction = NextNetworkActions.Stop;
			_payload = null;
		}

		public static implicit operator NextNetworkInstruction(Node input) => new(input);
	}

	public class NextNetworkInstruction<T> : NextNetworkInstruction
	{
		public new T? Payload => (T?)_payload;

		public NextNetworkInstruction(NextNetworkInstruction<T> parent) : base(parent)
		{
		}

		public NextNetworkInstruction(Node nodeStep, T payload)
		{
		}
	}

	public enum NextNetworkActions
	{
		Continue,
		Stop,
	}
}