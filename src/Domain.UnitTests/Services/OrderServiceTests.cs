using Domain.Services;
using Domain.Entities;
using Xunit;
using System;
using System.Linq;
using FluentAssertions;
using Moq;
using System.Collections.Immutable;

namespace Domain.UnitTests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly MockRepository _mockRepository;

        public OrderServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            // Limpiar el estado estático antes de cada prueba
            var lastOrdersField = typeof(OrderService).GetField("_lastOrders", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Static);
            
            if (lastOrdersField != null)
            {
                lastOrdersField.SetValue(null, System.Collections.Immutable.ImmutableList<Order>.Empty);
            }
            else
            {
                throw new InvalidOperationException("No se pudo encontrar el campo _lastOrders en OrderService");
            }
        }

        public void Dispose()
        {
            _mockRepository.VerifyAll();
        }
        [Fact]
        public void CreateTerribleOrder_WithValidData_ReturnsOrderWithCorrectProperties()
        {
            // Arrange
            var customer = "Test Customer";
            var product = "Test Product";
            var quantity = 5;
            var price = 10.5m;

            // Act
            var order = OrderService.CreateTerribleOrder(customer, product, quantity, price);

            // Assert
            order.Should().NotBeNull();
            order.Id.Should().BeInRange(1, 9999999);
            order.CustomerName.Should().Be(customer);
            order.ProductName.Should().Be(product);
            order.Quantity.Should().Be(quantity);
            order.UnitPrice.Should().Be(price);
            
            // Verificar que el pedido se agregó a LastOrders
            OrderService.LastOrders.Should().Contain(order);
        }

        [Fact]
        public void CreateTerribleOrder_WhenCalled_AddsOrderToLastOrders()
        {
            // Arrange
            var initialCount = OrderService.LastOrders.Count;
            
            // Act
            var order = OrderService.CreateTerribleOrder("Test", "Product", 1, 10m);
            var updatedOrders = OrderService.LastOrders;
            
            // Assert
            updatedOrders.Count.Should().Be(initialCount + 1);
            updatedOrders.Should().Contain(order);
            
            // Verificar que el pedido tiene un ID válido
            order.Id.Should().BeInRange(1, 9999999);
        }

        [Fact]
        public void LastOrders_ReturnsImmutableList()
        {
            // Arrange
            var order1 = OrderService.CreateTerribleOrder("Test1", "Product1", 1, 10m);
            var order2 = OrderService.CreateTerribleOrder("Test2", "Product2", 2, 20m);
            
            // Act
            var orders = OrderService.LastOrders;
            
            // Assert
            orders.Should().NotBeNull();
            orders.Count.Should().BeGreaterThanOrEqualTo(2);
            orders.Should().Contain(order1);
            orders.Should().Contain(order2);
            
            // Verificar que la lista es de solo lectura intentando convertirla a lista y agregar un elemento
            var ordersList = orders.ToList();
            var newOrder = new Order();
            ordersList.Add(newOrder);
            
            // La lista original no debería haber cambiado
            orders.Should().NotContain(newOrder);
        }

        [Fact]
        public void CreateTerribleOrder_WithEmptyCustomer_ThrowsNoException()
        {
            // Act & Assert (no exception should be thrown)
            var order = OrderService.CreateTerribleOrder("", "Product", 1, 10m);
            
            // Assert
            order.Should().NotBeNull();
            order.CustomerName.Should().Be("");
            order.ProductName.Should().Be("Product");
            order.Quantity.Should().Be(1);
            order.UnitPrice.Should().Be(10m);
        }

        [Fact]
        public void CreateTerribleOrder_WithNegativeQuantity_ThrowsNoException()
        {
            // Act & Assert (no exception should be thrown)
            var order = OrderService.CreateTerribleOrder("Test", "Product", -1, 10m);
            
            // Assert
            order.Should().NotBeNull();
            order.Quantity.Should().Be(-1); // Acepta cantidades negativas según la implementación actual
        }

        [Fact]
        public void CreateTerribleOrder_WithNegativePrice_ThrowsNoException()
        {
            // Act & Assert (no exception should be thrown)
            var order = OrderService.CreateTerribleOrder("Test", "Product", 1, -10m);
            
            // Assert
            order.Should().NotBeNull();
            order.UnitPrice.Should().Be(-10m);
        }

        [Fact]
        public void CreateTerribleOrder_WithNullCustomer_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => OrderService.CreateTerribleOrder(null, "Product", 1, 10m);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTerribleOrder_WithNullProduct_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => OrderService.CreateTerribleOrder("Customer", null, 1, 10m);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTerribleOrder_WithMaxIntQuantity_DoesNotThrow()
        {
            // Act & Assert
            var order = OrderService.CreateTerribleOrder("Customer", "Product", int.MaxValue, 1m);
            
            // Assert
            order.Should().NotBeNull();
            order.Quantity.Should().Be(int.MaxValue);
        }

        [Fact]
        public void CreateTerribleOrder_WithMaxDecimalPrice_DoesNotThrow()
        {
            // Act & Assert
            var order = OrderService.CreateTerribleOrder("Customer", "Product", 1, decimal.MaxValue);
            
            // Assert
            order.Should().NotBeNull();
            order.UnitPrice.Should().Be(decimal.MaxValue);
        }

        [Fact]
        public void CreateTerribleOrder_MultipleCalls_IncrementsLastOrders()
        {
            // Arrange
            var initialCount = OrderService.LastOrders.Count;
            
            // Act
            OrderService.CreateTerribleOrder("Test1", "Product1", 1, 10m);
            OrderService.CreateTerribleOrder("Test2", "Product2", 2, 20m);
            
            // Assert
            OrderService.LastOrders.Count.Should().Be(initialCount + 2);
        }

        [Fact]
        public void LastOrders_IsReadOnly_WhenModifiedExternally_ThrowsException()
        {
            // Arrange
            var orders = OrderService.LastOrders;
            
            // Act & Assert - Intentar modificar la colección debería fallar
            orders.Invoking(c => ((IList<Order>)c).Add(new Order()))
                .Should().Throw<NotSupportedException>("La colección LastOrders debería ser de solo lectura");
            
            // Verificar que la colección sigue siendo la misma
            orders.Should().BeSameAs(OrderService.LastOrders);
        }
    }
}
