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