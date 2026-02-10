using System;
using Xunit;
using Tour_Management;
using System.Web;

namespace Tour_Management.Tests
{
    public class userloginTests
    {
        [Fact]
        public void userlogin_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var instance = new userlogin();

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void userlogin_ShouldInherit_FromSystemWebUIPage()
        {
            // Arrange & Act
            var instance = new userlogin();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(instance);
        }

        [Fact]
        public void userlogin_Namespace_ShouldBeTour_Management()
        {
            // Arrange
            var type = typeof(userlogin);

            // Act
            var namespaceName = type.Namespace;

            // Assert
            Assert.Equal("Tour_Management", namespaceName);
        }

        [Fact]
        public void userlogin_ShouldHave_PageLoadMethod()
        {
            // Arrange
            var type = typeof(userlogin);

            // Act
            var method = type.GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void userlogin_ShouldHave_BtnSubmitMethod()
        {
            // Arrange
            var type = typeof(userlogin);

            // Act
            var method = type.GetMethod("Btn_Submit",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void userlogin_ShouldHave_BtnRegMethod()
        {
            // Arrange
            var type = typeof(userlogin);

            // Act
            var method = type.GetMethod("Btn_reg",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void userlogin_BtnSubmitMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(userlogin);
            var method = type.GetMethod("Btn_Submit",
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
        public void userlogin_BtnRegMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(userlogin);
            var method = type.GetMethod("Btn_reg",
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
        public void userlogin_PageLoadMethod_ShouldHaveCorrectSignature()
        {
            // Arrange
            var type = typeof(userlogin);
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
        public void userlogin_Type_ShouldBePublic()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.True(type.IsPublic);
        }

        [Fact]
        public void userlogin_Type_ShouldNotBeAbstract()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.False(type.IsAbstract);
        }

        [Fact]
        public void userlogin_Type_ShouldNotBeSealed()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.False(type.IsSealed);
        }

        [Fact]
        public void userlogin_ClassType_ShouldBePartial()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.NotNull(type);
            Assert.Equal("userlogin", type.Name);
        }
    }
}
