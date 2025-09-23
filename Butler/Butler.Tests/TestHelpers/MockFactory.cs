using System.Linq.Expressions;

namespace Butler.Tests.TestHelpers;

using Moq;

public static class MockFactory {
    public static Mock<T> CreateReturning<T, TResult>(
        Expression<Func<T, TResult>> expression,
        TResult result
    ) where T : class {
        Mock<T> mock = new();
        mock.Setup(expression).Returns(result);
        return mock;
    }

    public static Mock<T> CreateReturningAsync<T, TResult>(
        Expression<Func<T, Task<TResult>>> expression,
        TResult result) where T : class
    {
        Mock<T> mock = new();
        mock.Setup(expression).ReturnsAsync(result);
        return mock;
    }

    public static Mock<T> CreateThrowing<T, TResult>(
        Expression<Func<T, TResult>> expression, 
        Exception exception
    ) where T : class {
        Mock<T> mock = new();
        mock.Setup(expression).Throws(exception);
        return mock;
    }

    public static void VerifyCalled<T>(
        Mock<T> mock,
        Expression<Action<T>> expression,
        Times? times = null
    ) where T : class {
        mock.Verify(expression, times ?? Times.AtLeastOnce());
    }

    public static void VerifyCalled<T, TResult>(
        Mock<T> mock,
        Expression<Func<T, TResult>> expression,
        Times? times = null
    ) where T : class {
        mock.Verify(expression, times ?? Times.AtLeastOnce());
    }
}