## About
- This is a parser for site *ru.inshaker.com/* that collects data about cocktails.   
- The idea of implementation may be used to efficiently parse some sites data. The key is **workers scalability**.
- At the time of writing it collects 1165 cocktails and stores the data:
  - Cocktail name
  - Cocktail's what to say about
  - Ingredients (what, unit, count)
  - Required stuff to prepare cocktail (what, unit, count)
  - Link to cocktail's image
  - Receipt of cocktail 

## Stack
- .NET 5.0
- RabbitMQ
- MongoDB
- DockerCompose

## Implementation
Parsing consists of two steps
1. Request list view of cocktails with pagination while we'll not receive the empty list
2. Push the list items with their url to details page that contains the main information to the MongoDb. Items have 'Unprocessed' flag
3. Get the batch from mongo with 'Unprocessed' flag, mark these items as 'Processing' and push items ids to RabbitMQ's working queue
4. Workers listening to RMQ's queue requesting details info, parsing the html and then putting data back to MongoDB setting 'Processing' flag to false

## Running requirements
- Docker

## Running
- Open terminal in root folder
- `docker-compose build`
- `docker-compose up --scale worker=5`
- Parsing takes about 3-5 mins and there is no signal about finish right now. You can ensure the completion by
  - RMQ queue is empty
  - mongo collection has no unprocessed records `db.getCollection('Cocktail').find({Processed: false}).count()` is 0
  - notify that new messages doesn't appear in log

### To save the data you may use the next approach
1. Open mongo's container CLI
2. `mongoexport -d inshaker-parser-mq -c Cocktail --out output.json`
3. Open terminal on your machine 
4. `docker cp mongo:/output.json .` to copy file at the current location



