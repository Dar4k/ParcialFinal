using Domain.Entities;
using Xunit;
using Moq;
using System;
using System.IO;
using FluentAssertions;
using System.Globalization;

namespace Domain.UnitTests.Entities
{
    public class OrderTests : IDisposable
    {
        private readonly StringWriter _consoleOutput;
        private readonly TextWriter _originalOutput;

        public OrderTests()
        {
            // Redirigir la salida de consola para las pruebas
            _consoleOutput = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_consoleOutput);
        }

        public void Dispose()
        {
            // Restaurar la salida de consola original
            Console.SetOut(_originalOutput);
            _consoleOutput.Dispose();
        }

        [Fact]
        public void CalculateTotal_WithValidValues_ReturnsCorrectTotal()
        {
            // Arrange
            var order = new Order
            {
                Quantity = 2,
                UnitPrice = 10.5m
            };

            // Act
            var result = order.CalculateTotal();

            // Assert
            result.Should().Be(21m);
        }

        [Fact]
        public void CalculateTotalAndLog_WithValidValues_LogsCorrectMessage()
        {
            // Arrange
            var order = new Order
            {
                Quantity = 2,
                UnitPrice = 10.5m
            };
            var expectedMessage = "Total (maybe): ";

            // Act
            order.CalculateTotalAndLog();

            // Assert
            var logOutput = _consoleOutput.ToString();
            logOutput.Should().Contain(expectedMessage);
        }

        [Theory]
        [InlineData(0, 10.5, 0)]
        [InlineData(2, 0, 0)]
        [InlineData(3, 3.5, 10.5)]
        [InlineData(100, 1.5, 150)]
        [InlineData(2, 0.1, 0.2)]
        public void CalculateTotal_WithDifferentValues_ReturnsCorrectTotal(
            int quantity, decimal unitPrice, decimal expectedTotal)
        {
            // Arrange
            var order = new Order
            {
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            // Act
            var result = order.CalculateTotal();

            // Assert
            result.Should().Be(expectedTotal);
        }

        [Fact]
        public void CalculateTotal_WithDecimalPrecision_HandlesPrecisionCorrectly()
        {
            // Arrange
            var order = new Order
            {
                Quantity = 3,
                UnitPrice = 0.1m
            };
            var expected = 0.3m;

            // Act
            var result = order.CalculateTotal();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void CalculateTotalAndLog_WithNegativeValues_LogsCorrectMessage()
        {
            // Arrange
            var order = new Order
            {
                Quantity = -2,
                UnitPrice = -10.5m
            };
            // Usamos una expresión regular para manejar diferentes formatos de números decimales
            var expectedMessagePattern = @"Total \(maybe\): (21[.,]0|21)";

            // Act
            order.CalculateTotalAndLog();

            // Assert
            var logOutput = _consoleOutput.ToString();
            logOutput.Should().MatchRegex(expectedMessagePattern);
        }

        [Fact]
        public void Properties_WhenSet_CanBeRetrieved()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerName = "Test Customer",
                ProductName = "Test Product",
                Quantity = 2,
                UnitPrice = 10.5m
            };

            // Assert
            order.Id.Should().Be(1);
            order.CustomerName.Should().Be("Test Customer");
            order.ProductName.Should().Be("Test Product");
            order.Quantity.Should().Be(2);
            order.UnitPrice.Should().Be(10.5m);
        }

        [Fact]
        public void CalculateTotal_WithMaxIntQuantity_DoesNotOverflow()
        {
            // Arrange
            var order = new Order
            {
                Quantity = int.MaxValue,
                UnitPrice = 1
            };

            // Act
            var result = order.CalculateTotal();

            // Assert
            result.Should().Be((long)int.MaxValue);
        }

        [Fact]
        public void CalculateTotal_WithMaxDecimalPrice_DoesNotOverflow()
        {
            // Arrange
            var order = new Order
            {
                Quantity = 1,
                UnitPrice = decimal.MaxValue
            };

            // Act
            var result = order.CalculateTotal();

            // Assert
            result.Should().Be(decimal.MaxValue);
        }
    }
}
