Commands, Events, Views and everything in-between
=================================================


### 2 Layers
 - "Rich" client connecting directly to data storage
 - Data model enforcing some constraints
 - Insert a row, Read it back, Update it

 - Too much business logic in the client


### n-Tier
 - Separating Domain Logic from Client views
 - Model/View/Controller pattern
 - ORM
 - Add an object -> Insert a row
 - Read a row -> Build an object
 - Save the object -> Update a row

 - Integration difficult


### Services
 - Clear boundaries
 - Explicit contracts
 - Request object -> ORM -> Database
 - Database -> ORM -> Response object

### Service Oriented Architecture
 - Clients can stay independent of services
 - Overhead of transforming between Database -> Objects -> Contracts


Data stores
-----------
Relational vs. Document database

### Relational Data Model
 - Denormalized tables
 - No redundant data
 - Quite efficient for writing
 - Not so good for reading
 - Good for ad-hoc queries

#### Sample queries
 - Get an order, with order lines and shipping address
 - The products I buy the most

### Document Data Model
 - Each Order is a document
 - Very quick to read a single document
 - No good for ad-hoc queries
 - Aggregation requires Map/Reduce

### What to choose?
 - Optimized for read or write?


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
 

### Commit
 - Several events that go together as one atomic unit/transaction
 - You often want to commit several events at once
 - Reading is always done on the entire stream

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

### Competing consumer - Command side
 - 

### Competing consumer - Denormalizers
 - 


### Event granularity
 - Coarse events
   - Easier on the infrastructure
   - Often good for integration
 - Granular events
   - Easier to break apart later
   - Example - price on order line or as separate event

### Command granularity
 - Granular commands
   - Flexible
   - Tricky with competing consumers
 - Coarse commands
   - One command per user interaction
   - 


### Event-based workflows
 - 

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



