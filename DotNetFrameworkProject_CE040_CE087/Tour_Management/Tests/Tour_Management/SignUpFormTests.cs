using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class SignUpFormTests
    {
        [Fact]
        public void SignUpForm_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new SignUpForm();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void SignUpForm_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new SignUpForm();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void SignUpForm_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void SignUpForm_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void SignUpForm_ShouldHave_RegisterClickMethod()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Act
            var method = type.GetMethod("Register_Click",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void SignUpForm_RegisterClickMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(SignUpForm);
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
        public void SignUpForm_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(SignUpForm);
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
        public void SignUpForm_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void SignUpForm_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void SignUpForm_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void SignUpForm_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("SignUpForm", type.Name);
        }
    }
}
