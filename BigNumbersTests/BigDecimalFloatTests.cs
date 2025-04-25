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
    }
}
