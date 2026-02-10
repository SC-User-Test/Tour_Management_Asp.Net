using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class mybookingTests
    {
        [Fact]
        public void mybooking_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new mybooking();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void mybooking_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new mybooking();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void mybooking_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(mybooking);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void mybooking_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(mybooking);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void mybooking_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(mybooking);
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
        public void mybooking_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void mybooking_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void mybooking_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void mybooking_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("mybooking", type.Name);
        }

        [Fact]
        public void mybooking_Type_ShouldHaveDefaultConstructor()
        {
            // Arrange
            var type = typeof(mybooking);

            // Act
            var constructor = type.GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
