# Good Hamburger

Sistema de uma Hamburgueria, projeto desenvolvido para o desafio técnico da STgenetics

## Visão geral

Este repositório possui dois projetos principais:

- `frontend/`: aplicação web em React + TypeScript (Ainda será implementado)
- `backend/`: API em ASP.NET Core com arquitetura em camadas (`Web`, `Application`, `Domain` e `Tests`) (Em desenvolvimento)

## Requisitos

Para rodar o projeto localmente, você vai precisar de:

- Node.js (versão LTS recomendada)
- .NET SDK 10.0
- PostgreSQL

## Instruções de Execução

## Decisão de arquitetura

### Backend

- Arquitetura em camadas, pois corroborará para o desenvolvimento de um sistema mais limpo com base nas noções do Clear Achitecture.
  - Web: camada responsável por lidar com as requisições HTTP, controladores e rotas. (Enxergando apenas a camada Application e Domain)
  - Application: camada responsável pela lógica de negócio, serviços e casos de uso. (Enxergando apenas a camada Domain)
  - Domain: camada responsável por conter as entidades, regras de negócio (Não enxergando nenhuma outra camada)
  - Tests: camada responsável por conter os testes (Enxergando apenas a camada Application)
- Modelagem da `Domain`:
  - Entidades: `BaseEntity`, `MenuItem`, `Order` e `OrderItem`
  - Enum: `MenuItemType`
  - Repositórios: As interfaces `IGenericRepository<T>` e `IOrderRepository` e o repositório concreto `OrderRepository` para abstrair o acesso aos dados, seguindo o princípio de inversão de dependência.
  - Migrations: Histórico de alterações no banco de dados, permitindo a evolução do esquema ao longo do tempo. Eu criei um seed_bank para popular o banco com os MenuItens do desafio.
  - ApplicationDBContext: classe responsável por configurar o Entity Framework Core, o banco de dados PostgreSQL e todos as cofigurações sobre as entidades, foquei em manter as entities o mais simples possível, sem regras de negócio, apenas com as propriedades e as relações entre elas.
- Na camada da `Web`:
  - Configuração do banco de dados utilizando Entity Framework Core com PostgreSQL
  - Connection String configurada no `appsettings.json` para conectar ao banco de dados local (Coloquei no User Secrets para não expor a senha do banco de dados)
  - Configuração do CORS para permitir requisições do frontend (Ainda será implementado)
  - Controller `OrdersController` para lidar com as requisições relacionadas aos pedidos, utilizando os serviços da camada `Application` para processar a lógica de negócio.

## O que ficou de fora
