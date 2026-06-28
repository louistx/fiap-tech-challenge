# Requisitos Funcionais

## RF01 - Cadastro do cliente
- O sistema deve permitir que administradores e vendedores cadastrem, consultem, atualizem e excluam clientes.
- O cadastro deve conter nome, RG, CPF e endereço.
- O CPF deve ser válido, normalizado e único.

## RF02 - Cadastro do veículo
- O sistema deve permitir que administradores e vendedores cadastrem, consultem, atualizem e excluam veículos.
- O cadastro deve conter tipo, placa, modelo, marca, cor, ano, quilometragem, valor e cliente responsável.
- Os tipos aceitos são carro, moto e caminhão.
- A placa deve seguir o padrão antigo ou Mercosul, ser normalizada e única.

## RF03 - Cadastro de funcionários
- O sistema deve permitir que o administrador cadastre, consulte, atualize e exclua funcionários.
- O cadastro deve conter nome, CPF, RG, endereço e cargo.
- Os cargos aceitos são vendedor, mecânico e administrador.

## RF04 - Cadastro de OS
- O sistema deve permitir que administradores e vendedores criem uma OS.
- A OS deve conter veículo, cliente responsável, funcionário responsável e relato inicial.
- A OS deve ser criada diretamente com o estado `Recebida`.

## RF05 - Cadastro de inventário
- O sistema deve permitir que administradores e vendedores cadastrem, consultem, atualizem e excluam produtos do inventário.

## RF06 - Cadastro de serviços
- O sistema deve permitir que administradores e vendedores cadastrem, consultem, atualizem e excluam os serviços oferecidos pela oficina.

## RF07 - Listagem de inventário
- O sistema deve permitir que usuários autenticados listem e consultem produtos do inventário.

## RF08 - Listagem de OS
- O sistema deve permitir que usuários autenticados listem todas as OS.
- A listagem deve aceitar filtro opcional por estado.
- O sistema deve permitir a consulta de uma OS pelo identificador.

## RF09 - Listagem de OS para a oficina
- O sistema deve fornecer a listagem das OS em diagnóstico para exibição na oficina.
- A resposta deve conter o identificador da OS, a placa do veículo, o mecânico atribuído e o relato inicial.

## RF10 - Atribuição de OS
- O sistema deve permitir que um mecânico assuma uma OS no estado `Recebida`.
- O mecânico pode ter apenas uma OS ativa por vez.
- Ao ser atribuída, a OS deve passar para `EmDiagnostico`.

## RF11 - Registro de diagnóstico
- O mecânico deve poder associar serviços e produtos a uma OS em diagnóstico.
- O preço vigente de cada serviço e produto deve ser registrado no item da OS.

## RF12 - Envio do orçamento
- Administradores, vendedores e mecânicos devem poder enviar o orçamento de uma OS em diagnóstico.
- A OS deve possuir ao menos um serviço ou produto.
- O sistema deve calcular o total considerando valores, descontos e acréscimos.
- Após o envio, a OS deve passar para `AguardandoAprovacao`.

## RF13 - Decisão do orçamento
- Um usuário autenticado deve poder aprovar ou reprovar o orçamento.
- A aprovação deve mover a OS de `AguardandoAprovacao` para `EmExecucao`.
- A reprovação deve mover a OS para `Reprovada`.
- Uma OS reprovada deve poder retornar para `EmDiagnostico` para revisão.

## RF14 - Finalização e entrega
- O mecânico deve poder mover uma OS de `EmExecucao` para `Finalizada`.
- A finalização deve registrar a data correspondente.
- Administradores e vendedores devem poder registrar a entrega, movendo a OS para `Entregue`.

## RF15 - Cancelamento
- O administrador deve poder cancelar uma OS ainda não finalizada ou entregue.
- O estado `Cancelada` deve ser terminal.

## RF16 - Verificação de estoque
- Antes do envio para aprovação, o sistema deve verificar se os produtos necessários estão disponíveis.
- Quando não houver estoque, o sistema deve notificar o administrador e o mecânico.
- Este requisito ainda não está implementado.

## RF17 - Autenticação e autorização
- O sistema deve autenticar usuários com JWT e permitir renovação por refresh token rotativo.
- O sistema deve permitir troca de senha, logout e revogação de refresh tokens.
- O acesso às operações deve respeitar os perfis administrador, vendedor e mecânico.
- Por padrão, toda rota deve exigir autenticação, exceto as explicitamente públicas.
