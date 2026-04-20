using RendaControl.Domain.Enums;

namespace RendaControl.Domain.Entities;

public sealed class Cliente
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public int QuantidadeServicos { get; private set; }
    public SituacaoCliente Situacao { get; private set; } = SituacaoCliente.Novo;
    public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;

    private Cliente()
    {
    }

    public Cliente(string nome, string telefone, string email, string endereco)
    {
        AtualizarContato(nome, telefone, email, endereco);
    }

    public void AtualizarContato(string nome, string telefone, string email, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do cliente e obrigatorio.", nameof(nome));
        }

        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new ArgumentException("O telefone do cliente e obrigatorio.", nameof(telefone));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail do cliente e obrigatorio.", nameof(email));
        }

        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Email = email.Trim();
        Endereco = endereco.Trim();
    }

    public void AtualizarSituacao(SituacaoCliente situacao)
    {
        Situacao = situacao;
    }

    public void DefinirQuantidadeServicos(int quantidadeServicos)
    {
        if (quantidadeServicos < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidadeServicos), "A quantidade de servicos nao pode ser negativa.");
        }

        QuantidadeServicos = quantidadeServicos;
    }
}
