# Cadastro de currículos

Aplicação para cadastrar e consultar candidatos, com cadastro manual
ou a partir de um currículo em PDF.

## Tecnologias e versões

- Backend: ASP.NET Core (.NET 8), Entity Framework Core 8
- Leitura de PDF: PdfPig (VERSÃO: )
- Banco de dados: SQL Server 2022
- Frontend: React (VERSÃO:)
- Testes: xUnit

## O que você precisa ter instalado

- .NET 8 SDK
- Node.js 20 ou superior
- Um SQL Server, ou o Docker para subir um pronto

## 1. Banco de dados (SQL Server)

**Opção A: usar um SQL Server que você já tem**

1. Copie `backend/Curriculos.Api/appsettings.Example.json` para
   `backend/Curriculos.Api/appsettings.Development.json`.
2. Edite a connection string com o seu servidor, usuário e senha.

**Opção B: subir um SQL Server com Docker**

1. Na raiz do projeto: `docker compose up -d`
2. Copie `appsettings.Example.json` para `appsettings.Development.json`
   e use a senha `Curriculos@123`, a mesma do docker-compose.

## 2. Criar a estrutura do banco


## 3. Rodar o backend

    cd backend/Curriculos.Api
    dotnet run



## 4. Rodar o frontend



## 5. Rodar os testes

    dotnet test

Se o Windows bloquear a execução dos testes, rode pelo Docker, na raiz:

    docker run --rm -v "${PWD}:/src:ro" mcr.microsoft.com/dotnet/sdk:8.0 sh -c "cp -r /src /work && cd /work && rm -rf backend/*/bin backend/*/obj && dotnet test"

## Como usar


O arquivo `exemplos/curriculo-ficticio.pdf` serve para testar a importação.

## Limitações

- O nome é identificado pela primeira linha do PDF que parece um nome.
  Pode errar em currículos que começam por um cargo.
- O telefone usa um padrão brasileiro e pode confundir outros números.
- PDFs escaneados (imagem) não têm texto e não são lidos.
- Quando algo não é identificado, o campo fica em branco para
  preenchimento manual.