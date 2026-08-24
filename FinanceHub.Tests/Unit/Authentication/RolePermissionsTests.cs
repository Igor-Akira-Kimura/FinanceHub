using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Authentication;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Authentication;

public class RolePermissionsTests
{
    [Fact]
    public void User_DevePossuirPermissaoDeVisualizarCarteira()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.User);

        permissions.Should()
            .Contain(
                UsuarioPermission.VisualizarCarteira);
    }

    [Fact]
    public void User_DevePossuirPermissaoDeComprarAtivos()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.User);

        permissions.Should()
            .Contain(
                UsuarioPermission.ComprarAtivos);
    }

    [Fact]
    public void User_DevePossuirPermissaoDeVenderAtivos()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.User);

        permissions.Should()
            .Contain(
                UsuarioPermission.VenderAtivos);
    }

    [Fact]
    public void User_NaoDevePossuirPermissaoDeAdministrarUsuarios()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.User);

        permissions.Should()
            .NotContain(
                UsuarioPermission.AdministrarUsuarios);
    }

    [Fact]
    public void Admin_DevePossuirPermissaoDeVisualizarCarteira()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.Admin);

        permissions.Should()
            .Contain(
                UsuarioPermission.VisualizarCarteira);
    }

    [Fact]
    public void Admin_DevePossuirPermissaoDeComprarAtivos()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.Admin);

        permissions.Should()
            .Contain(
                UsuarioPermission.ComprarAtivos);
    }

    [Fact]
    public void Admin_DevePossuirPermissaoDeVenderAtivos()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.Admin);

        permissions.Should()
            .Contain(
                UsuarioPermission.VenderAtivos);
    }

    [Fact]
    public void Admin_DevePossuirPermissaoDeAdministrarUsuarios()
    {
        var permissions =
            RolePermissions.GetPermissions(
                UsuarioRole.Admin);

        permissions.Should()
            .Contain(
                UsuarioPermission.AdministrarUsuarios);
    }
}