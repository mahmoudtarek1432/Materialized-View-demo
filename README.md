The Repo should be implementing a materialized view as a type of replication when dealing with sparate bounded context databases.
the course of flow will be:
1- an update/add/delete command takes place at the main bounded context.
2- an event is fired marking a change in the entity state.
3- the event is consumed in another context that is concerned with the changes in an eventual consisency fashion by digesting the event
and updating the external models representing the changed entity
