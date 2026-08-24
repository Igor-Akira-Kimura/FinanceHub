using FinanceHub.Domain.Enums;

namespace FinanceHub.Infrastructure.Authentication;

public static class RolePermissions
{
    public static IReadOnlyCollection<UsuarioPermission> GetPermissions(
        UsuarioRole role)
    {
        return role switch
        {
            UsuarioRole.Admin =>
            [
                UsuarioPermission.VisualizarCarteira,
                UsuarioPermission.ComprarAtivos,
                UsuarioPermission.VenderAtivos,
                UsuarioPermission.AdministrarUsuarios
            ],

            UsuarioRole.User =>
            [
                UsuarioPermission.VisualizarCarteira,
                UsuarioPermission.ComprarAtivos,
                UsuarioPermission.VenderAtivos
            ],

            _ => []
        };
    }
}