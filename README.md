# DataBaseBooks

**Постановка задачи**

Разработать консольное приложение для работы с «базой данных (БД)», хранящейся в бинарном файле. Перечень полей, достаточно полно
характеризующих заданную в варианте предметную область, предложить
самостоятельно (постараться отразить в перечне полей такие, которые
требуют разных типов данных). При проектировании БД учитывать
требования к проектированию из теории реляционных БД. Приложение должно выполнять следующие функции. 
1. Создание базы данных, содержащей записи указанного формата.
2. Просмотр базы данных.
3. Удаление элементов из базы данных (по ключу/ по значению поля).
4. Корректировка элементов в базе данных (по ключу / по значению поля).
5. Добавление элементов в базу данных (в начало / в конец/ с заданным номером).
6. Реализация запросов к БД (5-6 запросов разной сложности).

Предусмотреть обработку возможных ошибок при работе с БД (с помощью исключений).
В сеансе работы с БД можно использовать вспомогательные (буферные) файлы, которые создаются с началом сеанса и удаляются по его завершению. Во время всего сеанса работы с БД ведется полное протоколирование действий в текстовом файле (в начале сеанса запросить, будет ли это новый файл или дописывать в уже существующий).
Предметная область: Каталог книг

**Особенности реализации**

Структура Field

Представляет собой отдельно взятую запись из базы данных. Содержит в себе перечень значимых полей и служебное поле Header, доступное только для
чтения. Последнее используется для вывода на экран заголовка таблицы БД. 

Класс DataBase

Реализует логическое ядро программы. Оперирует файловыми потоками и
экземплярами структуры Field, абстрагируясь от её внутреннего устройства. Благодаря этому возможно изменение числа и типа полей без изменения кода
данного класса. Использует словарь ключей для реализации системы выбора
записей и контроля за уникальностью ключей в базе данных. 

Класс Logger

Реализует систему протоколирования. Фактически представляет собой
обёртку класса StreamWriter. Для каждого входящего сообщения фиксируется
время его получения, а файл журнала синхронизируется после каждой записи. 

Класс UI

Реализует командный пользовательский интерфейс программы. Также
предоставляет статические методы, предназначенные для создания запросов
к пользователю на получение значений определённого типа, со встроенной
проверкой на правильность.
