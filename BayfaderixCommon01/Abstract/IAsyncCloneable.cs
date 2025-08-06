namespace Name.Bayfaderix.Darxxemiyur.Abstract;

public interface IAsyncCloneable<TResult>
{
    Task<TResult> CloneAsync();
    Task<TResult> CloneAsync(object input);
}