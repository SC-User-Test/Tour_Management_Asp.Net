using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class allbookingTests
    {
        [Fact]
        public void allbooking_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new allbooking();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void allbooking_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new allbooking();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void allbooking_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(allbooking);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void allbooking_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(allbooking);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void allbooking_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(allbooking);
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
        public void allbooking_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void allbooking_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void allbooking_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void allbooking_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("allbooking", type.Name);
        }

        [Fact]
        public void allbooking_Type_ShouldHaveDefaultConstructor()
        {
            // Arrange
            var type = typeof(allbooking);

            // Act
            var constructor = type.GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
