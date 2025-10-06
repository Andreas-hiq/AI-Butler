namespace Butler.Tests;

public static class TestDatafactory {
    public static string RandomString(int length) {
        return new string('x', length);
    }
}