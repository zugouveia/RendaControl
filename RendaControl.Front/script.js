const campoBusca = document.querySelector(".campo-busca");
const linhasClientes = document.querySelectorAll(".tabela-clientes tbody tr");
const statusCliente = document.querySelectorAll(".status-cliente");

const tituloHistorico = document.querySelector(".painel-historico h2");

const btnCancelar = document.querySelector(".btn-cancelar");
const formCadastro = document.querySelectorAll(".form-cadastro input");

const formulario = document.querySelector(".form-cadastro");
const btnSalvar = document.querySelector(".btn-salvar");

// Buscar Client
campoBusca.addEventListener("input", () => {
  const termoBusca = campoBusca.value.toLowerCase();

  filtrarClientes(termoBusca);
});

function filtrarClientes(termo) {
  linhasClientes.forEach((linha) => {
    const nomeCliente = linha.querySelector("strong").innerText.toLowerCase();

    if (nomeCliente.includes(termo)) {
      linha.style.display = "";
    } else {
      linha.style.display = "none";
    }
  });
}

// Selecionar nome e status
linhasClientes.forEach((linha) => {
  linha.addEventListener("click", () => {
    linhasClientes.forEach((l) => l.classList.remove("linha-selecionada"));
    linha.classList.add("linha-selecionada");

    const nomeSelecionado = linha.querySelector("td strong").innerText;

    tituloHistorico.innerText = `Histórico — ${nomeSelecionado}`;
  });
});

statusCliente.forEach((select) => {
  select.addEventListener("change", () => {
    const valorSorteado = select.value;

    select.classList.remove("ativo", "inadimplente", "novo");

    if (valorSorteado === "ativo") {
      select.classList.add("ativo");
    } else if (valorSorteado === "inadimplente") {
      select.classList.add("inadimplente");
    } else if (valorSorteado === "novo") {
      select.classList.add("novo");
    }
  });
});

// Cadastro de Cliente
// Cancelar
btnCancelar.addEventListener("click", () => {
  formCadastro.forEach((input) => {
    input.value = "";
  });
});

//Salvar
formulario.addEventListener("submit", (evento) => {
  evento.preventDefault();

  const novoCliente = {
    nome: document.querySelector('input[placeholder="Nome do cliente"]').value,
    telefone: document.querySelector('input[placeholder="(11) 99999-0000"]')
      .value,
    email: document.querySelector('input[placeholder="email@exemplo.com"]')
      .value,
    endereco: document.querySelector('input[placeholder="Rua, número, cidade"]')
      .value,
    status: "Novo",
  };

  // Teste
  console.log("Dados:");
  console.table(novoCliente);
  alert(`Cliente ${novoCliente.nome} cadastrado com sucesso!`);

  formulario.reset();
});
