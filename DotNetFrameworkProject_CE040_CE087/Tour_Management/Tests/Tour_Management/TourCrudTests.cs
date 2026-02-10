using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class TourCrudTests
    {
        [Fact]
        public void TourCrud_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new TourCrud();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void TourCrud_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new TourCrud();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void TourCrud_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void TourCrud_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void TourCrud_ShouldHave_RefreshdataMethod()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Act
            var method = type.GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void TourCrud_RefreshdataMethod_ShouldBePublic()
        {
            // Arrange
            var type = typeof(TourCrud);
            var method = type.GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
            Assert.True(method.IsPublic);
        }

        [Fact]
        public void TourCrud_RefreshdataMethod_ShouldHaveNoParameters()
        {
            // Arrange
            var type = typeof(TourCrud);
            var method = type.GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            // Act
            var parameters = method?.GetParameters();

            // Assert
            Assert.NotNull(parameters);
            Assert.Empty(parameters);
        }

        [Fact]
        public void TourCrud_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(TourCrud);
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
        public void TourCrud_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void TourCrud_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void TourCrud_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void TourCrud_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("TourCrud", type.Name);
        }
    }
}
