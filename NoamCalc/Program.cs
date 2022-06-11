var calculator = new BinaryCalculator(x => x
    .On("+", (a, b) => a + b)
    .On("-", (a, b) => a - b)
    .On("*", (a, b) => a * b)
    .On("/", (a, b) => a / b)
);


TestBinaryOperation(calculator, 1, 2, "+");
TestBinaryOperation(calculator, 1, 2, "-");
TestBinaryOperation(calculator, 5, 6, "*");
TestBinaryOperation(calculator, 21, 3, "/");


void TestBinaryOperation(BinaryCalculator calculator, decimal a, decimal b, string op)
{
    var request = new BinaryOperation(a, b, op);
    var response = calculator.Calculate(request);

    Console.WriteLine($"{a} {op} {b} = {response.Result}");
}


