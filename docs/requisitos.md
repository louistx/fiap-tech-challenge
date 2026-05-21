# Requisitos Funcionais

## RF01 - Cadastro do cliente
 - o sistema deve permitir que o administrador faça o cadastro do cliente
 - O cadastro do cliente deve conter Nome, Endereço, RG, CPF

## RF02 - Cadastrado do veículo
 - o sistema deve permitir que o administrador faça o cadastro do veículo
 - o cadastro do veiculo deve conter Tipo (Carro/Moto/Caminhao), Placa, Modelo, Ano, Quilometragem inicial
 - Um veucilo deve ter um cliente como responsavel

## RF03 - Cadastro de funcionários
 - O sistema deve permitir que o administrador faça o cadastro do veículo.
 - O cadastro do funcionario deve conter Nome, CPF, RG, Endereco, Cargo.
 - Os cargos podem ser Vendedor, Mecânico ou Administrador

## RF04 - Cadastro de OS
- O sistema deve permitir que o administrador e vendedores possam gerar OS.
- A OS deve conter Veiculo, cliente responsável, status, relato inicial ao ser criada.
- Ao ser criada a OS fica com status Criada.
- Apos ser criada a OS deve ser encaminhada para fila, alterando seu status para Recebida. 

## RF05 - Cadastro de Inventário
- O sistema deve permitir que o administrador e vendedores possam cadastrar itens no inventario.

## RF06 - Cadastro de Serviços
- O sistema deve permitir que o administrador e vendedores possam cadastrar itens no inventario.

## RF07 - Listagem de Inventario
- O sistema deve permitir que os itens de inventario sejam listados

## RF08 - Listagem de Inventario
- O sistema deve permitir que as OS sejam listadas de acordo com o status.

## RF09 - Listagem de OS
- O sistema deve permitir que as OS sejam listadas de acordo com o status.
- à refinar 

## RF10 - Atribuição de OS
 - O sistema deve permitir que o mecanico se atribua uma OS com status Recebida.
 - O mecanico pode ter apenas uma OS atribuida por vez.
 - Ao ser atribuida a um mecanico, a OS deve alterar o status para Em Diagnóstico

## RF11 - Registro de diagnóstico
- O mecanico devera efetuar o registo de serviços e itens na OS.

## RF - Verificacao de Estoque
- Antes de ser enviado para aprovacao ao cliente, o sistema deve verificar se os itens necessarios para OS existem em estoque.
- Caso o item nao tenha no estoque o sistema deve notificar administrador e mecanico.  

## RF12 - Exibicao de Servicos para oficina
 - O sistema deve ter um endpoint retornando a listagem de os com status em diagnostico com informacoes basicas (Placa do carro, Mecanico atribuido, relato inicial) para ser exibido na oficina

 ## RF - Aprovação de OS para Execução
 - 











