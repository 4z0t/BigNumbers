using BigNumbers;

namespace BigNumbersTests
{
    public class BigDecimalIntTest
    {
        [Theory]
        [InlineData(4, "4")]
        [InlineData(-4, "-4")]
        [InlineData(1_000_000_000, "1000000000")]
        [InlineData(0, "0")]
        public void ToStringTest(int value, string expectedS)
        {
            Assert.Equal(expectedS, new BigDecimalInt(value).ToString());
        }

        [Theory]
        [InlineData(2, 3, 5)]
        [InlineData(-2, 3, 1)]
        [InlineData(-2, -3, -5)]
        [InlineData(3, -3, 0)]
        [InlineData(-2, 2, 0)]
        [InlineData(2, -3, -1)]
        [InlineData(1, 999_999_999, 1_000_000_000)]
        public void AddTest(int left, int right, int result)
        {
            Assert.Equal(new BigDecimalInt(left) + new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(2, 3, -1)]
        [InlineData(-2, 3, -5)]
        [InlineData(-2, -3, 1)]
        [InlineData(2, -3, 5)]
        [InlineData(2, 2, 0)]
        [InlineData(-3, -3, 0)]
        [InlineData(1_000_000_000, 999_999_999, 1)]
        [InlineData(999_999_999, 1_000_000_000, -1)]
        public void SubTest(int left, int right, int result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) - new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1_000_000_000, 1_000_000_000, true)]
        public void TestEqual(int left, int right, bool result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) == new BigNumbers.BigDecimalInt(right), result);
            Assert.Equal(new BigNumbers.BigDecimalInt(left) != new BigNumbers.BigDecimalInt(right), !result);
        }

        [Theory]
        [InlineData(0, -1, 0)]
        [InlineData(-1, 0, 0)]
        [InlineData(2, 3, 6)]
        [InlineData(-2, 3, -6)]
        [InlineData(-2, -3, 6)]
        [InlineData(2, -3, -6)]
        public void MultTestInt(int left, int right, int result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) * new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(999_999_999, 999_999_999, 999_999_998_000_000_001)]
        [InlineData(999_999_999, -999_999_999, -999_999_998_000_000_001)]
        public void MultTestLong(long left, long right, long result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) * new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(4, 2, 2)]
        [InlineData(-4, 2, -2)]
        [InlineData(4, -2, -2)]
        [InlineData(-4, -2, 2)]
        [InlineData(1_000_000_000, 10, 100_000_000)]
        public void DivTestLong(long left, long right, long result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) / new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(4, 2, 0)]
        [InlineData(1_000_000_000, 10, 0)]
        [InlineData(1_000_000_000, 999_999_999, 1)]
        public void RemTestLong(long left, long right, long result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(left) % new BigNumbers.BigDecimalInt(right), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(1, 2, 100)]
        [InlineData(1_000, 0, 1_000)]
        [InlineData(1, 10, 10_000_000_000)]
        public void DecimalShiftTest(int value, int shift, long result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(value).DecimalLeftShift(shift), new BigNumbers.BigDecimalInt(result));
        }

        [Theory]
        [InlineData(10_000_000_000, 0, 10_000_000_000)]
        [InlineData(1,1,0)]
        [InlineData(10_000_000_000, 1, 10)]
        public void CellsRightShiftTest(long value, int shift, long result)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(result), new BigNumbers.BigDecimalInt(value).CellsRightShift(shift));
        }

        [Theory]
        [InlineData("10123456789000000000", 1, 10_123_456_789)]
        [InlineData("10123456789000000000", 2, 10)]
        public void CellsRightShiftStringTest(string value, int shift, long result)
        {
            Assert.Equal(new BigDecimalInt(result), new BigDecimalInt(value).CellsRightShift(shift));
        }

        [Theory]
        [InlineData("10123456789", 0, "10123456789")]
        [InlineData("10123456789", 1, "10123456789000000000")]
        [InlineData("10123456789", 2, "10123456789000000000000000000")]
        public void CellsLeftStringTest(string value, int shift, string result)
        {
            Assert.Equal(new BigDecimalInt(result), new BigDecimalInt(value).CellsLeftShift(shift));
        }



        [Theory]
        [InlineData("1", 1)]
        [InlineData("01", 1)]
        [InlineData("0000000000000000000001", 1)]
        [InlineData("-1", -1)]
        [InlineData("0", 0)]
        [InlineData("00000000000000000000000", 0)]
        [InlineData("-0", 0)]
        [InlineData("1000000000", 1_000_000_000)]
        [InlineData("-1000000000", -1_000_000_000)]
        [InlineData("10123456789", 10_123_456_789)]

        public void StringParseTest(string s, long value)
        {
            Assert.Equal(new BigNumbers.BigDecimalInt(value), new BigNumbers.BigDecimalInt(s));
        }



    }
}
