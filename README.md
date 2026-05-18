# OnlineShop

Учебный pet-проект: онлайн-магазин на ASP.NET Core 9 MVC.

## Стек

- ASP.NET Core 9 MVC
- Entity Framework Core + PostgreSQL (Npgsql)
- ASP.NET Core Identity (аутентификация и роли)
- AutoMapper
- Serilog (JSON-логи в файл)
- Bootstrap 5 (через CDN)

## Структура решения

```
OnlineShop.sln
├── OnlineShop.Db/      — слой данных: модели, DbContext, хранилища, миграции
├── OnlineShop.Core/    — бизнес-логика: DTO, сервисы, AutoMapper-профили
└── OnlineShop.Web/     — веб-слой: контроллеры, Views, ViewModels, DI
```

Зависимости: `Web → Core → Db`. Web НЕ ссылается напрямую на Db —
все обращения к данным идут через сервисы Core.

## Локальный запуск

### 1. Поднять PostgreSQL в Docker

```bash
docker compose up -d
```

Подробности — в комментариях к `docker-compose.yml`.

### 2. Применить миграции

> Появится в Фазе 2.

### 3. Запустить веб-приложение

В Visual Studio: F5 (профиль `https`).
В терминале: `dotnet run --project OnlineShop.Web`.

## Статус

Проект в активной разработке (по фазам). Текущий прогресс — см. историю коммитов.
