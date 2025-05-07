# MeteoApp
Progetto del modulo: Sviluppo di Applicazioni Mobile
Autori: Sara Bressan e Alessandro Formato
Semestre: SA 2025
Istituto: SUPSI DTI

## Abstract

Questo progetto corrisponde ad un'applicazione mobile sviluppata in MAUI per la consultazione di informazioni metereologiche.

## Informazioni 


### Avvertenze

Per far funzionare correttamente il progetto, è necessario aggiungere il file con le API all'interno di un file "keys.json" localizzato nelle risorse del progetto "Resources/Raw".
Il file deve essere strutturato come segue:

```JSON
{
  "OpenWeatherApiKey": "openweathermap",
  "AppWriteEndpoint": "endpoint",
  "AppWriteProjectId": "projectid",
  "AppWriteDatabaseId": "meteoapp",
  "AppWriteCollectionId": "locations",
  "AppWriteApiKey": "apikey"
}
```
