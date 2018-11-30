# TvMazeScraper

A sample application that scrapes data about (TV) shows and their cast from TVMaze, stores it locally and can return it (paged) as JSON.

This should investigate industry-standard practices in coding style and modern application structure.

## Background
For a new metadata ingester we need a service that provides the cast of all the tv shows in the TVMaze database, so we can enrich our metadata system with this information. The TVMaze database provides a public REST API that you can query for this data:
http://www.tvmaze.com/api  
This API requires no authentication but it is rate limited, so keep that in mind.

## Assignment
We want you to create an application that:
1.	scrapes the TVMaze API for show and cast information;
2.	persists the data in storage;
3.	provides the scraped data using a REST API.

We want the REST API to satisfy the following business requirements.
1.	It should provide a paginated list of all tv shows containing the id of the TV show and a list of all the cast that are playing in that TV show.
2.	The list of the cast must be ordered by birthday descending.


The REST API should provide a JSON response when a call to a HTTP endpoint is made (it's up to you what URI).
Example response:

```json
[
    {
    "id": 1,
    "name": "Game of Thrones",
    "cast": [
        {
            "id": 9,
            "name": "Dean Norris",
            "birthday": "1963-04-08"
        },
        {
            "id": 7,
            "name": "Mike Vogel",
            "birthday": "1979-07-17"
        }
        ]
    },
    {
    "id": 4,
    "name": "Big Bang Theory",
    "cast": [
        {
            "id": 6,
            "name": "Michael Emerson",
            "birthday": "1950-01-01"
        }
        ]
    }
]
```

## Additions
Later extensions of the original assignment.

### SignalR
Add a SignalR system to communicate between a background scraper and the browser. Add "start" and "stop" buttons.


### MongoDB
Use MongoDB as storage medium instead of SqlServer. Switch between them through a configuration setting.
Some configuration depends on the specific storage medium used.

### Enrich data through other system
When a Show is stored with data from TvMaze, fire an event that causes this show to be enriched with data from another webservice.
Use the OMDb API to get the IMDb rating and store it as part of the show.

As the OMDB API accepts at most 1000 requests per day, add this rating asynchrounously. 
Use an Azure Function between this application and OMDB that can queue queries and cache the results.

## Structure

The solution is structured in several parts:
* Core - contains the core functionality of the app: the services. Includes DTOs and interfaces.
* UI - the UI that presents the results to the user
* Infrastructure - the part that talks to the outside world (microservices, database, external APIs). There is a dependency *to* Core.
* Microservices - Azure Functions for specific tasks.
