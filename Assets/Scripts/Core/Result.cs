namespace App.Core
{
    // Discriminated union: a Result is exactly one of Success or Error.
    // Private constructor prevents any other subtype from being declared outside this file.
    public abstract record Result<TData, TError>
    {
        private Result() { }

        public sealed record Success(TData data) : Result<TData, TError>;

        public sealed record Error(TError error) : Result<TData, TError>;
    }
}
