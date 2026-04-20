using Microsoft.EntityFrameworkCore;
using RendaControl.Domain.Entities;
using RendaControl.Domain.Enums;
using RendaControl.Persistence.Context;

namespace RendaControl.Web.Infrastructure;

public static class MockDataSeeder
{
    public static async Task SemearTudoAsync(ApplicationDbContext dbContext)
    {
        await SemearClientesAsync(dbContext);
        await SemearGastosAsync(dbContext);
    }

    private static async Task SemearClientesAsync(ApplicationDbContext dbContext)
    {
        var clientesMock = new[]
        {
            CriarCliente("Ana Silva", "(21) 99999-0001", "ana.silva@mock.com", "Rua das Flores, 12 - Rio de Janeiro", 8, SituacaoCliente.Ativo),
            CriarCliente("Joao Costa", "(21) 99999-0002", "joao.costa@mock.com", "Av. Brasil, 100 - Rio de Janeiro", 3, SituacaoCliente.Ativo),
            CriarCliente("Maria Lima", "(21) 99999-0003", "maria.lima@mock.com", "Rua A, 23 - Niteroi", 5, SituacaoCliente.Inadimplente),
            CriarCliente("Pedro Alves", "(21) 99999-0004", "pedro.alves@mock.com", "Rua B, 44 - Niteroi", 1, SituacaoCliente.Novo),
            CriarCliente("Carla Souza", "(21) 99999-0005", "carla.souza@mock.com", "Rua C, 55 - Sao Goncalo", 12, SituacaoCliente.Ativo),
            CriarCliente("Lucas Ferreira", "(21) 99999-0006", "lucas.ferreira@mock.com", "Rua D, 66 - Duque de Caxias", 6, SituacaoCliente.Ativo),
            CriarCliente("Julia Ramos", "(21) 99999-0007", "julia.ramos@mock.com", "Rua E, 77 - Nova Iguacu", 2, SituacaoCliente.Novo),
            CriarCliente("Rafael Mendes", "(21) 99999-0008", "rafael.mendes@mock.com", "Rua F, 88 - Niteroi", 9, SituacaoCliente.Ativo),
            CriarCliente("Paula Martins", "(21) 99999-0009", "paula.martins@mock.com", "Rua G, 99 - Marica", 4, SituacaoCliente.Inadimplente),
            CriarCliente("Bruno Cardoso", "(21) 99999-0010", "bruno.cardoso@mock.com", "Rua H, 10 - Rio de Janeiro", 7, SituacaoCliente.Ativo),
            CriarCliente("Fernanda Rocha", "(21) 99999-0011", "fernanda.rocha@mock.com", "Rua I, 11 - Rio de Janeiro", 3, SituacaoCliente.Novo),
            CriarCliente("Thiago Neves", "(21) 99999-0012", "thiago.neves@mock.com", "Rua J, 12 - Petropolis", 5, SituacaoCliente.Ativo),
            CriarCliente("Camila Borges", "(21) 99999-0013", "camila.borges@mock.com", "Rua K, 13 - Petropolis", 2, SituacaoCliente.Novo),
            CriarCliente("Diego Freitas", "(21) 99999-0014", "diego.freitas@mock.com", "Rua L, 14 - Cabo Frio", 10, SituacaoCliente.Ativo),
            CriarCliente("Mariana Pires", "(21) 99999-0015", "mariana.pires@mock.com", "Rua M, 15 - Buzios", 11, SituacaoCliente.Ativo),
            CriarCliente("Gustavo Rezende", "(21) 99999-0016", "gustavo.rezende@mock.com", "Rua N, 16 - Araruama", 1, SituacaoCliente.Inadimplente),
            CriarCliente("Patricia Melo", "(21) 99999-0017", "patricia.melo@mock.com", "Rua O, 17 - Itaborai", 4, SituacaoCliente.Ativo),
            CriarCliente("Renato Araujo", "(21) 99999-0018", "renato.araujo@mock.com", "Rua P, 18 - Macae", 2, SituacaoCliente.Novo),
            CriarCliente("Aline Nunes", "(21) 99999-0019", "aline.nunes@mock.com", "Rua Q, 19 - Campos", 8, SituacaoCliente.Ativo),
            CriarCliente("Cesar Duarte", "(21) 99999-0020", "cesar.duarte@mock.com", "Rua R, 20 - Campos", 5, SituacaoCliente.Inadimplente),
            CriarCliente("Helena Tavares", "(21) 99999-0021", "helena.tavares@mock.com", "Rua S, 21 - Rio das Ostras", 6, SituacaoCliente.Ativo),
            CriarCliente("Vitor Santana", "(21) 99999-0022", "vitor.santana@mock.com", "Rua T, 22 - Rio das Ostras", 2, SituacaoCliente.Novo),
            CriarCliente("Larissa Coelho", "(21) 99999-0023", "larissa.coelho@mock.com", "Rua U, 23 - Saquarema", 3, SituacaoCliente.Ativo),
            CriarCliente("Igor Teixeira", "(21) 99999-0024", "igor.teixeira@mock.com", "Rua V, 24 - Saquarema", 1, SituacaoCliente.Novo)
        };

        var emailsExistentes = await dbContext.Clientes
            .Select(cliente => cliente.Email)
            .ToHashSetAsync();

        var clientesParaInserir = clientesMock
            .Where(cliente => !emailsExistentes.Contains(cliente.Email))
            .ToList();

        if (clientesParaInserir.Count == 0)
        {
            return;
        }

        await dbContext.Clientes.AddRangeAsync(clientesParaInserir);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SemearGastosAsync(ApplicationDbContext dbContext)
    {
        var clientes = await dbContext.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nome)
            .ToListAsync();

        if (clientes.Count == 0)
        {
            return;
        }

        foreach (var cliente in clientes)
        {
            var possuiGastos = await dbContext.Gastos.AnyAsync(gasto => gasto.ClienteId == cliente.Id);
            if (possuiGastos)
            {
                continue;
            }

            var gastosMock = CriarGastosMockDoCliente(cliente);
            await dbContext.Gastos.AddRangeAsync(gastosMock);
        }

        foreach (var cliente in clientes)
        {
            var quantidadeGastos = await dbContext.Gastos.CountAsync(gasto => gasto.ClienteId == cliente.Id);
            var clienteRastreado = await dbContext.Clientes.FirstAsync(clienteEncontrado => clienteEncontrado.Id == cliente.Id);
            clienteRastreado.DefinirQuantidadeServicos(quantidadeGastos);
        }

        await dbContext.SaveChangesAsync();
    }

    private static IReadOnlyCollection<Gasto> CriarGastosMockDoCliente(Cliente cliente)
    {
        var hoje = DateTime.UtcNow.Date;
        var sementes = BitConverter.ToInt32(cliente.Id.ToByteArray(), 0);
        var random = new Random(Math.Abs(sementes));

        var servicos = new[]
        {
            "Design de logo",
            "Identidade visual completa",
            "Gestao de redes sociais",
            "Pacote de artes para Instagram",
            "Landing page institucional",
            "Criacao de site one-page",
            "Campanha de anuncios",
            "SEO local",
            "Edicao de videos curtos",
            "Consultoria de branding",
            "Email marketing mensal",
            "Automacao de atendimento",
            "Material grafico para evento",
            "Proposta comercial personalizada",
            "Sessao fotografica de produtos"
        };

        var quantidadeGastos = random.Next(2, 7);
        var gastos = new List<Gasto>(quantidadeGastos);
        var descricoesUsadas = new HashSet<int>();

        for (var indice = 0; indice < quantidadeGastos; indice++)
        {
            var indiceDescricao = random.Next(servicos.Length);
            while (descricoesUsadas.Contains(indiceDescricao))
            {
                indiceDescricao = random.Next(servicos.Length);
            }

            descricoesUsadas.Add(indiceDescricao);

            var valorBase = 180 + random.Next(0, 2600);
            var valor = Math.Round(valorBase / 10m, 2) * 10m;
            var diaReferencia = random.Next(-120, 45);
            var dataVencimento = hoje.AddDays(diaReferencia);

            var gasto = new Gasto(cliente.Id, servicos[indiceDescricao], valor, dataVencimento);
            if (dataVencimento <= hoje && random.NextDouble() >= 0.35)
            {
                gasto.MarcarComoPago();
            }

            gastos.Add(gasto);
        }

        return gastos
            .OrderByDescending(gasto => gasto.DataVencimento)
            .ToList();
    }

    private static Cliente CriarCliente(
        string nome,
        string telefone,
        string email,
        string endereco,
        int quantidadeServicos,
        SituacaoCliente situacao)
    {
        var cliente = new Cliente(nome, telefone, email, endereco);
        cliente.DefinirQuantidadeServicos(quantidadeServicos);
        cliente.AtualizarSituacao(situacao);
        return cliente;
    }
}
