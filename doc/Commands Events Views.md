Commands, Events, Views and everything in-between
=================================================

n-Tier Architecture
------------

### Client/Server
 - "Rich" client connecting directly to data storage
 - Data model enforcing some constraints
 - Business logic in the client
 - Insert a row, Read it back, Update it


### n-Tier
 - Separating Domain Logic from Client views
 - Model/View/Controller pattern
 - ORM
 - Add an object -> Insert a row
 - Read a row -> Build an object
 - Save the object -> Update a row


### Services
 - Clear boundaries
 - Request object -> ORM -> Database
 - Database -> ORM -> Response object



### What is CQRS
 - Command
 - Projection / Denormalizer


### What is Domain Driven Design
 - Aggregate
 - Entity
 - Bounded Context


### What is Event Sourcing
 - Audit log/Event Store


### Do they go hand-in-hand?
 - Example of CQRS without ES

 - Example of ES without DDD

 - ES without CQRS?
   * Probably not a good idea


### Snapshots
 - Are your bounding contexts correct?
 - Should you be using Event Sourcing?
 - Do you need them?
 - Are you sure?


Projections
-----------

### Two basic types
 - Per aggregate
 - Global


### In-process
 - Fast
 - Not as "clean"
 - Not as scalable
 - What happens on failure?


### Events -> Message bus -> Denormalizers 
 - Slow
 - Ordering of events
 - Events lost depending on the bus
 - Replaying difficult


### Catch-up subscriptions
 - Works with GES (GetEventStore)
 - Partitioning?

### Batch
 - Poll Event Store every x milliseconds

### Notify per commit
 - Message on service bus per command
 - Easy to scale out


Testing
-------

### BDD - Behavior-Driven Design

 - Given some events
 - When this command
 - Then these events 


ProTips(TM)
-----------

### Things to do early on

 - Logging
 - Replay of Events
 - Simple admin view
 - Think through your bounded contexts
 - Deployment chain


### Things that can wait
	
 - Storage for view models
 - Service bus
 - Real-time client / SignalR



