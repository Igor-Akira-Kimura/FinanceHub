using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class UsuarioRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UsuarioRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task AdicionarAsync_DevePersistirUsuario()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new UsuarioRepository(context);

        var usuario = CriarUsuario();

        await repository.AdicionarAsync(usuario);

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Usuarios
                .FindAsync(usuario.Id);

        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be(usuario.Email);
    }

    [Fact]
    public async Task BuscarPorEmailAsync_UsuarioExistente_DeveRetornarUsuario()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = CriarUsuario();

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorEmailAsync(usuario.Email);

        result.Should().NotBeNull();
        result!.Id.Should().Be(usuario.Id);
    }

    [Fact]
    public async Task BuscarPorEmailAsync_EmailInexistente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorEmailAsync(
                "naoexiste@teste.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorEmailAsync_UsuarioInativo_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = CriarUsuario();

        usuario.Desativar();

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorEmailAsync(usuario.Email);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdAsync_UsuarioExistente_DeveRetornarUsuario()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = CriarUsuario();

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorIdAsync(usuario.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(usuario.Id);
    }

    [Fact]
    public async Task BuscarPorIdAsync_UsuarioInexistente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdAsync_UsuarioInativo_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = CriarUsuario();

        usuario.Desativar();

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarPorIdAsync(usuario.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarTodosAsync_DeveRetornarSomenteUsuariosAtivos()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario1 = CriarUsuario();
        var usuario2 = CriarUsuario();

        usuario2.Desativar();

        await context.Usuarios.AddRangeAsync(
            usuario1,
            usuario2);

        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        var result =
            await repository.BuscarTodosAsync();

        result.Should().Contain(x => x.Id == usuario1.Id);
        result.Should().NotContain(x => x.Id == usuario2.Id);
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = CriarUsuario();

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var repository = new UsuarioRepository(context);

        usuario.Atualizar(
            "Nome Atualizado",
            usuario.Email);

        await repository.SalvarAlteracoesAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Usuarios
                .FindAsync(usuario.Id);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Nome Atualizado");
    }

    private static Usuario CriarUsuario()
    {
        return new Usuario(
            "Usuario Teste",
            $"teste{Guid.NewGuid():N}@email.com",
            "hash");
    }
}