namespace CatNip.Presentation.Extensions;

internal static class ModelStateDictionaryExtensions
{
    public static ModelStateDictionary WithError(this ModelStateDictionary modelState, string key, string errorMessage)
    {
        modelState.AddModelError(key, errorMessage);

        return modelState;
    }
}
