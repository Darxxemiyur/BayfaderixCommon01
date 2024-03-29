﻿using Name.Bayfaderix.Darxxemiyur.Collections;

namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// An object to call external actions on finilization.
/// </summary>
public sealed class ExternalOnFinalization : IDisposable
{
	#region Private memebers

	private bool _disposedValue;
	private bool _called;
	private readonly LinkedList<Action> _actions;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "Following the snippet IDisposable interface pattern.")]
	private void Dispose(bool disposing)
	{
		if (_disposedValue)
			return;

		this.Call();

		_disposedValue = true;
	}

	~ExternalOnFinalization() => this.Dispose(disposing: false);

	#endregion Private memebers

	/// <summary>
	/// List of actions.
	/// </summary>
	public LinkedList<Action> Actions => _called ? new LinkedList<Action>() : _actions;

	/// <summary>
	/// Initilizes the instance.
	/// </summary>
	/// <param name="actions">List of actions to add initially.</param>
	public ExternalOnFinalization(IEnumerable<Action>? actions = null) => _actions = actions?.ToLinkedList() ?? new LinkedList<Action>();

	/// <summary>
	/// Calls placed actions.
	/// </summary>
	public void Call()
	{
		if (_called)
			return;

		foreach (var action in Actions)
			action.Invoke();

		_called = true;
	}

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

/// <summary>
/// An object to call external actions with arguments on finilization.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ExternalOnFinalization<T> : IDisposable
{
	#region Private memebers

	private bool _disposedValue;
	private bool _called;
	private readonly T _default;
	private readonly LinkedList<Action<T>> _actions;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "Following the snippet IDisposable interface pattern.")]
	private void Dispose(bool disposing)
	{
		if (_disposedValue)
			return;

		this.Call(_default);

		_disposedValue = true;
	}

	~ExternalOnFinalization() => this.Dispose(disposing: false);

	#endregion Private memebers

	/// <summary>
	/// List of actions.
	/// </summary>
	public LinkedList<Action<T>> Actions => _called ? new LinkedList<Action<T>>() : _actions;

	/// <summary>
	/// Initilizes the instance.
	/// </summary>
	/// <param name="actions">List of actions to add initially.</param>
	public ExternalOnFinalization(T @default, IEnumerable<Action<T>>? actions = null) => (_default, _actions) = (@default, actions?.ToLinkedList() ?? new LinkedList<Action<T>>());

	/// <summary>
	/// Calls placed actions with an argument.
	/// </summary>
	/// <param name="arg">Argument</param>
	public void Call(T arg)
	{
		if (_called)
			return;

		foreach (var action in Actions)
			action.Invoke(arg);

		_called = true;
	}

	/// <summary>
	/// Calls placed actions with default argument.
	/// </summary>
	public void Call() => this.Call(_default);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
