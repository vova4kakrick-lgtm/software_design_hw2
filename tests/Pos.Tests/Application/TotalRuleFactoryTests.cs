using Pos.Application.Totals;
using Pos.Domain.Services.Totals;

namespace Pos.Tests.Application;

public class TotalRuleFactoryTests
{
    private readonly StrictTotalStrategy _strict = new();
    private readonly CurrentTotalStrategy _current = new();

    [Fact]
    public void WithoutRounding_ReturnsBaseRule()
    {
        var factory = new TotalRuleFactory(_strict, _current);

        Assert.Same(_strict, factory.Create(TotalMode.Strict, rounded: false));
        Assert.Same(_current, factory.Create(TotalMode.Current, rounded: false));
    }

    [Fact]
    public void WithRounding_WrapsRuleInDecorator()
    {
        var rule = new TotalRuleFactory(_strict, _current).Create(TotalMode.Current, rounded: true);

        Assert.IsType<RoundedTotalStrategy>(rule);
    }

    [Fact]
    public void UnknownMode_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TotalRuleFactory(_strict, _current).Create((TotalMode)42, rounded: false));
    }
}
