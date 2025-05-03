using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigNumbers;

namespace BigNumbersTests
{
    public class BigDecimalFloatTests
    {
        [Theory]
        [InlineData(0, "0.0")]
        [InlineData(1, "1.0")]
        [InlineData(-1, "-1.0")]
        public void ToStringTest(int value, string expectedS)
        {
            Assert.Equal(expectedS, new BigDecimalFloat(new BigDecimalInt(value)).ToString());
        }

        [Fact]
        public void DivisionTests()
        {
            var five = new BigDecimalFloat(new BigDecimalInt(5), new BigDecimalInt(0));
            var two = new BigDecimalFloat(new BigDecimalInt(2), new BigDecimalInt(0));
            var twoPointFive = new BigDecimalFloat(new BigDecimalInt(2), new BigDecimalInt(500_000_000));
            Assert.Equal(five / two, twoPointFive);
            Assert.Equal("2.5", twoPointFive.ToString());
        }
    }
}
