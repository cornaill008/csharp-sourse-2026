namespace App.Composition.Mock
{
    // Selectable in the Inspector on MockPixabaySearchDataSource. Read fresh on every
    // SearchAsync call, so changing it during Play takes effect on the next search.
    public enum MockSearchScenario
    {
        Success,
        EmptyResult,
        ConnectionFailure,
        ServerError,
    }
}
