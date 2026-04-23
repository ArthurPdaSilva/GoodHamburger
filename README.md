# Good Hamburger

Sistema de uma Hamburgueria, projeto desenvolvido para o desafio técnico da STgenetics

## Visão geral

Este repositório possui dois projetos principais:

- `frontend/`: aplicação web em React + TypeScript (Em desenvolvimento)
- `backend/`: API em ASP.NET Core com arquitetura em camadas (`Web`, `Application`, `Domain` e `Tests`)

## Requisitos

Para rodar o projeto localmente, você vai precisar de:

- Node.js (versão LTS recomendada)
- .NET SDK 10.0
- PostgreSQL
- Visual Studio (Aconselhável para desenvolvimento do backend, mas não é obrigatório)
- Um editor de código (Eu uso e prefiro o VS Code)

## Instruções de Execução

## Backend (recomendado: Visual Studio)

### Passo a passo - Back

1. Abra a solução `Backend.slnx` no Visual Studio.  
2. Defina o projeto **Web.csproj** como *Startup Project*.  
3. Configure a connection string `DefaultConnection` via **User Secrets**
4. Rode as migrations que estão na camada de `Domain` para criar as tabelas no banco de dados PostgreSQL. Você pode usar o comando `dotnet ef database update` ou um script SQL para criar as tabelas manualmente.
5. Restaure e compile a solução.  
6. Execute utilizando o perfil **https** conforme definido no `launchSettings.json`.  
7. Acesse o Swagger na URL apresentada ao iniciar a API (ex.: `/swagger/index.html`).

## Frontend (recomendado: VS Code)

### Passo a passo - Front

1. Navegue até a pasta `frontend/` no terminal.
2. Instale as dependências com `npm install`.
3. Configure a variável de ambiente `VITE_API_URL` para apontar para a URL da API backend (ex.: `http://localhost:5000`).
4. Inicie a aplicação com `npm start`.

## Decisão de arquitetura

### Backend

- Arquitetura em camadas, pois corroborará para o desenvolvimento de um sistema mais limpo com base nas noções do Clear Achitecture.
  - `Web`: camada responsável por lidar com as requisições HTTP, controladores e rotas.
  - `Application`: camada responsável pela lógica de negócio, serviços e casos de uso.
  - `Domain`: camada responsável por conter as entidades, regras de negócio
  - `Tests`: camada responsável por conter os testes
- Modelagem de `Domain`:
  - Entidades: `BaseEntity`, `MenuItem`, `Order` e `OrderItem`
  - Enums: `MenuItemType`
  - Repositórios: `IOrderRepository` e `IMenuItemRepository`, ambos com métodos específicos para as necessidades do projeto, evitando a implementação de um repositório genérico que poderia ser desnecessário, além de suas respectivas implementações `OrderRepository` e `MenuItemRepository` utilizando Entity Framework Core para interagir com o banco de dados PostgreSQL.
  - Migrations: Não costumo subir as migrations para o repositório, mas posso disponibilizar um script SQL para criar as tabelas no banco de dados, caso seja necessário.
  - ApplicationDBContext: classe responsável por configurar o Entity Framework Core, o banco de dados PostgreSQL e todos as cofigurações sobre as entidades, foquei em manter as entities o mais simples possível, sem regras de negócio, apenas com as propriedades e as relações entre elas.
- Modelagem da `Application`:
  - Serviços: `OrderService` e `MenuItemService`, responsáveis por implementar a lógica de negócio relacionada aos pedidos e itens de menu, utilizando os repositórios para acessar os dados.
  - DTOs: `OrderDTO`, `OrderItemDTO` e `MenuItemDTO`.
  - Utils: `MenuItemTypeTranslator` para traduzir os tipos enums de itens do menu para uma representação para string.
  - Services: `IOrderService` e `IMenuItemService` para lidar com a lógica de negócio relacionada aos pedidos e itens de menu, utilizando os repositórios para acessar os dados. E suas respectivas implementações `OrderService` e `MenuItemService`.
  - Mapping: `MappingContext` para configurar o AutoMapper facilitando a conversão dos dtos para entities e vice-versa.
- Modelagem da `Web`:
  - Configuração do banco de dados utilizando Entity Framework Core com PostgreSQL
  - Connection String configurada no `appsettings.json` para conectar ao banco de dados local (Coloquei no User Secrets para não expor a senha do banco de dados)
  - Configuração do CORS para permitir requisições do frontend (Ainda será implementado)
  - Controller `OrdersController` para lidar com as requisições relacionadas aos pedidos, utilizando os serviços da camada `Application` para processar a lógica de negócio.
  - Configuração do Swagger para documentação da API e testes das rotas.
- Modelagem de `Tests`:
  - Apenas o testes unitários para o `OrderService` e `MenuItemService`.

### Frontend

- React + TypeScript, pois é a stack que mais tenho experiência e acredito ser a mais adequada para o desenvolvimento de uma aplicação web moderna.
- Mui Material UI, pois é uma biblioteca de componentes React simples.
- Arquitetura mais genérica sem tanta complexidade.

## O que ficou de fora

### Backend - Remoções

- Removi o `IGenericService` e o `IGenericRepository`, pois só o `Order` possui um CRUD completo, seria Over Engineering manter essas interfaces no momento.
- Não foi usando o Fluent Validation, pois as validações são simples e podem ser feitas diretamente nos serviços da camada `Application`, evitando a complexidade adicional de configurar e manter uma biblioteca de validação.
- Autenticação, dado que não há um requisito claro para isso, e o foco do projeto é mais na modelagem do domínio e na implementação dos casos de uso relacionados aos pedidos e itens de menu.
- Troquei o tipo `float` para `decimal` nas propriedades de preço, subtotal e total, pois o tipo `decimal`, pois revi que o tipo `float` não é recomendado para valores monetários.

### Frontend - Remoções

- Tailwind CSS, como o foco não era o frontend, optei por usar o Material UI para estilização, pois é uma biblioteca de componentes React simples e rápida de implementar, evitando a necessidade de configurar e escrever estilos personalizados com Tailwind CSS.
- React Query, dado que a aplicação é simples e não há uma necessidade clara de gerenciamento avançado de estado ou cache.
- Cypress: como o foco do projeto é mais na modelagem do domínio e na implementação dos casos de uso relacionados aos pedidos e itens de menu, optei por não incluir testes end-to-end com Cypress.
- Arquitetura DDD com features, pois era uma aplicação simples
- Next.js, eu não usei, pois o React tradicional já era o suficiente.
