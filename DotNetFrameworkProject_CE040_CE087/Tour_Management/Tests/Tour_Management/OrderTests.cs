using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new Order();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void Order_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new Order();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void Order_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(Order);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void Order_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(Order);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void Order_ShouldHave_BtnClickMethod()
        {
            // Arrange
            var type = typeof(Order);

            // Act
            var method = type.GetMethod("btn_click",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void Order_BtnClickMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(Order);
            var method = type.GetMethod("btn_click",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Act
            var parameters = method?.GetParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Equal(2, parameters.Length);
            Assert.Equal(typeof(object), parameters[0].ParameterType);
            Assert.Equal(typeof(EventArgs), parameters[1].ParameterType);
        }

        [Fact]
        public void Order_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(Order);
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Act
            var parameters = method?.GetParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Equal(2, parameters.Length);
            Assert.Equal(typeof(object), parameters[0].ParameterType);
            Assert.Equal(typeof(EventArgs), parameters[1].ParameterType);
        }

        [Fact]
        public void Order_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void Order_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void Order_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void Order_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("Order", type.Name);
        }
    }
}
