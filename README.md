# StreamingCoursesBot
Перед началом работы необходимо:
1. Создать проект в [https://console.firebase.google.com/](https://console.firebase.google.com/u/0/

2. прописать следующие данные в appsettings:
   2.1. Firebase - BasePath и Secret
   2.2. ConnectionStrings - строку подключения к БД
   2.3. BotConfiguration - настройки бота

3. В файле добавить в коде данные администратора

По умолчанию есть 4 роли:
1. Администратор
2. Куратор
3. Студент
4. Пользователь
   
Функции администратора:
1. Создавать (загружать данные по шаблону):
   1.1. Курсы
   1.2. Кураторов
   1.3. Телеграм группы
   1.4. Студентов
2. Связывать сущности Курс - Группа - Куратор - Студент
3. Запускать новый курс
4. Выгружать статистику по каждому курсу

