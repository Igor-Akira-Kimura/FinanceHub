using Castle.Core.Logging;
using FinanceHub.Application.Common;
using FinanceHub.Application.Interfaces.Cache;
using FinanceHub.Application.Interfaces.Observability;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Application.Services;
using FinanceHub.Domain.Entities;
using FinanceHub.Tests.Builders;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceHub.Tests.Fixtures;

public class CarteiraServiceFixture
{
    public Mock<ICarteiraRepository> CarteiraRepository { get; } = new();

    public Mock<IUsuarioRepository> UsuarioRepository { get; } = new();

    public Mock<IAtivoRepository> AtivoRepository { get; } = new();

    public Mock<IPosicaoRepository> PosicaoRepository { get; } = new();

    public Mock<IMovimentacaoRepository> MovimentacaoRepository { get; } = new();

    public Mock<ICurrentUserService> CurrentUserService { get; } = new();

    public Mock<IValidator<ComprarAtivoRequest>> ComprarValidator { get; } = new();

    public Mock<IValidator<VenderAtivoRequest>> VenderValidator { get; } = new();

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public Mock<ICompraRepository> CompraRepository { get; } = new();

    public Mock<IOutboxRepository> OutboxRepository { get; } = new();

    public Mock<IValidator<CriarCarteiraRequest>> CriarCarteiraValidator { get; } = new();

    public Mock<ICacheService> CacheService { get; } = new();

    public Mock<ILogger<CarteiraService>> _logger = new();

    public Mock<ICompraMetrics> _compraMetrics = new();

    public CarteiraService Service { get; }

    public CarteiraServiceFixture()
    {
        Service = new CarteiraService(
            CarteiraRepository.Object,
            UsuarioRepository.Object,
            AtivoRepository.Object,
            PosicaoRepository.Object,
            MovimentacaoRepository.Object,
            ComprarValidator.Object,
            VenderValidator.Object,
            CurrentUserService.Object,
            UnitOfWork.Object,
            CompraRepository.Object,
            OutboxRepository.Object,
            CriarCarteiraValidator.Object,
            CacheService.Object,
            _logger.Object,
            _compraMetrics.Object);

        ComprarValidator
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<ComprarAtivoRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        VenderValidator
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<VenderAtivoRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    public void ConfigurarUsuarioLogado(Usuario usuario)
    {
        CurrentUserService
            .Setup(x => x.Usuario)
            .Returns(new CurrentUser
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            });
    }

    public Usuario ConfigurarUsuarioValido()
    {
        var usuario = new UsuarioBuilder().Build();

        ConfigurarUsuarioLogado(usuario);

        UsuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        return usuario;
    }

    public void ConfigurarCarteiraNaoExistente(Guid usuarioId)
    {
        CarteiraRepository
            .Setup(x => x.BuscarPorNomeAsync(
                usuarioId,
                It.IsAny<string>()))
            .ReturnsAsync((Carteira?)null);
    }

    public void ConfigurarCarteiraExistente(Guid usuarioId)
    {
        CarteiraRepository
            .Setup(x => x.BuscarPorNomeAsync(
                usuarioId,
                It.IsAny<string>()))
            .ReturnsAsync(
                new CarteiraBuilder()
                    .ComUsuario(usuarioId)
                    .Build());
    }

    public Usuario ConfigurarUsuarioInativo()
    {
        var usuario = new UsuarioBuilder().Build();

        usuario.Desativar();

        ConfigurarUsuarioLogado(usuario);

        UsuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        return usuario;
    }
}