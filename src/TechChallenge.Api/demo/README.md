# Demo do fluxo completo da OS

Roteiro para demonstrar a aplicação usando arquivos `.http`.

## Preparar

1. Suba a API via docker-compose:

```bash
docker compose -f docker-compose/docker-compose.yml -f docker-compose/docker-compose.override.yml up --build
```

2. A API deve ficar em `http://localhost:8080`.
3. Confirme que `Seed:DemoData` está `true` em `appsettings.Development.json`.
4. Execute os arquivos na ordem numérica.
5. Observe o terminal da API durante os passos de criação de OS e falta de estoque. As notificações simuladas aparecem como logs `Information`.

Os arquivos usam scripts para salvar tokens e IDs em variáveis globais. Se seu editor não executar scripts de `.http`, copie manualmente o `accessToken` dos logins e o ID retornado na criação da OS para as variáveis indicadas.

## Credenciais demo

Todos usam senha `Demo@123`.

| Perfil | Login |
| --- | --- |
| Administrador | `admin.demo` |
| Vendedor | `vendedor.demo` |
| Mecanico | `mecanico.demo` |

## IDs fixos do seed

| Recurso | ID |
| --- | --- |
| Cliente | `aaaaaaaa-0000-0000-0000-000000000001` |
| Vendedor | `aaaaaaaa-0000-0000-0000-000000000003` |
| Mecanico | `aaaaaaaa-0000-0000-0000-000000000005` |
| Administrador | `aaaaaaaa-0000-0000-0000-000000000007` |
| Veiculo | `aaaaaaaa-0000-0000-0000-000000000008` |
| Servico revisao | `aaaaaaaa-0000-0000-0000-000000000009` |
| Servico diagnostico | `aaaaaaaa-0000-0000-0000-000000000010` |
| Produto com estoque | `aaaaaaaa-0000-0000-0000-000000000011` |
| Produto com pouco estoque | `aaaaaaaa-0000-0000-0000-000000000012` |

## Fluxos

- `00-auth.http`: autentica admin, vendedor e mecanico.
- `01-cadastros-demo.http`: confere seed e inventario.
- `02-os-fluxo-completo.http`: executa caminho feliz da OS.
- `03-os-alerta-estoque.http`: força falta de estoque e gera alertas via logger.
- `04-metricas-e-acompanhamento.http`: consulta acompanhamento publico e metricas.

No fluxo completo, copie o `codigoAcompanhamento` retornado no passo 10 para a variavel `codigoAcompanhamento` no arquivo `04`.
