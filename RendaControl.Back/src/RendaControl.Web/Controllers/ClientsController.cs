using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RendaControl.Domain.Entities;
using RendaControl.Domain.Enums;
using RendaControl.Persistence.Context;

namespace RendaControl.Web.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ClientesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClienteResposta>>> ObterTodos([FromQuery] string? busca = null)
    {
        IQueryable<Cliente> consulta = _dbContext.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var buscaNormalizada = busca.Trim().ToLower();
            consulta = consulta.Where(cliente => cliente.Nome.ToLower().Contains(buscaNormalizada));
        }

        var clientes = await consulta
            .OrderBy(cliente => cliente.Nome)
            .Select(cliente => new ClienteResposta(
                cliente.Id,
                cliente.Nome,
                cliente.Telefone,
                cliente.Email,
                cliente.Endereco,
                cliente.QuantidadeServicos,
                ConverterSituacaoParaApi(cliente.Situacao),
                cliente.DataCriacao))
            .ToListAsync();

        return Ok(clientes);
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<ResumoClientesResposta>> ObterResumo()
    {
        var totalClientes = await _dbContext.Clientes.CountAsync();
        var clientesAtivos = await _dbContext.Clientes.CountAsync(cliente => cliente.Situacao == SituacaoCliente.Ativo);
        var clientesInadimplentes = await _dbContext.Clientes.CountAsync(cliente => cliente.Situacao == SituacaoCliente.Inadimplente);
        var clientesNovos = await _dbContext.Clientes.CountAsync(cliente => cliente.Situacao == SituacaoCliente.Novo);

        return Ok(new ResumoClientesResposta(totalClientes, clientesAtivos, clientesInadimplentes, clientesNovos));
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResposta>> Criar([FromBody] CriarClienteRequisicao requisicao)
    {
        var cliente = new Cliente(requisicao.Nome, requisicao.Telefone, requisicao.Email, requisicao.Endereco);
        _dbContext.Clientes.Add(cliente);
        await _dbContext.SaveChangesAsync();

        var resposta = new ClienteResposta(
            cliente.Id,
            cliente.Nome,
            cliente.Telefone,
            cliente.Email,
            cliente.Endereco,
            cliente.QuantidadeServicos,
            ConverterSituacaoParaApi(cliente.Situacao),
            cliente.DataCriacao);

        return CreatedAtAction(nameof(ObterTodos), new { id = cliente.Id }, resposta);
    }

    [HttpPatch("{id:guid}/situacao")]
    public async Task<ActionResult<ClienteResposta>> AtualizarSituacao(Guid id, [FromBody] AtualizarSituacaoClienteRequisicao requisicao)
    {
        var cliente = await _dbContext.Clientes.FirstOrDefaultAsync(clienteEncontrado => clienteEncontrado.Id == id);

        if (cliente is null)
        {
            return NotFound();
        }

        if (!TentarConverterSituacao(requisicao.Situacao, out var situacaoConvertida))
        {
            return BadRequest("Situacao invalida. Use: novo, ativo, inadimplente.");
        }

        cliente.AtualizarSituacao(situacaoConvertida);
        await _dbContext.SaveChangesAsync();

        return Ok(new ClienteResposta(
            cliente.Id,
            cliente.Nome,
            cliente.Telefone,
            cliente.Email,
            cliente.Endereco,
            cliente.QuantidadeServicos,
            ConverterSituacaoParaApi(cliente.Situacao),
            cliente.DataCriacao));
    }

    [HttpGet("{id:guid}/gastos")]
    public async Task<ActionResult<IReadOnlyCollection<GastoClienteResposta>>> ObterGastos(Guid id)
    {
        var clienteExiste = await _dbContext.Clientes.AnyAsync(cliente => cliente.Id == id);
        if (!clienteExiste)
        {
            return NotFound();
        }

        var gastos = await _dbContext.Gastos
            .AsNoTracking()
            .Where(gasto => gasto.ClienteId == id)
            .OrderByDescending(gasto => gasto.DataVencimento)
            .Select(gasto => new GastoClienteResposta(
                gasto.Id,
                gasto.Descricao,
                gasto.Valor,
                gasto.DataVencimento,
                gasto.Pago))
            .ToListAsync();

        return Ok(gastos);
    }

    private static bool TentarConverterSituacao(string situacaoTexto, out SituacaoCliente situacao)
    {
        switch (situacaoTexto.Trim().ToLowerInvariant())
        {
            case "novo":
                situacao = SituacaoCliente.Novo;
                return true;
            case "ativo":
                situacao = SituacaoCliente.Ativo;
                return true;
            case "inadimplente":
                situacao = SituacaoCliente.Inadimplente;
                return true;
            default:
                situacao = SituacaoCliente.Novo;
                return false;
        }
    }

    private static string ConverterSituacaoParaApi(SituacaoCliente situacao)
    {
        return situacao switch
        {
            SituacaoCliente.Novo => "novo",
            SituacaoCliente.Ativo => "ativo",
            SituacaoCliente.Inadimplente => "inadimplente",
            _ => "novo"
        };
    }
}

public sealed record CriarClienteRequisicao(string Nome, string Telefone, string Email, string Endereco);

public sealed record AtualizarSituacaoClienteRequisicao(string Situacao);

public sealed record ClienteResposta(
    Guid Id,
    string Nome,
    string Telefone,
    string Email,
    string Endereco,
    int QuantidadeServicos,
    string Situacao,
    DateTime DataCriacao);

public sealed record ResumoClientesResposta(int TotalClientes, int ClientesAtivos, int ClientesInadimplentes, int ClientesNovos);

public sealed record GastoClienteResposta(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateTime DataVencimento,
    bool Pago);
