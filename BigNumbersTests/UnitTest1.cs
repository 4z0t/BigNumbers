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

        [Theory]
        [InlineData(2, 3, 5)]
        [InlineData(-2, 3, 1)]
        [InlineData(-2, -3, -5)]
        [InlineData(2, -3, -1)]
        [InlineData(1, 999_999_999, 1_000_000_000)]
        public void AddTest(int left, int right, int result)
        {
            Assert.Equal(new BigDecimalInt(left)+ new BigDecimalInt(right), new BigDecimalInt(result));
        }

        [Theory]
        [InlineData(2, 3, -1)]
        [InlineData(-2, 3, -5)]
        [InlineData(-2, -3, 1)]
        [InlineData(2, -3, 5)]
        [InlineData(1_000_000_000, 999_999_999, 1)]
        [InlineData(999_999_999, 1_000_000_000, -1)]
        public void SubTest(int left, int right, int result)
        {
            Assert.Equal(new BigDecimalInt(left) - new BigDecimalInt(right), new BigDecimalInt(result));
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        public void TestEqual(int left, int right, bool result)
        {
            Assert.Equal(left == right, result);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(-2, 3, -6)]
        [InlineData(-2, -3, 6)]
        [InlineData(2, -3, -6)]
        public void MultTestInt(int left, int right, int result)
        {
            Assert.Equal(new BigDecimalInt(left) * new BigDecimalInt(right), new BigDecimalInt(result));
        }

        [Theory]
        [InlineData(999_999_999, 999_999_999, 999_999_998_000_000_001)]
        public void MultTestLong(long left, long right, long result)
        {
            Assert.Equal(new BigDecimalInt(left) * new BigDecimalInt(right), new BigDecimalInt(result));
        }


    }
}
