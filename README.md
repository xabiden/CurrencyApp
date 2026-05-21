# CurrencyApp

Тестовое задание. Микросервисное приложение на .NET 8 для работы с курсами валют.

## Архитектура

```
CurrencyApp/
├── src/
│   ├── BuildingBlocks/
│   │   ├── Shared/               # Общие контракты, результаты, аутентификация
│   │   └── Shared.Api/           # Extension methods для контроллеров
│   ├── Gateway/
│   │   └── ApiGateway/           # YARP Reverse Proxy (порт 5000)
│   ├── Services/
│   │   ├── UserService/          # Регистрация, логин, логаут (порт 5001)
│   │   └── FinanceService/       # Курсы валют, избранное (порт 5002)
│   ├── Workers/
│   │   └── BackgroundWorker/     # Синхронизация курсов с ЦБР (раз в час)
│   └── Tools/
│       └── MigrationService/     # Применение миграций БД при старте
└── tests/
    ├── UserService.Tests/
    └── FinanceService.Tests/
```

## Стек

- .NET 8, ASP.NET Core
- PostgreSQL + Entity Framework Core
- MediatR (CQRS)
- YARP (API Gateway)
- JWT аутентификация
- Docker + Docker Compose
- NUnit + Moq (тесты)

## Быстрый старт

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Запуск

1. Убедитесь что Docker Desktop запущен

2. Склонируйте репозиторий и перейдите в папку проекта:
```bash
git clone https://github.com/xabiden/CurrencyApp.git
cd CurrencyApp
```

3. Создайте файл `.env` в корне репозитория (рядом с `docker-compose.yml`):
```
JWT_SECRET_KEY=super-secret-development-key-change-me-32chars
```

4. Запустите все сервисы:
```bash
docker compose up --build
```

Первый запуск занимает несколько минут — Docker скачает образы и соберёт проекты.

### Порядок запуска контейнеров

```
postgres (healthy)
  → migration-service (применяет миграции, завершается с кодом 0)
      → user-service, finance-service, background-worker
          → api-gateway (стартует когда сервисы healthy)
```

## Проверка


### .http файлы в Visual Studio

Откройте `ApiGateway.http` или `UserService.Api.http`/ `FinanceService.Api.http` и нажмите `Send Request`.

### Swagger UI (только в Development)

```
http://localhost:5001/swagger   # UserService
http://localhost:5002/swagger   # FinanceService
```

### Health checks

```
http://localhost:5000/health   # API Gateway
http://localhost:5001/health   # UserService
http://localhost:5002/health   # FinanceService
```

## API

### UserService — `http://localhost:5001` или через Gateway `http://localhost:5000/user`

| Метод | Endpoint | Описание | Авторизация |
|---|---|---|---|
| POST | `/api/users/register` | Регистрация | Нет |
| POST | `/api/users/login` | Вход, возвращает JWT | Нет |
| POST | `/api/users/logout` | Выход (stateless) | Да |
| GET | `/api/users/me` | Текущий пользователь | Да |

### FinanceService — `http://localhost:5002` или через Gateway `http://localhost:5000/finance`

| Метод | Endpoint | Описание | Авторизация |
|---|---|---|---|
| GET | `/api/currencies` | Все валюты | Да |
| GET | `/api/favorites` | Избранные валюты пользователя | Да |
| POST | `/api/favorites` | Добавить валюту в избранное | Да |

### Пример использования

**1. Регистрация:**
```http
POST http://localhost:5001/api/users/register
Content-Type: application/json

{
  "name": "denis",
  "password": "Password123!"
}
```

**2. Вход:**
```http
POST http://localhost:5001/api/users/login
Content-Type: application/json

{
  "name": "denis",
  "password": "Password123!"
}
```

Из ответа скопируйте `data.token`.

**3. Добавить валюту в избранное:**
```http
POST http://localhost:5001/api/favorites
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "currencyCode": "USD"
}
```

**4. Получить избранные валюты:**
```http
GET http://localhost:5002/api/favorites
Authorization: Bearer YOUR_TOKEN
```

## Тесты

```bash
dotnet test
```

## Остановка

```bash
# Остановить контейнеры (данные сохранятся)
docker compose down

# Остановить и удалить все данные
docker compose down -v
```
