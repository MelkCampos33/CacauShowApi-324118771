# CacauShow API 


## Sobre o Projeto

API RESTful desenvolvida com **ASP.NET Core 8** que simula o controle de qualidade e logística de uma rede de chocolates. O sistema gerencia o ciclo completo: desde a produção de lotes até a venda em franquias, com validações de negócio, controle de capacidade de estoque e relatório regional de vendas.

---

## Tecnologias

| Tecnologia | Versão | Uso |
|---|---|---|
| .NET / ASP.NET Core | 8.0 | Framework principal |
| Entity Framework Core | 8.0 | ORM e migrations |
| SQLite | — | Banco de dados persistente |
| Swashbuckle / Swagger | 6.5 | Documentação e testes da API |

---

## Arquitetura

```
CacauShowApi/
├── Controllers/
│   ├── ProdutosController.cs
│   ├── FranquiasController.cs
│   ├── LotesProducaoController.cs
│   ├── PedidosController.cs
│   └── ChocolateIntelligenceController.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── Produto.cs
│   ├── Franquia.cs
│   ├── LoteProducao.cs
│   └── Pedido.cs
├── Program.cs
├── appsettings.json
└── cacaushow.db           ← gerado automaticamente no primeiro run
```

### Diagrama de Entidades

```
Produto (1) ──────< LoteProducao (N)
   │
   └──────────────< Pedido (N) >────── Franquia (1)
```

### Modelos

**Produto**
```
Id | Nome | Tipo (Gourmet / Linha Regular / Sazonal) | PrecoBase
```

**Franquia**
```
Id | NomeLoja | Cidade | CapacidadeEstoque
```

**LoteProducao**
```
Id | CodigoLote | DataFabricacao | ProdutoId (FK) | Status
```

**Pedido**
```
Id | UnidadeId (FK) | ProdutoId (FK) | Quantidade | ValorTotal
```

---

## Como Rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Passo a passo

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/una-sdm-lista-14.git
cd una-sdm-lista-14

# 2. Restaure as dependências
dotnet restore

# 3. Execute a aplicação
dotnet run
```

> O banco de dados `cacaushow.db` é criado automaticamente na primeira execução.

Acesse o **Swagger UI** em: [http://localhost:5000](http://localhost:5000)

---

##  Endpoints

###  Produtos — `/api/produtos`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/produtos` | Lista todos os produtos |
| `GET` | `/api/produtos/{id}` | Busca produto por ID |
| `POST` | `/api/produtos` | Cria novo produto |
| `PUT` | `/api/produtos/{id}` | Atualiza produto |
| `DELETE` | `/api/produtos/{id}` | Remove produto |

**Exemplo de corpo (POST/PUT):**
```json
{
  "nome": "Ovo LaNut",
  "tipo": "Sazonal",
  "precoBase": 89.90
}
```

---

### 🏪 Franquias — `/api/franquias`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/franquias` | Lista todas as franquias |
| `GET` | `/api/franquias/{id}` | Busca franquia por ID |
| `POST` | `/api/franquias` | Cria nova franquia |
| `PUT` | `/api/franquias/{id}` | Atualiza franquia |
| `DELETE` | `/api/franquias/{id}` | Remove franquia |

**Exemplo de corpo (POST/PUT):**
```json
{
  "nomeLoja": "Cacau Show BH Centro",
  "cidade": "Belo Horizonte",
  "capacidadeEstoque": 500
}
```

---

### Lotes de Produção — `/api/lotesproducao`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/lotesproducao` | Lista todos os lotes |
| `GET` | `/api/lotesproducao/{id}` | Busca lote por ID |
| `POST` | `/api/lotesproducao` | Cria lote (com validações) |
| `PUT` | `/api/lotesproducao/{id}` | Atualiza lote |
| `PATCH` | `/api/lotesproducao/{id}/status` | Atualiza apenas o status |
| `DELETE` | `/api/lotesproducao/{id}` | Remove lote |

**Exemplo de corpo (POST/PUT):**
```json
{
  "codigoLote": "BATCH-2026-X",
  "dataFabricacao": "2026-04-20T08:00:00",
  "produtoId": 1,
  "status": "Em Produção"
}
```

**PATCH — atualizar status:**
```json
"Qualidade Aprovada"
```

**Status válidos:** `Em Produção` · `Qualidade Aprovada` · `Distribuído` · `Descartado`

---

### Pedidos — `/api/pedidos`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/pedidos` | Lista todos os pedidos |
| `GET` | `/api/pedidos/{id}` | Busca pedido por ID |
| `POST` | `/api/pedidos` | Cria pedido (com validações) |
| `PUT` | `/api/pedidos/{id}` | Atualiza pedido |
| `DELETE` | `/api/pedidos/{id}` | Remove pedido |

**Exemplo de corpo (POST/PUT):**
```json
{
  "unidadeId": 1,
  "produtoId": 1,
  "quantidade": 5
}
```

> `ValorTotal` é calculado automaticamente pelo servidor.

---

### Intelligence — `/api/intelligence`

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/intelligence/estoque-regional` | Soma de itens vendidos por cidade |

**Resposta de exemplo:**
```json
[
  { "cidade": "São Paulo", "totalItens": 1500 },
  { "cidade": "Curitiba", "totalItens": 800 },
  { "cidade": "Belo Horizonte", "totalItens": 430 }
]
```

> Esta rota possui um `Thread.Sleep(2000)` intencional simulando latência de servidor central.

---

## ⚙️ Regras de Negócio

### Controle de Qualidade de Lote
- `POST /api/lotesproducao` valida se o `ProdutoId` existe. Se não existir → `404 Not Found`.
- Se `DataFabricacao` for uma data **futura** → `409 Conflict`:
  > *"Lote inválido: Data de fabricação não pode ser maior que a data atual."*

### Sistema de Venda de Franquia
- A soma de `Quantidade` de todos os pedidos de uma mesma franquia não pode ultrapassar `CapacidadeEstoque`. Se ultrapassar → `400 Bad Request`:
  > *"Capacidade logística da loja excedida. Não é possível receber mais produtos."*
- Se o produto for do tipo **Sazonal**, R$ 15,00 de embalagem especial é adicionado automaticamente ao `ValorTotal`.

### Fluxo de Vida do Lote (`PATCH`)
- Um lote com status **`Descartado`** **nunca** pode ser alterado para `Qualidade Aprovada` ou `Distribuído`. Tentativa → `400 Bad Request`:
  > *"Lote descartado não pode ser aprovado ou distribuído."*

---

## Massa de Teste

Siga esta sequência no Swagger para validar todos os cenários:

**1. Criar Franquia com capacidade limitada:**
```json
POST /api/franquias
{ "nomeLoja": "Loja Teste", "cidade": "BH", "capacidadeEstoque": 10 }
```

**2. Criar Produto:**
```json
POST /api/produtos
{ "nome": "Trufa Tradicional", "tipo": "Gourmet", "precoBase": 25.00 }
```

**3. Testar limite de estoque (deve retornar 400):**
```json
POST /api/pedidos
{ "unidadeId": 1, "produtoId": 1, "quantidade": 11 }
```

**4. Testar lote com data futura (deve retornar 409):**
```json
POST /api/lotesproducao
{ "codigoLote": "BATCH-FUTURO", "dataFabricacao": "2099-01-01", "produtoId": 1, "status": "Em Produção" }
```

**5. Testar lote descartado (criar lote, depois tentar PATCH para "Qualidade Aprovada" → 400):**
```json
POST /api/lotesproducao
{ "codigoLote": "BATCH-2026-A", "dataFabricacao": "2026-01-10", "produtoId": 1, "status": "Descartado" }

PATCH /api/lotesproducao/1/status
"Qualidade Aprovada"
```

---

## Desafio de Sistemas Distribuídos

A rota `GET /api/intelligence/estoque-regional` simula uma **consulta a um servidor central de logística** com `Thread.Sleep(2000)`.

### Impacto da Latência de 2 Segundos

Em um sistema **síncrono**, um franqueado fechando um pedido em um tablet esperaria 2 segundos bloqueado apenas para consultar o estoque regional — experiência inaceitável na frente do cliente.

Em pico de Páscoa, com centenas de requisições simultâneas, o pool de threads do servidor se esgotaria rapidamente, causando **timeout em cascata** para todos os usuários.

**Solução ideal:**
- Substituir `Thread.Sleep` por `await Task.Delay` (operação assíncrona, não bloqueia thread)
- Servir dados regionais de um **cache (Redis)** atualizado periodicamente, desacoplando a consulta pesada do fluxo crítico de venda

---

## Pensamento Crítico: Race Conditions

> *"Em grandes datas como a Páscoa, a Cacau Show recebe milhares de pedidos por segundo. Como evitar que dois franqueados 'comprem' o último lote de Trufas disponível simultaneamente?"*

### O Problema

Dois franqueados consultam o estoque ao mesmo tempo → ambos veem 1 unidade disponível → ambos concluem a compra → estoque fica negativo. Isso é uma **Race Condition**.

### Soluções

**Pessimistic Locking**

Ao iniciar a transação de compra, aplica-se um lock exclusivo (`SELECT FOR UPDATE`) no registro de estoque. Nenhuma outra transação pode ler ou escrever aquele registro até o lock ser liberado.

- ✅ Garante consistência total
- ❌ Gera filas e pode causar lentidão em alta concorrência

**Optimistic Locking** *(recomendado para alta concorrência)*

O sistema não trava o registro imediatamente. Cada registro possui um campo `RowVersion`. Ao salvar, verifica-se se o `RowVersion` ainda é o mesmo lido anteriormente. Se outro processo alterou enquanto isso, a transação falha com `DbUpdateConcurrencyException` e o sistema solicita nova tentativa.

- ✅ Eficiente em leitura, ideal para muitos leitores e poucos conflitos
- ❌ Requer lógica de retry no lado da aplicação

No **Entity Framework Core**, o Optimistic Locking é implementado assim:

```csharp
// No modelo:
[Timestamp]
public byte[] RowVersion { get; set; }

// No DbContext:
modelBuilder.Entity<Produto>()
    .Property(p => p.RowVersion)
    .IsRowVersion();
```

**Arquitetura ideal para Páscoa:**

Combinar Optimistic Locking com uma **fila de mensagens** (RabbitMQ / Azure Service Bus) para serializar os pedidos críticos, evitando sobrecarga direta no banco de dados e garantindo que cada lote seja processado exatamente uma vez.

---

<div align="center">

Desenvolvido para **Centro Universitário UNA** — Sistemas Distribuídos e Mobile  
Prof. Daniel Henrique Matos de Paiva

</div>
