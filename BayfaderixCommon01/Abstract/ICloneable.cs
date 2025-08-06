namespace Name.Bayfaderix.Darxxemiyur.Abstract;

public interface ICloneable<TResult>
{
    TResult Clone();
    TResult Clone(object input);
}
