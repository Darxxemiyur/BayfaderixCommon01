using System.Runtime.CompilerServices;

namespace Name.Bayfaderix.Darxxemiyur.Extensions;

/// <summary>
/// Extension class that provides extensions for nullable types to guarantee non-null values at run-time by throwing exceptions.
/// </summary>
public static class NullableExtensions
{
	//Copy of https://stackoverflow.com/a/75946595
	/// <summary>
	/// Throws <see cref="InvalidOperationException"/> when <paramref name="input"/> is null. Returns its value otherwise.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="input">Nullable value.</param>
	/// <param name="description">String representation of expression.</param>
	/// <returns>Non null value of <paramref name="input"/></returns>
	/// <exception cref="InvalidOperationException">The thrown exception when value is null.</exception>
	public static T ThrowIfNull<T>(this T? input, [CallerArgumentExpression("input")] string? description = null) where T : struct => input ?? ThrowMustNotBeNull<T>(description);

	/// <summary>
	/// Throws <see cref="InvalidOperationException"/> when <paramref name="input"/> is null. Returns its value otherwise.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="input">Nullable value.</param>
	/// <param name="description">String representation of expression.</param>
	/// <returns>Non null value of <paramref name="input"/></returns>
	/// <exception cref="InvalidOperationException">The thrown exception when value is null.</exception>
	public static T ThrowIfNull<T>(this T? input, [CallerArgumentExpression("input")] string? description = null) where T : class => input ?? ThrowMustNotBeNull<T>(description);

	/// <summary>
	/// Generates the <see cref="InvalidOperationException"/>.
	/// </summary>
	/// <typeparam name="T">Type of the value for convenient hack.</typeparam>
	/// <param name="description">Expression that was null</param>
	/// <returns>Dummy return. Throws an exception.</returns>
	/// <exception cref="InvalidOperationException">The exception</exception>
	private static T ThrowMustNotBeNull<T>(string? description) => throw new InvalidOperationException($"{description} must not be null");
}
