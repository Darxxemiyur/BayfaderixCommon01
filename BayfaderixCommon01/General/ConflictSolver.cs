namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Conflict solving delegate.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="ours">Our version of data</param>
/// <param name="theirs"></param>
/// <returns></returns>
public delegate T? ConflictSolver<T>(T? ours, T? theirs);