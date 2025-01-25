using Microsoft.AspNetCore.Mvc;
using Moq;
using Prog.Controllers;
using Prog.Model;
using Prog.Services;
using Xunit;
using Microsoft.Extensions.Logging;

public class AuthControllerTests
{
    private readonly Mock<IDBManager> _mockDbManager;
    private readonly Mock<IAuthJwt> _mockAuthJwt;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockDbManager = new Mock<IDBManager>();
        _mockAuthJwt = new Mock<IAuthJwt>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _authController = new AuthController(_mockDbManager.Object, _mockAuthJwt.Object, _mockLogger.Object);
    }
    
    [Fact]
    public void GAYTEST()
    {
        Assert.Equal(1,1);
    }
    [Fact]
    public void SignUpValidRequestReturnsOkResult()
    {
        var request = new SignUpRequest { Login = "testuser", Password = "testpass" };
        _mockDbManager.Setup(x => x.AddUser(request.Login, request.Password)).Returns(true);
        var result = _authController.SignUp(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"User {request.Login} registered successfully", okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value));
    }

    [Fact]
    public void SignUpInvalidRequestReturnsBadRequest()
    {
        var request = new SignUpRequest { Login = "testuser", Password = "testpass" };
        _mockDbManager.Setup(x => x.AddUser(request.Login, request.Password)).Returns(false);
        var result = _authController.SignUp(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Failed to register user {request.Login}", badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value));
    }

    // Тесты для метода Login
    [Fact]
    public void Login_ValidCredentials_ReturnsOkResultWithToken()
    {
        var request = new LoginRequest { Login = "testuser", Password = "testpass" };
        _mockDbManager.Setup(x => x.CheckUser(request.Login, request.Password)).Returns(true);
        _mockAuthJwt.Setup(x => x.LogIn(request.Login, request.Password)).Returns("testtoken");
        var result = _authController.Login(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("testtoken", loginResponse.Token);
    }

    [Fact]
    public void LoginInvalidCredentialsReturnsUnauthorizedResult()
    {
        var request = new LoginRequest { Login = "testuser", Password = "testpass" };
        _mockDbManager.Setup(x => x.CheckUser(request.Login, request.Password)).Returns(false);
        var result = _authController.Login(request);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid login or password", unauthorizedResult.Value.GetType().GetProperty("Message").GetValue(unauthorizedResult.Value));
    }

    [Fact]
    public void DeleteUserValidRequestReturnsOkResult()
    {
        var request = new DeleteUserRequest { Login = "testuser" };
        _mockDbManager.Setup(x => x.DeleteUser(request.Login)).Returns(true);
        var result = _authController.DeleteUser(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"User {request.Login} deleted successfully", okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value));
    }

    [Fact]
    public void DeleteUserInvalidRequestReturnsBadRequest()
    {
        var request = new DeleteUserRequest { Login = "testuser" };
        _mockDbManager.Setup(x => x.DeleteUser(request.Login)).Returns(false);
        var result = _authController.DeleteUser(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Failed to delete user {request.Login}", badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value));
    }


    // Тесты для метода ChangePassword
    [Fact]
    public void ChangePasswordValidRequestReturnsOkResult()
    {
        var request = new ChangePasswordRequest
        {
            Login = "testuser",
            OldPassword = "oldpass",
            NewPassword = "newpass"
        };
        _mockDbManager.Setup(x => x.CheckUser(request.Login, request.OldPassword)).Returns(true);
        _mockDbManager.Setup(x => x.UpdatePassword(request.Login, request.NewPassword)).Returns(true);
        var result = _authController.ChangePassword(request);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"Password for user {request.Login} updated successfully", okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value));
    }

    [Fact]
    public void ChangePasswordInvalidOldPasswordReturnsUnauthorizedResult()
    {
        var request = new ChangePasswordRequest
        {
            Login = "testuser",
            OldPassword = "wrongpass",
            NewPassword = "newpass"
        };
        _mockDbManager.Setup(x => x.CheckUser(request.Login, request.OldPassword)).Returns(false);
        var result = _authController.ChangePassword(request);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid login or old password", unauthorizedResult.Value.GetType().GetProperty("Message").GetValue(unauthorizedResult.Value));
    }

    [Fact]
    public void ChangePasswordSameOldAndNewPasswordReturnsBadRequest()
    {
        var request = new ChangePasswordRequest
        {
            Login = "testuser",
            OldPassword = "samepass",
            NewPassword = "samepass"
        };
        var result = _authController.ChangePassword(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("New password must be different from the old password.", badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value));
    }
}
