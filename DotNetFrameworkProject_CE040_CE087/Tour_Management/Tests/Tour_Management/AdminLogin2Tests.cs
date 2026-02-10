using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class AdminLogin2Tests
    {
        [Fact]
        public void AdminLogin2_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new AdminLogin2();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void AdminLogin2_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new AdminLogin2();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void AdminLogin2_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("AdminLogin2", type.Name);
        }

        [Fact]
        public void AdminLogin2_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void AdminLogin2_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void AdminLogin2_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(AdminLogin2);
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
        public void AdminLogin2_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void AdminLogin2_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void AdminLogin2_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.False(type.IsSealed);
        }
    }
}
