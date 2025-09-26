Sistema de Gerenciamento de Usuários

Este é um sistema completo de gerenciamento de usuários desenvolvido com React.js frontend e API .NET backend.
📋 Pré-requisitos

    Docker e Docker Compose

    .NET 7.0 ou superior

    Node.js 16 ou superior

    npm ou yarn

🚀 Como Executar o Projeto
1. Banco de Dados (PostgreSQL)

# Execute o container do PostgreSQL
sudo docker compose up -d

O banco de dados estará disponível em:

    Host: localhost

    Porta: 5432

    Database: myappdb

    Usuário: admin

    Senha: admin123

2. API Backend (.NET)

# Navegue até a pasta da API
cd ArqPay.API

# Restaure as dependências
dotnet restore

# Execute a aplicação
dotnet run

A API estará disponível em: http://localhost:5278

🧪 Executando os Testes

# Execute todos os testes do projeto
dotnet test

# Para executar testes de um projeto específico
dotnet test ArqPay.Tests

# Com cobertura de código (se configurado)
dotnet test --collect:"XPlat Code Coverage"

🔑 Funcionalidades

    ✅ Autenticação JWT

    ✅ CRUD completo de usuários

    ✅ Validação de dados

    ✅ Interface responsiva

    ✅ Testes automatizados

🌐 Endpoints da API

    POST /Login - Autenticação

    POST /Authenticate - Refresh token

    GET /User - Listar usuários

    GET /User/{id} - Obter usuário

    POST /User - Criar usuário

    PUT /User - Atualizar usuário

    DELETE /User/{id} - Excluir usuário

🐛 Solução de Problemas
Erro de CORS

Se encontrar erros de CORS, verifique se a API está rodando na porta 5278.
Banco não conecta

Certifique-se que o Docker está rodando e o container do PostgreSQL foi iniciado.
Dependências não encontradas

Execute dotnet restore na pasta da API e npm install na pasta do frontend.