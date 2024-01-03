namespace Forum.Application.Common.Extensions;

public static class StringExtensions
{
    public static bool CheckStringEquality(this string toCheck, IEnumerable<string> toEquals)
    {
        return toEquals.Any(toEqual => CheckStringEquality(toCheck, toEqual));
    }

    public static bool CheckStringEquality(this string toCheck, string toEqual)
    {
        return toCheck.Equals(toEqual, StringComparison.CurrentCultureIgnoreCase);
    }
}
