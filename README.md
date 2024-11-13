# EmployeeHub

Это консольное приложение на C# для управления информацией о сотрудниках. Пользователи могут добавлять, обновлять, удалять и просматривать данные сотрудников, используя базу данных SQL.

## Использованные технологии

- **C#**: Основной язык программирования, использованный в проекте.
- **ADO.NET**: Библиотека для доступа к данным в .NET, использованная для взаимодействия с базой данных.
- **SQL Server**: Система управления базами данных, служащая для хранения информации о сотрудниках.

## Приемы и паттерны

### 1. Singleton

Паттерн Singleton используется для обеспечения единственного экземпляра класса `DatabaseManager`, отвечающего за подключение к базе данных. Этот подход позволяет избежать создания нескольких подключений и повышает производительность приложения.

### 2. Использование `IDisposable`

Класс `DatabaseManager` реализует интерфейс `IDisposable`, обеспечивая корректное освобождение ресурсов и закрытие соединений с базой данных. Это помогает управлять жизненным циклом объектов и избегать утечек ресурсов.

### 3. Ленивая инициализация

Для создания экземпляра `DatabaseManager` используется ленивая инициализация с помощью класса `Lazy<T>`. Это позволяет инициализировать объект только при первом обращении к нему, что улучшает производительность.

### 4. ADO.NET

Проект использует классы из пространства имен `System.Data.SqlClient` для выполнения CRUD-операций (Create, Read, Update, Delete) с базой данных. Это позволяет гибко управлять запросами и работать с данными.

### 5. Классы и методы для управления данными

Методы приложения, такие как `AddEmployee`, `ViewEmployees`, `UpdateEmployee` и `DeleteEmployee`, предназначены для выполнения операций с данными. Запросы к базе данных используют параметры, что помогает предотвратить атаки типа SQL Injection.

## Как использовать

1. Склонируйте репозиторий на свою локальную машину.
2. Откройте проект в среде разработки (например, Visual Studio).
3. Измените строку подключения в классе `DatabaseManager`, указав необходимые параметры для вашей базы данных.
4. Соберите проект и запустите приложение через консоль.
5. Следуйте инструкциям в меню для управления данными о сотрудниках.
