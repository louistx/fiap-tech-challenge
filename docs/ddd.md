# Domain-Driven Design (DDD)

## Visão geral

O domínio deste projeto representa o funcionamento de uma oficina mecânica. O fluxo principal começa com a solicitação de um orçamento pelo cliente e acompanha todo o ciclo de vida de uma Ordem de Serviço (OS), desde a sua criação até a finalização.

![Visualização DDD do projeto](assets/DDD_fluxo.png)

## Linguagem do domínio

Os principais termos utilizados no projeto são:

- **Cliente:** proprietário ou responsável pelo veículo e pela aprovação do orçamento.
- **Veículo:** automóvel vinculado ao atendimento realizado pela oficina.
- **Orçamento:** estimativa dos serviços e produtos necessários para o atendimento.
- **Ordem de Serviço (OS):** registro central que acompanha o atendimento do veículo.
- **Administrador:** responsável pelos cadastros, abertura e encerramento administrativo da OS.
- **Vendedor:** também pode abrir uma OS, conforme definido nos requisitos funcionais.
- **Mecânico:** responsável pelo diagnóstico, inclusão dos serviços e produtos, execução e conclusão técnica do trabalho.
- **Serviço:** atividade executada pelo mecânico, como revisão, troca ou reparo.
- **Produto:** peça ou item de inventário utilizado na execução do serviço.
- **Diagnóstico:** identificação dos serviços e produtos necessários para resolver o problema relatado.

## Agregado Ordem de Serviço

A `OrdemServico` é o principal candidato a **raiz de agregado** do domínio. As operações do fluxo devem acontecer por meio dela, garantindo que as regras de negócio sejam respeitadas em cada mudança de estado.

Atualmente, a entidade relaciona:

- um cliente responsável;
- um veículo;
- um funcionário responsável;
- uma coleção de serviços;
- uma coleção de produtos;
- uma descrição ou relato inicial;
- data de criação;
- data de atualização;
- data de finalização.

As entidades `Cliente`, `Veiculo`, `Funcionario`, `Servico` e `Produto` possuem identidade própria. Por isso, são referenciadas pela OS por meio de seus identificadores.

## Fluxo do orçamento

Antes da abertura da Ordem de Serviço, o diagrama apresenta a solicitação inicial:

1. O **cliente solicita um orçamento** para a oficina.
2. A solicitação é encaminhada ao **administrador**.
3. A partir das informações do cliente e do veículo, o administrador inicia o atendimento e cria a Ordem de Serviço.

O orçamento aparece no desenho como a entrada do processo, mas ainda não existe como entidade ou funcionalidade implementada no código.

## Fluxo de execução da Ordem de Serviço

### 1. Criação da OS

O administrador cria uma OS informando o cliente responsável, o veículo e o relato inicial. De acordo com os requisitos funcionais, um vendedor também pode executar essa operação.

**Comando de domínio:** `CriarOrdemServico`  
**Evento esperado:** `OrdemServicoCriada`

### 2. Recebimento da OS

Após a criação, o sistema encaminha a OS para a fila da oficina e altera seu estado para **Recebida**. Nesse estado, ela está disponível para ser assumida por um mecânico.

**Evento esperado:** `OrdemServicoRecebida`

### 3. Diagnóstico e inclusão de itens

O mecânico assume a OS e registra o diagnóstico, adicionando os serviços e produtos necessários. Pelos requisitos, o mecânico somente pode assumir uma OS que esteja com estado **Recebida** e não pode possuir outra OS em andamento.

Ao ser atribuída ao mecânico, a OS deve passar pelo estado **Em diagnóstico**.

**Comandos de domínio:**

- `AtribuirOrdemServico`;
- `RegistrarDiagnostico`;
- `AdicionarServico`;
- `AdicionarProduto`.

**Eventos esperados:**

- `OrdemServicoAtribuida`;
- `DiagnosticoRegistrado`;
- `ItemAdicionadoAoOrcamento`.

Antes de enviar o orçamento ao cliente, o sistema também deve verificar se os produtos solicitados estão disponíveis no estoque. Quando não houver estoque, administrador e mecânico devem ser avisados.

### 4. Envio para aprovação

Depois do diagnóstico, o sistema envia ao cliente uma notificação contendo os serviços e produtos propostos. A OS passa para o estado **Aguardando aprovação**.

**Comando de domínio:** `EnviarOrcamentoParaAprovacao`  
**Eventos esperados:**

- `OrcamentoEnviado`;
- `OrdemServicoAguardandoAprovacao`;
- `ClienteNotificado`.

### 5. Aprovação do cliente

O cliente analisa os itens e aprova a execução. Após a confirmação, o sistema altera o estado da OS para **Aprovada**.

**Comando de domínio:** `AprovarOrcamento`  
**Eventos esperados:**

- `OrcamentoAprovado`;
- `OrdemServicoAprovada`.

O fluxo de recusa ou aprovação parcial não está representado no diagrama e ainda precisa ser definido.

### 6. Início da execução

Com a OS aprovada, o mecânico inicia o trabalho. O sistema registra a mudança para **Em execução**.

Uma OS não deve ser iniciada antes da aprovação do cliente.

**Comando de domínio:** `IniciarExecucao`  
**Evento esperado:** `ExecucaoIniciada`

### 7. Conclusão técnica

Ao terminar os serviços, o mecânico conclui a execução da OS. O sistema registra a conclusão técnica e envia uma notificação ao cliente e ao administrador.

**Comando de domínio:** `ConcluirExecucao`  
**Eventos esperados:**

- `ExecucaoConcluida`;
- `ClienteNotificado`;
- `AdministradorNotificado`.

O diagrama utiliza a palavra “finaliza” tanto para a ação do mecânico quanto para a ação posterior do administrador. Para evitar ambiguidade, a ação do mecânico é tratada nesta documentação como **conclusão técnica**.

### 8. Finalização administrativa

Depois da conclusão técnica, o administrador encerra definitivamente a OS. O sistema registra a data de finalização e altera seu estado para **Finalizada**.

**Comando de domínio:** `FinalizarOrdemServico`  
**Evento esperado:** `OrdemServicoFinalizada`

## Ciclo de estados

O fluxo descrito pelo diagrama pode ser representado pela seguinte sequência:

```text
Criada
  ↓
Recebida
  ↓
Em diagnóstico
  ↓
Aguardando aprovação
  ↓
Aprovada
  ↓
Em execução
  ↓
Execução concluída
  ↓
Finalizada
```

As transições devem ser controladas pela própria Ordem de Serviço. Por exemplo:

- somente uma OS `Recebida` pode ser atribuída a um mecânico;
- somente uma OS `Em diagnóstico` pode receber o diagnóstico;
- somente uma OS com diagnóstico e estoque validado pode aguardar aprovação;
- somente uma OS `Aprovada` pode entrar em execução;
- somente uma OS com execução concluída pode ser finalizada pelo administrador.

## Contextos do domínio

Com base no desenho e nos requisitos atuais, o domínio pode ser dividido nos seguintes contextos:

- **Atendimento:** clientes, veículos, solicitação de orçamento e abertura da OS.
- **Execução da oficina:** atribuição do mecânico, diagnóstico, execução e conclusão técnica.
- **Catálogo e inventário:** serviços oferecidos, produtos e disponibilidade em estoque.
- **Aprovação:** apresentação do orçamento e decisão do cliente.
- **Notificações:** comunicação com cliente, mecânico e administrador.

Essa separação é uma proposta inicial. Os limites devem ser refinados conforme as regras do negócio forem implementadas.

## Relação entre o desenho e o código atual

| Parte do domínio | Situação atual |
| --- | --- |
| Entidades `Cliente`, `Veiculo`, `Funcionario`, `Servico`, `Produto` e `OrdemServico` | Modeladas no projeto de domínio |
| Relacionamentos da OS | Configurados com Entity Framework |
| Criação, consulta, atualização e exclusão da OS | Rotas existentes, mas ainda retornam respostas simuladas |
| Atribuição da OS ao mecânico | Rota, comando e serviço iniciados, sem regra implementada |
| Registro do diagnóstico | Rota, comando e serviço iniciados, sem regra implementada |
| Listagem de OS para a oficina | Estrutura iniciada, sem implementação |
| Persistência da OS | Interface e repositório ainda sem operações |
| Estados da OS | Ainda não modelados na entidade nem no banco |
| Orçamento e aprovação do cliente | Ainda não modelados |
| Verificação de estoque | Prevista nos requisitos, ainda não implementada |
| Notificações | Representadas no diagrama, ainda não implementadas |
| Início, conclusão técnica e finalização da OS | Ainda não implementados |

Portanto, o diagrama documenta o **fluxo de negócio desejado**, enquanto o código atual representa uma estrutura inicial da API e do domínio. Para implementar o fluxo completo, será necessário adicionar os estados da OS, suas regras de transição, os casos de uso pendentes, persistência e notificações.
