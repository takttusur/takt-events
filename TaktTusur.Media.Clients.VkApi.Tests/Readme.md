﻿ Для получения API ключа ВКонтакте необходимо:
1. Зарегистрироваться как разработчик на сайте https://vk.com/dev.
2. Выбрать Standalone - приложение (далее вам предложат перейти в другой сервис)
3. В нем выбрать физическое лицо
4. Ввести название приложеия, выбрать Web 
5. Ввести домены (как пример testapp.ru)
6. Зайти в приложениеи найти раздел "Ключи доступа"
7. Нужно нажать на знак глаза для "Сервисного ключа доступа" - это и есть наш API ключ

Для того чтобы добавить его в код необходимо:
1. ПКМ щелкнуть по проекту с тестами (TaktTusur.Media.Clients.VkApi.Tests)
2. Выбрать пункт "Управление секретами пользователей
3. и между фигурных скобок вставить ("VkApiKey": " ***Ваш API ключ*** ") 
4. Сохранить файл 

Программа готова к работе