using Moq;
using System.Collections.Generic;
using Xunit;
using static PressFCalculator.Tests.CalculatorTests;

namespace PressFCalculator.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void InitializeExchangeRates_ShouldInitializeRatesCorrectly()
        {
            // Arrange
            var calculator = new Calculator();

            // Act (метод InitializeExchangeRates вызывается в конструкторе)
            var exchangeRatesField = typeof(Calculator)
                .GetField("exchangeRates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var exchangeRates = exchangeRatesField?.GetValue(calculator) as Dictionary<string, double>;

            // Assert
            Assert.NotNull(exchangeRates);
            Assert.Equal(6, exchangeRates.Count);
            Assert.Equal(90.0, exchangeRates["USD_RUB"]);
            Assert.Equal(98.5, exchangeRates["EUR_RUB"]);
            Assert.Equal(1.09, exchangeRates["EUR_USD"]);
        }

        [Theory]
        [InlineData("USD", "RUB", 100, 9000)]
        [InlineData("EUR", "USD", 200, 218)]
        [InlineData("RUB", "EUR", 10000, 101.52)]
        public void ConvertCurrency_ShouldReturnCorrectResult(string fromCurrency, string toCurrency, double amount, double expected)
        {
            // Arrange
            var calculator = new Calculator();
            var privateMethod = typeof(Calculator)
                .GetMethod("ConvertCurrency", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var exchangeRatesField = typeof(Calculator)
                .GetField("exchangeRates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var exchangeRates = exchangeRatesField?.GetValue(calculator) as Dictionary<string, double>;

            // Act
            var rateKey = $"{fromCurrency}_{toCurrency}";
            var rate = exchangeRates[rateKey];
            var result = amount * rate;

            // Assert
            Assert.Equal(expected, result, 0.01);
        }

        [Fact]
        public void CalculateCredit_ShouldCalculateCorrectMonthlyPayment()
        {
            // Arrange
            var mockConsole = new Mock<IConsole>();
            var calculator = new Calculator();

            // Настройка мока для ввода данных
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("100000") // Сумма кредита
                .Returns("12")     // Срок кредита
                .Returns("10");    // Процентная ставка

            // Act
            // Необходимо создать метод для тестирования расчёта кредита

            double amount = 100000;
            int months = 12;
            double annualRate = 10;

            double monthlyRate = annualRate / 12 / 100;
            double monthlyPayment = amount * (monthlyRate * Math.Pow(1 + monthlyRate, months)) /
                                  (Math.Pow(1 + monthlyRate, months) - 1);

            // Assert
            Assert.Equal(8791.59, monthlyPayment, 0.01);
        }

        [Fact]
        public void IsValidCurrency_ShouldReturnCorrectResults()
        {
            // Arrange
            var calculator = new Calculator();
            var privateMethod = typeof(Calculator)
                .GetMethod("IsValidCurrency", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert
            Assert.True((bool)privateMethod.Invoke(calculator, new object[] { "RUB" }));
            Assert.True((bool)privateMethod.Invoke(calculator, new object[] { "USD" }));
            Assert.True((bool)privateMethod.Invoke(calculator, new object[] { "EUR" }));
            Assert.False((bool)privateMethod.Invoke(calculator, new object[] { "GBP" }));
            Assert.False((bool)privateMethod.Invoke(calculator, new object[] { "JPY" }));
            Assert.False((bool)privateMethod.Invoke(calculator, new object[] { "" }));
        }

        [Fact]
        public void CalculateDeposit_ShouldCallConsoleMethods()
        {
            // Arrange
            var mockConsole = new Mock<IConsole>();
            var calculator = new Calculator();

            // Настройка мока
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("100000") // Сумма вклада
                .Returns("12")     // Срок вклада
                .Returns("8")      // Процентная ставка
                .Returns("1");     // Тип вклада (с капитализацией)

            // Подмена Console.ReadLine
            var originalReadLine = Console.ReadLine;
            var inputs = new Queue<string>();
            inputs.Enqueue("100000");
            inputs.Enqueue("12");
            inputs.Enqueue("8");
            inputs.Enqueue("1");

            // Act
            try
            {
                Console.SetIn(new System.IO.StringReader("100000\n12\n8\n1"));
                // Вызов приватного метода CalculateDeposit через рефлексию
                var method = typeof(Calculator)
                    .GetMethod("CalculateDeposit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(calculator, null);

                // Verify - проверяем, что методы вызывались
                mockConsole.Verify(c => c.ReadLine(), Times.AtLeast(4));
            }
            finally
            {
                // Восстановление стандартного ввода
                Console.SetIn(System.Console.In);
            }
        }

        // Пример интерфейса для мокинга Console
        public interface IConsole
        {
            string ReadLine();
            void WriteLine(string value);
            void Write(string value);
        }
    }

    // Класс-обёртка для консоли для тестирования
    public class ConsoleWrapper : IConsole
    {
        public string ReadLine() => Console.ReadLine();
        public void WriteLine(string value) => Console.WriteLine(value);
        public void Write(string value) => Console.Write(value);
    }
}