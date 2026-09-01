# Demo executável da Fase 2

Esta coleção exercita o fluxo principal da oficina e as duas integrações
externas exigidas na Fase 2: decisão de orçamento por webhook e notificação do
cliente por e-mail.

## Preparação

Na raiz do repositório, execute:

```bash
docker compose \
  -f docker-compose/docker-compose.yml \
  -f docker-compose/docker-compose.override.yml \
  up -d --build db mailpit techchallenge.api
```

Serviços da demonstração:

| Serviço | Endereço |
| --- | --- |
| API e Swagger | `http://localhost:8080` |
| Mailpit | `http://localhost:8025` |
| PostgreSQL | `localhost:5432` |

Os dados fictícios são criados pelo seed de desenvolvimento. Todos os usuários
abaixo usam a senha `Demo@123`:

| Perfil | Login |
| --- | --- |
| Administrador | `admin.demo` |
| Vendedor | `vendedor.demo` |
| Mecânico | `mecanico.demo` |

A chave local padrão do webhook é `demo-integration-key-change-me`. Ela existe
somente para a demonstração e deve ser sobrescrita pela variável
`EXTERNAL_INTEGRATION_API_KEY` fora deste ambiente.

## Ordem sugerida

1. Execute `00-auth.http` para salvar os tokens.
2. Execute `01-aprovacao-externa.http` do início ao fim.
3. Abra o Mailpit e mostre os e-mails das mudanças de status.
4. Execute `02-recusa-externa.http` para demonstrar o caminho alternativo.
5. Use `03-metricas-e-acompanhamento.http` para fechar a demonstração.

Os arquivos usam scripts compatíveis com o cliente HTTP da JetBrains para
guardar tokens, IDs e identificadores de evento. Em outro cliente, copie os
valores retornados para as variáveis no topo de cada arquivo.

## O que destacar no vídeo

- o webhook não usa o JWT interno; ele exige `X-Integration-Key`;
- repetir o mesmo evento retorna sucesso com `duplicado: true` e não muda a OS;
- reutilizar o identificador com outro conteúdo retorna `409 Conflict`;
- a decisão, a mudança da OS e a mensagem de outbox são persistidas juntas;
- o worker envia o e-mail pelo SMTP local e registra a entrega na outbox;
- se o SMTP estiver indisponível, a mensagem permanece pendente e recebe nova
  tentativa com espera progressiva.
