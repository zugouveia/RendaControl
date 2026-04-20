namespace RendaControl.Domain.Entities;

public sealed class Gasto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClienteId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public bool Pago { get; private set; }

    private Gasto()
    {
    }

    public Gasto(Guid clienteId, string descricao, decimal valor, DateTime dataVencimento)
    {
        DefinirCliente(clienteId);
        Atualizar(descricao, valor, dataVencimento);
    }

    public void Atualizar(string descricao, decimal valor, DateTime dataVencimento)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descricao da despesa e obrigatoria.", nameof(descricao));
        }

        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da despesa deve ser maior que zero.");
        }

        Descricao = descricao.Trim();
        Valor = valor;
        DataVencimento = dataVencimento;
    }

    public void MarcarComoPago()
    {
        Pago = true;
    }

    private void DefinirCliente(Guid clienteId)
    {
        if (clienteId == Guid.Empty)
        {
            throw new ArgumentException("O cliente da despesa e obrigatorio.", nameof(clienteId));
        }

        ClienteId = clienteId;
    }
}
