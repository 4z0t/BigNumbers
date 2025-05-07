using BigNumbers;

BigDecimalInt a = new(2);
BigDecimalInt two = new(2);
for (int i = 0; i < 100; i++)
{
    a *= two;
}

for (int i = 0; i < 100; i++)
{
    Console.WriteLine(a);
    a /= two;
}

BigDecimalFloat f = BigDecimalFloat.BigIntAsFloat(new BigDecimalInt(10), 1);
Console.WriteLine(f);

BigDecimalFloat f2 = new BigDecimalFloat(new BigDecimalInt(1), new BigDecimalInt(500_000_000));
Console.WriteLine(f2);

BigDecimalFloat f1 = new BigDecimalFloat(new BigDecimalInt(1), 4);
Console.WriteLine(f1 / new BigDecimalInt(3));