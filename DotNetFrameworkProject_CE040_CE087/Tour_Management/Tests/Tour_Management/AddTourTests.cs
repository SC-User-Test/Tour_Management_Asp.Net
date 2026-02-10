using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class AddTourTests
    {
        [Fact]
        public void AddTour_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new AddTour();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void AddTour_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new AddTour();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void AddTour_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(AddTour);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void AddTour_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(AddTour);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void AddTour_ShouldHave_RegisterClickMethod()
        {
            // Arrange
            var type = typeof(AddTour);

            // Act
            var method = type.GetMethod("Register_Click",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void AddTour_RegisterClickMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(AddTour);
            var method = type.GetMethod("Register_Click",
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
        public void AddTour_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(AddTour);
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
        public void AddTour_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void AddTour_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void AddTour_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void AddTour_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("AddTour", type.Name);
        }
    }
}
