using FinanceHub.Application.Common;
using FinanceHub.Application.Common.Events;
using FinanceHub.Application.Common.Outbox;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Application.Services;
using FinanceHub.Application.Validators;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace FinanceHub.Tests.Application.Services;

public class CarteiraServiceTests
{
    private readonly Mock<ICarteiraRepository> _carteiraRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IAtivoRepository> _ativoRepository = new();
    private readonly Mock<IPosicaoRepository> _posicaoRepository = new();
    private readonly Mock<IMovimentacaoRepository> _movimentacaoRepository = new();
    private readonly ComprarAtivoRequestValidator _comprarValidator = new();
    private readonly VenderAtivoRequestValidator _venderValidator = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICompraRepository> _compraRepository = new();
    private readonly Mock<IOutboxRepository> _outboxRepository = new();
    private readonly CriarCarteiraRequestValidator _criarCarteiraValidator = new();

    private readonly Guid _usuarioId = Guid.NewGuid();

    private CarteiraService CriarService()
    {
        _currentUserService
            .Setup(x => x.Usuario)
            .Returns(new CurrentUser
            {
                Id = _usuarioId,
                Nome = "Igor",
                Email = "igor@email.com"
            });

        return new CarteiraService(
            _carteiraRepository.Object,
            _usuarioRepository.Object,
            _ativoRepository.Object,
            _posicaoRepository.Object,
            _movimentacaoRepository.Object,
            _comprarValidator,
            _venderValidator,
            _currentUserService.Object,
            _unitOfWork.Object,
            _compraRepository.Object,
            _outboxRepository.Object,
            _criarCarteiraValidator);
    }

    [Fact]
    public async Task CriarAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var request = new CriarCarteiraRequest
        {
            Nome = "Minha Carteira"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();
    }

    [Fact]
    public async Task CriarAsync_UsuarioInativo_DeveLancarException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        usuario.Desativar();

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync(usuario);

        var request = new CriarCarteiraRequest
        {
            Nome = "Minha Carteira"
        };

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioInativoException>();
    }

    [Fact]
    public async Task CriarAsync_CarteiraJaExistente_DeveLancarException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var carteira = new Carteira(
            "Minha Carteira",
            _usuarioId);

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync(usuario);

        _carteiraRepository
            .Setup(x => x.BuscarPorNomeAsync(
                _usuarioId,
                "Minha Carteira"))
            .ReturnsAsync(carteira);

        var request = new CriarCarteiraRequest
        {
            Nome = "Minha Carteira"
        };

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraJaCadastradaException>();
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarCarteiraECommitar()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync(usuario);

        _carteiraRepository
            .Setup(x => x.BuscarPorNomeAsync(
                _usuarioId,
                "Minha Carteira"))
            .ReturnsAsync((Carteira?)null);

        var request = new CriarCarteiraRequest
        {
            Nome = "Minha Carteira"
        };

        var service = CriarService();

        // Act

        var result = await service.CriarAsync(request);

        // Assert

        result.Should().NotBeEmpty();

        _carteiraRepository.Verify(
            x => x.CriarAsync(It.Is<Carteira>(c =>
                c.Nome == request.Nome &&
                c.UsuarioId == _usuarioId)),
            Times.Once);

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ErroAoCriar_DeveFazerRollback()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync(usuario);

        _carteiraRepository
            .Setup(x => x.BuscarPorNomeAsync(
                _usuarioId,
                "Minha Carteira"))
            .ReturnsAsync((Carteira?)null);

        _carteiraRepository
            .Setup(x => x.CriarAsync(It.IsAny<Carteira>()))
            .ThrowsAsync(new Exception("Erro de banco"));

        var request = new CriarCarteiraRequest
        {
            Nome = "Minha Carteira"
        };

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<Exception>();

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Once);
    }

    [Fact]
    public async Task BuscarTodasAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var usuarioId = Guid.NewGuid();

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.BuscarTodasAsync(usuarioId);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();

        _carteiraRepository.Verify(
            x => x.BuscarTodasAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task BuscarTodasAsync_UsuarioInativo_DeveLancarException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        usuario.Desativar();

        var usuarioId = usuario.Id;

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(usuario);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.BuscarTodasAsync(usuarioId);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioInativoException>();
    }

    [Fact]
    public async Task BuscarTodasAsync_UsuarioValido_DeveRetornarCarteiras()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var carteiras = new[]
        {
            new Carteira("Carteira A", usuario.Id),
            new Carteira("Carteira B", usuario.Id)
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        _carteiraRepository
            .Setup(x => x.BuscarTodasAsync(usuario.Id))
            .ReturnsAsync(carteiras);

        var service = CriarService();

        // Act

        var result =
            (await service.BuscarTodasAsync(usuario.Id))
            .ToList();

        // Assert

        result.Should().HaveCount(2);

        result.Select(x => x.Nome)
            .Should()
            .Contain(new[] { "Carteira A", "Carteira B" });
    }

    [Fact]
    public async Task BuscarMinhasAsync_DeveUsarUsuarioAtual()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        _usuarioRepository
            .Setup(x => x.BuscarPorIdAsync(_usuarioId))
            .ReturnsAsync(usuario);

        _carteiraRepository
            .Setup(x => x.BuscarTodasAsync(_usuarioId))
            .ReturnsAsync([]);

        var service = CriarService();

        // Act

        await service.BuscarMinhasAsync();

        // Assert

        _usuarioRepository.Verify(
            x => x.BuscarPorIdAsync(_usuarioId),
            Times.Once);

        _carteiraRepository.Verify(
            x => x.BuscarTodasAsync(_usuarioId),
            Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraNaoEncontrada_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Carteira?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.BuscarPorIdAsync(id);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoEncontradaException>();
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraDeOutroUsuario_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.BuscarPorIdAsync(carteira.Id);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoPertenceAoUsuarioException>();
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraDoUsuario_DeveRetornarResponse()
    {
        // Arrange

        var carteira = new Carteira(
            "Minha Carteira",
            _usuarioId);

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        var service = CriarService();

        // Act

        var result =
            await service.BuscarPorIdAsync(carteira.Id);

        // Assert

        result.Id.Should().Be(carteira.Id);
        result.Nome.Should().Be(carteira.Nome);
    }

    [Fact]
    public async Task ComprarAtivoAsync_CarteiraNaoEncontrada_DeveLancarException()
    {
        // Arrange

        var request = new ComprarAtivoRequest
        {
            CarteiraId = Guid.NewGuid(),
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(request.CarteiraId))
            .ReturnsAsync((Carteira?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.ComprarAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoEncontradaException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_CarteiraDeOutroUsuario_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Outra Carteira",
            Guid.NewGuid());

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.ComprarAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoPertenceAoUsuarioException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(request.AtivoId))
            .ReturnsAsync((Ativo?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.ComprarAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_SaldoInsuficiente_DeveFazerRollback()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            20m);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync((Posicao?)null);

        _carteiraRepository
            .Setup(x => x.DebitarSaldoAsync(
                carteira.Id,
                200m))
            .ReturnsAsync(0);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.ComprarAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<SaldoInsuficienteException>();

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Never);
    }

    [Fact]
    public async Task ComprarAtivoAsync_NovaPosicao_DeveCriarPosicaoMovimentacaoCompraEOutbox()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            20m);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync((Posicao?)null);

        _carteiraRepository
            .Setup(x => x.DebitarSaldoAsync(
                carteira.Id,
                200m))
            .ReturnsAsync(1);

        var service = CriarService();

        // Act

        await service.ComprarAtivoAsync(request);

        // Assert

        _posicaoRepository.Verify(
            x => x.CriarAsync(It.Is<Posicao>(p =>
                p.CarteiraId == carteira.Id &&
                p.AtivoId == ativo.Id &&
                p.Quantidade == 10 &&
                p.PrecoMedio == 20)),
            Times.Once);

        _movimentacaoRepository.Verify(
            x => x.CriarAsync(It.Is<Movimentacao>(m =>
                m.Quantidade == 10 &&
                m.Preco == 20)),
            Times.Once);

        _compraRepository.Verify(
            x => x.CriarAsync(It.Is<Compra>(c =>
                c.CarteiraId == carteira.Id &&
                c.AtivoId == ativo.Id &&
                c.Quantidade == 10 &&
                c.Preco == 20)),
            Times.Once);

        _outboxRepository.Verify(
            x => x.CriarAsync(It.IsAny<OutboxMessage>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }

    [Fact]
    public async Task ComprarAtivoAsync_PosicaoExistente_DeveCriarMovimentacaoEAtualizarPosicao()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            30m);

        var posicao = new Posicao(
            carteira.Id,
            ativo.Id,
            10,
            20);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        _carteiraRepository
            .Setup(x => x.DebitarSaldoAsync(
                carteira.Id,
                150m))
            .ReturnsAsync(1);

        var service = CriarService();

        // Act

        await service.ComprarAtivoAsync(request);

        // Assert

        posicao.Quantidade.Should().Be(15);
        posicao.PrecoMedio.Should().BeApproximately(
            23.3333m,
            0.0001m);

        _movimentacaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Movimentacao>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Once);
    }

    [Fact]
    public async Task VenderAtivoAsync_CarteiraNaoEncontrada_DeveLancarException()
    {
        // Arrange

        var request = new VenderAtivoRequest
        {
            CarteiraId = Guid.NewGuid(),
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(request.CarteiraId))
            .ReturnsAsync((Carteira?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.VenderAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoEncontradaException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_CarteiraDeOutroUsuario_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Outra",
            Guid.NewGuid());

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.VenderAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CarteiraNaoPertenceAoUsuarioException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(request.AtivoId))
            .ReturnsAsync((Ativo?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.VenderAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_PosicaoNaoEncontrada_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            20m);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 10
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync((Posicao?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.VenderAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<PosicaoNaoEncontradaException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_VendaValida_DeveCriarMovimentacaoCreditarSaldoECommitar()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            20m);

        var posicao = new Posicao(
            carteira.Id,
            ativo.Id,
            10,
            20);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        _carteiraRepository
            .Setup(x => x.CreditarSaldoAsync(
                carteira.Id,
                100m))
            .ReturnsAsync(1);

        var service = CriarService();

        // Act

        await service.VenderAtivoAsync(request);

        // Assert

        posicao.Quantidade.Should().Be(5);

        _movimentacaoRepository.Verify(
            x => x.CriarAsync(It.Is<Movimentacao>(m =>
                m.Quantidade == 5 &&
                m.Preco == 20 &&
                m.PosicaoId == posicao.Id)),
            Times.Once);

        _carteiraRepository.Verify(
            x => x.CreditarSaldoAsync(
                carteira.Id,
                100m),
            Times.Once);

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }

    [Fact]
    public async Task VenderAtivoAsync_ErroAoCriarMovimentacao_DeveFazerRollback()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            _usuarioId);

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            20m);

        var posicao = new Posicao(
            carteira.Id,
            ativo.Id,
            10,
            20);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5
        };

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _posicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        _movimentacaoRepository
            .Setup(x => x.CriarAsync(It.IsAny<Movimentacao>()))
            .ThrowsAsync(new Exception("Erro"));

        var service = CriarService();

        // Act

        Func<Task> act = () =>
            service.VenderAtivoAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<Exception>();

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _carteiraRepository.Verify(
            x => x.CreditarSaldoAsync(
                It.IsAny<Guid>(),
                It.IsAny<decimal>()),
            Times.Never);
    }

    [Fact]
    public async Task ComprarAtivoAsync_ComIdempotencyKeyJaProcessada_NaoDeveProcessarNovamente()
    {
        // Arrange
        var carteira = new Carteira(
            "Carteira teste",
            _usuarioId);

        var ativo = new Ativo(
            "PETR4",
            "Petrobras",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            100m);

        var idempotencyKey = "ABC-123";

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5,
            IdempotencyKey = idempotencyKey
        };

        var compraExistente = new Compra(
            carteira.Id,
            ativo.Id,
            5,
            100m,
            idempotencyKey);

        _carteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _compraRepository
            .Setup(x => x.BuscarPorIdempotencyKeyAsync(idempotencyKey))
            .ReturnsAsync(compraExistente);

        var service = CriarService();

        // Act

        await service.ComprarAtivoAsync(request);

        // Assert

        _carteiraRepository.Verify(
            x => x.DebitarSaldoAsync(
                It.IsAny<Guid>(),
                It.IsAny<decimal>()),
            Times.Never);

        _posicaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Posicao>()),
            Times.Never);

        _movimentacaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Movimentacao>()),
            Times.Never);

        _compraRepository.Verify(
            x => x.CriarAsync(It.IsAny<Compra>()),
            Times.Never);

        _outboxRepository.Verify(
            x => x.CriarAsync(It.IsAny<OutboxMessage>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.BeginTransactionAsync(),
            Times.Never);

        _unitOfWork.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWork.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }
}