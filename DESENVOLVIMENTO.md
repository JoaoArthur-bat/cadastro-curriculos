# Relato de desenvolvimento
## Como organizei e executei o trabalho

Comecei em 29/09. Dividi o desafio em partes pequenas e fui fechando uma de cada vez, testando antes de passar para a próxima:

1. **Base do backend e do banco:** estrutura da solução em .NET, SQL Server rodando no Docker, modelo `Candidato`, `DbContext` e a primeira migration.
2. **API de candidatos:** cadastrar, listar e buscar por id, com validação dos campos, e um primeiro teste automatizado.
3. **Leitura do PDF:** endpoint que recebe o arquivo, valida (PDF, até 5 MB), extrai o texto e identifica nome, e-mail e telefone, mais os testes dessa lógica.
4. **Frontend em React:** formulário único (usado no cadastro manual e no com PDF), listagem e tela de detalhes.
5. **Documentação:** README, este relato e o teste final seguindo o README como se eu fosse quem avalia.

Em todas as partes eu pedi para a IA explicar o motivo de cada escolha, para eu conseguir explicar a solução depois.

Sobre o histórico de commits: o primeiro commit foi feito logo no começo (estrutura inicial). Depois, durante um tempo, eu escrevi código sem commitar. Quando percebi, separei o que já existia em commits por etapa (limpeza e Docker, banco de dados, API) e passei a commitar a cada parte concluída. Então os commits dessas etapas foram organizados depois de o código estar escrito, e não no exato momento em que cada coisa foi feita.

## Principais decisões técnicas e motivos

- **ASP.NET Core (C#) no backend.** É uma das opções do desafio, e o Entity Framework já gera migrations para SQL Server, o que atende ao requisito de scripts ou migrations. Cheguei a considerar trocar para Node.js por achar JavaScript mais familiar, mas descartei: eu já tinha parte do backend pronta, e o importante era entender o que está no repositório.
- **React com Vite, em JavaScript.** Escolhi JavaScript em vez de TypeScript e não usei biblioteca de rotas nem de componentes, para ter poucos arquivos e menos coisas para explicar. As três telas (cadastro, lista e detalhes) são controladas por uma variável de estado.
- **SQL Server no Docker, como opção.** O `docker-compose.yml` facilita para quem não tem SQL Server instalado. Quem já tem um pode apenas editar a connection string. O código é o mesmo nos dois casos. Testei os dois tipos de conexão: o do Docker (usuário `sa` e senha) no meu computador, e o do SQL Server Express (autenticação do Windows) na máquina virtual.
- **Configuração sem credenciais reais.** O repositório tem só o `appsettings.Example.json`. O arquivo `appsettings.Development.json`, com a conexão usada de verdade, está no `.gitignore`. A senha `Curriculos@123` aparece no `docker-compose.yml` e no README de propósito: é a senha de um banco de teste local criado pelo Docker, e as duas pontas precisam combinar (o contêiner define a senha e a API usa a mesma para entrar). Antes de publicar, conferi que nenhuma senha tinha entrado no histórico do Git.
- **Uma única classe de validação (`CandidatoRequest`).** O desafio pede as mesmas regras nos dois caminhos de cadastro. Como os dois salvam pelo mesmo endpoint (`POST /api/candidatos`), é impossível as regras ficarem diferentes.
- **Validação só no backend; o frontend apenas mostra as mensagens.** Para não duplicar regras, o formulário usa `noValidate` e exibe os erros que a API devolve, campo por campo.
- **A leitura do PDF não salva nada.** O endpoint `extrair-pdf` só devolve o que encontrou. Quem salva é o mesmo `POST /api/candidatos` do cadastro manual, depois que a pessoa revisa o formulário.
- **PdfPig para ler o PDF.** Lê PDF inteiramente em C#, sem programa externo, então quem avalia não precisa instalar nada além do .NET.
- **Validação do arquivo.** Confiro o tamanho (5 MB), a extensão `.pdf` e se o conteúdo começa com `%PDF-`. Só olhar o nome não basta, porque dá para renomear qualquer arquivo.
- **Códigos de erro diferentes.** `400` quando o arquivo enviado é inadequado (grande demais, não é PDF), e `422` quando parece um PDF mas não foi possível lê-lo. Um `try/catch` transforma falha de leitura em mensagem amigável, em vez de um erro 500, e o cadastro manual segue disponível.
- **Extração com regras simples.** E-mail e telefone são achados por expressões regulares. O nome é a primeira linha com duas ou mais palavras, sem números e sem `@`. Não existe regra perfeita para nome, então preferi uma regra fácil de explicar e documentar as limitações.
- **Migrations aplicadas ao iniciar (só em Development).** Assim, quem avalia sobe o banco, roda o backend e a estrutura já está criada, com um passo a menos para dar errado.

## Ferramentas de IA utilizadas

Usei o **Claude**.

Não usei outras ferramentas de IA. Também usei VS Code, Git, Docker Desktop e PowerShell.

## Em que etapas a IA ajudou, com exemplos

A IA participou de todas as etapas e escreveu a primeira versão do código de cada parte. Eu executei os comandos, rodei os testes, li os erros, tomei decisões e pedi mudanças. Eu tinha pouco conhecimento de C# no início, então pedi explicações de cada trecho.

Alguns exemplos de pedidos e do que fiz com as respostas:

- **Colei o enunciado e pedi ajuda para começar.** Recebi um plano em partes e a sugestão de stack (.NET, React e SQL Server). Aceitei o plano e a stack, e fui executando por partes.
- **"Quero algo que eu consiga explicar, sem mudar a lógica."** A primeira versão do controller usava recursos que eu não dominava. Pedi uma versão mais simples, e ela passou a ter construtor comum, passos em sequência e comentários.
- **Mandei prints da estrutura de pastas e perguntei o que excluir.** A resposta apontou os arquivos de exemplo do template e um `appsettings` fora do lugar. Eu removi o que era sobra e movi o resto.
- **Perguntei como quem avalia iria testar, e se teria que instalar o Docker.** A resposta me fez deixar duas opções de banco no README (SQL Server próprio ou Docker) e priorizar a facilidade de configurar e executar.
- **Colei erros do terminal e dos testes.** Foi assim que identifiquei o bloqueio do Windows aos testes e o arquivo que estava na pasta errada.
- **Disse que o código da leitura de PDF estava difícil de explicar.** Pedi uma versão mais simples do extrator, que ficou como está hoje.

## O que precisei corrigir, adaptar ou descartar

- **Controller:** a versão inicial devolvia uma classe de resposta separada (`CandidatoResponse`) e usava construtor enxuto. Descartei a classe e simplifiquei. Uma diferença de comportamento dessa mudança: campo opcional em branco é gravado como texto vazio, e não como nulo. Não afeta o desafio, e o frontend mostra "Não informado" nesses casos.
- **Extrator de PDF:** a primeira versão tinha uma lista de palavras de título e expressões regulares mais elaboradas. Troquei por uma versão mais curta, que entendo melhor. A regra "duas ou mais palavras" já descarta títulos de uma palavra só, como "CURRÍCULO".
- **Arquivos do template** (exemplo de previsão do tempo, arquivo `.http`, teste vazio) foram removidos.
- **Configuração:** o `appsettings.Example.json` estava na pasta errada e o `appsettings.Development.json` estava sendo acompanhado pelo Git. Movi o exemplo e passei o arquivo real para o `.gitignore` antes de publicar. A IA também corrigiu uma explicação dela: eu havia entendido que o arquivo guardava uma senha "real", quando é só a senha de teste do Docker.
- **Projeto de testes vazio:** o primeiro `dotnet test` mostrou que não havia nenhum teste, porque o do template tinha sido apagado e o meu ainda não existia. Criei os testes de validação e depois os do extrator.
- **Arquivo na pasta errada:** o `ExtratorDeCurriculo.cs` foi criado em `backend/Services`, fora do projeto, e a compilação falhava. Movi para `backend/Curriculos.Api/Services`.
- **Problemas do README achados na máquina virtual:**
  - A primeira versão só mostrava a connection string com usuário e senha (a do Docker), mas o SQL Server Express usa a autenticação do Windows. Reescrevi o README com as duas opções de banco separadas, cada uma com a sua connection string. Aproveitei para reorganizar o texto em seis passos numerados, porque a primeira versão estava confusa.
  - O PowerShell bloqueou o `npm` (execução de scripts desabilitada). Passei a documentar o uso do `npm.cmd` e acrescentei o problema à seção "Problemas comuns".

## Como verifiquei se a solução estava correta

- **Testes automatizados (9):** 4 de validação do cadastro (nome e e-mail obrigatórios, formato do e-mail) e 5 da lógica de extração (nome, e-mail e telefone, telefone fixo, nada encontrado e reconhecimento de PDF). Passaram. O Windows bloqueou a execução do `dotnet test` no meu computador, por uma política de controle de aplicativo, então rodei os testes dentro de um contêiner Docker. O comando está no README.
- **Teste manual da leitura com um PDF de verdade:** enviei `exemplos/curriculo-ficticio.pdf` ao endpoint com `curl` e recebi nome, e-mail e telefone corretos. Os testes automatizados cobrem a lógica em cima de texto, mas não abrem um PDF real, e foi esse teste manual que cobriu a leitura.
- **Casos de erro da leitura (arquivo que não é PDF, PDF corrompido, arquivo acima de 5 MB, requisição sem arquivo)**.
- **Frontend no navegador: ** salvar vazio, e-mail inválido, cadastro manual, lista, detalhes, PDF, arquivo inválido e backend desligado**. O frontend não tem testes automatizados.
- **Histórico do Git:** busquei a palavra `Password` no histórico do arquivo de configuração antes de publicar e não encontrei nada.
- **Teste do zero em uma máquina virtual:** criei uma máquina virtual Windows limpa, sem nada do projeto, e segui o README para rodar tudo do começo. Instalei os pré-requisitos, incluindo o **SQL Server Express** (instalação Básica, sem Docker), cloneei o repositório do GitHub, criei o `appsettings.Development.json` com a conexão do Express (autenticação do Windows), rodei o backend (porta 5195) e o frontend, e usei a aplicação no navegador. Funcionou. O backend criou o banco e a tabela sozinho. Esse teste também achou dois problemas no README, descritos na seção seguinte.

## Tempo aproximado dedicado

**total aproximado em horas: cerca de 25 horas, divididas entre 29/09 e 03/10**

## Dificuldades, limitações e melhorias

**Dificuldades**

- Eu tinha pouca experiência com C# e com a estrutura de um projeto .NET, então boa parte do tempo foi entender o código e os erros que apareceram.
- O bloqueio do Windows aos testes e os caminhos relativos de pasta (comandos que só funcionam na pasta certa) tomaram mais tempo do que o código em si.
- Não há regra perfeita para achar o nome no texto de um currículo.

**Limitações** (as mesmas estão no README)

- Nome por heurística: erra se o currículo começa por um cargo.
- Telefone pode confundir outros números de 10 ou 11 dígitos sem formatação.
- PDFs escaneados não são lidos (não há OCR), e currículos em colunas podem embaralhar as linhas.
- Não bloqueio e-mails duplicados.
- O frontend não tem rotas (F5 volta ao cadastro), não tem testes automatizados, e a URL da API está fixa no código.

**O que faria com mais tempo**

- Índice único para o e-mail e tratamento amigável do erro de duplicidade.
- Testes de integração da API (incluindo o envio de um PDF de verdade) e testes do frontend.
- OCR para PDFs escaneados e uma extração do nome que usasse a posição do texto na página.
- Rotas no frontend, URL da API por variável de ambiente, paginação e busca na listagem.
- Subir backend e frontend também pelo Docker Compose, para a execução ficar com um comando só.
