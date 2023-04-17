using System.Runtime.CompilerServices;

namespace Name.Bayfaderix.Darxxemiyur.Extensions;

public static class NullableExtensions
{
	public static T ThrowIfNull<T>(
		this T? input, [CallerArgumentExpression("input")] string? description = null)
		where T : struct =>
		input ?? ThrowMustNotBeNull<T>(description);

	public static T ThrowIfNull<T>(
		this T? input, [CallerArgumentExpression("input")] string? description = null)
		where T : class =>
		input ?? ThrowMustNotBeNull<T>(description);

	private static T ThrowMustNotBeNull<T>(string? description) =>
		throw new InvalidOperationException($"{description} must not be null");
}
