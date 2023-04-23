using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RockPaperScissorsAPI.Controllers;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;
using RockPaperScissorsAPI.Validation;
using System.Security.Claims;

namespace RockPaperScissorsAPITests;

public class AuthControllerTests : ControllerBase
{
    private readonly Mock<IUserService> mock;
    private readonly AuthController controller;
    private readonly IUserService userService;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;


    public AuthControllerTests()
    {
        mock = new Mock<IUserService>();
        controller = new AuthController(mock.Object, configuration: null, httpContextAccessor: null);
    }

}