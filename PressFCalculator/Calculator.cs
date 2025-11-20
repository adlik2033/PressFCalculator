using Xunit;
namespace PressFCalculator
{
    public class Calculator
    {
        private Dictionary<string, double> exchangeRates;

        public Calculator()
        {
            InitializeExchangeRates();
        }
       
        [Fact]
        private void InitializeExchangeRates()
        {
            exchangeRates = new Dictionary<string, double>
            {
                ["USD_RUB"] = 90.0,
                ["EUR_RUB"] = 98.5,
                ["EUR_USD"] = 1.09,
                ["RUB_USD"] = 1.0 / 90.0,
                ["RUB_EUR"] = 1.0 / 98.5,
                ["USD_EUR"] = 1.0 / 1.09
            };

            // Добавьте проверки (assertions)
            Assert.NotNull(exchangeRates);
            Assert.Equal(6, exchangeRates.Count);
            Assert.Equal(90.0, exchangeRates["USD_RUB"]);
        }

        public void Run()
        {
            while (true)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CalculateCredit();
                        break;
                    case "2":
                        ConvertCurrency();
                        break;
                    case "3":
                        CalculateDeposit();
                        break;
                    case "4":
                        Console.WriteLine("Спасибо за использование финансового калькулятора! До свидания!");
                        return;
                    default:
                        Console.WriteLine("Ошибка: выберите опцию от 1 до 4!");
                        break;
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        private void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine(new string('=', 40));
            Console.WriteLine("=== ФИНАНСОВЫЙ КАЛЬКУЛЯТОР ===");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine("1. Расчет кредита");
            Console.WriteLine("2. Конвертер валют");
            Console.WriteLine("3. Калькулятор вкладов");
            Console.WriteLine("4. Выход");
            Console.WriteLine(new string('-', 40));
            Console.Write("Выберите опцию: ");
        }

        private void CalculateCredit()
        {
            Console.WriteLine("\n--- РАСЧЕТ КРЕДИТА ---");

            try
            {
                Console.Write("Сумма кредита (руб): ");
                double amount = double.Parse(Console.ReadLine());

                Console.Write("Срок кредита (месяцев): ");
                int months = int.Parse(Console.ReadLine());

                Console.Write("Процентная ставка (% годовых): ");
                double annualRate = double.Parse(Console.ReadLine());

                if (amount <= 0 || months <= 0 || annualRate <= 0)
                {
                    Console.WriteLine("Ошибка: все значения должны быть положительными!");
                    return;
                }

                // Расчет аннуитетного платежа
                double monthlyRate = annualRate / 12 / 100;
                double monthlyPayment = amount * (monthlyRate * Math.Pow(1 + monthlyRate, months)) /
                                      (Math.Pow(1 + monthlyRate, months) - 1);
                double totalPayment = monthlyPayment * months;
                double overpayment = totalPayment - amount;

                Console.WriteLine("\n--- РЕЗУЛЬТАТЫ РАСЧЕТА ---");
                Console.WriteLine($"Ежемесячный платеж: {monthlyPayment:F2} руб");
                Console.WriteLine($"Общая сумма выплат: {totalPayment:F2} руб");
                Console.WriteLine($"Переплата по кредиту: {overpayment:F2} руб");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите корректные числовые значения!");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Ошибка: введено слишком большое число!");
            }
        }

        private void ConvertCurrency()
        {
            Console.WriteLine("\n--- КОНВЕРТЕР ВАЛЮТ ---");
            Console.WriteLine("Доступные валюты: RUB, USD, EUR");

            try
            {
                Console.Write("Исходная валюта: ");
                string fromCurrency = Console.ReadLine().ToUpper();

                Console.Write("Целевая валюта: ");
                string toCurrency = Console.ReadLine().ToUpper();

                Console.Write("Сумма для конвертации: ");
                double amount = double.Parse(Console.ReadLine());

                if (!IsValidCurrency(fromCurrency) || !IsValidCurrency(toCurrency))
                {
                    Console.WriteLine("Ошибка: поддерживаются только RUB, USD, EUR!");
                    return;
                }

                if (amount <= 0)
                {
                    Console.WriteLine("Ошибка: сумма должна быть положительной!");
                    return;
                }

                double result;
                if (fromCurrency == toCurrency)
                {
                    result = amount;
                }
                else
                {
                    string rateKey = $"{fromCurrency}_{toCurrency}";
                    if (exchangeRates.ContainsKey(rateKey))
                    {
                        result = amount * exchangeRates[rateKey];
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: курс для данной пары валют не найден!");
                        return;
                    }
                }

                Console.WriteLine("\n--- РЕЗУЛЬТАТ КОНВЕРТАЦИИ ---");
                Console.WriteLine($"{amount:F2} {fromCurrency} = {result:F2} {toCurrency}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите корректную сумму!");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Ошибка: введено слишком большое число!");
            }
        }

        private bool IsValidCurrency(string currency)
        {
            return currency == "RUB" || currency == "USD" || currency == "EUR";
        }

        private void CalculateDeposit()
        {
            Console.WriteLine("\n--- КАЛЬКУЛЯТОР ВКЛАДОВ ---");

            try
            {
                Console.Write("Сумма вклада (руб): ");
                double amount = double.Parse(Console.ReadLine());

                Console.Write("Срок вклада (месяцев): ");
                int months = int.Parse(Console.ReadLine());

                Console.Write("Процентная ставка (% годовых): ");
                double annualRate = double.Parse(Console.ReadLine());

                Console.Write("Тип вклада (1 - с капитализацией, 2 - без капитализации): ");
                string depositType = Console.ReadLine();

                if (amount <= 0 || months <= 0 || annualRate <= 0)
                {
                    Console.WriteLine("Ошибка: все значения должны быть положительными!");
                    return;
                }

                double income, totalAmount;

                if (depositType == "1")
                {
                    // С капитализацией
                    double monthlyRate = annualRate / 12 / 100;
                    totalAmount = amount * Math.Pow(1 + monthlyRate, months);
                    income = totalAmount - amount;
                }
                else if (depositType == "2")
                {
                    // Без капитализации
                    income = amount * annualRate * months / 12 / 100;
                    totalAmount = amount + income;
                }
                else
                {
                    Console.WriteLine("Ошибка: выберите 1 или 2 для типа вклада!");
                    return;
                }

                Console.WriteLine("\n--- РЕЗУЛЬТАТЫ РАСЧЕТА ---");
                Console.WriteLine($"Доход по вкладу: {income:F2} руб");
                Console.WriteLine($"Итоговая сумма: {totalAmount:F2} руб");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите корректные числовые значения!");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Ошибка: введено слишком большое число!");
            }
        }

    }
}