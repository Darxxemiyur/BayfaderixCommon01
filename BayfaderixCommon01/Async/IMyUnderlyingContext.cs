namespace Name.Bayfaderix.Darxxemiyur.Async;

/// <summary>
/// Made for future one Sync context object which is configurable on creation, to allow for
/// centralized control or something May or may not leave or delete
/// </summary>
public interface IMyUnderlyingContext
{
	SynchronizationContext ThisContext {
		get;
	}

	TaskScheduler? MyTaskScheduler {
		get;
	}

	TaskFactory? MyTaskFactory {
		get;
	}

	void Send(SendOrPostCallback d, object? state);

	void Post(SendOrPostCallback d, object? state);

	Task Place(AsyncOpBuilder asyncOp);
}
