using BigNumbers;

namespace BigNumbersTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(4, "4")]
        [InlineData(-4, "-4")]
        public void ToStringTest(int value, string expectedS)
        {
            Assert.Equal(expectedS,new BigDecimalInt(value).ToString());
        }
    }
}
