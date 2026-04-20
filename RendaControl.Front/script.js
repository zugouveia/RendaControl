const API_BASE_URL = "http://localhost:5160/api";

const campoBusca = document.querySelector(".campo-busca");
const tabelaBody = document.querySelector(".tabela-clientes tbody");
const tituloHistorico = document.querySelector(".painel-historico h2");
const historicoLista = document.querySelector(".historico-lista");
const formulario = document.querySelector(".form-cadastro");
const btnCancelar = document.querySelector(".btn-cancelar");

const totalClientesEl = document.querySelector("#resumo-total-clientes");
const clientesAtivosEl = document.querySelector("#resumo-clientes-ativos");
const clientesInadimplentesEl = document.querySelector("#resumo-clientes-inadimplentes");
const clientesNovosEl = document.querySelector("#resumo-clientes-novos");

let clienteSelecionadoId = null;
let debounceId = null;

async function fetchJson(url, options = {}) {
  const response = await fetch(url, options);

  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(errorBody || "Falha ao comunicar com a API.");
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function aplicarClasseSituacao(selectEl, situacao) {
  selectEl.classList.remove("ativo", "inadimplente", "novo");
  selectEl.classList.add(situacao);
}

function formatarMoeda(valor) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(valor);
}

function formatarData(dataIso) {
  return new Intl.DateTimeFormat("pt-BR").format(new Date(dataIso));
}

function renderizarHistorico(gastos) {
  if (!gastos.length) {
    historicoLista.innerHTML = `
      <article class="historico-item">
        <div class="historico-info">
          <h3>Sem historico ainda</h3>
          <time>-</time>
        </div>
        <div class="historico-valores">
          <strong>R$ 0</strong>
          <span class="status-pago">-</span>
        </div>
      </article>
    `;
    return;
  }

  historicoLista.innerHTML = gastos
    .map((gasto) => {
      const statusLabel = gasto.pago ? "Pago" : "Pendente";
      const tituloPagamento = gasto.pago ? "Pagamento confirmado" : "Pagamento pendente";
      const classePagamento = gasto.pago ? "status-pago--pago" : "status-pago--pendente";
      return `
        <article class="historico-item">
          <div class="historico-info">
            <h3>${tituloPagamento}</h3>
            <time>${formatarData(gasto.dataVencimento)}</time>
          </div>
          <div class="historico-valores">
            <strong>${formatarMoeda(gasto.valor)}</strong>
            <span class="status-pago ${classePagamento}">${statusLabel}</span>
          </div>
        </article>
      `;
    })
    .join("");
}

function renderizarClientes(clientes) {
  if (!clientes.length) {
    tabelaBody.innerHTML = `
      <tr>
        <td colspan="4">Nenhum cliente encontrado.</td>
      </tr>
    `;
    tituloHistorico.textContent = "Histórico — selecione um cliente";
    return;
  }

  tabelaBody.innerHTML = clientes
    .map((cliente) => {
      const rowClass = cliente.id === clienteSelecionadoId ? "linha-selecionada" : "";

      return `
        <tr class="${rowClass}" data-client-id="${cliente.id}" data-client-name="${cliente.nome}">
          <td><strong>${cliente.nome}</strong></td>
          <td>${cliente.telefone}</td>
          <td>${cliente.quantidadeServicos}</td>
          <td>
            <select class="status-cliente ${cliente.situacao}" data-client-id="${cliente.id}">
              <option value="novo" ${cliente.situacao === "novo" ? "selected" : ""}>Novo</option>
              <option value="ativo" ${cliente.situacao === "ativo" ? "selected" : ""}>Ativo</option>
              <option value="inadimplente" ${cliente.situacao === "inadimplente" ? "selected" : ""}>Inadimplente</option>
            </select>
          </td>
        </tr>
      `;
    })
    .join("");
}

async function carregarResumo() {
  const resumo = await fetchJson(`${API_BASE_URL}/clientes/resumo`);
  totalClientesEl.textContent = resumo.totalClientes;
  clientesAtivosEl.textContent = resumo.clientesAtivos;
  clientesInadimplentesEl.textContent = `${resumo.clientesInadimplentes} inadimplentes`;
  clientesNovosEl.textContent = resumo.clientesNovos;
}

async function carregarClientes(busca = "") {
  const query = busca ? `?busca=${encodeURIComponent(busca)}` : "";
  const clientes = await fetchJson(`${API_BASE_URL}/clientes${query}`);

  if (clienteSelecionadoId && !clientes.some((cliente) => cliente.id === clienteSelecionadoId)) {
    clienteSelecionadoId = null;
  }

  renderizarClientes(clientes);

  if (!clienteSelecionadoId && clientes.length > 0) {
    clienteSelecionadoId = clientes[0].id;
    tituloHistorico.textContent = `Histórico — ${clientes[0].nome}`;
    renderizarClientes(clientes);
    await carregarGastosDoCliente(clienteSelecionadoId);
  } else if (!clientes.length) {
    renderizarHistorico([]);
  }
}

async function carregarGastosDoCliente(clienteId) {
  const gastos = await fetchJson(`${API_BASE_URL}/clientes/${clienteId}/gastos`);
  renderizarHistorico(gastos);
}

async function atualizarPainel(busca = campoBusca.value.trim()) {
  await Promise.all([carregarClientes(busca), carregarResumo()]);
}

campoBusca.addEventListener("input", () => {
  clearTimeout(debounceId);
  debounceId = setTimeout(() => {
    atualizarPainel(campoBusca.value.trim()).catch((error) => {
      alert(`Erro ao buscar clientes: ${error.message}`);
    });
  }, 250);
});

tabelaBody.addEventListener("click", (event) => {
  const row = event.target.closest("tr[data-client-id]");
  if (!row) return;

  clienteSelecionadoId = row.dataset.clientId;
  tituloHistorico.textContent = `Histórico — ${row.dataset.clientName}`;

  tabelaBody.querySelectorAll("tr[data-client-id]").forEach((tableRow) => {
    tableRow.classList.toggle("linha-selecionada", tableRow.dataset.clientId === clienteSelecionadoId);
  });

  carregarGastosDoCliente(clienteSelecionadoId).catch((error) => {
    alert(`Erro ao carregar historico: ${error.message}`);
  });
});

tabelaBody.addEventListener("change", async (event) => {
  const select = event.target.closest(".status-cliente");
  if (!select) return;

  const clienteId = select.dataset.clientId;
  const situacao = select.value;
  aplicarClasseSituacao(select, situacao);

  try {
    await fetchJson(`${API_BASE_URL}/clientes/${clienteId}/situacao`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ situacao }),
    });
    await carregarResumo();
  } catch (error) {
    alert(`Erro ao atualizar situacao: ${error.message}`);
    await carregarClientes(campoBusca.value.trim());
  }
});

btnCancelar.addEventListener("click", () => {
  formulario.reset();
});

formulario.addEventListener("submit", async (event) => {
  event.preventDefault();

  const payload = {
    nome: document.querySelector('input[placeholder="Nome do cliente"]').value,
    telefone: document.querySelector('input[placeholder="(11) 99999-0000"]').value,
    email: document.querySelector('input[placeholder="email@exemplo.com"]').value,
    endereco: document.querySelector('input[placeholder="Rua, número, cidade"]').value,
  };

  try {
    await fetchJson(`${API_BASE_URL}/clientes`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    formulario.reset();
    campoBusca.value = "";
    clienteSelecionadoId = null;
    await atualizarPainel("");
    alert(`Cliente ${payload.nome} cadastrado com sucesso!`);
  } catch (error) {
    alert(`Erro ao cadastrar cliente: ${error.message}`);
  }
});

atualizarPainel().catch((error) => {
  tabelaBody.innerHTML = `
    <tr>
      <td colspan="4">Nao foi possivel carregar dados da API.</td>
    </tr>
  `;
  alert(`Falha ao carregar dados: ${error.message}`);
});
