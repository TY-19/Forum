namespace Forum.Application.Common.Extensions;

public static class StringExtensions
{
    public static bool CheckStringEquality(this string toCheck, IEnumerable<string> toEquals)
        => toEquals.Any(toEqual => CheckStringEquality(toCheck, toEqual));

    public static bool CheckStringEquality(this string toCheck, string toEqual)
        => toCheck.Equals(toEqual, StringComparison.CurrentCultureIgnoreCase);
}
