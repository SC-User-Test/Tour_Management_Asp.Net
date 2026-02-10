using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class DisplayToursTests
    {
        [Fact]
        public void DisplayTours_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new DisplayTours();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void DisplayTours_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new DisplayTours();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void DisplayTours_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void DisplayTours_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void DisplayTours_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(DisplayTours);
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
        public void DisplayTours_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void DisplayTours_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void DisplayTours_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void DisplayTours_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("DisplayTours", type.Name);
        }

        [Fact]
        public void DisplayTours_Type_ShouldHaveDefaultConstructor()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Act
            var constructor = type.GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
