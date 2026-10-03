# Cadastro de currículos

Aplicação para a equipe de recrutamento cadastrar e consultar candidatos.

Há duas formas de cadastrar, com o **mesmo formulário** e as **mesmas regras de validação**:

- **Manual:** a pessoa preenche o formulário e salva.
- **Com PDF:** a pessoa envia um currículo. O backend lê o texto e tenta achar nome, e-mail e telefone. Os dados encontrados preenchem o formulário e podem ser corrigidos antes de salvar.

O PDF é opcional: se o arquivo for inválido ou a leitura falhar, aparece uma mensagem e o cadastro manual continua funcionando. Depois de salvo, o candidato aparece em uma lista, com uma tela de detalhes.

O relato de como o trabalho foi feito (inclusive o uso de IA) está em [DESENVOLVIMENTO.md](DESENVOLVIMENTO.md).

## Tecnologias e versões

| Parte | Tecnologia | Versão |
|---|---|---|
| Backend | ASP.NET Core (C#) | .NET 8 |
| Banco de dados | SQL Server | Express ou Developer, ou 2022 pelo Docker |
| Acesso ao banco | Entity Framework Core (com migrations) | 8.x |
| Leitura de PDF | PdfPig | 0.1.16 |
| Frontend | React com Vite (JavaScript) | Vit:8.3.2 react: 19.3.0 |
| Testes | xUnit | 2.5.3 |

## O que instalar antes

- Git
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org) 20 ou superior
- Um SQL Server (veja o passo 2): o **SQL Server Express** (Windows) ou o **Docker**

## Como rodar

São 6 passos, na ordem. Você vai usar **dois terminais**: um para o backend e outro para o frontend. Todos os comandos partem da pasta do projeto (a raiz do repositório).

### 1. Baixar o projeto

```
git clone https://github.com/JoaoArthur-bat/cadastro-curriculos.git
cd cadastro-curriculos
```

### 2. Ter um SQL Server rodando

Escolha **uma** das opções abaixo e guarde a connection string dela, que será usada no passo 3.

**Opção 1: SQL Server Express (Windows, sem Docker)**

1. Instale o SQL Server Express (gratuito, no site da Microsoft) escolhendo a instalação **Básica**.
2. Confira se o serviço está rodando:

   ```powershell
   Get-Service | Where-Object { $_.Name -like "MSSQL*" }
   ```

   Deve aparecer `MSSQL$SQLEXPRESS` com status `Running`.

3. A connection string desta opção é:

   ```
   Server=localhost\\SQLEXPRESS;Database=Curriculos;Trusted_Connection=True;TrustServerCertificate=True
   ```

   Ela entra com o seu usuário do Windows, sem senha. Dentro do arquivo JSON a barra invertida é escrita em dobro (`\\`), como acima.

**Opção 2: Docker (Windows, Linux ou macOS)**

1. Instale o Docker e deixe-o aberto.
2. Na raiz do projeto, rode:

   ```
   docker compose up -d
   ```

   Isso sobe um SQL Server na porta 1433, com o usuário `sa` e a senha `Curriculos@123`. Essa senha é só do banco de teste local criado pelo Docker (é a mesma do `docker-compose.yml`).
3. Espere cerca de 20 segundos para o SQL Server ficar pronto.
4. A connection string desta opção é:

   ```
   Server=localhost,1433;Database=Curriculos;User Id=sa;Password=Curriculos@123;TrustServerCertificate=True
   ```

**Outro SQL Server (com usuário e senha):** use o formato da Opção 2, trocando servidor, usuário e senha pelos seus.

### 3. Criar o arquivo de configuração

O arquivo com a conexão real (`appsettings.Development.json`) não vai para o Git. Crie-o a partir do exemplo:

Windows (PowerShell):

```powershell
copy backend\Curriculos.Api\appsettings.Example.json backend\Curriculos.Api\appsettings.Development.json
```

Linux ou macOS:

```
cp backend/Curriculos.Api/appsettings.Example.json backend/Curriculos.Api/appsettings.Development.json
```

Abra o `backend/Curriculos.Api/appsettings.Development.json` e troque o valor de `Default` pela connection string do passo 2. Exemplo com a Opção 1 (Express):

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost\\SQLEXPRESS;Database=Curriculos;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Exemplo com a Opção 2 (Docker):

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=Curriculos;User Id=sa;Password=Curriculos@123;TrustServerCertificate=True"
  }
}
```

### 4. Rodar o backend (terminal 1)

```
cd backend/Curriculos.Api
dotnet run
```

Na primeira execução, o backend **cria o banco `Curriculos` e a tabela `Candidatos` sozinho** (aplica as migrations). Não é preciso rodar nenhum comando de banco. Quando aparecer `Now listening on: http://localhost:5195`, está pronto. Deixe este terminal aberto.

Para conferir, abra `http://localhost:5195/api/candidatos` no navegador: deve aparecer `[]`.

### 5. Rodar o frontend (terminal 2)

Abra outro terminal, na raiz do projeto:

```
cd frontend
npm install
npm run dev
```

> **Windows (PowerShell):** se aparecer o erro "a execução de scripts foi desabilitada", use `npm.cmd` no lugar de `npm`:
> `npm.cmd install` e depois `npm.cmd run dev`.

### 6. Abrir no navegador

Acesse **http://localhost:5173**. Deve aparecer o título "Cadastro de currículos", com os botões **Novo cadastro** e **Candidatos**.

O frontend chama a API em `http://localhost:5195/api` (definido em `frontend/src/api.js`), e a API só aceita chamadas vindas de `http://localhost:5173`. Por isso o frontend sempre usa essa porta.

## Como usar

1. **Cadastro manual:** em **Novo cadastro**, preencha nome e e-mail (obrigatórios) e, se quiser, os outros campos. Clique em **Salvar candidato**.
2. **Cadastro com PDF:** em **Novo cadastro**, escolha um PDF no topo do formulário. Os campos encontrados são preenchidos. Corrija o que for preciso e salve. Para testar, use `exemplos/curriculo-ficticio.pdf`.
3. **Candidatos:** lista os candidatos, do mais recente para o mais antigo.
4. **Ver detalhes:** mostra os dados completos de um candidato.

## Regras de validação

As regras ficam no backend e valem para os dois caminhos de cadastro.

- **Nome completo:** obrigatório, até 200 caracteres.
- **E-mail:** obrigatório, em formato válido, até 200 caracteres.
- **Telefone:** opcional, até 30 caracteres.
- **Área ou cargo de interesse:** opcional, até 150 caracteres.
- **Resumo profissional:** opcional, até 2000 caracteres.
- **Arquivo enviado:** precisa ser PDF (extensão `.pdf` e conteúdo de PDF), com no máximo 5 MB.

Mensagens exibidas: campos obrigatórios ou inválidos, arquivo ausente, arquivo maior que 5 MB, arquivo que não é PDF, falha na leitura do PDF, PDF sem texto, falha de conexão com o servidor e "Candidato salvo com sucesso!".

## Rodar os testes

Na raiz do projeto:

```
dotnet test
```

São 9 testes: 4 de validação do cadastro e 5 da extração de dados do PDF. O frontend não tem testes automatizados.

Se o seu Windows bloquear a execução dos testes (política de controle de aplicativo), rode-os dentro de um contêiner Docker, na raiz do projeto.

Windows (PowerShell):

```powershell
docker run --rm -v "${PWD}:/src:ro" mcr.microsoft.com/dotnet/sdk:8.0 sh -c "cp -r /src /work && cd /work && rm -rf backend/*/bin backend/*/obj && dotnet test"
```

Linux ou macOS:

```
docker run --rm -v "$(pwd):/src:ro" mcr.microsoft.com/dotnet/sdk:8.0 sh -c "cp -r /src /work && cd /work && rm -rf backend/*/bin backend/*/obj && dotnet test"
```

## API

| Método | Rota | O que faz |
|---|---|---|
| `POST` | `/api/candidatos` | Valida e salva um candidato (usada pelos dois caminhos de cadastro) |
| `GET` | `/api/candidatos` | Lista os candidatos |
| `GET` | `/api/candidatos/{id}` | Devolve um candidato (404 se não existir) |
| `POST` | `/api/candidatos/extrair-pdf` | Recebe um PDF (campo `arquivo`) e devolve o nome, e-mail e telefone encontrados. **Não salva nada.** |

## Estrutura do repositório

```
backend/Curriculos.Api/     API em ASP.NET Core (controllers, modelos, migrations, leitura de PDF)
backend/Curriculos.Tests/   testes automatizados (xUnit)
frontend/                   interface em React
exemplos/                   currículo fictício em PDF para testar a importação
docker-compose.yml          SQL Server para usar com Docker (opcional)
```

## Limitações conhecidas

- **A extração não é perfeita.** Quando algo não é identificado, o campo fica em branco para preenchimento manual.
- **Nome:** é a primeira linha do PDF com duas ou mais palavras, sem números e sem `@`. Pode errar se o currículo começar por um cargo antes do nome.
- **Telefone:** usa um padrão brasileiro (DDD mais 8 ou 9 dígitos) e pode confundir outros números de 10 ou 11 dígitos, como um CPF sem pontuação.
- **E-mail:** pega o primeiro e-mail encontrado no texto.
- **PDFs escaneados** (imagem, sem texto) não são lidos, porque não há OCR. Currículos em várias colunas podem ter as linhas embaralhadas.
- Não há bloqueio de **e-mails duplicados**.
- Campos opcionais deixados em branco são gravados como texto vazio.
- O frontend não usa rotas: ao recarregar a página (F5), volta para a tela de cadastro.
- A URL da API está fixa em `frontend/src/api.js`.
- Não há autenticação, e o CORS aceita apenas `http://localhost:5173`.

## Problemas comuns

- **`npm` diz que a execução de scripts está desabilitada (Windows):** use `npm.cmd` no lugar de `npm`, ou rode `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned`.
- **`Login failed for user 'sa'`:** a senha do `appsettings.Development.json` é diferente da senha do SQL Server. Com o Docker, ela deve ser `Curriculos@123`.
- **`Login failed` com o SQL Server Express:** rode o backend com o mesmo usuário do Windows que instalou o SQL Server.
- **O backend não conecta logo depois de `docker compose up`:** o SQL Server ainda está iniciando. Espere uns 20 segundos e rode o backend de novo.
- **Erro de CORS no navegador:** confirme que o frontend está em `http://localhost:5173` e que o backend está rodando.
- **A página abre, mas não carrega dados:** confira se a porta em `frontend/src/api.js` é a mesma que aparece no log do `dotnet run` (`5195`).
- **`dotnet test` diz que não há testes e cita bloqueio por política de aplicativo:** use o comando do Docker da seção de testes.

## Criar o banco manualmente (opcional)

O backend já aplica as migrations ao iniciar, então isso só é necessário se você preferir criar a estrutura por conta própria:

```
dotnet tool install --global dotnet-ef --version 8.*
cd backend/Curriculos.Api
dotnet ef database update
```

Para gerar um script SQL com a mesma estrutura:

```
cd backend/Curriculos.Api
dotnet ef migrations script -o criar-banco.sql
```

As migrations ficam em `backend/Curriculos.Api/Migrations`.
