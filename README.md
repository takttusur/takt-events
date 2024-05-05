# TAKT Events API

Сервис предоставляет информацию о новосятх клуба, событиях, в перспективе - событиях туристско-альпинисткого сообщества Томска. 

## Технологии

* .NET
* Swagger
* Redis
* REST API
* VK API

## Dev environment

### Configure

Add user-secrets for `TaktTusur.Media.Worker`:
```json
{
  "VkSource": {
    "VkApiKey": "YOUR_VK_KEY",
    "GroupId": "ID of group with events"
  }
}
```

### Start

1. `docker-compose up` - start Redis
2. Start `TaktTusur.Media.Worker` project 