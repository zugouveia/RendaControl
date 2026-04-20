# RendaControl - Guia rapido para Cursor

## Visao geral
- Projeto dividido em `RendaControl.Front` (HTML/CSS/JS) e `RendaControl.Back` (API .NET).
- O backend usa arquitetura em camadas no estilo DDD: `Domain`, `Application`, `Persistence` e `Web`.
- Em desenvolvimento, o banco atual esta em SQLite local (`rendacontrol-dev.db`) para subir rapido.
- A API expoe dados reais para o front em `http://localhost:5160/api/clientes`.

## Estrutura backend
- `RendaControl.Back/src/RendaControl.Domain`: entidades e regras de negocio (`Cliente`, `Gasto`, `SituacaoCliente`).
- `RendaControl.Back/src/RendaControl.Application`: camada de aplicacao (hoje base para crescer casos de uso).
- `RendaControl.Back/src/RendaControl.Persistence`: `ApplicationDbContext` e mapeamentos EF Core.
- `RendaControl.Back/src/RendaControl.Web`: API, controllers, CORS e seed de dados mockados.

## Por que existe pasta src
- A pasta `src` e um padrao comum para separar codigo-fonte de arquivos de raiz.
- Nao e obrigatoria, mas ajuda organizacao em projetos com multiplos subprojetos.
- O backend nao "depende" de voce digitar `src` manualmente: use o script `RendaControl.Back/iniciar-back.ps1`.

## Como rodar (jeito simples)
- Backend:
  - `powershell -ExecutionPolicy Bypass -File ".\RendaControl.Back\iniciar-back.ps1"`
- Front:
  - `python -m http.server 5500` dentro de `RendaControl.Front`
- URLs:
  - Front: `http://localhost:5500`
  - API: `http://localhost:5160/api/clientes`

## Endpoints principais (padrao atual)
- `GET /api/clientes`
- `GET /api/clientes/resumo`
- `POST /api/clientes`
- `PATCH /api/clientes/{id}/situacao`
- `GET /api/clientes/{id}/gastos`

## Seed e dados de desenvolvimento
- Ao subir em Development, a API recria o banco local e semeia dados mockados.
- Sao criados clientes e gastos de historico automaticamente.
- Isso facilita validacao visual do front sem cadastro manual.

## Padrao de nomenclatura no projeto
- C#:
  - classes, records e metodos publicos em `PascalCase`.
  - nomes de dominio em portugues: `Cliente`, `Gasto`, `Situacao`.
- Frontend/JSON:
  - payloads e respostas em `camelCase` com termos em portugues:
    - `nome`, `telefone`, `endereco`, `situacao`, `quantidadeServicos`.
- Objetivo: manter linguagem do negocio clara para o time.

## Proximos passos recomendados
- Mover DTOs para pasta propria em `Web` para reduzir tamanho de controller.
- Criar migrations formais (em vez de `EnsureDeleted/EnsureCreated`) quando entrar em ambiente persistente.
- Adicionar autenticacao e validacoes de entrada com FluentValidation.
