using Pos.Domain.Model;

namespace Pos.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Ids_AreComparedByValue()
    {
        Assert.Equal(new StudentId(3), new StudentId(3));
        Assert.Equal(new CriterionId(3), new CriterionId(3));
        Assert.NotEqual(new StudentId(3), new StudentId(4));
    }

    [Theory]
    [InlineData("0.2")]
    [InlineData("30")]
    [InlineData("1000")]
    public void Weight_AcceptsPositiveValues(string value)
    {
        var weight = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Equal(weight, new Weight(weight).Value);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1000.01")]
    [InlineData("79228162514264337593543950335")]
    public void Weight_RejectsValuesOutOfRange(string value)
    {
        var weight = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Throws<DomainException>(() => new Weight(weight));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(12)]
    public void Score_AcceptsNonNegative(int value)
    {
        Assert.Equal(value, new Score(value).Value);
    }

    [Fact]
    public void Score_RejectsNegative()
    {
        Assert.Throws<DomainException>(() => new Score(-1));
    }

    [Fact]
    public void Score_IsComparedByValue()
    {
        Assert.Equal(new Score(7), new Score(7));
    }
}
