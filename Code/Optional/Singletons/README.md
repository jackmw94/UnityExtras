# Singleton MonoBehaviours

Singletons aren't bad, they're just misunderstood. They had a bad upbringing.


This is a nice little wrapper that handles all the singleton stuff. Assumes that the instance will be in the scene when required rather than creating one, I found that generating instances of monobehaviours that I expected to be singletons tended to be more confusing than useful.